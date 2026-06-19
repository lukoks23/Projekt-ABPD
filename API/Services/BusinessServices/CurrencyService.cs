using System.Net;
using API.DTOs.BusinessDTOs;
using API.Exceptions;

namespace API.Services.BusinessServices;

public class CurrencyService : ICurrencyService
{
    public async Task<decimal> GetCurrencyRateAsync(string currencyCode, CancellationToken ct)
    {
        using var client = new HttpClient();

        var response = await client.GetAsync($"http://api.nbp.pl/api/exchangerates/rates/a/{currencyCode}?format=json",ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            response = await client.GetAsync($"http://api.nbp.pl/api/exchangerates/rates/b/{currencyCode}?format=json",ct);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException("Currency code not found");
            }
        }
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<CurrencyDto>(ct);
        if (result is not null)
        {
                
            return result.Rates.First().Mid;
        }
        else
        {
            throw new NotFoundException("Currency exchange rate not found");
        }
    }
}