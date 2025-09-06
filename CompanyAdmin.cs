using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyProfessionalss.Data.Model.Enums;

namespace MyProfessionalss.Data.Model
{
    public class CompanyAdmin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public CompanyEmployeeType EmployeeType { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
