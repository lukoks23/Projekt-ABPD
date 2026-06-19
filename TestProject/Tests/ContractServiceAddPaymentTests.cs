using API.DTOs.BusinessDTOs;
using API.Entities.BusinessEntities;
using API.Exceptions;
using API.Services.BusinessServices;
using Xunit;

namespace API.Tests;

public class ContractServiceAddPaymentTests : TestBase
{
    private InPaymentDto CreatePaymentDto(int contractId, decimal amount) =>
        new(
            PaymentDate: DateTime.Today,
            TransferredAmount: amount,
            ContractId: contractId,
            BankAccountNumber: "PL00123456789012345678901234"
        );

    [Fact]
    public async Task AddPaymentAsync_Throws_WhenContractNotFound()
    {
        using var ctx = CreateContext();
        var service = new ContractService(ctx);

        var dto = CreatePaymentDto(999, 100);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.AddPaymentAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task AddPaymentAsync_Throws_WhenContractAlreadySigned()
    {
        using var ctx = CreateContext();

        ctx.Contracts.Add(new Contract
        {
            Id = 1,
            Signed = true,
            Billing = new Billing { License = new License { FinalPrice = 1000 } }
        });

        ctx.SaveChanges();

        var service = new ContractService(ctx);

        var dto = CreatePaymentDto(1, 100);

        await Assert.ThrowsAsync<ConflictException>(() =>
            service.AddPaymentAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task AddPaymentAsync_Throws_WhenPaymentTooLate()
    {
        using var ctx = CreateContext();

        ctx.Contracts.Add(new Contract
        {
            Id = 1,
            Signed = false,
            To = DateTime.Today.AddDays(-1),
            Billing = new Billing { License = new License { FinalPrice = 1000 } }
        });

        ctx.SaveChanges();

        var service = new ContractService(ctx);

        var dto = CreatePaymentDto(1, 100);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            service.AddPaymentAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task AddPaymentAsync_AddsPayment_ContractNotSigned()
    {
        using var ctx = CreateContext();

        ctx.Contracts.Add(new Contract
        {
            Id = 1,
            Signed = false,
            To = DateTime.Today.AddDays(10),
            Billing = new Billing { License = new License { FinalPrice = 1000 } }
        });

        ctx.SaveChanges();

        var service = new ContractService(ctx);

        var dto = CreatePaymentDto(1, 200);

        var result = await service.AddPaymentAsync(dto, CancellationToken.None);

        Assert.Equal(800, result.LeftToPay);
        Assert.False(result.Signed);
    }

    [Fact]
    public async Task AddPaymentAsync_SignsContract_WhenPaidEnough()
    {
        using var ctx = CreateContext();

        ctx.Contracts.Add(new Contract
        {
            Id = 1,
            Signed = false,
            To = DateTime.Today.AddDays(10),
            Billing = new Billing { License = new License { FinalPrice = 1000 } }
        });

        ctx.Payments.Add(new Payment
        {
            ContractId = 1,
            TransferredMoney = 600
        });

        ctx.SaveChanges();

        var service = new ContractService(ctx);

        var dto = CreatePaymentDto(1, 500);

        var result = await service.AddPaymentAsync(dto, CancellationToken.None);

        Assert.Equal(-100, result.LeftToPay);
        Assert.True(result.Signed);
    }
}
