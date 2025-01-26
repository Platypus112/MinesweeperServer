using MinesweeperServer.Models;

namespace MinesweeperServer.DTO
{
    public class GameDataDTO
    {
        public DateTime? Date { get; set; }
        public double TimeInSeconds { get; set; }
        public DifficultyDTO Difficulty { get; set; }
        public UserDTO User { get; set; }
        //public List<GameReportDTO> Reports { get; set; }

        public GameDataDTO() { }

        public GameDataDTO(DateTime date_, double timeInSeconds_, DifficultyDTO difficulty_, UserDTO user_/*,List<GameReportDTO> reports_*/)
        {
            Date = date_;
            TimeInSeconds = timeInSeconds_;
            Difficulty = difficulty_;
            User = user_;
            //Reports = reports_;
        }

        public GameDataDTO(FinishedGame game)
        {
            Date = game.Date;
            TimeInSeconds = game.TimeInSeconds;
            Difficulty = new(game.Difficulty);
            User = new(game.User);
            //Reports = new List<GameReportDTO>();
            //foreach(GameReport r in game.GameReports)
            //{
            //    Reports.Add(new GameReportDTO(r));
            //}
        }
    }
}
