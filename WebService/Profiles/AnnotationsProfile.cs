using AutoMapper;
using DatabaseService.Modules;
using WebService.DTOs;

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
