using WoodenWorkshop.Core.Models.Enums;

namespace WoodenWorkshop.Core.Models;

public class ProductSocialLink : BaseModel
{
    public Guid ProductId { get; set; }

    public Uri Url { get; set; }

    public SocialLinkType SocialLinkType { get; set; } = SocialLinkType.Other;
}