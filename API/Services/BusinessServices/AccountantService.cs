using System.Net;
using API.DTOs.BusinessDTOs;
using API.Exceptions;
using API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace API.Services.BusinessServices;

public class AccountantService(DatabaseContext ctx) : IAccountantService
{
    public async Task<OutRealIncome> GetIncomeAsync(int? softwareId,string? currencyCode, bool? expected, CancellationToken ct)
    {
        decimal sum = 0;
        if (softwareId is null)
        {
            if (expected is null || expected == false)
            {
                sum = await ctx.Contracts.Where(c => c.Signed).Select(c => c.Billing.License).Where(l => l != null)
                    .SumAsync(l => l.FinalPrice, ct);
            }
            else
            {
                sum = await ctx.Contracts.Select(c => c.Billing.License).Where(l => l != null)
                    .SumAsync(l => l.FinalPrice, ct);
            }
        }
        else
        {
            if (expected is null || expected == false)
            {
                sum = await ctx.Softwares.Where(s => s.Id == softwareId).SelectMany(s => s.Versions)
                    .SelectMany(v => v.AvailableVersions).Select(av => av.Contract).Distinct().Where(c => c.Signed)
                    .Select(c => c.Billing.License).Where(l => l != null).SumAsync(l => l.FinalPrice, ct);
            }
            else
            {
                sum = await ctx.Softwares.Where(s => s.Id == softwareId).SelectMany(s => s.Versions)
                    .SelectMany(v => v.AvailableVersions).Select(av => av.Contract).Distinct()
                    .Select(c => c.Billing.License).Where(l => l != null).SumAsync(l => l.FinalPrice, ct);
            }
        }

        if (currencyCode is not null && currencyCode != "PLN")
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
                return new OutRealIncome(sum / result.Rates.First().Mid, currencyCode);
            }
            else
            {
                throw new NotFoundException("Currency exchange rate not found");
            }
        }

        return new OutRealIncome(sum, "PLN");
    }
}