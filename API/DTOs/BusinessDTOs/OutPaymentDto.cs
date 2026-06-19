namespace API.DTOs.BusinessDTOs;

public record OutPaymentDto(
    decimal LeftToPay,
    bool Signed
    );