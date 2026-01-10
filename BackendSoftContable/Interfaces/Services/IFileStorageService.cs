using Microsoft.AspNetCore.Http;

public interface IFileStorageService
{
    Task<string?> SaveAsync(IFormFile file);
    Task DeleteAsync(string path);
}
