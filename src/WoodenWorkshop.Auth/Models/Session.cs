using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Auth.Models;

public class Session
{
    public string? DeviceName { get; set; }

    public string? IpAddress { get; set; }

    [Required]
    public DateTime ExpireDate { get; set; }
    
    [Required, StringLength(200)]
    public string RefreshToken { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
}