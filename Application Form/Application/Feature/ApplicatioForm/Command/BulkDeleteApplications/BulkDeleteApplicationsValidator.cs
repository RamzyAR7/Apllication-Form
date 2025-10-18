using FluentValidation;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.BulkDeleteApplications
{
    public class BulkDeleteApplicationsValidator : AbstractValidator<BulkDeleteApplicationsCommand>
    {
        public BulkDeleteApplicationsValidator()
        {
            RuleFor(x => x.Ids).NotNull().WithMessage("Ids are required.");
            RuleFor(x => x.Ids).Must(ids => ids != null && ids.Any()).WithMessage("At least one id must be provided.");
        }
    }
}
