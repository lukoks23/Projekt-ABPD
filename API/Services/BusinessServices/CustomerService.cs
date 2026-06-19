using API.DTOs.BusinessDTOs;
using API.Entities.BusinessEntities;
using API.Exceptions;
using API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace API.Services.BusinessServices;

public class CustomerService(DatabaseContext ctx) : ICustomerService
{
    public async Task<OutCustomerDto> AddCustomerAsync(InCustomerDto inCustomerDto, CancellationToken ct)
    {
        if ((inCustomerDto.Company is null && inCustomerDto.Individual is null) ||
            (inCustomerDto.Individual is not null && inCustomerDto.Company is not null))
        {
            throw new BadRequestException("Bad Customer Data");
        }

        await ctx.Database.BeginTransactionAsync(ct);
        try
        {
            if (inCustomerDto.Company is not null)
            {
                var companyC = await ctx.Companies.Where(c => c.Krs == inCustomerDto.Company.Krs)
                    .FirstOrDefaultAsync(ct);
                if (companyC is not null)
                {
                    throw new ConflictException("Company already exists");
                }
            }

            if (inCustomerDto.Individual is not null)
            {
                var invidualC = await ctx.Inviduals.Where(i => i.Pesel == inCustomerDto.Individual.Pesel)
                    .FirstOrDefaultAsync(ct);
                if (invidualC is not null)
                {
                    throw new ConflictException("Individual already exists");
                }
            }

            var country = await ctx.Countries.Where(c => c.Name == inCustomerDto.Address.Country)
                .FirstOrDefaultAsync(ct);
            if (country is null)
            {
                country = new Country { Name = inCustomerDto.Address.Country };
                await ctx.Countries.AddAsync(country, ct);
            }

            var city = await ctx.Cities.Where(c =>
                c.Name == inCustomerDto.Address.City &&
                c.Country == country
            ).FirstOrDefaultAsync(ct);
            if (city is null)
            {
                city = new City { Name = inCustomerDto.Address.City, Country = country };
                await ctx.Cities.AddAsync(city, ct);
            }

            var street = await ctx.Streets.Where(s =>
                s.Name == inCustomerDto.Address.Street &&
                s.City == city
            ).FirstOrDefaultAsync(ct);
            if (street is null)
            {
                street = new Street { Name = inCustomerDto.Address.Street, City = city };
                await ctx.Streets.AddAsync(street, ct);
            }

            var addr = await ctx.Addresses.Where(ad =>
                ad.ApartmentNumber == inCustomerDto.Address.ApartmentNumber &&
                ad.BuildingNumber == inCustomerDto.Address.BuildingNumber &&
                ad.PostCode == inCustomerDto.Address.PostalCode &&
                ad.Street.Name == inCustomerDto.Address.Street &&
                ad.Street.City.Name == inCustomerDto.Address.City &&
                ad.Street.City.Country.Name == inCustomerDto.Address.Country
            ).FirstOrDefaultAsync(ct);
            if (addr is null)
            {
                addr = new Address
                {
                    BuildingNumber = inCustomerDto.Address.BuildingNumber,
                    ApartmentNumber = inCustomerDto.Address.ApartmentNumber,
                    PostCode = inCustomerDto.Address.PostalCode,
                    Street = street
                };
                await ctx.Addresses.AddAsync(addr, ct);
            }

            Invidual? invidual = null;
            if (inCustomerDto.Individual is not null)
            {
                invidual = new Invidual
                {
                    Pesel = inCustomerDto.Individual.Pesel,
                    Email = inCustomerDto.Individual.Email,
                    FirstName = inCustomerDto.Individual.FirstName,
                    LastName = inCustomerDto.Individual.LastName,
                    PhoneNumber = inCustomerDto.Individual.Phone,
                    Address = addr
                };
                await ctx.Inviduals.AddAsync(invidual, ct);
                var entity = new Entity { Pesel = invidual.Pesel };
                await ctx.Entities.AddAsync(entity, ct);
            }

            Company? company = null;
            if (inCustomerDto.Company is not null)
            {
                company = new Company
                {
                    Krs = inCustomerDto.Company.Krs,
                    Email = inCustomerDto.Company.Email,
                    PhoneNumber = inCustomerDto.Company.Phone,
                    Address = addr
                };
                await ctx.Companies.AddAsync(company, ct);
                var entity = new Entity { Company = company };
                await ctx.Entities.AddAsync(entity, ct);
            }

            await ctx.SaveChangesAsync(ct);
            await ctx.Database.CommitTransactionAsync(ct);


            OutCompanyDto? companyDto = null;
            OutInvidualDto? invidualDto = null;

            if (company is not null)
            {
                companyDto = new OutCompanyDto(company.Krs, company.Email, company.PhoneNumber);
            }

            if (invidual is not null)
            {
                invidualDto = new OutInvidualDto(invidual.Pesel, invidual.FirstName, invidual.LastName, invidual.Email,
                    invidual.PhoneNumber);
            }

            var addrDto = new OutAddrDto
            (
                addr.Street.City.Country.Name,
                addr.Street.City.Name,
                addr.Street.Name,
                addr.PostCode,
                addr.BuildingNumber,
                addr.ApartmentNumber
            );
            return new OutCustomerDto(addrDto, companyDto, invidualDto);
        }
        catch
        {
            await ctx.Database.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task DeleteCustomerAsync(string pesel, CancellationToken ct)
    {
        var invidual = await ctx.Inviduals.Where(i => i.Pesel == pesel).FirstOrDefaultAsync(ct);
        if (invidual is null)
        {
            throw new NotFoundException("Individual not found");
        }
        invidual.Email = string.Empty;
        invidual.PhoneNumber = string.Empty;
        invidual.FirstName = string.Empty;
        invidual.LastName = string.Empty;
        await ctx.SaveChangesAsync(ct);
    }

    public async Task<OutCustomerDto> UpdateCustomerAsync(InUpdateCustomerDto inCustomerDto, CancellationToken ct)
    {
        if ((inCustomerDto.Company is null && inCustomerDto.Individual is null) ||
            (inCustomerDto.Individual is not null && inCustomerDto.Company is not null))
        {
            throw new BadRequestException("Bad Customer Data");
        }
        await ctx.Database.BeginTransactionAsync(ct);
        try
        {
            Invidual? invidual = null;
            Company? company = null;
            if (inCustomerDto.Company is not null)
            {
                company = await ctx.Companies.Where(c => c.Krs == inCustomerDto.Company.Krs).FirstOrDefaultAsync(ct);
                if (company is null)
                {
                    throw new NotFoundException("Company not found");
                }

                if (inCustomerDto.Company.Email is not null)
                {
                    company.Email = inCustomerDto.Company.Email;
                }

                if (inCustomerDto.Company.Phone is not null)
                {
                    company.PhoneNumber = inCustomerDto.Company.Phone;
                }
            }

            if (inCustomerDto.Individual is not null)
            {
                invidual = await ctx.Inviduals.Where(i => i.Pesel == inCustomerDto.Individual.Pesel)
                    .FirstOrDefaultAsync(ct);
                if (invidual is null)
                {
                    throw new NotFoundException("Individual not found");
                }

                if (inCustomerDto.Individual.Email is not null)
                {
                    invidual.Email = inCustomerDto.Individual.Email;
                }

                if (inCustomerDto.Individual.Phone is not null)
                {
                    invidual.PhoneNumber = inCustomerDto.Individual.Phone;
                }

                if (inCustomerDto.Individual.FirstName is not null)
                {
                    invidual.FirstName = inCustomerDto.Individual.FirstName;
                }

                if (inCustomerDto.Individual.LastName is not null)
                {
                    invidual.LastName = inCustomerDto.Individual.LastName;
                }
            }

            Address? addr = null;
            if (inCustomerDto.Address is not null)
            {
                var country = await ctx.Countries.Where(c => c.Name == inCustomerDto.Address.Country)
                    .FirstOrDefaultAsync(ct);
                if (country is null)
                {
                    country = new Country { Name = inCustomerDto.Address.Country };
                    await ctx.Countries.AddAsync(country, ct);
                }

                var city = await ctx.Cities.Where(c =>
                    c.Name == inCustomerDto.Address.City &&
                    c.Country == country
                ).FirstOrDefaultAsync(ct);
                if (city is null)
                {
                    city = new City { Name = inCustomerDto.Address.City, Country = country };
                    await ctx.Cities.AddAsync(city, ct);
                }

                var street = await ctx.Streets.Where(s =>
                    s.Name == inCustomerDto.Address.Street &&
                    s.City == city
                ).FirstOrDefaultAsync(ct);
                if (street is null)
                {
                    street = new Street { Name = inCustomerDto.Address.Street, City = city };
                    await ctx.Streets.AddAsync(street, ct);
                }

                addr = await ctx.Addresses.Where(ad =>
                    ad.ApartmentNumber == inCustomerDto.Address.ApartmentNumber &&
                    ad.BuildingNumber == inCustomerDto.Address.BuildingNumber &&
                    ad.PostCode == inCustomerDto.Address.PostalCode &&
                    ad.Street.Name == inCustomerDto.Address.Street &&
                    ad.Street.City.Name == inCustomerDto.Address.City &&
                    ad.Street.City.Country.Name == inCustomerDto.Address.Country
                ).FirstOrDefaultAsync(ct);
                if (addr is null)
                {
                    addr = new Address
                    {
                        BuildingNumber = inCustomerDto.Address.BuildingNumber,
                        ApartmentNumber = inCustomerDto.Address.ApartmentNumber,
                        PostCode = inCustomerDto.Address.PostalCode,
                        Street = street
                    };
                    await ctx.Addresses.AddAsync(addr, ct);
                }
            }
            else
            {
                if (inCustomerDto.Individual is not null)
                {
                    addr = await ctx.Inviduals.Where(i => i.Pesel == inCustomerDto.Individual.Pesel)
                        .Select(i => i.Address).FirstOrDefaultAsync(ct);
                }

                if (inCustomerDto.Company is not null)
                {
                    addr = await ctx.Companies.Where(c => c.Krs == inCustomerDto.Company.Krs).Select(c => c.Address)
                        .FirstOrDefaultAsync(ct);
                }

                if (addr is null)
                {
                    throw new NotFoundException("Address not found");
                }
            }

            if (invidual is not null)
            {
                invidual.Address = addr;
            }

            if (company is not null)
            {
                company.Address = addr;
            }

            await ctx.SaveChangesAsync(ct);
            await ctx.Database.CommitTransactionAsync(ct);

            OutCompanyDto? companyDto = null;
            OutInvidualDto? invidualDto = null;

            if (company is not null)
            {
                companyDto = new OutCompanyDto(company.Krs, company.Email, company.PhoneNumber);
            }

            if (invidual is not null)
            {
                invidualDto = new OutInvidualDto(invidual.Pesel, invidual.FirstName, invidual.LastName, invidual.Email,
                    invidual.PhoneNumber);
            }

            addr = await ctx.Addresses
                .Include(a => a.Street)
                .ThenInclude(s => s.City)
                .ThenInclude(c => c.Country)
                .FirstOrDefaultAsync(a => a.Id == addr.Id, ct);
            if (addr is null)
            {
                throw new NotFoundException("Address not found, during processing");
            }


            var addrDto = new OutAddrDto
            (
                addr.Street.City.Country.Name,
                addr.Street.City.Name,
                addr.Street.Name,
                addr.PostCode,
                addr.BuildingNumber,
                addr.ApartmentNumber
            );
            return new OutCustomerDto(addrDto, companyDto, invidualDto);
        }
        catch
        {
            await ctx.Database.RollbackTransactionAsync(ct);
            throw;
        }
    }
}