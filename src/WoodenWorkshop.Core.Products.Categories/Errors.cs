namespace WoodenWorkshop.Core.Products.Categories;

public static class Errors
{
    public static string CategoryNameIsInUse(string name) => $"Категория товара с наименованием '{name}' уже существует.";
    
    public static string CategoryNotFound(Guid id) => $"Категория товаров с идентификатором '{id}' не найдена.";
}