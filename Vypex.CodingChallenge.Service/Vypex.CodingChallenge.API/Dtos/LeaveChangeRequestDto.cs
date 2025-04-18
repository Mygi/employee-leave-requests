using System.ComponentModel.DataAnnotations;

namespace Vypex.CodingChallenge.API.Dtos {
    public class LeaveChangeRequestDto {
        public Guid? Id {get; set;}     
        
        [Required]   
        public Guid EmployeeId {get; set;}
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate {get; set;}
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate {get; set;}
        
    }
    public class LeaveChangeResponseDto {
        public Guid LeaveId {get; set;}
        public Guid EmployeeId {get; set;}
        public DateTime StartDate {get; set;}
        public DateTime EndDate {get; set;}
        // Check if needed
        public int LeaveDaysTaken {get; set;}
        public int UpdatedEmployeAnnualLeave {get; set;}
        
    }
}