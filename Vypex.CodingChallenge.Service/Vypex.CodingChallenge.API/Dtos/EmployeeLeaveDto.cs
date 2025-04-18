namespace Vypex.CodingChallenge.API.Dtos {
    public class EmployeeLeaveDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int AccumulatedLeaveDays { get; set; }

        public List<AnnualLeaveDto> LeaveTaken { get; set; } = [];
    }
}