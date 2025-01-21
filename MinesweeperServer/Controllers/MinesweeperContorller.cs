using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinesweeperServer.DTO;
using MinesweeperServer.Models;
using System.Net;

namespace MinesweeperServer.Controllers
{
    [Route("api")]
    [ApiController]
    public class MinesweeperContorller : ControllerBase
    {
        private IWebHostEnvironment webHostEnvironment;
        private MinesweeperDbContext context;

        public MinesweeperContorller(IWebHostEnvironment webHostEnvironment_, MinesweeperDbContext context_)
        {
            webHostEnvironment= webHostEnvironment_;
            context = context_;
        }

        [HttpPost("RecordGame")]
        public async Task<IActionResult> RecordGame([FromBody] FinishedGameDTO gameDTO)
        {
            try
            {
                if (context.GetUserByDTO(gameDTO.User) == null)
                {
                    return Conflict("User doesn't exist");
                }
                if (context.GetDifficultyByDTO(gameDTO.Difficulty) == null)
                {
                    return Conflict("Difficulty doesn't exist");
                }

                FinishedGame finished = new()
                {
                    Date = gameDTO.Date,
                    Difficulty = context.GetDifficultyByDTO(gameDTO.Difficulty),
                    User = context.GetUserByDTO(gameDTO.User),
                    TimeInSeconds = ((int)gameDTO.TimeInSeconds),
                };

                context.FinishedGames.Add(finished);
                context.SaveChanges();

                FinishedGameDTO toReturn = new(finished);

                return Ok(toReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDifficulties")]
        public async Task<IActionResult> GetDifficulties()
        {
            try
            {
                List<Difficulty> difficulties = await context.GetDifficultyList();

                if (difficulties==null||difficulties.Count <= 0)
                {
                    return NotFound("no difficulties found");
                }

                List<DifficultyDTO> resultList = new List<DifficultyDTO>();
                foreach (Difficulty d in difficulties)
                {
                    resultList.Add(new DifficultyDTO(d));
                }
                return Ok(resultList);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            try
            {
                if (await context.GetUserByEmail(userDTO.Email) != null || await context.GetUserByName(userDTO.Name) != null)
                {
                    return Conflict("this username or email have already been used");
                }

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

                HttpContext.Session.SetString("loggedUserEmail", userDTO.Email);

                AppUserDTO toReturn = new(user)
                {
                    PicPath = GetProfileImageVirtualPath(user.Id)
                };

                return Ok(toReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]UserDTO userDTO)
        {
            try
            {
                HttpContext.Session.Clear();
                User? user=null;

                if (!string.IsNullOrEmpty(userDTO.Email))
                    user = await context.GetUserByEmail(userDTO.Email);
                else if (!string.IsNullOrEmpty(userDTO.Name))
                    user = await context.GetUserByName(userDTO.Name);
                else return NoContent();
                if(string.IsNullOrEmpty(userDTO.Password)) return NoContent();

                if (user ==null)return NotFound("no user with this email or username");

                if(user.Password!=userDTO.Password) return Unauthorized("password is wrong");


                HttpContext.Session.SetString("loggedUserEmail", user.Email);

                AppUserDTO toReturn = new(user)
                {
                    PicPath = GetProfileImageVirtualPath(user.Id)
                };
                return Ok(toReturn);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("UploadProfileImage")]
        public async Task<IActionResult> UploadProfileImageAsync(IFormFile file)
        {
            //Check if who is logged in
            string? userEmail = HttpContext.Session.GetString("loggedInUser");
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("User is not logged in");
            }

            //Get model user class from DB with matching email. 
            Models.User? user = await context.GetUserByEmail(userEmail);
            //Clear the tracking of all objects to avoid double tracking
            context.ChangeTracker.Clear();

            if (user == null)
            {
                return Unauthorized("User is not found in the database");
            }


            //Read all files sent
            long imagesSize = 0;

            if (file.Length > 0)
            {
                //Check the file extention!
                string[] allowedExtentions = { ".png", ".jpg" };
                string extention = "";
                if (file.FileName.LastIndexOf(".") > 0)
                {
                    extention = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                }
                if (!allowedExtentions.Where(e => e == extention).Any())
                {
                    //Extention is not supported
                    return BadRequest("File sent with non supported extention");
                }

                //Build path in the web root (better to a specific folder under the web root
                string filePath = $"{this.webHostEnvironment.WebRootPath}\\profileImages\\{user.Id}{extention}";

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);

                    if (IsImage(stream))
                    {
                        imagesSize += stream.Length;
                    }
                    else
                    {
                        //Delete the file if it is not supported!
                        System.IO.File.Delete(filePath);
                    }

                }

            }
            AppUserDTO dtoUser = new AppUserDTO(user);
            dtoUser.PicPath = GetProfileImageVirtualPath(user.Id);
            return Ok(dtoUser);
        }
        private static bool IsImage(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            List<string> jpg = new List<string> { "FF", "D8" };
            List<string> bmp = new List<string> { "42", "4D" };
            List<string> gif = new List<string> { "47", "49", "46" };
            List<string> png = new List<string> { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" };
            List<List<string>> imgTypes = new List<List<string>> { jpg, bmp, gif, png };

            List<string> bytesIterated = new List<string>();

            for (int i = 0; i < 8; i++)
            {
                string bit = stream.ReadByte().ToString("X2");
                bytesIterated.Add(bit);

                bool isImage = imgTypes.Any(img => !img.Except(bytesIterated).Any());
                if (isImage)
                {
                    return true;
                }
            }

            return false;
        }
        private string GetProfileImageVirtualPath(int userId)
        {
            string virtualPath = $"/profileImages/{userId}";
            string path = $"{this.webHostEnvironment.WebRootPath}\\profileImages\\{userId}.png";
            if (System.IO.File.Exists(path))
            {
                virtualPath += ".png";
            }
            else
            {
                path = $"{this.webHostEnvironment.WebRootPath}\\profileImages\\{userId}.jpg";
                if (System.IO.File.Exists(path))
                {
                    virtualPath += ".jpg";
                }
                else
                {
                    virtualPath = $"/profileImages/default.png";
                }
            }

            return virtualPath;
        }
    }
}
