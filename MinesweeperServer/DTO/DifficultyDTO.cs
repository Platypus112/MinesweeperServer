using MinesweeperServer.Models;

namespace MinesweeperServer.DTO
{
    public class DifficultyDTO
    {
        public string Name { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Bombs { get; set; }
        public DifficultyDTO()
        {
            Name = "";
        }
        public DifficultyDTO(Difficulty difficulty)
        {
            Name= difficulty.Name;
            Height=difficulty.Height;
            Width=difficulty.Width;
            Bombs=difficulty.Bombs;
        }

        public DifficultyDTO(string name_,int height_, int width_,int bombs_)
        {
            Name = name_;
            Height = height_;
            Width = width_;
            Bombs = bombs_;
        }
    }
}
