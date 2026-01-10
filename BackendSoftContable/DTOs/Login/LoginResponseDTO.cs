namespace BackendSoftContable.DTOs.Login
{
    public class LoginResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}
}
