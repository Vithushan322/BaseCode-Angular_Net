using API.Data;
using API.DTO;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public AccountController(
            DataContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region POST
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Regiester(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.Email)) return BadRequest("Email already exists!");

            using var hmac = new HMACSHA512();

            var user = _mapper.Map<User>(registerDTO, opt =>
            {
                opt.Items["PasswordHash"] = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
                opt.Items["PasswordSalt"] = hmac.Key;
            });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<UserDTO>(user));
        }
        #endregion

        private async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email.ToLower());
        }
    }
}
