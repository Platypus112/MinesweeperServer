using MinesweeperServer.Models;

namespace MinesweeperServer.Models
{
    public partial class MinesweeperDbContext
    {
        public async Task<User?> GetUserByEmail(string email)
        {
            return await new Task<User?>(() => this.Users.Where(x=>x.Email==email).FirstOrDefault());
        }
        public async Task<User?> GetUserByName(string name)
        {
            return await new Task<User?>(()=>this.Users.Where(x => x.Name == name).FirstOrDefault());
        }
    }
}
