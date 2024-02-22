using EST.BL.Interfaces;
using EST.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAll();
            if (users == null || users.Count == 0)
                return BadRequest("There are no users!");
            else
                return Ok(users);
        }
        [HttpGet("GetByGuid")]
        public async Task<IActionResult> GetUserById([FromQuery] Guid userId)
        {
            var user = await _userService.GetById(userId);
            if (user == null)
                return BadRequest();
            return Ok(user);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (user == null)
                return BadRequest("No user");

            if (await _userService.Exist(user.Id))
                return BadRequest("User already exists");

            if (await _userService.Create(user))
                return Ok("User is created");
            else
                return StatusCode(500, "Error occured while creating user on server");
        }
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateUser(User user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (user == null)
                return BadRequest("No user");

            if (!await _userService.Exist(user.Id))
                return BadRequest("User doesn't exist");

            if (await _userService.Update(user))
                return Ok("User is updated!");
            else
                return StatusCode(500, "Error occured while updating user on server");
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteUser([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("No guid");

            if (!await _userService.Exist(userId))
                return BadRequest("User doesn't exist");
            
            if (await _userService.Delete(userId))
                return Ok("User is deleted!");
            else
                return StatusCode(500, "Error occured while deleting user on server");
        }
    }
}
