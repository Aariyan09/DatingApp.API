using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet.Core;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> _configCloudinary)
        {
            var acc = new Account
            (
                _configCloudinary.Value.CloudName,
                _configCloudinary.Value.ApiKey,
                _configCloudinary.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "Aryan_New"
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string PublicId)
        {
            var deleteParams = new DeletionParams(PublicId);

            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
