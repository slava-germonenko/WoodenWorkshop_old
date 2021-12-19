using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Crm.Api.Dtos;

public record UserCredentialsDto(
    [Required] string Username,
    [Required] string Password,
    string? DeviceName
);
