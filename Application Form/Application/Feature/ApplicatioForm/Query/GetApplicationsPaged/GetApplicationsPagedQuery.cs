using Application_Form.Application.DTOs;
using Application_Form.Application.Models;
using Application_Form.Domain.Common;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPaged
{
    public class GetApplicationsPagedQuery: IRequest<Result<PaginatedList<ApplicationFormListResponseDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "desc";
        public string Status { get; set; } = "All";
    }
}
