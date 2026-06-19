using API.DTOs.BusinessDTOs;

namespace API.Services.BusinessServices;

public interface IContractService
{
    Task<OutContractDto> AddContractAsync(InContractDto inContractDto, CancellationToken ct);
    Task<OutPaymentDto> AddPaymentAsync(InPaymentDto inPaymentDto, CancellationToken ct);
}