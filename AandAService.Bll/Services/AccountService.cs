using System;
using System.Threading.Tasks;
using AandAService.Bll.Models;
using AandAService.Bll.Models.RequestModels;
using AandAService.Bll.Models.ResponseModels;
using AandAService.Dal.Entities;
using AandAService.Bll.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Net;
using AutoMapper;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AandAService.Bll.Services
{
    public class AccountService
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<MyRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly JwtConfig _jwtConfig;

        public AccountService(
            UserManager<MyUser> userManager
            ,RoleManager<MyRole> roleManager
            ,IMapper mapper
            ,JwtConfig jwtConfig)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _jwtConfig = jwtConfig;
        }

        public async Task<BaseResponse<AuthenticationResponse>> RegisterAsync(RegistrationDto model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
               return BaseResponseCreator
                    .CreateResponse<AuthenticationResponse>(new AuthenticationResponse()
                    ,HttpStatusCode.BadRequest, new List<string>() { $"{model.Email} is already exists" });
            }
            var newUser = _mapper.Map<MyUser>(model);
            var createUser = await _userManager.CreateAsync(newUser, model.Password);
            if (createUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "User");
                var jwtToken = await GenerateJwtToken(newUser);
                var authResponse = _mapper.Map<AuthenticationResponse>(newUser);
                authResponse.JwtToken = jwtToken;
                return BaseResponseCreator
                    .CreateResponse<AuthenticationResponse>(authResponse
                    , HttpStatusCode.OK, new List<string> { $"User {model.UserName} created successfully" });
            }
            else
            {
                return BaseResponseCreator
                    .CreateResponse<AuthenticationResponse>(new AuthenticationResponse()
                    ,HttpStatusCode.BadRequest , new List<string>() { $"Something went wrong with database, please try again later" });
            }
        }

        public async Task<BaseResponse<AuthenticationResponse>> LoginAsync(LoginDto model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists == null)
            {
                return BaseResponseCreator
                    .CreateResponse<AuthenticationResponse>(new AuthenticationResponse(),
                    HttpStatusCode.BadRequest, new List<string>() { $"The user with {model.Email} email doesn't exists" });
            }
            var isCorrect = await _userManager.CheckPasswordAsync(userExists, model.Password);
            if (isCorrect)
            {
                var response = _mapper.Map<AuthenticationResponse>(userExists);
                response.JwtToken = await GenerateJwtToken(userExists);
                return  BaseResponseCreator
                        .CreateResponse<AuthenticationResponse>(response,
                        HttpStatusCode.OK, new List<string>() { $"Logged in" });
            }

            return  BaseResponseCreator
                    .CreateResponse<AuthenticationResponse>(new AuthenticationResponse(),
                    HttpStatusCode.Unauthorized, new List<string>() { $"Password isn't correct please try again" });
        }

        public async Task<BaseResponse<List<MyUser>>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return BaseResponseCreator.CreateResponse<List<MyUser>>(users, HttpStatusCode.OK, new List<string>());
        }

        public async Task<BaseResponse<MyUser>> GetUserById(Guid id)
        {
            var userExists = await _userManager.FindByIdAsync(id.ToString());
            if (userExists == null)
            {
                return BaseResponseCreator
                    .CreateResponse<MyUser>(new MyUser(),
                    HttpStatusCode.BadRequest, new List<string>() { $"The user with id {id} doesn't exists" });
            }

            return BaseResponseCreator.CreateResponse<MyUser>(userExists, HttpStatusCode.OK, new List<string>());
        }

        public async Task<BaseResponse<MyUser>> GetUserByEmail(string email)
        {
            var userExists = await _userManager.FindByEmailAsync(email);
            if (userExists == null)
            {
                return BaseResponseCreator
                    .CreateResponse<MyUser>(new MyUser(),
                    HttpStatusCode.BadRequest, new List<string>() { $"The user with email {email} doesn't exists" });
            }
            return BaseResponseCreator.CreateResponse<MyUser>(userExists, HttpStatusCode.OK, new List<string>());
        }

        public BaseResponse<List<MyRole>> GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return BaseResponseCreator.CreateResponse<List<MyRole>>(roles, HttpStatusCode.OK, new List<string>());
        }

        public async Task<BaseResponse<dynamic>> CreateRoleAsync(string name)
        {
            var roleExists = await _roleManager.RoleExistsAsync(name);
            if (roleExists)
            {
                return BaseResponseCreator
                    .CreateResponse<dynamic>(null,
                    HttpStatusCode.Continue, new List<string>() { $"The role name {name} is already exists, please try another name" });
            }
            var newRole = new MyRole();
            newRole.Name = name;
            var createRole = await _roleManager.CreateAsync(newRole);
            if (createRole.Succeeded)
            {
                return BaseResponseCreator
                   .CreateResponse<dynamic>(null,
                   HttpStatusCode.OK, new List<string>() { $"The role {name} created successfully" });
            }
            else
            {
                return BaseResponseCreator
                  .CreateResponse<dynamic>(null,
                  HttpStatusCode.Continue, new List<string>() { $"The role {name} has not been added successfully" });
            }
        }

        public async Task<BaseResponse<dynamic>> AddUserToRole(UserRoleRelationDto model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists == null)
            {
                return BaseResponseCreator
                    .CreateResponse<dynamic>(null,
                    HttpStatusCode.BadRequest, new List<string>() { $"The user with email {model.Email} doesn't exists" });
            }
            var roleExists = await _roleManager.FindByNameAsync(model.RoleName);
            if (roleExists == null)
            {
                return BaseResponseCreator
                    .CreateResponse<dynamic>(null,
                    HttpStatusCode.BadRequest, new List<string>() { $"The role with name {model.RoleName} doesn't exists" });
            }
            var result = await _userManager.AddToRoleAsync(userExists, model.RoleName);
            if (result.Succeeded)
            {
                return BaseResponseCreator
                    .CreateResponse<dynamic>(null,
                    HttpStatusCode.OK, new List<string>() { $"The user {userExists.UserName} added successfully to the role {model.RoleName}" });
            }
            return BaseResponseCreator
                    .CreateResponse<dynamic>(null,
                    HttpStatusCode.BadRequest, new List<string>() { $"The user {userExists.UserName} wasn't able to be added to the role {model.RoleName}" });

        }

        public async Task<BaseResponse<dynamic>> RemoveUserFromRole(UserRoleRelationDto model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists == null)
            {
                return BaseResponseCreator
                    .CreateResponse<dynamic>(null,
                    HttpStatusCode.BadRequest, new List<string>() { $"The user with email {model.Email} doesn't exists" });
            }
            var roleExists = await _roleManager.FindByNameAsync(model.RoleName);
            if (roleExists == null)
            {
                return BaseResponseCreator
                    .CreateResponse<dynamic>(null,
                    HttpStatusCode.BadRequest, new List<string>() { $"The role with name {model.RoleName} doesn't exists" });
            }
            var userRoles = await _userManager.GetRolesAsync(userExists);
            var userHaveThisRole = false;
            foreach (var role in userRoles)
            {
                if (role == model.RoleName)
                {
                    userHaveThisRole = true;
                }
            }
            if (!userHaveThisRole)
            {
                return BaseResponseCreator
                    .CreateResponse<dynamic>(null,
                    HttpStatusCode.BadRequest, new List<string>() { $"The user {userExists.UserName} doesn't have the role {model.RoleName}" });
            }
            var result = await _userManager.RemoveFromRoleAsync(userExists, model.RoleName);
            if (result.Succeeded)
            {
                return BaseResponseCreator
                    .CreateResponse<dynamic>(null,
                    HttpStatusCode.OK, new List<string>() { $"The user {userExists.UserName} removed successfully from the role {model.RoleName}" });
            }
            return BaseResponseCreator
                    .CreateResponse<dynamic>(null,
                    HttpStatusCode.BadRequest, new List<string>() { $"The user {userExists.UserName} wasn't able to be removed from the role {model.RoleName}" });
        }

        public async Task<BaseResponse<IList<string>>> GetUserRoles(string email)
        {
            var userExists = await _userManager.FindByEmailAsync(email);
            if (userExists == null)
            {
                return BaseResponseCreator
                    .CreateResponse<IList<string>>(null,
                    HttpStatusCode.BadRequest, new List<string>() { $"The user with email {email} doesn't exists" });
            }
            var userRoles = await _userManager.GetRolesAsync(userExists);
            return BaseResponseCreator
                    .CreateResponse<IList<string>>(userRoles,
                    HttpStatusCode.OK, new List<string>());
        }

        private async Task<string> GenerateJwtToken(MyUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.SecretKey);
            var claims = await GetAllValidClaims(user);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;

        }

        private async Task<List<Claim>> GetAllValidClaims(MyUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            return claims;
        } 
    }
}
