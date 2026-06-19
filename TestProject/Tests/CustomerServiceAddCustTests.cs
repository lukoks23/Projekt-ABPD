using API.DTOs.BusinessDTOs;
using API.Entities.BusinessEntities;
using API.Exceptions;
using API.Services.BusinessServices;
using Xunit;

namespace API.Tests;

public class CustomerServiceAddCustTests : TestBase
{
    private InAddressDto CreateAddress() =>
        new("Poland", "Warsaw", "Main", 10, null, "00-001");

    private InInvidualDto CreateIndividual() =>
        new("12345678901", "Jan", "Kowalski", "jan@test.com", "123456789");

    private InCompanyDto CreateCompany() =>
        new("1234567890", "firma@test.com", "987654321");

    [Fact]
    public async Task AddCustomerAsync_Throws_WhenBothCompanyAndIndividualProvided()
    {
        using var ctx = CreateContext();
        var service = new CustomerService(ctx);

        var dto = new InCustomerDto(CreateAddress(), CreateIndividual(), CreateCompany());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            service.AddCustomerAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task AddCustomerAsync_Throws_WhenNeitherCompanyNorIndividualProvided()
    {
        using var ctx = CreateContext();
        var service = new CustomerService(ctx);

        var dto = new InCustomerDto(CreateAddress(), null, null);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            service.AddCustomerAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task AddCustomerAsync_Throws_WhenCompanyAlreadyExists()
    {
        using var ctx = CreateContext();

        ctx.Companies.Add(new Company
        {
            Krs = "1234567890",
            Email = "old@test.com",
            PhoneNumber = "111111111"
        });
        ctx.SaveChanges();

        var service = new CustomerService(ctx);

        var dto = new InCustomerDto(CreateAddress(), null, CreateCompany());

        await Assert.ThrowsAsync<ConflictException>(() =>
            service.AddCustomerAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task AddCustomerAsync_Throws_WhenIndividualAlreadyExists()
    {
        using var ctx = CreateContext();

        ctx.Inviduals.Add(new Invidual
        {
            Pesel = "12345678901",
            FirstName = "Old",
            LastName = "User",
            Email = "old@test.com",
            PhoneNumber = "111111111"
        });
        ctx.SaveChanges();

        var service = new CustomerService(ctx);

        var dto = new InCustomerDto(CreateAddress(), CreateIndividual(), null);

        await Assert.ThrowsAsync<ConflictException>(() =>
            service.AddCustomerAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task AddCustomerAsync_AddsIndividualCustomerSuccessfully()
    {
        using var ctx = CreateContext();
        var service = new CustomerService(ctx);

        var dto = new InCustomerDto(CreateAddress(), CreateIndividual(), null);

        var result = await service.AddCustomerAsync(dto, CancellationToken.None);

        Assert.NotNull(result.Invidual);
        Assert.Equal("12345678901", result.Invidual.Pesel);
        Assert.Null(result.Company);
    }

    [Fact]
    public async Task AddCustomerAsync_AddsCompanyCustomerSuccessfully()
    {
        using var ctx = CreateContext();
        var service = new CustomerService(ctx);

        var dto = new InCustomerDto(CreateAddress(), null, CreateCompany());

        var result = await service.AddCustomerAsync(dto, CancellationToken.None);

        Assert.NotNull(result.Company);
        Assert.Equal("1234567890", result.Company.Krs);
        Assert.Null(result.Invidual);
    }
}