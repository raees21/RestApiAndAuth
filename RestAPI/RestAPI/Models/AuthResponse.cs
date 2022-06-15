namespace RestAPI.Models;

public class AuthResponse
{
    public string AccessToken { get; set; }
    public Guid UserId { get; set; }
    
    public AuthResponse() {}
}