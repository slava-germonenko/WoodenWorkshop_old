namespace WoodenWorkshop.Core.Assets.Enums;

public static class Errors
{
    public static string AssetAlreadyExists(string name) => $"Файл с именем {name} уже существует.";
    public static string AssetFormatNotSupported(string fileExt) => $"Файлы с рашерением {fileExt} не поддерживаются";
    public static string AssetNotFound(Guid assetId) => $"Файл идентификатором {assetId} не найден";
    public static string FolderAlreadyExists(string name) => $"Директория с именем {name} уже существует";
    public static string FolderNotFound(Guid folderId) => $"Директория c идентификатором {folderId} не найдена.";
}