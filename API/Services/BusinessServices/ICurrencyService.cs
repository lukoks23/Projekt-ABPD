namespace API.Services.BusinessServices;

public interface ICurrencyService
{
    Task<decimal> GetCurrencyRateAsync(string currencyCode, CancellationToken ct);
}