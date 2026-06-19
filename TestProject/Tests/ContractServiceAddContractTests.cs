using API.DTOs.BusinessDTOs;
using API.Entities.BusinessEntities;
using API.Exceptions;
using API.Services.BusinessServices;
using Xunit;
using Version = API.Entities.BusinessEntities.Version;

namespace API.Tests;

public class ContractServiceAddContractTests : TestBase
{
    private InContractDto CreateValidDto() =>
        new(
            Pesel: "12345678901",
            Krs: null,
            DateFrom: DateTime.Today,
            DateTo: DateTime.Today.AddDays(10),
            BillingType: "License",
            YearsOfSupport: 1,
            SoftwareId: 1,
            VersionsIds: new List<int> { 1 }
        );

    [Fact]
    public async Task AddContractAsync_Throws_WhenPeselAndKrsProvided()
    {
        using var ctx = CreateContext();
        var service = new ContractService(ctx);

        var dto = new InContractDto(
            Pesel: "12345678901",
            Krs: "1234567890",
            DateFrom: DateTime.Today,
            DateTo: DateTime.Today.AddDays(10),
            BillingType: "License",
            YearsOfSupport: 1,
            SoftwareId: 1,
            VersionsIds: new List<int> { 1 }
        );

        await Assert.ThrowsAsync<BadRequestException>(() =>
            service.AddContractAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task AddContractAsync_Throws_WhenIndividualNotFound()
    {
        using var ctx = CreateContext();
        var service = new ContractService(ctx);

        var dto = CreateValidDto();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.AddContractAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task AddContractAsync_Throws_WhenSoftwareNotFound()
    {
        using var ctx = CreateContext();

        ctx.Inviduals.Add(new Invidual
        {
            Pesel = "12345678901",
            Entity = new Entity()
        });
        ctx.SaveChanges();

        var service = new ContractService(ctx);

        var dto = CreateValidDto();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.AddContractAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task AddContractAsync_Throws_WhenVersionsMissing()
    {
        using var ctx = CreateContext();

        ctx.Inviduals.Add(new Invidual { Pesel = "12345678901", Entity = new Entity() });
        ctx.Softwares.Add(new Software { Id = 1, Name = "Soft" });
        ctx.SaveChanges();

        var service = new ContractService(ctx);

        var dto = CreateValidDto();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.AddContractAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task AddContractAsync_AddsContractSuccessfully()
    {
        using var ctx = CreateContext();

        var entity = new Entity();
        ctx.Inviduals.Add(new Invidual { Pesel = "12345678901", Entity = entity });

        ctx.Softwares.Add(new Software { Id = 1, Name = "Soft" });

        ctx.Versions.Add(new Version { Id = 1, SoftwareId = 1 });

        ctx.BillingTypes.Add(new BillingType { Id = 1, Type = "License" });
        ctx.SoftwareCosts.Add(new SoftwareCost
        {
            SoftwareId = 1,
            BillingTypeId = 1,
            Price = 1000
        });

        ctx.SaveChanges();

        var service = new ContractService(ctx);

        var dto = CreateValidDto();

        var result = await service.AddContractAsync(dto, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Soft", result.SoftwareName);
    }
}
