using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MinesweeperServer.Models;

public partial class UserReport
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("userId")]
    public int UserId { get; set; }

    [Column("statusId")]
    public int StatusId { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [ForeignKey("StatusId")]
    [InverseProperty("UserReports")]
    public virtual Status Status { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserReports")]
    public virtual User User { get; set; } = null!;
}
