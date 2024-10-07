using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MinesweeperServer.Models;

public partial class FinishedGame
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("userId")]
    public int UserId { get; set; }

    [Column("difficultyId")]
    public int DifficultyId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Date { get; set; }

    public int TimeInSeconds { get; set; }

    [InverseProperty("Game")]
    public virtual ICollection<GameReport> GameReports { get; set; } = new List<GameReport>();

    [ForeignKey("UserId")]
    [InverseProperty("FinishedGames")]
    public virtual Difficulty User { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("FinishedGames")]
    public virtual User UserNavigation { get; set; } = null!;
}
