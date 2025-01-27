using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MinesweeperServer.Models;

public partial class FriendRequest
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("userSendingId")]
    public int UserSendingId { get; set; }

    [Column("userRecievingId")]
    public int UserRecievingId { get; set; }

    [Column("statusid")]
    public int Statusid { get; set; }

    [ForeignKey("Statusid")]
    [InverseProperty("FriendRequests")]
    public virtual Status Status { get; set; } = null!;

    [ForeignKey("UserRecievingId")]
    [InverseProperty("FriendRequestUserRecievings")]
    public virtual User UserRecieving { get; set; } = null!;

    [ForeignKey("UserSendingId")]
    [InverseProperty("FriendRequestUserSendings")]
    public virtual User UserSending { get; set; } = null!;
}
