using API.DTOs.BusinessDTOs;

namespace API.Services.BusinessServices;

public interface ICustomerService
{
    Task<OutCustomerDto> AddCustomerAsync(InCustomerDto inCustomerDto, CancellationToken ct);
    Task DeleteCustomerAsync(string pesel, CancellationToken ct);
    Task<OutCustomerDto> UpdateCustomerAsync(InUpdateCustomerDto inCustomerDto, CancellationToken ct);
}