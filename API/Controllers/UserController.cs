using API.Data;
using API.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UserController : BaseApiController
    {
        private readonly DataContext _dbContext;
        private readonly IMapper _mapper;

        public UserController(
            DataContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        #region GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _dbContext.Users.Include(p => p.Photos).ToListAsync();

            return Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            if (id <= 0) return BadRequest("Invalid user id!");

            var user = await _dbContext.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null) return NotFound("User does not exists!");

            return Ok(_mapper.Map<UserDTO>(user));
        }
        #endregion
    }
}
