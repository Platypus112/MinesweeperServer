using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MinesweeperServer.Models;

namespace MinesweeperServer.DTO
{
    public class GameReportDTO
    {
        public int Id {  get; set; }
        public StatusDTO Status { get; set; }

        public string? Description { get; set; }

        public FinishedGameDTO Game { get; set; }

        public GameReportDTO() { }
        public GameReportDTO(int id_, StatusDTO status_,string description_,FinishedGameDTO game_)
        {
            Id = id_;
            Status = status_;
            Description = description_;
            Game = game_;
        }
        public GameReportDTO(GameReport report)
        {
            Id= report.Id;
            Status=new(report.Status);
            Description = report.Description;
            Game = new(report.Game);
        }
    }
}
