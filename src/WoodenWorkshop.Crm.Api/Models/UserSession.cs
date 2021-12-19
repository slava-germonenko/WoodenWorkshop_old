namespace WoodenWorkshop.Crm.Api.Models;

public record UserSession(string RefreshToken, Guid UserId, string IpAddress, string DeviceName);
