using System.ComponentModel.DataAnnotations;
using Unidemo.DTOdepartment;
using Unidemo.Models;

namespace Unidemo.DTOteacher
{
    public class TeacherDetailsDto 
    {
       
        public string Firstname { get; set; }

        public string Lastname { get; set; }
        public string Email { get; set; }
        public string DepartmentName { get; set; }



    }
}
