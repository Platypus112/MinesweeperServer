using Microsoft.AspNetCore.Identity;
using MinesweeperServer.Models;

namespace MinesweeperServer.DTO
{
    public class UserDataDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string? PicPath { get; set; }

        public bool IsAdmin { get; set; }

        public string? Description { get; set; }

        public List<UserReportDTO> Reports { get; set; }

        //public List<GameDataDTO> Games { get; set; }

        public UserDataDTO() { }
        public UserDataDTO(string name_,string email_,string password_,string picPath_,bool isAdmin_,string description_, List<UserReportDTO> reports_/*, List<GameDataDTO> games_*/)
        {
            Name = name_;
            Email = email_;
            Password = password_;
            PicPath = picPath_;
            IsAdmin = isAdmin_;
            Description = description_;
            Reports = reports_;
            //Games = games_;
        }

        public UserDataDTO(Models.User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            Password = user.Password;
            IsAdmin = user.Admin;
            Description = user.Description;
            Reports = new List<UserReportDTO>();
            foreach (UserReport r in user.UserReports)
            {
                Reports.Add(new UserReportDTO(r));
            }
            //Games = new List<GameDataDTO>();
            //foreach (FinishedGame g in user.FinishedGames)
            //{
            //    Games.Add(new GameDataDTO(g));
            //}
        }

    }
}
