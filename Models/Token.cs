using System.ComponentModel.DataAnnotations;

namespace AshishGeneralStore.Models;

public class Token
{
    public int Id { get; set; }

    [Required]
    public string RefreshToken { get; set; }

    [Required]
    public int UserId { get; set; }

    public User User { get; set; }

    public DateTime IssuedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }
}