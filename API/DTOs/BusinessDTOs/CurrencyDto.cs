namespace API.DTOs.BusinessDTOs;

public record CurrencyDto(
    string Code,
    ICollection<RatesDto> Rates
    );