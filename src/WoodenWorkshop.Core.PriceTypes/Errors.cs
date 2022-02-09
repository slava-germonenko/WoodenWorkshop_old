namespace WoodenWorkshop.Core.PriceTypes;

public static class Errors
{
    public static string PriceTypeNameIsInUse(string name) => $"тип цены с наименованием '{name}' уже существует.";

    public static string PriceTypeNotFound(Guid id) => $"Тип цены с идентификатором '{id}' не найден.";
}