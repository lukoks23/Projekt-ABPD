using System.Net;
using API.DTOs.BusinessDTOs;
using API.Exceptions;
using API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace API.Services.BusinessServices;

public class AccountantService(DatabaseContext ctx, ICurrencyService currencyService) : IAccountantService
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
            var val = currencyService.GetCurrencyRateAsync(currencyCode, ct);
            return new OutRealIncome(sum / await val, currencyCode);
        }

        return new OutRealIncome(sum, "PLN");
    }
}