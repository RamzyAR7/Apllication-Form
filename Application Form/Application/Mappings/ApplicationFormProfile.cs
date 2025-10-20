using Application_Form.Application.DTOs;
using Application_Form.Domain.Entities;
using AutoMapper;

namespace Application_Form.Application.Mappings
{
    public class ApplicationFormProfile:Profile
    {
        public ApplicationFormProfile()
        {
            // Create / Update mapping (from DTO → Entity)
            CreateMap<CreateApplicationFormDto, ApplicationForm>()
                .ForMember(dest => dest.ApprovalStatus, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModified, opt => opt.Ignore())
                .ForMember(dest => dest.ApiKey, opt => opt.Ignore())
                .ForMember(dest => dest.ApiClientId, opt => opt.Ignore())
                .ForMember(dest => dest.ApiClientSecret, opt => opt.Ignore());

            // Response mapping (from Entity → Response DTO)
            CreateMap<ApplicationForm, ApplicationFormResponseDto>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client != null ? src.Client.Name : string.Empty))
                .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified.HasValue ? src.LastModified.Value : src.CreatedAt))
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate.HasValue ? src.ExpirationDate.Value : default));

            // List response mapping (Entity → List DTO) used for paginated list views
            CreateMap<ApplicationForm, ApplicationFormListResponseDto>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client != null ? src.Client.Name : string.Empty))
                .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified.HasValue ? src.LastModified.Value : src.CreatedAt))
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate.HasValue ? src.ExpirationDate.Value : default));

            // Update mapping from UpdateApplicationFormDto -> ApplicationForm (used for PATCH/PUT)
            CreateMap<UpdateApplicationFormDto, ApplicationForm>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovalStatus, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModified, opt => opt.Ignore())
                .ForMember(dest => dest.ApiKey, opt => opt.Ignore())
                .ForMember(dest => dest.ApiClientId, opt => opt.Ignore())
                .ForMember(dest => dest.ApiClientSecret, opt => opt.Ignore())
                .ForMember(dest => dest.AcceptTerms, opt => opt.Ignore());
        }
    }
}
