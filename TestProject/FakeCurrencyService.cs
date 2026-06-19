using API.Services.BusinessServices;

public class FakeCurrencyService : ICurrencyService
{
    private readonly decimal _rate;
    private readonly Exception? _exception;

    public FakeCurrencyService(decimal rate)
    {
        _rate = rate;
        _exception = null;
    }

    public FakeCurrencyService(Exception exception)
    {
        _exception = exception;
    }

    public Task<decimal> GetCurrencyRateAsync(string currencyCode, CancellationToken ct)
    {
        if (_exception != null)
            throw _exception;

        return Task.FromResult(_rate);
    }
}