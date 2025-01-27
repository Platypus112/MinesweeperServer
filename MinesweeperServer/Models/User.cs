using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MinesweeperServer.Models;

public partial class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [StringLength(30)]
    public string Name { get; set; } = null!;

    [StringLength(70)]
    public string Email { get; set; } = null!;

    [StringLength(30)]
    public string Password { get; set; } = null!;

    [StringLength(5)]
    public string? PicPath { get; set; }

    [StringLength(300)]
    public string? Description { get; set; }

    public bool Admin { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<FinishedGame> FinishedGames { get; set; } = new List<FinishedGame>();

    [InverseProperty("UserRecieving")]
    public virtual ICollection<FriendRequest> FriendRequestUserRecievings { get; set; } = new List<FriendRequest>();

    [InverseProperty("UserSending")]
    public virtual ICollection<FriendRequest> FriendRequestUserSendings { get; set; } = new List<FriendRequest>();

    [InverseProperty("User")]
    public virtual ICollection<UserReport> UserReports { get; set; } = new List<UserReport>();
}
