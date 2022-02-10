namespace WoodenWorkshop.Core.Products;

public static class Errors
{
    public static string MissingProductAssets(Guid productId, IEnumerable<Guid> missingAssetIds)
        => $"Не все ассеты были указаны для продукта {productId}. " +
           $"Индентификаторы пропущенных ассетов: {string.Join(", ", missingAssetIds)}.";

    public static string ProductNotFound(Guid productId) => $"Товар с идентификатором '{productId}' не найден.";

    public static string ProductDeleted(Guid prodcutId) => $"Товар с идентификатором '{prodcutId}' был удалён.";

    public static string ProductVendorCodeUnavailable(string vendorCode) => $"Невозможно использовать артикул товара '{vendorCode}' так как он уже был использован.";
        
    public static string RedundantProductAssets(Guid productId, IEnumerable<Guid> redundantAssetIds) 
        => $"Не все указанные ассеты принадлежат продукут {productId}. " + 
           $"Идентификаторы лишних ассетов: {string.Join(", ", redundantAssetIds)}";
}