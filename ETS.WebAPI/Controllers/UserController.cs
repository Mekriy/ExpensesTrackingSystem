using EST.BL.Interfaces;
using EST.DAL.Models;
using EST.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ETS.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetById(CancellationToken token)
        {
            //TODO: try parse
            var userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _userService.GetById(userParseId, token);

            if (user == null)
                return NotFound("User doesn't exist");
            return Ok(user);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser()
        {
            var user = new UserDTO()
            {
                //TODO: try parse
                Id = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                RoleName = HttpContext.User.FindFirstValue(ClaimTypes.Role),
            };
            if (user == null)
                return BadRequest("No user to create");

            if (await _userService.Create(user))
                return Ok("User is created");
            else
                return StatusCode(500, "Error occured while creating user on server");
        }
        //[HttpPut]
        //public async Task<IActionResult> UpdateUser([FromBody] UserDTO user)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest();

        //    if (user == null)
        //        return BadRequest("No user");

        //    if (!await _userService.Exist(user.Name))
        //        return BadRequest("User doesn't exist");

        //    if (await _userService.Update(user))
        //        return Ok("User is updated!");
        //    else
        //        return StatusCode(500, "Error occured while updating user on server");
        //}
        //[HttpDelete("{userId:Guid}")]
        //public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
        //{
        //    if (userId == Guid.Empty)
        //        return BadRequest("No guid");

        //    if (!await _userService.Exist(userId))
        //        return BadRequest("User doesn't exist");

        //    if (await _userService.Delete(userId))
        //        return Ok("User is deleted!");
        //    else
        //        return StatusCode(500, "Error occured while deleting user on server");
        //}

    }
}
