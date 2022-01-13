namespace WoodenWorkshop.Core.Assets.Dtos;

public record AddFolderDto(string Name, Guid? ParentFolderId);