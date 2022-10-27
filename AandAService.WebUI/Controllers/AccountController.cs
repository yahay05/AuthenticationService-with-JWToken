using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AandAService.Bll.Models.RequestModels;
using AandAService.Bll.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AandAService.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromQuery] RegistrationDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.RegisterAsync(model);
                return Ok(result);
            }
            else
            {
                return BadRequest(model);
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromQuery] LoginDto model)
        {
            var result = await _accountService.LoginAsync(model);
            return Ok(result);
        }

        [HttpGet("GetUserById")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _accountService.GetUserById(id);
            return Ok(result);
        }

        [HttpGet("GetUserByEmail")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var result = await _accountService.GetUserByEmail(email);
            return Ok(result);
        }

        [HttpGet("GetAllUsers"),Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _accountService.GetAllUsersAsync();
            return Ok(result);
        }

        [HttpGet("GetAllRoles"),Authorize(Roles = "Admin")]
        public IActionResult GetAllRoles()
        {
            var result = _accountService.GetAllRoles();
            return Ok(result);
        }

        [HttpPost("CreateRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole([FromQuery]string name)
        { 
             var result = await _accountService.CreateRoleAsync(name);
             return Ok(result);
        }

        [HttpPost("AddUserToRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserToRole([FromQuery]UserRoleRelationDto model)
        {
            var result = await _accountService.AddUserToRole(model);
            return Ok(result);
        }

        [HttpPost("RemoveUserFromRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUserToRole([FromQuery]UserRoleRelationDto model)
        {
            var result = await _accountService.RemoveUserFromRole(model);
            return Ok(result);
        }

        [HttpGet("GetUserRoles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var result = await _accountService.GetUserRoles(email);
            return Ok(result);
        }

    }
}
