using API.DTOs.BusinessDTOs;
using API.Entities.BusinessEntities;
using API.Exceptions;
using API.Services.BusinessServices;
using Xunit;

namespace API.Tests;

public class CustomerServiceDeleteCustTests : TestBase
{
    private InAddressDto CreateAddress() =>
        new("Poland", "Warsaw", "Main", 10, null, "00-001");

    private InInvidualDto CreateIndividual() =>
        new("12345678901", "Jan", "Kowalski", "jan@test.com", "123456789");

    private InCompanyDto CreateCompany() =>
        new("1234567890", "firma@test.com", "987654321");
    [Fact]
    public async Task DeleteCustomerAsync_Throws_WhenIndividualNotFound()
    {
        using var ctx = CreateContext();
        var service = new CustomerService(ctx);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.DeleteCustomerAsync("99999999999", CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCustomerAsync_ClearsIndividualData()
    {
        using var ctx = CreateContext();

        var indiv = new Invidual
        {
            Pesel = "12345678901",
            FirstName = "Jan",
            LastName = "Kowalski",
            Email = "jan@test.com",
            PhoneNumber = "123456789"
        };

        ctx.Inviduals.Add(indiv);
        ctx.SaveChanges();

        var service = new CustomerService(ctx);

        await service.DeleteCustomerAsync("12345678901", CancellationToken.None);

        var updated = ctx.Inviduals.First(i => i.Pesel == "12345678901");

        Assert.Equal("", updated.FirstName);
        Assert.Equal("", updated.LastName);
        Assert.Equal("", updated.Email);
        Assert.Equal("", updated.PhoneNumber);
    }
}