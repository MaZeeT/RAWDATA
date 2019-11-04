using AutoMapper;
using DatabaseService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebService.Models;

namespace WebService.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Questions, CategoryDto>();
            CreateMap<CategoryForCreation, Questions>();
        }
    }
}
