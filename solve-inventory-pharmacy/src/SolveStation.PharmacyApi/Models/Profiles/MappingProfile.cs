using AutoMapper;
using SolveStation.Data.Models;
using SolveStation.PharmacyApi.Models.DTOs;

namespace SolveStation.PharmacyApi.Models.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => UserId.NewId()))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => new Email(src.Email)))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role)));

        // Drug mappings
        CreateMap<Drug, DrugDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.IsLowStock()))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired()));

        CreateMap<CreateDrugRequest, Drug>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => DrugId.NewId()));

        // Prescription mappings
        CreateMap<Prescription, PrescriptionDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId.Value))
            .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId.Value))
            .ForMember(dest => dest.FilledByPharmacistId, opt => opt.MapFrom(src => src.FilledByPharmacistId != null ? src.FilledByPharmacistId.Value : (Guid?)null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreatePrescriptionRequest, Prescription>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => PrescriptionId.NewId()))
            .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => new UserId(src.PatientId)))
            .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => new UserId(src.DoctorId)));

        // Prescription Item mappings
        CreateMap<PrescriptionItem, PrescriptionItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.DrugId, opt => opt.MapFrom(src => src.DrugId.Value));

        CreateMap<CreatePrescriptionItemRequest, PrescriptionItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => PrescriptionItemId.NewId()))
            .ForMember(dest => dest.DrugId, opt => opt.MapFrom(src => new DrugId(src.DrugId)));
    }
}
