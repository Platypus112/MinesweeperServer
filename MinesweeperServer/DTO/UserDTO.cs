using System.ComponentModel.DataAnnotations;
using MinesweeperServer.Models;

namespace MinesweeperServer.DTO
{
    public class UserDTO
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }


        public UserDTO() { }

        public UserDTO(string name_, string email_, string password_,string picPath_)
        {
            Name = name_;
            Email = email_;
            Password = password_;
        }
        public UserDTO(User user_)
        {
            Name = user_.Name;
            Email = user_.Email;
            Password = user_.Password;
        }
    }
}
