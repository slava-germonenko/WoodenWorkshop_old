using WoodenWorkshop.Common.Attributes.Validation;
using WoodenWorkshop.Core.Models.Enums;

namespace WoodenWorkshop.Core.Models;

public class Permission : BaseModel
{
    public Guid RoleId { get; set; }

    [ConstantsRange(typeof(Permissions))]
    public string Name { get; set; }
}