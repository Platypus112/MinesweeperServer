using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinesweeperServer.DTO;
using MinesweeperServer.Models;
using System.Net;

namespace MinesweeperServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MinesweeperContorller : ControllerBase
    {
        private IWebHostEnvironment webHostEnvironment;
        private MinesweeperDbContext context;

        public MinesweeperContorller(IWebHostEnvironment webHostEnvironment_/*,TasksManagementDbContext context_*/)
        {
            webHostEnvironment= webHostEnvironment_;
            //context= context_;
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserDTO userDTO)
        {
            try
            {
                HttpContext.Session.Clear();
                User user = new User()
                {
                    Name = userDTO.Name,
                    Email = userDTO.Email,
                    Password = userDTO.Password,
                    Admin = false,
                };
                context.Users.Add(user);
                context.SaveChanges();

                UserDTO toReturn = new UserDTO(user);

                return Ok("succussful reigster");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
