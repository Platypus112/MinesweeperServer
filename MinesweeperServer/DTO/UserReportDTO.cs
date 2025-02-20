using MinesweeperServer.Models;

namespace MinesweeperServer.DTO
{
    public class UserReportDTO
    {
        public int Id { get; set; }

        public StatusDTO Status { get; set; }

        public string? Description { get; set; }

        public AppUserDTO User { get; set; }

        public UserReportDTO() { }
        public UserReportDTO(int id_, StatusDTO status_, string description_, AppUserDTO user_)
        {
            Id = id_;
            Status = status_;
            Description = description_;
            User = user_;
        }
        public UserReportDTO(UserReport report)
        {
            Id = report.Id;
            Status = new(report.Status);
            Description = report.Description;
            User = new(report.User);
        }
    }
}
