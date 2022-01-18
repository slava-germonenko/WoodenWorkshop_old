namespace WoodenWorkshop.Core.Assets.Dtos;

public record CreateAssetDto(string AssetName, Stream FileStream, Guid? FolderId = null);