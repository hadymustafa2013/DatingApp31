using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp31.Data;
using DatingApp31.Dtos;
using DatingApp31.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp31.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            this._repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegDto)
        {
            // Validate request

            if (await _repo.UserExists(userForRegDto.UserName.ToLower()))
                return BadRequest("User already exists!");

            var userToCreate = new User()
            {
                UserName = userForRegDto.UserName.ToLower()
            };

            var createdUser = await _repo.Register(userToCreate, userForRegDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async void Login()
        {
        }
    }
}
