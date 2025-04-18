
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vypex.CodingChallenge.Domain.Models
{
    public class Employee
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;

        public List<EmployeeLeave> AllocatedLeave {get; set;} = [];

        /// <summary>
        /// A Calculated field. SQLLite doesn't support computed fields.
        /// Consider adding a count(Leave) in a query and adding this to the DTO only
        /// Seems like it is view only so dropping from database model
        /// </summary>
        // public int AccumulatedAnnualLeaveDays { get; set; }
    }
}
