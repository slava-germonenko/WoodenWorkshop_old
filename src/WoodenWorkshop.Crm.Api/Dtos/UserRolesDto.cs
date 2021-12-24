using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Crm.Api.Dtos;

public record UserRolesDto(ICollection<Role> Roles);
