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

    public byte[]? Pic { get; set; }

    [StringLength(300)]
    public string? Description { get; set; }

    public bool Admin { get; set; }

    [InverseProperty("UserNavigation")]
    public virtual ICollection<FinishedGame> FinishedGames { get; set; } = new List<FinishedGame>();
}
