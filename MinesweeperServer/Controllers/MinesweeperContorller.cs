using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                return Ok("Session cleared");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetUserGames")]
        public async Task<IActionResult> GetUserGames([FromQuery]string email)
        {
            try
            {
                string emailLogged = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(emailLogged))
                {
                    return Unauthorized("User must be logged to get games");
                }
                List<FinishedGame> finishedGames = await context.GetAllGamesByEmail(email);
                List<GameDataDTO> games = new();
                foreach (FinishedGame toAdd in finishedGames)
                {
                    games.Add(new(toAdd));
                }
                return Ok(games);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AcceptUserReport")]
        public async Task<IActionResult> AcceptUserReport([FromBody] UserReportDTO r)
        {
            try
            {
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to accept report");
                }
                else if (!(await context.GetUserByEmail(email)).Admin)
                {
                    return Unauthorized("Cannot accept report without being an admin");
                }
                UserReport report = await context.GetUserReportById(r.Id);
                if (report == null)
                {
                    return NotFound("No report found with corrosponding id");
                }
                report.StatusId = 2;
                context.SaveChanges();
                return Ok("succussful");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AbsolveUserReport")]
        public async Task<IActionResult> AbsolveUserReport([FromBody] UserReportDTO r)
        {
            try
            {
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to absolve report");
                }
                else if (!(await context.GetUserByEmail(email)).Admin)
                {
                    return Unauthorized("Cannot absolve report without being an admin");
                }
                UserReport report = await context.GetUserReportById(r.Id);
                if (report == null)
                {
                    return NotFound("No report found with corrosponding id");
                }
                report.StatusId = 3;
                context.SaveChanges();
                return Ok("succussful");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RemoveUserReport")]
        public async Task<IActionResult> RemoveUserReport([FromBody]UserReportDTO r)
        {
            try
            {
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to remove report");
                }
                else if (!(await context.GetUserByEmail(email)).Admin)
                {
                    return Unauthorized("Cannot remove report without being an admin");
                }
                UserReport report = await context.GetUserReportById(r.Id);
                if (report == null)
                {
                    return NotFound("No report found with corrosponding id");
                }
                context.UserReports.Remove(report);
                context.SaveChanges();
                return Ok("succussful");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RemoveUser")]
        public async Task<IActionResult> RemoveUser([FromBody]AppUserDTO u)
        {
            try
            {
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to remove game");
                }
                else if (!(await context.GetUserByEmail(email)).Admin)
                {
                    return Unauthorized("Cannot remove game without being an admin");
                }
                User user = await context.GetUserById(u.Id.Value);
                if (user == null)
                {
                    return NotFound("No user found with corrosponding id");
                }
                foreach (UserReport report in context.UserReports.Where(r=>r.UserId == user.Id))
                {
                    context.UserReports.Remove(report);
                }
                foreach(FriendRequest request in context.FriendRequests.Where(f => f.UserRecievingId == user.Id || f.UserSendingId == user.Id))
                {
                    context.FriendRequests.Remove(request);
                }
                List<FinishedGame> finishedGames = context.FinishedGames.Where(g => g.UserId == user.Id).ToList();
                foreach (FinishedGame game in finishedGames)
                {
                    context.GameReports.Where(r=>r.GameId==game.Id).ExecuteDelete();
                    context.FinishedGames.Remove(game);
                }
                context.Remove(user);
                context.SaveChanges();
                return Ok("succussful");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ReportUser")]
        public async Task<IActionResult> ReportUser([FromBody]UserReportDTO userReportDTO)
        {
            try
            {
                User user = await context.GetUserById(userReportDTO.User.Id.Value);
                if (user == null)
                {
                    return Conflict("User doesn't exist");
                }
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to report game");
                }

                UserReport report = new()
                {
                    StatusId = 1,
                    UserId = user.Id,
                    Description=userReportDTO.Description,
                };

                context.UserReports.Add(report);
                context.SaveChanges();

                UserReportDTO toReturn = new(await context.GetUserReportById(report.Id));

                return Ok(toReturn);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetRecords")]
        public async Task<IActionResult> GetRecords([FromQuery]string email) 
        {
            try
            {
                string emailLogged = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(emailLogged))
                {
                    return Unauthorized("User must be logged to get records");
                }
                List<FinishedGame> finishedGames = await context.GetAllGamesByEmail(email);
                List<GameDataDTO> games = new();
                for(int i = 1;i <= 5; i++)
                {
                    FinishedGame toAdd = finishedGames.Where(g => g.Difficulty.Id == i&&g.GameReports.Count(r=>r.StatusId==2)==0).MaxBy(g=>g.TimeInSeconds);
                    if(toAdd != null)
                    {
                        GameDataDTO toAddDTO = new(toAdd);
                        games.Add(toAddDTO);
                    }
                }
                return Ok(games);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("EditUser")]
        public async Task<IActionResult> EditUser([FromBody]AppUserDTO user)
        {
            try
            {
                string? email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to edit user");
                }
                User logged = await context.GetUserByEmail(email);
                if (logged.Email!=user.Email)
                {
                    return Conflict("Logged user can't edit a different user's details");
                }

                if (logged.Name!=user.Name&&await context.GetUserByName(user.Name) != null)
                {
                    return Conflict("This username has already been used");
                }
                logged.Name = user.Name;
                logged.Password = user.Password;
                logged.Description= user.Description;
                context.SaveChanges();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UnblockUser")]
        public async Task<IActionResult> UnblockUser([FromBody]AppUserDTO user)
        {
            try
            {
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to unblock user");
                }
                User logged=await context.GetUserByEmail(email);
                if(!(await context.CheckIfUserIsBlockedByName(logged.Name,user.Name)))
                {
                    return Conflict("Logged user isn't blocking given user");
                }
                FriendRequest blockRequest = await context.GetFriendRequestByNameDTO(new FriendRequestDTO()
                {
                    UserSending=new()
                    {
                        Name= logged.Name
                    },
                    UserRecieving = new()
                    {
                        Name=user.Name
                    },
                });
                context.FriendRequests.Remove(blockRequest);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("RemoveFriend")]
        public async Task<IActionResult> RemoveFriend([FromBody]LoginInfoDTO user)
        {
            try
            {
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to block friend reuqest");
                }
                User loggedUser = await context.GetUserByEmail(email);
                User sadUser = await context.GetUserByName(user.Name);
                if (user == null||sadUser==null)
                {
                    return BadRequest("No user sent to remove friend status");
                }
                if(!await context.CheckIfFriendsByEmail(loggedUser.Email, sadUser.Email))
                {
                    return Conflict("User isn't logged user's friend");
                }

                await context.RemoveFriendRequestFromUserToUserByEmail(loggedUser.Email, sadUser.Email);
                context.SaveChanges();

                return Ok(new UserDataDTO(sadUser));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("BlockUser")]
        public async Task<IActionResult> BlockUser([FromBody]LoginInfoDTO user)
        {
            try
            {
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to block friend reuqest");
                }
                User loggedUser=await context.GetUserByEmail(email);
                User blockedUser=await context.GetUserByName(user.Name);
                if (user == null||blockedUser==null)
                {
                    return BadRequest("No user sent to block");
                }
                else if (loggedUser.Email==blockedUser.Email)
                {
                    return Conflict("Cannot block yourself");
                }
                await context.RemoveFriendRequestFromUserToUserByEmail(loggedUser.Email, blockedUser.Email);
                FriendRequest blockRequest = new()
                {
                    UserRecievingId = blockedUser.Id,
                    UserSendingId = loggedUser.Id,
                    Statusid = 3,
                };
                context.FriendRequests.Add(blockRequest);
                context.SaveChanges();

                return Ok(new AppUserDTO(blockedUser));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("DeclineFriendRequest")]
        public async Task<IActionResult> DeclineFriendRequest([FromBody]FriendRequestDTO requestDTO)
        {
            try
            {
                if (requestDTO == null || requestDTO.UserSending == null || requestDTO.UserRecieving == null)
                {
                    return BadRequest("No request sent");
                }
                User recieving = await context.GetUserByName(requestDTO.UserRecieving.Name);
                if (recieving == null)
                {
                    return NotFound("No recieving user found");
                }
                User sending = await context.GetUserByName(requestDTO.UserSending.Name);
                if (sending == null)
                {
                    return NotFound("No sending user found");
                }
                FriendRequest request = await context.GetFriendRequestByNameDTO(requestDTO);
                if (request == null)
                {
                    return NotFound("Request doesn't exist");
                }
                if (request.Status.Name != "pending")
                {
                    return Conflict("Request isn't pending");
                }
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to decline friend reuqest");
                }
                else if ((await context.GetUserByEmail(email)).Id != recieving.Id)
                {
                    return Unauthorized("Cannot decline friend request for a different user than the one logged in");
                }
                context.FriendRequests.Remove(request);
                context.SaveChanges();

                return Ok(new AppUserDTO(sending));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AcceptFriendRequest")]
        public async Task<IActionResult> AcceptFriendRequest([FromBody]FriendRequestDTO requestDTO)
        {
            try
            {
                if (requestDTO == null || requestDTO.UserSending == null || requestDTO.UserRecieving == null)
                {
                    return BadRequest("No request sent");
                }
                User recieving = await context.GetUserByName(requestDTO.UserRecieving.Name);
                if (recieving == null)
                {
                    return NotFound("No recieving user found");
                }
                User sending = await context.GetUserByName(requestDTO.UserSending.Name);
                if (sending == null)
                {
                    return NotFound("No sending user found");
                }
                FriendRequest request = await context.GetFriendRequestByNameDTO(requestDTO);
                if(request == null)
                {
                    return NotFound("Request doesn't exist");
                }
                if (request.Status.Name!="pending")
                {
                    return Conflict("Request isn't pending");
                }
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to accept friend reuqest");
                }
                User logged = await context.GetUserByEmail(email);
                if (logged.Id != recieving.Id)
                {
                    return Unauthorized("cannot accept friend request for a different user than the one logged in");
                }
                request.Statusid = 2;
                FriendRequest other = new()
                {
                    UserRecievingId = request.UserSendingId,
                    UserSendingId = request.UserRecievingId,
                    Statusid = 2,
                };
                context.FriendRequests.Add(other);
                context.SaveChanges();

                return Ok(new AppUserDTO(sending));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SendFriendRequest")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestDTO request)
        {
            try
            {
                if (request == null|| request.UserSending == null|| request.UserRecieving == null)
                {
                    return BadRequest("No request sent");
                }
                User recieving = await context.GetUserByName(request.UserRecieving.Name);
                if (recieving == null)
                {
                    return NotFound("No recieving user found");
                }
                User sending = await context.GetUserByName(request.UserSending.Name);
                if (sending == null)
                {
                    return NotFound("No sending user found");
                }
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to send friend reuqest");
                }
                else if ((await context.GetUserByEmail(email)).Id!=sending.Id)
                {
                    return Unauthorized("Cannot send friend request from a different user than the one logged in");
                }
                if((await context.CheckIfUserIsBlockedByName(recieving.Name, sending.Name)))
                {
                    return Unauthorized("User had been blocked by recieving user");//users who have approved reports won't be able to add new friends or be shown on the leaderboard
                }
                if (sending.UserReports.Where(r => r.Status.Name == "Approved").Count() > 0)
                {
                    return Unauthorized("Can't make friends while being a criminal");
                }
                FriendRequest check = await context.GetFriendRequestByNameDTO(request);
                if (check != null)
                {
                    if (check.Statusid == 2) return Conflict("Users are already friends");
                    else if (check.Statusid == 3) return Unauthorized("User had blocked recieving user");
                    else return Conflict("Request already sent");
                }
                check=await context.GetFriendRequestByNameDTO(new FriendRequestDTO()
                {
                    UserRecieving=request.UserSending,
                    UserSending=request.UserRecieving,
                    Status=request.Status,
                });
                if(check != null)
                {
                    if (check.Statusid == 1) return Conflict("User has already sent you friend request");
                }

                FriendRequest addRequest = new()
                {
                    UserRecievingId = recieving.Id,
                    UserSendingId= sending.Id,
                    Statusid=1,
                };
                context.Add(addRequest);
                context.SaveChanges();
                return Ok("Friend request sent successfuly");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AcceptGameReport")]
        public async Task<IActionResult> AcceptGameReport([FromBody]GameReportDTO r)
        {
            try
            {
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to accept report");
                }
                else if (!(await context.GetUserByEmail(email)).Admin)
                {
                    return Unauthorized("Cannot accept report without being an admin");
                }
                GameReport report = await context.GetGameReportById(r.Id);
                if (report == null)
                {
                    return NotFound("No report found with corrosponding id");
                }
                report.StatusId = 2;
                context.SaveChanges();
                return Ok("succussful");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AbsolveGameReport")]
        public async Task<IActionResult> AbsolveGameReport([FromBody] GameReportDTO r)
        {
            try
            {
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))   
                {
                    return Unauthorized("User must be logged to absolve report");
                }
                else if (!(await context.GetUserByEmail(email)).Admin)
                {
                    return Unauthorized("Cannot absolve report without being an admin");
                }
                GameReport report = await context.GetGameReportById(r.Id);
                if (report == null)
                {
                    return NotFound("No report found with corrosponding id");
                }
                report.StatusId = 3;
                context.SaveChanges();
                return Ok("succussful");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RemoveGameReport")]
        public async Task<IActionResult> RemoveGameReport([FromBody] GameReportDTO r)
        {
            try
            {
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to remove report");
                }
                else if (!(await context.GetUserByEmail(email)).Admin)
                {
                    return Unauthorized("Cannot remove report without being an admin");
                }
                GameReport report = await context.GetGameReportById(r.Id);
                if (report == null)
                {
                    return NotFound("No report found with corrosponding id");
                }
                context.GameReports.Remove(report);
                context.SaveChanges();
                return Ok("succussful");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RemoveGame")]
        public async Task<IActionResult> RemoveGame([FromBody]GameDataDTO g)
        {
            try
            {
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to remove game");
                }
                else if (!(await context.GetUserByEmail(email)).Admin)
                {
                    return Unauthorized("Cannot remove game without being an admin");
                }
                FinishedGame game = await context.GetGameById(g.Id);
                if(game == null)
                {
                    return NotFound("No game found with corrosponding id");
                }
                foreach(GameReport report in context.GameReports)
                {
                    if(report.GameId==game.Id) context.GameReports.Remove(report);
                }
                context.FinishedGames.Remove(game);
                context.SaveChanges();
                return Ok("succussful");
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ReportGame")]
        public async Task<IActionResult> ReportGame([FromBody]GameReportDTO gameReportDTO)
        {
            try
            {
                FinishedGame game = await context.GetGameById(gameReportDTO.Game.Id);
                if (game == null)
                {
                    return Conflict("Game doesn't exist");
                }
                string email = HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to report game");
                }

                GameReport report = new()
                {
                    StatusId=1,
                    Description=gameReportDTO.Description,
                    GameId=game.Id,
                };

                context.GameReports.Add(report);
                context.SaveChanges();

                GameReportDTO toReturn = new(await context.GetGameReportById(report.Id));

                return Ok(toReturn);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCollection")]
        #region GetCollection
        public async Task<IActionResult> GetCollection([FromQuery] string type)
        {
            try
            {
                List<Object> result = new List<Object>();
                if (type.Contains("users") && type.Contains("games")) return Conflict("Collection can't be of type users and games");
                else if (!(type.Contains("users") || type.Contains("games")) && type.Contains("social"))
                {
                    string? email = HttpContext.Session.GetString("loggedUserEmail");
                    if (!string.IsNullOrEmpty(email))
                    {
                        List<FriendRequest> r = await context.GetAllFriendsRequestsByEmail(email);
                        foreach (FriendRequest f in r)
                        {
                            result.Add(new FriendRequestDTO(f, email));
                        }
                    }
                    else
                    {
                        return Conflict("Can't access friend requests without a user logged in");
                    }
                    return Ok(result);
                }
                else if (!(type.Contains("users") || type.Contains("games")) && type.Contains("difficulties"))
                {
                    List<Difficulty> difficulties = await context.GetDifficultyList();
                    foreach (Difficulty d in difficulties)
                    {
                        result.Add(new DifficultyDTO(d));
                    }
                }
                else if (!(type.Contains("users") || type.Contains("games"))) return Conflict("Collection must contain a type of either users or gamers but not both");

                if (type.Contains("games"))
                {
                    if (type.Contains("reports") && type.Contains("admin"))
                    {
                        string email = HttpContext.Session.GetString("loggedUserEmail");
                        if (!(await context.GetUserByEmail(email)).Admin)
                        {
                            return Unauthorized("cannot access this information without being an admin");
                        }
                        List<GameReport> r = await context.GetAllGameReports();
                        foreach (GameReport R in r)
                        {
                            result.Add(new GameReportDTO(R));
                        }
                    }
                    else if (type.Contains("social"))
                    {
                        string? email = HttpContext.Session.GetString("loggedUserEmail");
                        if (!string.IsNullOrEmpty(email))
                        {
                            List<FinishedGame> r = await context.GetAllFriendGamesByEmail(email);
                            foreach (FinishedGame g in r)
                            {
                                GameDataDTO gameDTO = new(g);
                                //if user is reported their pic won't be shown and instead will show default value
                                if (!g.User.UserReports.Any(r => r.StatusId == 2)) gameDTO.User.PicPath = GetProfileImageVirtualPath(g.User.Id);
                                else gameDTO.User.PicPath = $"/profileImages/default.png";
                                result.Add(gameDTO);
                            }
                            //adding all logged user games
                            List<FinishedGame> finishedGames = await context.GetAllGamesByEmail(email);
                            foreach (FinishedGame g in finishedGames)
                            {
                                GameDataDTO gameDTO = new(g);
                                gameDTO.User.PicPath = GetProfileImageVirtualPath(g.User.Id);
                                result.Add(gameDTO);
                            }
                        }
                        else
                        {
                            return Unauthorized("Can't access friend games list without a user logged in");
                        }
                    }
                    else
                    {
                        List<FinishedGame> r = await context.GetAllGamesWithData();
                        foreach (FinishedGame g in r)
                        {
                            GameDataDTO gameDTO = new(g);
                            //if user is reported their pic won't be shown and instead will show default value
                            if (!g.User.UserReports.Any(r => r.StatusId == 2)) gameDTO.User.PicPath = GetProfileImageVirtualPath(g.User.Id);
                            else gameDTO.User.PicPath = $"/profileImages/default.png";
                            result.Add(gameDTO);
                        }
                    }
                }
                if (type.Contains("users"))
                {
                    if (type.Contains("reports") && type.Contains("admin"))
                    {
                        string email = HttpContext.Session.GetString("loggedUserEmail");
                        if (!(await context.GetUserByEmail(email)).Admin)
                        {
                            return Unauthorized("Cannot access this information without being an admin");
                        }
                        List<UserReport> r = await context.GetAllUserReports();
                        foreach (UserReport R in r)
                        {
                            result.Add(new UserReportDTO(R));
                        }
                    }
                    else if (type.Contains("social"))
                    {
                        string? email = HttpContext.Session.GetString("loggedUserEmail");
                        if (!string.IsNullOrEmpty(email))
                        {
                            List<User> r = await context.GetAllFriendUsersByEmail(email);
                            foreach (User u in r)
                            {
                                result.Add(new UserDataDTO(u)
                                {
                                    PicPath = GetProfileImageVirtualPath(u.Id)
                                });
                            }
                        }
                        else
                        {
                            return Unauthorized("Can't access friend list without a user logged in");
                        }
                    }
                    else if (type.Contains("blocked"))
                    {
                        string? email = HttpContext.Session.GetString("loggedUserEmail");
                        if (!string.IsNullOrEmpty(email))
                        {
                            List<User> r = await context.GetAllBlockedUsersByEmail(email);
                            foreach (User u in r)
                            {
                                result.Add(new UserDataDTO(u)
                                {
                                    PicPath = GetProfileImageVirtualPath(u.Id)
                                });
                            }
                        }
                        else
                        {
                            return Unauthorized("Can't access block list without a user logged in");
                        }
                    }
                    else
                    {
                        List<User> r = await context.GetAllUsersWithData();
                        foreach (User u in r)
                        {
                            result.Add(new UserDataDTO(u)
                            {
                                PicPath = GetProfileImageVirtualPath(u.Id)
                            });
                        }
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } 
        #endregion

        [HttpPost("RecordGame")]
        public async Task<IActionResult> RecordGame([FromBody] FinishedGameDTO gameDTO)
        {
            try
            {
                User user = await context.GetUserByDTO(gameDTO.User);
                if (user== null)
                {
                    return Conflict("User doesn't exist");
                }
                string email= HttpContext.Session.GetString("loggedUserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("User must be logged to record game");
                }
                else if (email != user.Email)
                {
                    return Unauthorized("Cannot record game for other players");
                }
                Difficulty difficulty = await context.GetDifficultyByDTO(gameDTO.Difficulty);
                if (difficulty == null)
                {
                    return Conflict("Difficulty doesn't exist");
                }

                FinishedGame finished = new()
                {
                    Date = gameDTO.Date,
                    DifficultyId = difficulty.Id,
                    UserId = user.Id,
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


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] LoginInfoDTO userDTO)
        {
            try
            {
                if (await context.GetUserByEmail(userDTO.Email) != null)
                {
                    return Conflict("This email has already been used");
                }
                if(await context.GetUserByName(userDTO.Name) != null)
                {
                    return Conflict("This username has already been used");
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
        public async Task<IActionResult> Login([FromBody]LoginInfoDTO userDTO)
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

                if (user ==null)return NotFound("No user with this email or username");

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
            string? userEmail = HttpContext.Session.GetString("loggedUserEmail");
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
            AppUserDTO dtoUser = new(user);
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
