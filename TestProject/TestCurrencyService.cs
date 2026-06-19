using API.Services.BusinessServices;

public class TestCurrencyService : ICurrencyService
{
    private readonly decimal _rate;
    private readonly Exception? _exception;

    public TestCurrencyService(decimal rate)
    {
        _rate = rate;
        _exception = null;
    }

    public TestCurrencyService(Exception exception)
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