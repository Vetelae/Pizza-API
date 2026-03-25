namespace Pizza_API.Services
{
    public interface IImageService
    {
        Task<(string imagePath, string imageFileName)> UploadImageAsync(IFormFile file, string id, string uploadSubFolder);
        void DeleteImage(string? imageFileName, string uploadSubFolder);
    }
}
