using FluentValidation;

namespace API.DTOs.BusinessDTOs;

public class ContractValidator : AbstractValidator<InContractDto>
{
    public ContractValidator()
    {
        RuleFor(x => x.DateFrom.AddDays(30)).GreaterThanOrEqualTo(x => x.DateTo).WithMessage("Contract time must be lower than or equal to 30 days");
        RuleFor(x => x.DateFrom.AddDays(3)).LessThanOrEqualTo(x => x.DateTo).WithMessage("Contract time must be higher than or equal to 3 days");
        RuleFor(x => x.YearsOfSupport).Must(v => new[] {1,2,3}.Contains(v)).WithMessage("Years of Support must be 1,2 or 3");
    }
}