using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MinesweeperServer.Models;

public partial class Status
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [InverseProperty("Status")]
    public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();

    [InverseProperty("Status")]
    public virtual ICollection<GameReport> GameReports { get; set; } = new List<GameReport>();

    [InverseProperty("Status")]
    public virtual ICollection<UserReport> UserReports { get; set; } = new List<UserReport>();
}
