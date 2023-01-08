using AutoMapper;
using DatabaseService.Modules;

namespace WebService.Profiles
{
    public class AnnotationsProfile : Profile
    {
        public AnnotationsProfile()
        {
            CreateMap<Annotations, AnnotationsDto>();
        }
    }
}