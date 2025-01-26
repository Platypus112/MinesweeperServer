using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MinesweeperServer.Models;

namespace MinesweeperServer.DTO
{
    public class GameReportDTO
    {
        public StatusDTO Status { get; set; }

        public string? Description { get; set; }

        public FinishedGameDTO Game { get; set; }

        public GameReportDTO() { }
        public GameReportDTO(StatusDTO status_,string description_,FinishedGameDTO game_)
        {
            Status = status_;
            Description = description_;
            Game = game_;
        }
        public GameReportDTO(GameReport report)
        {
            Status=new(report.Status);
            Description = report.Description;
            Game = new(report.Game);
        }
    }
}
