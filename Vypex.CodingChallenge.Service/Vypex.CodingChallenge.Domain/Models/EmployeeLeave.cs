
using System.ComponentModel.DataAnnotations;

namespace Vypex.CodingChallenge.Domain.Models
{
    /// <summary>
    /// Not a necessary class for the purpose of the challenge.
    /// As we only need to manage the User's accumulated leave
    /// But leaving it in for reference as to design consderations.
    /// </summary>
    public class EmployeeLeave
    {
        [Key]
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Since business days and holidays may be part of the window
        /// We add a field for calculating leave days
        /// </summary>
        public int CalculatedLeaveDays { get; set; }

        public Employee Employee { get; } = new Employee();
    }
}
