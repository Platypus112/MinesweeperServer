using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MinesweeperServer.Models;

public partial class Difficulty
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("height")]
    public int Height { get; set; }

    [Column("width")]
    public int Width { get; set; }

    [Column("bombs")]
    public int Bombs { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<FinishedGame> FinishedGames { get; set; } = new List<FinishedGame>();
}
