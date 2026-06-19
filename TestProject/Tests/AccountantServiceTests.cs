using API.DTOs.BusinessDTOs;
using API.Entities.BusinessEntities;
using API.Exceptions;
using API.Infrastructure;
using API.Services.BusinessServices;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Version = API.Entities.BusinessEntities.Version;

namespace API.Tests;

public class AccountantServiceTests : TestBase
{
    private DatabaseContext SeedBasicContext()
    {
        var ctx = CreateContext();

        ctx.Contracts.Add(new Contract
        {
            Id = 1,
            Signed = true,
            Billing = new Billing
            {
                License = new License { FinalPrice = 1000 }
            }
        });

        ctx.SaveChanges();
        return ctx;
    }
    
    [Fact]
    public async Task GetIncomeAsync_ReturnsSum_ForPLN_WithoutSoftwareId_ExpectedFalse()
    {
        using var ctx = SeedBasicContext();
        var currency = new FakeCurrencyService(1m);

        var service = new AccountantService(ctx, currency);

        var result = await service.GetIncomeAsync(null, "PLN", false, CancellationToken.None);

        Assert.Equal(1000, result.Amount);
        Assert.Equal("PLN", result.Currency);
    }

    [Fact]
    public async Task GetIncomeAsync_ReturnsSum_ForSoftwareId_ExpectedFalse()
    {
        using var ctx = CreateContext();

        var software = new Software { Id = 1, Name = "Soft" };
        var version = new Version { Id = 1, SoftwareId = 1 };
        var av = new AvailableVersion { Id = 1, VersionId = 1 };

        ctx.Softwares.Add(software);
        ctx.Versions.Add(version);
        ctx.AvailableVersions.Add(av);

        ctx.Contracts.Add(new Contract
        {
            Id = 1,
            Signed = true,
            Billing = new Billing
            {
                License = new License { FinalPrice = 500 }
            },
            AvailableVersions = new List<AvailableVersion> { av }
        });

        ctx.SaveChanges();

        var currency = new FakeCurrencyService(1m);
        var service = new AccountantService(ctx, currency);

        var result = await service.GetIncomeAsync(1, "PLN", false, CancellationToken.None);

        Assert.Equal(500, result.Amount);
        Assert.Equal("PLN", result.Currency);
    }

    // ---------------------------
    // CURRENCY CONVERSION TESTS
    // ---------------------------

    [Fact]
    public async Task GetIncomeAsync_ConvertsCurrency_WhenValidCode()
    {
        using var ctx = SeedBasicContext();

        var currency = new FakeCurrencyService(4m); // USD = 4 PLN
        var service = new AccountantService(ctx, currency);

        var result = await service.GetIncomeAsync(null, "USD", false, CancellationToken.None);

        Assert.Equal(250, result.Amount); // 1000 / 4
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public async Task GetIncomeAsync_Throws_WhenCurrencyServiceThrows()
    {
        using var ctx = SeedBasicContext();

        var currency = new FakeCurrencyService(new NotFoundException("Currency not found"));
        var service = new AccountantService(ctx, currency);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetIncomeAsync(null, "XYZ", false, CancellationToken.None));
    }
}
