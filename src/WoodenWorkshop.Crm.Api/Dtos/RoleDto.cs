using System.ComponentModel.DataAnnotations;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Crm.Api.Dtos;

public record RoleDto(
    Guid Id,
    [Required, StringLength(100)] string Name,
    int AssigneeCount,
    ICollection<string> Permissions
)
{
    public static implicit operator RoleDto(Role role) => new(
        role.Id,
        role.Name,
        role.Users?.Count ?? 0,
        role.Permissions?.Select(p => p.Name)?.ToArray() ?? ArraySegment<string>.Empty
    );
}