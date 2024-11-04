using System.ComponentModel.DataAnnotations;

namespace APISolution.Model
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string Department { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Salary must be greater than 0")]
        public decimal Salary { get; set; }
    }
}
