using API.DTOs.BusinessDTOs;

namespace API.Services.BusinessServices;

public interface IAccountantService
{
    Task<OutRealIncome> GetIncomeAsync(int? softwareId, string? currencyCode, bool? expected, CancellationToken ct);
}