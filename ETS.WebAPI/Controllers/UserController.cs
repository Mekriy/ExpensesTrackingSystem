using EST.BL.Interfaces;
using EST.DAL.Models;
using EST.Domain.DTOs;
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
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAll();
            if (users == null || users.Count == 0)
                return BadRequest("There are no users!");
            else
                return Ok(users);
        }
        [HttpGet("{userId:Guid}")]
        public async Task<IActionResult> GetUserById([FromRoute] Guid userId)
        {
            var user = await _userService.GetById(userId);
            if (user == null)
                return BadRequest();
            return Ok(user);
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (user == null)
                return BadRequest("No user");

            if (await _userService.Exist(user.Name))
                return BadRequest("User already exists");

            if (await _userService.Create(user))
                return Ok("User is created");
            else
                return StatusCode(500, "Error occured while creating user on server");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (user == null)
                return BadRequest("No user");

            if (!await _userService.Exist(user.Name))
                return BadRequest("User doesn't exist");

            if (await _userService.Update(user))
                return Ok("User is updated!");
            else
                return StatusCode(500, "Error occured while updating user on server");
        }
        [HttpDelete("{userId:Guid}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
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
