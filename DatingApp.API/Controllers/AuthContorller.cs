using System;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
        [Route("api/[controller]")]

    public class AuthContorller : Controller
    {
        // inject with Constructor 
        private readonly IAuthRepository _repo;
        public AuthContorller(IAuthRepository repo)
        {
            _repo = repo;
        }
        
         [HttpPost("register")] 
         public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
              // validate reguest

              userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

              if(await _repo.UserExists(userForRegisterDto.Username))
                 return BadRequest("Username is already taken");
                 // store the user
                 var userToCreate = new User
                 {
                     Username = userForRegisterDto.Username
                 };
                 
                 var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
                 
                  return StatusCode(201);

        }
    }
}