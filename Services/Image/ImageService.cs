using Pizza_API.Exceptions;

namespace Pizza_API.Services
{
    public class ImageService : IImageService
    {
        private readonly string _baseUploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        public async Task<(string imagePath, string imageFileName)> UploadImageAsync(IFormFile file, string id, string uploadSubFolder)
        {
            if (file == null || file.Length == 0)
                throw new ValidationException("File is empty");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType))
                throw new ValidationException("Invalid file type. Allowed types: JPEG, PNG, WebP");

            const long maxFileSize = 5 * 1024 * 1024;
            if (file.Length > maxFileSize)
                throw new ValidationException("File too large. Maximum size is 5MB");

            var uploadFolder = Path.Combine(_baseUploadFolder, uploadSubFolder);
            Directory.CreateDirectory(uploadFolder);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{id}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return ($"/uploads/{uploadSubFolder}/{fileName}", fileName);
        }

        public void DeleteImage(string? imageFileName, string uploadSubFolder)
        {
            if (string.IsNullOrEmpty(imageFileName)) return;

            var filePath = Path.Combine(_baseUploadFolder, uploadSubFolder, imageFileName);
            if (File.Exists(filePath))
            {
                try { File.Delete(filePath); }
                catch (IOException ex) { Console.WriteLine($"Could not delete image: {ex.Message}"); }
            }
        }
    }
}