using Microsoft.AspNetCore.Identity;

namespace MinesweeperServer.DTO
{
    public class AppUserDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string? PicPath { get; set; }

        public bool IsAdmin { get; set; }

        public AppUserDTO(string name_,string email_,string password_,string picPath_,bool isAdmin_)
        {
            Name = name_;
            Email = email_;
            Password = password_;
            PicPath = picPath_;
            IsAdmin = isAdmin_;
        }

        public AppUserDTO(Models.User user)
        {
            Name = user.Name;
            Email = user.Email;
            Password = user.Password;
            IsAdmin = user.Admin;
        }

    }
}
