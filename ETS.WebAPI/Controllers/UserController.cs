using EST.BL.Interfaces;
using EST.DAL.Models;
using EST.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EST.Domain.Helpers;
using EST.Domain.Helpers.ErrorFilter;

namespace ETS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IManageImage _manageImage;
        public UserController(IUserService userService, IManageImage manageImage)
        {
            _userService = userService;
            _manageImage = manageImage;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetById(CancellationToken token)
        {
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception e)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }
            var user = await _userService.GetById(userParseId, token);

            if (user == null)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "User doesn't exist",
                    Detail = "Server didn't find user on database"
                };
            return Ok(user);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser()
        {
            var user = new UserDTO();
            try
            {
                user.Id = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                user.Email = HttpContext.User.FindFirstValue(ClaimTypes.Email);
                user.RoleName = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            }
            catch (Exception e)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }

            var createdUser = await _userService.Create(user);
            if (createdUser != null)
                return Created("/api/User", createdUser);
            else
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Title = "Can't create user",
                    Detail = "Error occured while creating user on server"
                };
        }
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteUser(CancellationToken token)
        {
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception e)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }

            if (!await _userService.Exist(userParseId))
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "No user exists",
                    Detail = "Server didn't find user on the database"
                };

            var auth = Request.Headers.Authorization;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", auth.ToString());
            
            var response = await client.DeleteAsync($"{ConstantVariables.ngrok}/api/User");
            if (response.IsSuccessStatusCode)
            {
                if (await _userService.Delete(userParseId, token))
                    return Ok("User is deleted!");
                else
                    throw new ApiException()
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Title = "Can't delete user",
                        Detail = "Error occured while deleting user on main server"
                    };
            }
            throw new ApiException()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Title = "Can't delete user",
                Detail = "Error occured while deleting user on security server"
            };
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadPhoto(
            IFormFile file)
        {
            Guid userParseId;
            try
            {
                userParseId = Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (Exception e)
            {
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Title = "Something wrong with user Guid",
                    Detail = "Error occured while parsing guid from user claims"
                };
            }
            
            var result = await _manageImage.UploadFile(file, userParseId);
            return Ok(result);
        }
        [HttpGet("fileName")]
        public async Task<IActionResult> DownloadPhoto([FromQuery] string fileName)
        {
            var result = await _manageImage.DownloadFile(fileName);
            return File(result.Item1, result.Item2, result.Item3);
        }
    }
}
