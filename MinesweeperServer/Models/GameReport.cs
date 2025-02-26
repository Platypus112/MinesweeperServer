 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MinesweeperServer.Models;

public partial class GameReport
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("gameId")]
    public int GameId { get; set; }

    [Column("statusId")]
    public int StatusId { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [ForeignKey("GameId")]
    [InverseProperty("GameReports")]
    public virtual FinishedGame Game { get; set; } = null!;

    [ForeignKey("StatusId")]
    [InverseProperty("GameReports")]
    public virtual Status Status { get; set; } = null!;
}
