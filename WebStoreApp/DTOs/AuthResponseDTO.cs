namespace WebStoreApp.DTOs
{
    public class AuthResponseDTO
    { 
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string UserId { get; set; } = null!; 
    }
}
