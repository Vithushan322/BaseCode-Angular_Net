using API.Data;
using API.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
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

        #region DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            if (id <= 0) return BadRequest("Invalid user id!");

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null) return NotFound("User does not exists");

            _dbContext.Users.Remove(user);

            if (await _dbContext.SaveChangesAsync() > 0) return Ok();

            return BadRequest("Could not complete. Issue with the data");
        }
        #endregion

        #region PUT
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBike(int id, UpdateUserDTO updateUserDTO)
        {
            if (id <= 0) return BadRequest("Invalid user id!");

            if (updateUserDTO == null) return BadRequest("Body can not be empty!");

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null) return NotFound();

            _mapper.Map(updateUserDTO, user);
            user.LastUpdated = DateTime.UtcNow;

            if (await _dbContext.SaveChangesAsync() > 0) return NoContent();

            return BadRequest("Failed to update bike");
        }
        #endregion

        #region PATCH
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdatePartialUser(int id, JsonPatchDocument<UpdateUserDTO> patchUserDTO)
        {
            if (id <= 0) return BadRequest("Invalid user id!");

            if (patchUserDTO == null) return BadRequest("Body can not be empty!");

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null) return NotFound();

            UpdateUserDTO updateUserDTO = _mapper.Map<UpdateUserDTO>(user);

            patchUserDTO.ApplyTo(updateUserDTO);

            _mapper.Map(updateUserDTO, user);
            user.LastUpdated = DateTime.UtcNow;

            if (await _dbContext.SaveChangesAsync() > 0) return NoContent();

            return BadRequest("Failed to update bike");
        }
        #endregion
    }
}
