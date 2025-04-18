namespace Vypex.CodingChallenge.API.Dtos {
    public class LeaveChangeRequestDto {
        public Guid? Id {get; set;}        
        public Guid EmployeeId {get; set;}
        public DateTime StartDate {get; set;}
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