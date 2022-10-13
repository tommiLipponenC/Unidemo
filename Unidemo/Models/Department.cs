using Microsoft.Build.Framework;
using Unidemo.Data;

namespace Unidemo.Models
{
    public class Department
    {
        public Guid DepartmentId { get; set; }

        [Required]
        public string DepartmentName { get; set; }
        public List<ApplicationUser>? AppUsers { get; set; }
    }
}
