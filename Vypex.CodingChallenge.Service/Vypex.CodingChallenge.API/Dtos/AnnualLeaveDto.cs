
namespace Vypex.CodingChallenge.API.Dtos {
    public class AnnualLeaveDto {
        public Guid Id {get; set; }
        public DateTime StartDate {get; set;}
        public DateTime EndDate {get; set;}

        public int LeaveDaysAllocated {get; set;}
    }
}