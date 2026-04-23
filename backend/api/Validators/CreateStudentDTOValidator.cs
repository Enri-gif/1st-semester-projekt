using api.DTOs;
using FluentValidation;

namespace api.Validators;

public class CreateStudentDTOValidator : AbstractValidator<CreateStudentDTO>
{
    public CreateStudentDTOValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Phone).MaximumLength(40);
        RuleFor(x => x.BirthDate).LessThan(DateTime.Today).When(x => x.BirthDate.HasValue);
    }
}

public class CreateAssignmentSheetDTOValidator : AbstractValidator<CreateAssignmentSheetDTO>
{
    public CreateAssignmentSheetDTOValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Year).InclusiveBetween(1900, 2100);
    }
}

public class UpdateAssignmentSheetDTOValidator : AbstractValidator<UpdateAssignmentSheetDTO>
{
    public UpdateAssignmentSheetDTOValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Year).InclusiveBetween(1900, 2100);
    }
}

public class CreateAssignmentDTOValidator : AbstractValidator<CreateAssignmentDTO>
{
    public CreateAssignmentDTOValidator()
    {
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Year).InclusiveBetween(1900, 2100);
        RuleFor(x => x.Number).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Points).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Subtest).GreaterThanOrEqualTo(0);
    }
}
