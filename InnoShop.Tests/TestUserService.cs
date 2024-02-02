using InnoShop.Domain;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace InnoShop.Tests;

public class TestUserService {
    Mock<UserManager<ShopUser>> mockUserManager = new Mock<UserManager<ShopUser>>();
    Mock<SignInManager<ShopUser>> mockSignInManager = new Mock<SignInManager<ShopUser>>();
    private IDictionary<string, ShopUser> registeredUsers = new Dictionary<string, ShopUser>();

    private IdentityOptions IdentityOptions;

    public UserManager<ShopUser> UserManager { get { return mockUserManager.Object; } }

    public SignInManager<ShopUser> SignInManager { get { return mockSignInManager.Object; } }
    public TestUserService(IdentityOptions identityOptions) {
        IdentityOptions = identityOptions;

        mockUserManager
            .Setup(userManager => userManager.CreateAsync(It.IsAny<ShopUser>(), It.IsAny<string>()))
            .Returns((ShopUser user, string password) => Task.FromResult(AddUser(user, password)));

        mockSignInManager
            .Setup(signInManager => signInManager.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false))
            .Returns((string username, string password, bool a, bool b) => Task.FromResult(LoginUser(username, password)));
    }

    private IdentityResult AddUser(ShopUser shopUser, string password) {
        if (password.Length < IdentityOptions.Password.RequiredLength) {
            return IdentityResult.Failed();
        }

        shopUser.PasswordHash = password;
        var name = shopUser.UserName;

        if (string.IsNullOrEmpty(name)) {
            return IdentityResult.Failed();
        }

        registeredUsers.Add(name, shopUser);
        return IdentityResult.Success;
    }

    private SignInResult LoginUser(string login, string password) {
        if (registeredUsers.TryGetValue(login, out var shopUser)) {
            if (shopUser.PasswordHash == password) {
                return SignInResult.Success;
            }
        }
        return SignInResult.Failed;
    }
}
