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
        // Check if needed
        public int LeaveDaysTaken {get; set;}
        public bool Allowed {get; set;}
        public string Reason {get; set;} = "";
        
    }
}