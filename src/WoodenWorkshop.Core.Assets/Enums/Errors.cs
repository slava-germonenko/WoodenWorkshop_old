namespace WoodenWorkshop.Core.Assets.Enums;

public static class Errors
{
    public static string AssetNotFound(Guid assetId) => $"Файл с идентификатором {assetId} не найден.";
    public static string AssetNameInUse(string assetName) => $"Файл с именем {assetName} уже существует.";
    public static string AssetTypeUnsupported(string fileExt) => $"Файлы с расшрением '{fileExt}' не поддерживаются";
    public static string FolderNotFound(Guid folderId) => $"Папка с идентификатором {folderId} не найдена.";
    public static string FolderNameInUse(string folderName) => $"Папка с именем {folderName} уже существует.";
}