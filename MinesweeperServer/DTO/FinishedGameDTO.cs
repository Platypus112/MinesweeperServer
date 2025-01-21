using MinesweeperServer.Models;

namespace MinesweeperServer.DTO
{
    public class FinishedGameDTO
    {
        public DateTime? Date { get; set; }
        public double TimeInSeconds { get; set; }
        public DifficultyDTO Difficulty { get; set; }
        public UserDTO User { get; set; }

        public FinishedGameDTO() { }

        public FinishedGameDTO(DateTime date_,double timeInSeconds_, DifficultyDTO difficulty_,UserDTO user_)
        {
            Date=date_;
            TimeInSeconds=timeInSeconds_;
            Difficulty = difficulty_;
            User = user_;
        }

        public FinishedGameDTO(FinishedGame game)
        {
            Date = game.Date;
            TimeInSeconds = game.TimeInSeconds;
            Difficulty = new(game.Difficulty);
            User = new(game.User);
        }
    }
}
