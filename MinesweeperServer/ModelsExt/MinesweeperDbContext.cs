using Microsoft.EntityFrameworkCore;
using MinesweeperServer.DTO;
using MinesweeperServer.Models;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;

namespace MinesweeperServer.Models
{
    public partial class MinesweeperDbContext
    {
        public async Task<GameReport> GetGameReportById(int id)
        {
            return this.GameReports.Include(r=>r.Status).Include(r=>r.Game).ThenInclude(g=>g.User).Include(r => r.Game).ThenInclude(g => g.Difficulty).First(r=> r.Id == id);
        }
        public async Task<FinishedGame> GetGameById(int id)
        {
            return this.FinishedGames.First(x=>x.Id == id);
        }
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
        public async Task<Difficulty> GetDifficultyByDTO(DifficultyDTO dto)
        {
            return this.Difficulties.FirstOrDefault(x => x.Name == dto.Name);
        }
        public async Task<User> GetUserByDTO(UserDTO dto)
        {
            return this.Users.FirstOrDefault(x => x.Name == dto.Name);
        }
        public async Task<List<FinishedGame>> GetAllGamesWithData()
        {
            return this.FinishedGames.Include(g => g.User).Include(g=>g.GameReports).ThenInclude(r=>r.Status).Include(g=>g.Difficulty).ToList();
        }
        public async Task<List<User>> GetAllUsersWithData()
        {
            return this.Users.Include(u=>u.UserReports).ThenInclude(r => r.Status).Include(u=>u.FinishedGames).ToList();
        }
        public async Task<List<UserReport>> GetAllUserReports()
        {
            return this.UserReports.Include(r=>r.User).Include(r=>r.Status).ToList();
        }
        public async Task<List<GameReport>> GetAllGameReports()
        {
            return this.GameReports.Include(r => r.Game).Include(r => r.Status).ToList();
        }
        public async Task<List<User>> GetAllUsers()
        {
            return this.Users.ToList();
        }
        public async Task<List<User>> GetAllFriendUsersByEmail(string email)
        {
            User u= this.Users.Include(u => u.FriendRequestUserSendings).ThenInclude(f => f.Status).Include(u => u.FriendRequestUserSendings).ThenInclude(f => f.UserRecieving).FirstOrDefault(x => x.Email == email);
            return u.FriendRequestUserSendings.Where(f=>f.Status.Name=="approved").Select(f=>f.UserRecieving).ToList();
        }
        public async Task<List<FinishedGame>> GetAllFriendGamesByEmail(string email)
        {
            return this.FinishedGames.Where(u =>
            u.User.FriendRequestUserSendings.Any(f=> f.Status.Name=="approved"&&f.UserRecieving.Email==email))
                .Include(g => g.User).Include(g => g.GameReports).ThenInclude(r=>r.Status).Include(g => g.Difficulty).ToList();
        }
        public async Task<List<FriendRequest>> GetAllFriendsRequestsByEmail(string email)
        {
            return this.FriendRequests.Where(f=>
                f.UserRecieving.Email==email||f.UserSending.Email==email)
                .Include(f=>f.UserRecieving).Include(f=>f.UserSending)
                .Include(f=>f.Status).ToList();
        }
    }
}
