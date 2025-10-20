using Application_Form.Domain.Constant;
using FluentValidation;

namespace Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPaged
{
    public class GetApplicationsPagedValidator : AbstractValidator<GetApplicationsPagedQuery>
    {
        public GetApplicationsPagedValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

            RuleFor(x => x.SortOrder)
                .Must(o => o.ToLower() == "asc" || o.ToLower() == "desc")
                .WithMessage("SortOrder must be either 'asc' or 'desc'.");

            RuleFor(x => x.Status)
                .Must(s => new[] { nameof(Status.Approved), nameof(Status.Revoked), nameof(Status.Pending), nameof(Status.Rejected), nameof(Status.Expired), "All" }
                .Contains(s, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Invalid status filter.");

            RuleFor(x => x.SortBy)
                .Must(sb => new[] { "CreatedAt", "LastModified", "ApplicationName", "ApprovalStatus", "ExpirationDate"}
                .Contains(sb, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Invalid SortBy field.");
        }
    }
}
