using System.ComponentModel.DataAnnotations;

namespace DI.Contracts.Models.DTO
{
    public class UserDTO
    {
        [Required(ErrorMessage ="Name cannot be empty")]
        public string Name { get; set; }
    }
}
