using MinesweeperServer.Models;

namespace MinesweeperServer.Models
{
    public partial class MinesweeperDbContext
    {
        public User? GetUserByEmail(string email)
        {
            return this.Users.Where(x=>x.Email==email).FirstOrDefault();
        }
    }
}
