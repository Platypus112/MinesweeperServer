using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MinesweeperServer.Models;

public partial class DataList
{
    [Key]
    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("report")]
    public bool Report { get; set; }

    [Column("games")]
    public bool Games { get; set; }

    [Column("user")]
    public bool User { get; set; }

    [Column("personal")]
    public bool Personal { get; set; }

    [Column("adminAccess")]
    public bool AdminAccess { get; set; }
}
