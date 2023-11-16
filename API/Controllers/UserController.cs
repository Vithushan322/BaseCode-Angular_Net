using API.Data;
using API.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
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
            var users = await _dbContext.Users.ToListAsync();

            return Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            if (id <= 0) return BadRequest("Invalid user id!");

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null) return NotFound("User does not exists!");

            return Ok(_mapper.Map<UserDTO>(user));
        }
        #endregion
    }
}
