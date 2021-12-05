using System.ComponentModel.DataAnnotations;

using WoodenWorkshop.Common.Helpers;

namespace WoodenWorkshop.Common.Attributes.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class ConstantsRangeAttribute : ValidationAttribute
{
    private readonly IEnumerable<object> _valuesToCompare;

    public ConstantsRangeAttribute(Type constantSource)
    {
        _valuesToCompare = ConstantsHelper.GetClassConstants(constantSource);
    }

    public override bool IsValid(object? value)
    {
        return value is not null && _valuesToCompare.Contains(value);
    }
}