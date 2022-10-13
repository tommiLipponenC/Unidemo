using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Unidemo.Data;
using Unidemo.DTO;
using Unidemo.DTOauthentication;
using Unidemo.DTOcourse;
using Unidemo.DTOdepartment;
using Unidemo.DTOteacher;
using Unidemo.DTOuser;
using Unidemo.Models;

namespace Unidemo.DTOmapper
{
    public class MapperInitializer : Profile
    {
        public MapperInitializer()
        {
            CreateMap<IdentityRole, RoleListDto>().ReverseMap();
            CreateMap<IdentityRole, EditRoleDto>().ReverseMap();
            CreateMap<Course, CourseDto>().ReverseMap();
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<Course, CreateCourseDto>().ReverseMap();
            CreateMap<Department, CreateDepartmentDto>().ReverseMap();
            CreateMap<Course, EditCourseDto>().ReverseMap();
            CreateMap<Department, EditDepartmentDto>().ReverseMap();
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
            CreateMap<ApplicationUser, UsersDepartmentDto>().ReverseMap();
            CreateMap<ApplicationUser, TeacherDetailsDto>().ReverseMap();
        }
    }
}
