using MinesweeperServer.DTO;
using MinesweeperServer.Models;

namespace MinesweeperServer.Models
{
    public partial class MinesweeperDbContext
    {
        public async Task<User?> GetUserByEmail(string email)
        {
            return this.Users.FirstOrDefault(x => x.Email == email);
        }
        public async Task<User?> GetUserByName(string name)
        {
            return this.Users.FirstOrDefault(x => x.Name == name);
        }

        public async Task<List<Difficulty>> GetDifficultyList()
        {
            return this.Difficulties.Where(x=>x!=null).ToList();
        }
        public  Difficulty GetDifficultyByDTO(DifficultyDTO dto)
        {
            return this.Difficulties.FirstOrDefault(x => x.Name == dto.Name);
        }
        public User GetUserByDTO(UserDTO dto)
        {
            return this.Users.FirstOrDefault(x => x.Name == dto.Name);
        }
    }
}
