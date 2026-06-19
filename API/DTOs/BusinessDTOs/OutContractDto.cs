namespace API.DTOs.BusinessDTOs;

public record OutContractDto(
    int Id,
    bool Signed,
    DateTime From,
    DateTime To,
    string SoftwareName,
    int YearsOfSupport,
    decimal Price
    );