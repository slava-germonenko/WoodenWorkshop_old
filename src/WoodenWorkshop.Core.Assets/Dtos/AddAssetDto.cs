namespace WoodenWorkshop.Core.Assets.Dtos;

public record AddAssetDto(
    string OriginalFileName,
    Guid? FolderId = null
);