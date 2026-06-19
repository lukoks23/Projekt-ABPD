using API.DTOs.BusinessDTOs;
using API.Entities.BusinessEntities;
using API.Exceptions;
using API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace API.Services.BusinessServices;

public class ContractService(DatabaseContext ctx) : IContractService
{
    public async Task<OutContractDto> AddContractAsync(InContractDto inContractDto, CancellationToken ct)
    {
        if ((inContractDto.Pesel is null && inContractDto.Krs is null) ||
            (inContractDto.Pesel is not null && inContractDto.Krs is not null))
        {
            throw new BadRequestException("Pesel and Krs are exclusive to each other");
        }

        await ctx.Database.BeginTransactionAsync(ct);
        try
        {
            Invidual? invidual = null;
            if (inContractDto.Pesel is not null)
            {
                invidual = await ctx.Inviduals.Where(i => i.Pesel == inContractDto.Pesel).Include(i => i.Entity)
                    .FirstOrDefaultAsync(ct);
                if (invidual is null)
                {
                    throw new NotFoundException("Invidual not found");
                }
            }

            Company? company = null;
            if (inContractDto.Krs is not null)
            {
                company = await ctx.Companies.Where(c => c.Krs == inContractDto.Krs).Include(c => c.Entity)
                    .FirstOrDefaultAsync(ct);
                if (company is null)
                {
                    throw new NotFoundException("Company not found");
                }
            }

            var software = await ctx.Softwares.Where(s => s.Id == inContractDto.SoftwareId).FirstOrDefaultAsync(ct);
            if (software is null)
            {
                throw new NotFoundException("Software not found");
            }

            var versions = await ctx.Versions.Where(v => inContractDto.VersionsIds.Contains(v.Id)).ToListAsync(ct);
            if (versions is null || versions.Count < inContractDto.VersionsIds.Count)
            {
                throw new NotFoundException("Versions not found");
            }

            Entity? entity = null;
            if (invidual is not null)
            {
                entity = invidual.Entity;
            }

            if (company is not null)
            {
                entity = company.Entity;
            }
            
            var cont = await ctx.Contracts.Where(c => c.Signed == true && c.Entity == entity &&
                                                     c.AvailableVersions.Any(v => 
                                                         v.Version.SoftwareId == inContractDto.SoftwareId)
            ).FirstOrDefaultAsync(ct);
            if (cont is not null)
            {
                throw new ConflictException("Active Contract for this customer and product already exists");
            }


            var cost = await ctx.SoftwareCosts
                .Where(c => c.SoftwareId == inContractDto.SoftwareId && c.BillingType.Type == inContractDto.BillingType)
                .FirstOrDefaultAsync(ct);
            if (cost is null)
            {
                throw new NotFoundException("Price for this specification not found");
            }

            decimal? discount = null;
            try
            {
                discount = await ctx.SoftDiscs.Where(c => c.SoftwareId == inContractDto.SoftwareId)
                    .Select(s => s.Discount).Where(d => d.BillingType.Type == inContractDto.BillingType)
                    .MaxAsync(d => d.Percent, ct);
            }
            catch (InvalidOperationException)
            {
                discount = 0;
            }

            var con = await ctx.Contracts.Where(c => c.Entity == entity).FirstOrDefaultAsync(ct);
            if (con is not null)
            {
                discount += 5;
            }

            var license = new License
            {
                YearsOfSupport = inContractDto.YearsOfSupport,
                FinalPrice = (cost.Price - cost.Price * discount / 100)+1000*(inContractDto.YearsOfSupport-1) ?? 0
            };
            await ctx.Licenses.AddAsync(license, ct);

            var billing = new Billing
            {
                License = license
            };

            var contract = new Contract
            {
                Entity = entity,
                Signed = false,
                Billing = billing,
                From = inContractDto.DateFrom,
                To = inContractDto.DateTo
            };
            await ctx.Contracts.AddAsync(contract, ct);

            var availableVersionsList = new List<AvailableVersion>();
            foreach (var version in versions)
            {
                var availableVersion = new AvailableVersion
                {
                    Contract = contract,
                    Version = version
                };
                availableVersionsList.Add(availableVersion);
            }

            await ctx.AvailableVersions.AddRangeAsync(availableVersionsList, ct);
            await ctx.SaveChangesAsync(ct);
            await ctx.Database.CommitTransactionAsync(ct);

            return new OutContractDto(contract.Id,contract.Signed, contract.From, contract.To, software.Name,
                inContractDto.YearsOfSupport, license.FinalPrice);
        }
        catch
        {
            await ctx.Database.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<OutPaymentDto> AddPaymentAsync(InPaymentDto inPaymentDto, CancellationToken ct)
    {
        await ctx.Database.BeginTransactionAsync(ct);
        try
        {
            var contract = await ctx.Contracts.Where(c => c.Id == inPaymentDto.ContractId).Include(c => c.Billing).ThenInclude(b => b.License).FirstOrDefaultAsync(ct);
            if (contract is null)
            {
                throw new NotFoundException("Contract not found");
            }

            if (contract.Signed)
            {
                throw new ConflictException("Contract already signed");
            }

            if (contract.To < inPaymentDto.PaymentDate)
            {
                throw new BadRequestException("Too late for payment");
            }

            var totalPayments = await ctx.Payments.Where(p => p.ContractId == inPaymentDto.ContractId).SumAsync(p => p.TransferredMoney,ct);
            var payment = new Payment
            {
                ContractId = inPaymentDto.ContractId,
                BankAccountNumber = inPaymentDto.BankAccountNumber,
                PaymentDate = inPaymentDto.PaymentDate,
                TransferredMoney = inPaymentDto.TransferredAmount
            };
            await ctx.Payments.AddAsync(payment, ct);
            var price = 0.0m;
            if (contract.Billing.License is not null)
            {
                price = contract.Billing.License.FinalPrice;
            }
            
            if (totalPayments + inPaymentDto.TransferredAmount >= price)
            {
                contract.Signed = true;
            }
            totalPayments += inPaymentDto.TransferredAmount;
            await ctx.SaveChangesAsync(ct);
            await ctx.Database.CommitTransactionAsync(ct);

            return new OutPaymentDto(price - totalPayments, contract.Signed);
        }
        catch
        {
            await ctx.Database.RollbackTransactionAsync(ct);
            throw;
        }
    }
}