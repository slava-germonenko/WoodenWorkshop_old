namespace WoodenWorkshop.Core.Materials;

public static class Errors
{
    public static string MaterialNameIsInUse(string name) => $"Материал с названием {name} уже существует.";
}