using System.ComponentModel.DataAnnotations;

namespace EstateLinkWpf.Models
{
    public class Realtor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Patronymic { get; set; }
        public decimal CommissionShare { get; set; } = 0;
    }
}