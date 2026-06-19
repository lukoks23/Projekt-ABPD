using API.DTOs.BusinessDTOs;
using API.Entities.BusinessEntities;
using API.Exceptions;
using API.Services.BusinessServices;
using Xunit;

namespace API.Tests;

public class CustomerServiceUpdateCustTests : TestBase
{
    private InAddressDto CreateAddress() =>
        new("Poland", "Warsaw", "Main", 10, null, "00-001");

    private InInvidualDto CreateIndividual() =>
        new("12345678901", "Jan", "Kowalski", "jan@test.com", "123456789");

    private InCompanyDto CreateCompany() =>
        new("1234567890", "firma@test.com", "987654321");
    [Fact]
    public async Task UpdateCustomerAsync_Throws_WhenBothCompanyAndIndividualProvided()
    {
        using var ctx = CreateContext();
        var service = new CustomerService(ctx);

        var dto = new InUpdateCustomerDto(
            null,
            new InUpdateInvidualDto("12345678901", null, null, null, null),
            new InUpdateCompanyDto("1234567890", null, null)
        );

        await Assert.ThrowsAsync<BadRequestException>(() =>
            service.UpdateCustomerAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateCustomerAsync_Throws_WhenNeitherProvided()
    {
        using var ctx = CreateContext();
        var service = new CustomerService(ctx);

        var dto = new InUpdateCustomerDto(null, null, null);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            service.UpdateCustomerAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateCustomerAsync_Throws_WhenIndividualNotFound()
    {
        using var ctx = CreateContext();
        var service = new CustomerService(ctx);

        var dto = new InUpdateCustomerDto(
            null,
            new InUpdateInvidualDto("12345678901", null, null, null, null),
            null
        );

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateCustomerAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateCustomerAsync_UpdatesIndividualSuccessfully()
    {
        using var ctx = CreateContext();

        var indiv = new Invidual
        {
            Pesel = "12345678901",
            FirstName = "Jan",
            LastName = "Kowalski",
            Email = "old@test.com",
            PhoneNumber = "111111111",
            Address = new Address
            {
                BuildingNumber = 10,
                PostCode = "00-001",
                Street = new Street
                {
                    Name = "Main",
                    City = new City
                    {
                        Name = "Warsaw",
                        Country = new Country { Name = "Poland" }
                    }
                }
            }
        };

        ctx.Inviduals.Add(indiv);
        ctx.SaveChanges();

        var service = new CustomerService(ctx);

        var dto = new InUpdateCustomerDto(
            null,
            new InUpdateInvidualDto("12345678901", "Adam", null, "new@test.com", null),
            null
        );

        var result = await service.UpdateCustomerAsync(dto, CancellationToken.None);

        Assert.Equal("Adam", result.Invidual.FirstName);
        Assert.Equal("new@test.com", result.Invidual.Email);
    }
}