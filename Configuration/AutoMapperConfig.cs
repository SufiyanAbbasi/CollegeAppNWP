using AutoMapper;
using CollegeApp.Data;
using CollegeApp.Models;

namespace CollegeApp.Configuration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<StudentDTO, Student>().ReverseMap();
        }
    }
}
