using API.Data;
using API.DTO;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class PhotoController : BaseApiController
    {
        private readonly DataContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public PhotoController(
            DataContext dbContext,
            IMapper mapper,
            IPhotoService photoService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _photoService = photoService;
        }

        #region POST
        [HttpPost("add-photo/user")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            if (file == null) return BadRequest("Invalid File");

            var user = await _dbContext.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Email == User.GetEmail());

            if (user == null) return BadRequest("You dont have access!");

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0) photo.IsMain = true;

            user.LastUpdated = DateTime.UtcNow;
            user.Photos.Add(photo);

            if (await _dbContext.SaveChangesAsync() > 0) return _mapper.Map<PhotoDTO>(photo);

            return BadRequest("Problem adding photo");
        }
        #endregion

        #region DELETE
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            if (photoId <= 0) return BadRequest("Invalid photo id!");

            var user = await _dbContext.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Email == User.GetEmail());

            if (user == null) return BadRequest("You dont have access!");

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You can not delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.LastUpdated = DateTime.UtcNow;
            user.Photos.Remove(photo);

            if (await _dbContext.SaveChangesAsync() > 0) return Ok();

            return BadRequest("Failed to delete the photo");
        }
        #endregion

        #region PUT
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _dbContext.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Email == User.GetEmail());

            if (user == null) return BadRequest("You dont have access!");

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            user.LastUpdated = DateTime.UtcNow;
            if (await _dbContext.SaveChangesAsync() > 0) return NoContent();

            return BadRequest("Failed to set main photo");
        }
        #endregion
    }
}