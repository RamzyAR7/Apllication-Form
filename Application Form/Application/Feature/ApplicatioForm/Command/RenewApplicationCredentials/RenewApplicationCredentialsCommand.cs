using Application_Form.Application.DTOs;
using Application_Form.Domain.Common;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationCredentials
{
    public class RenewApplicationCredentialsCommand : IRequest<Result<CustomEmptyResult>>
    {
        public Guid Id { get; set; }

        public RenewApplicationCredentialsCommand(Guid id)
        {
            Id = id;
        }
    }
}
