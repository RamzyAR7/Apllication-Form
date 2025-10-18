using FluentValidation;

namespace Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationById
{
    public class GetApplicationByIdValidator : AbstractValidator<GetApplicationByIdQuery>
    {
        public GetApplicationByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Application ID is required.");
        }
    }
}
