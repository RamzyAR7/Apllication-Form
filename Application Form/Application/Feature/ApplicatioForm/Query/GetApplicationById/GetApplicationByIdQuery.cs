using Application_Form.Application.DTOs;
using Application_Form.Domain.Common;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationById
{
    public class GetApplicationByIdQuery: IRequest<Result<ApplicationFormResponseDto>>
    {
        public long Id { get; set; }
        public GetApplicationByIdQuery(long id)
        {
            Id = id;
        }
    }
}
