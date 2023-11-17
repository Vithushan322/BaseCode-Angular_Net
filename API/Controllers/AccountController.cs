using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ITokenService _tokenService;

        public AccountController(
            DataContext context,
            IMapper mapper,
            ITokenService tokenService)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        #region POST
        [HttpPost("register")]
        public async Task<ActionResult<AuthorizedUserDTO>> Regiester(RegisterDTO registerDTO)
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

            var userDTO = _mapper.Map<AuthorizedUserDTO>(user, opt =>
            {
                opt.Items["Token"] = _tokenService.CreateToken(user);
            });

            return Ok(userDTO);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == loginDTO.Email.ToLower());

            if (user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            var userDTO = _mapper.Map<AuthorizedUserDTO>(user, opt =>
            {
                opt.Items["Token"] = _tokenService.CreateToken(user);
            });

            return Ok(userDTO);
        }
        #endregion

        private async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email.ToLower());
        }
    }
}
