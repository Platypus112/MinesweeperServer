﻿using Microsoft.AspNetCore.Identity;

namespace MinesweeperServer.DTO
{
    public class AppUserDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string? PicPath { get; set; }

        public string? FullPicPath { get; set; }

        public bool IsAdmin { get; set; }

        public string? Description { get; set; }

        public AppUserDTO() { }
        public AppUserDTO(string name_,string email_,string password_,string picPath_,bool isAdmin_,string description_)
        {
            Name = name_;
            Email = email_;
            Password = password_;
            PicPath = picPath_;
            IsAdmin = isAdmin_;
            Description = description_;
            FullPicPath= picPath_;
        }

        public AppUserDTO(Models.User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            Password = user.Password;
            IsAdmin = user.Admin;
            Description = user.Description;
            PicPath= user.PicPath;
            FullPicPath= user.PicPath;
        }

    }
}
