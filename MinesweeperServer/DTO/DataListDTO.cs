using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MinesweeperServer.Models;

namespace MinesweeperServer.DTO
{
    public class DataListDTO
    {

        public string Name { get; set; }

        public bool Report { get; set; }

        public bool Games { get; set; }

        public bool User { get; set; }

        public bool AdminAccess { get; set; }

        public bool Personal { get; set; }

        public DataListDTO() { }

        public DataListDTO(string name_, bool report_, bool games_, bool user_, bool adminAccess_, bool personal)
        {
            Name = name_;
            Report = report_;
            Games = games_;
            User = user_;
            AdminAccess = adminAccess_;
            Personal = personal;
        }
        public DataListDTO(DataList dl)
        {
            Name = dl.Name;
            Report = dl.Report;
            Games = dl.Games;
            User = dl.User;
            AdminAccess = dl.AdminAccess;
            Personal = dl.Personal;
        }
    }
}
