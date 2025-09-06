using System;
using MyProfessionalss.Data.Model.Enums;

namespace MyProfessionalss.Data.Model.DTO
{
    public class CompanyAdminDto
    {
        public int? Id { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public DateTime AssignedDate { get; set; }
        public CompanyEmployeeType EmployeeType { get; set; }

        // Display data 
        public string? UserFullName { get; set; }
        public string? CompanyName { get; set; }
    }
}
