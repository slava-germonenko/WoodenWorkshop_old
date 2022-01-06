using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Auth.Models;

public class Session
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public DateTime ExpireDate { get; set; }
    
    [Required, StringLength(200)]
    public string RefreshToken { get; set; }
    
    [StringLength(100)]
    public string? DeviceName { get; set; }

    [StringLength(40)]
    public string? IpAddress { get; set; }
}