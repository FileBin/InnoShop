namespace InnoShop.Domain.Entities.Roles;

public class AdminRole : ShopRole {
    public static readonly string RoleName = "admin";
    public static readonly string AdminName = "admin";
    public static readonly string AdminEmail = "admin@email.com";

    public AdminRole() : base(RoleName) { }
}