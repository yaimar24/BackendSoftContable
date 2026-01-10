using Microsoft.AspNetCore.Hosting;

namespace BackendSoftContable.Services.Storage;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    public FileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string?> SaveAsync(IFormFile file)
    {
        var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
        Directory.CreateDirectory(uploads);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var path = Path.Combine(uploads, fileName);

        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{fileName}";
    }

    public Task DeleteAsync(string path)
    {
        var fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", path.TrimStart('/'));
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}
