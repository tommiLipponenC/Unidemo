using System.ComponentModel.DataAnnotations;

namespace Unidemo.DTOgrade
{
    public class EditGradeDto
    {
    

        [Required]
        [Range(1, 10)]
        public int? GradeValue { get; set; }
      
  
    }
}
