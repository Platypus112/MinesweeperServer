using MinesweeperServer.Models;

namespace MinesweeperServer.DTO
{
    public class FinishedGameDTO
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public double TimeInSeconds { get; set; }
        public DifficultyDTO Difficulty { get; set; }
        public LoginInfoDTO User { get; set; }

        public FinishedGameDTO() { }

        public FinishedGameDTO(int id_,DateTime date_,double timeInSeconds_, DifficultyDTO difficulty_,LoginInfoDTO user_)
        {
            Id = id_;
            Date=date_;
            TimeInSeconds=timeInSeconds_;
            Difficulty = difficulty_;
            User = user_;
        }

        public FinishedGameDTO(FinishedGame game)
        {
            Id = game.Id;
            Date = game.Date;
            TimeInSeconds = game.TimeInSeconds;
            Difficulty = new(game.Difficulty);
            User = new(game.User);
        }
    }
}
