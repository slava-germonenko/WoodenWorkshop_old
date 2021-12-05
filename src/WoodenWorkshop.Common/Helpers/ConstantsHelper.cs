using System.Reflection;

namespace WoodenWorkshop.Common.Helpers;

public static class ConstantsHelper
{
    public static IEnumerable<TConstant> GetClassConstants<TConstant>(Type type)
    {
        return type
            .GetFields(BindingFlags.Static | BindingFlags.Public)
            .Where(f => f.GetType() == typeof(TConstant))
            .Select(f => f.GetValue(null))
            .Where(v => v != null)
            .Select(v => (TConstant) v);
    }

    public static IEnumerable<object> GetClassConstants(Type type)
    {
        return type
            .GetFields(BindingFlags.Static | BindingFlags.Public)
            .Select(f => f.GetValue(null))
            .Where(v => v != null);
    }
}