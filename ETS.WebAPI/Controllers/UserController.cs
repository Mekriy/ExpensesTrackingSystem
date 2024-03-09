﻿using EST.BL.Interfaces;
using EST.DAL.Models;
using EST.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EST.Domain.Helpers.ErrorFilter;

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

            if (await _userService.Create(user))
                return Ok("User is created");
            else
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Title = "Can't create user",
                    Detail = "Error occured while creating user on server"
                };
        }
        [HttpDelete("{userId:Guid}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId, CancellationToken token)
        {
            if (userId == Guid.Empty)
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Title = "Invalid guid",
                    Detail = "Guid is empty"
                };

            if (!await _userService.Exist(userId))
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Title = "No user exists",
                    Detail = "Server didn't find user on the database"
                };

            if (await _userService.Delete(userId, token))
                return Ok("User is deleted!");
            else
                throw new ApiException()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Title = "Can't delete user",
                    Detail = "Error occured while deleting user on server"
                };
        }

    }
}
