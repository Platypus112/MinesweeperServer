using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MinesweeperServer.Models;

namespace MinesweeperServer.DTO
{
    public class StatusDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public StatusDTO() { }
        public StatusDTO(int id_, string name_)
        {
            Id=id_;
            Name=name_;
        }
        public StatusDTO(Status s)
        {
            Id =  s.Id;
            Name = s.Name;
        }
    }
}
