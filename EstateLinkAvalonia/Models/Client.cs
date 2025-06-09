using System.ComponentModel.DataAnnotations;

namespace EstateLinkAvalonia.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}