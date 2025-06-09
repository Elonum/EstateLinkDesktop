using System.ComponentModel.DataAnnotations;

namespace EstateLinkAvalonia.Models
{
    public class Realtor
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public string Patronymic { get; set; } = string.Empty;
        
        public decimal CommissionShare { get; set; } = 0;
    }
}