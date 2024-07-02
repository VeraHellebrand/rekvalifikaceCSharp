using AutoMapper;
using RekvalifikaceApp.Dtos;
using RekvalifikaceApp.Models;
using RekvalifikaceApp.ViewModels;

namespace RekvalifikaceApp.Mappings
{
    /// <summary>
    /// Konfigurační třída pro AutoMapper, která definuje mapování mezi třídami:
    /// </summary>

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, StudentDto>().ReverseMap();

            CreateMap<Teacher, TeacherDto>().ReverseMap();


            CreateMap<Subject, SubjectDto>()
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.Teacher.Id))
                .ReverseMap();

            CreateMap<Teacher, TeacherSelectItem>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<Subject, SubjectViewModel>()
               .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => $"{src.Teacher.LastName} {src.Teacher.FirstName}"));


            CreateMap<Evaluation, EvaluationDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Student.Id))
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Subject.Id))
                .ForMember(dest => dest.CompletionDate, opt => opt.MapFrom(src => src.CompletionDate == default ? DateOnly.FromDateTime(DateTime.Now) : src.CompletionDate))
                .ReverseMap();

            CreateMap<Student, StudentSelectItem>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<Subject, SubjectSelectItem>();

            CreateMap<Evaluation, EvaluationViewModel>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"))
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.Name));

            CreateMap<AppUser, CreateUserViewModel>();

        }
    }
}

