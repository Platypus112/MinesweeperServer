using Microsoft.AspNetCore.Components.Web;
using MinesweeperServer.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesweeperServer.DTO
{
    public class FriendRequestDTO
    {
        public LoginInfoDTO UserSending { get; set; }

        public LoginInfoDTO UserRecieving { get; set; }

        public StatusDTO Status { get; set; }

        public bool Recieving { get; set; }

        public FriendRequestDTO() { }
        public FriendRequestDTO(LoginInfoDTO userSending_,LoginInfoDTO userRecieving_,StatusDTO status_,bool recieving_)
        {
            UserSending = userSending_;
            UserRecieving = userRecieving_;
            Status = status_;
            Recieving = recieving_;
        }
        public FriendRequestDTO(FriendRequest friendRequest,string email)
        {
            UserSending=new(friendRequest.UserSending);
            UserRecieving=new(friendRequest.UserRecieving);
            Status = new(friendRequest.Status);
            Recieving=UserRecieving.Email==email;
        }
    }
}
