using Microsoft.AspNetCore.Identity;

namespace InnoShop.Tests.TestUserAPI.Tests;

[TestFixture]
public class TestRegistration : SetupUserApi {
    static readonly string TestEmail = "waffle@example.com";
    static readonly string TestUsername = "onionWaffle";

    private UserCredentials validCredentials
    => new UserCredentials() {
        Email = TestEmail,
        Username = TestUsername,
        Password = TestGenerator.GenerateRandomPassword()
    };

    private string LongString
    => Enumerable.Range(1, 1200)
            .Select(i => "a")
            .Aggregate((x, y) => $"{x}{y}");

    [Test]
    public async Task ValidParamenters() {
        var userCredentials = validCredentials;

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode);
    }

    [Test]
    public async Task ShortPassword() {
        var userCredentials = validCredentials;
        userCredentials.Password = "abc";

        var result = await RegisterUser(userCredentials);

        Assert.That(!result.IsSuccessStatusCode);
    }

    [Test]
    public async Task InvalidCharsPassword() {
        var userCredentials = validCredentials;
        userCredentials.Password = TestGenerator.GenerateRandomInvalidString(12);

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode, Is.False);
    }

    [Test]
    public async Task LongPassword() {
        var longPasswordOptions = new PasswordOptions() {
            RequiredLength = 1200,
            RequiredUniqueChars = 10,
            RequireDigit = true,
            RequireLowercase = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true,
        };

        var userCredentials = validCredentials;
        userCredentials.Username = TestGenerator.GenerateRandomPassword(longPasswordOptions);

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode, Is.False);
    }

    [Test]
    public async Task EmptyUsername() {
        var userCredentials = validCredentials;
        userCredentials.Username = "";

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode, Is.False);
    }

    [Test]
    public async Task ShortUsername() {
        var userCredentials = validCredentials;
        userCredentials.Username = "a";

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode, Is.False);
    }

    [Test]
    public async Task InvalidCharsUsername() {
        var userCredentials = validCredentials;
        userCredentials.Username = TestGenerator.GenerateRandomInvalidString(8);

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode, Is.False);
    }

    [Test]
    public async Task LongUsername() {
        var userCredentials = validCredentials;
        userCredentials.Username = LongString;

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode, Is.False);
    }

    [Test]
    public async Task EmptyEmail() {
        var userCredentials = validCredentials;

        userCredentials.Email = "";

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode, Is.False);
    }

    [Test]
    public async Task InvalidEmail1() {
        var userCredentials = validCredentials;

        userCredentials.Email = Enumerable.Range(1, 10)
            .Select(i => "a")
            .Aggregate((x, y) => $"{x}{y}");

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode, Is.False);
    }

    [Test]
    public async Task InvalidEmail2() {
        var userCredentials = validCredentials;

        var chars = Enumerable.Range(1, 10)
            .Select(i => "a")
            .ToList();
        chars.Insert(5, "@");
        chars.Insert(2, " ");
        userCredentials.Email = chars.Aggregate((x, y) => $"{x}{y}");

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode, Is.False);
    }


    [Test]
    public async Task InvalidEmail3() {
        var userCredentials = validCredentials;

        var chars = Enumerable.Range(1, 10)
            .Select(i => "a")
            .ToList();
        chars.Insert(5, "@");
        userCredentials.Email = chars.Aggregate((x, y) => $"{x}{y}");

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode, Is.False);
    }

    [Test]
    public async Task ValidEmail1() {
        var userCredentials = validCredentials;

        var chars = Enumerable.Range(1, 10)
            .Select(i => "a")
            .ToList();
        chars.Insert(5, "@");
        userCredentials.Email = chars.Aggregate((x, y) => x + y) + ".com";

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task ValidEmail2() {
        var userCredentials = validCredentials;

        var chars = Enumerable.Range(1, 12)
            .Select(i => "a")
            .ToList();

        chars.Insert(2, ".");
        chars.Insert(5, "@");
        chars.Insert(7, ".");
        chars.Insert(9, ".");
        userCredentials.Email = chars.Aggregate((x, y) => x + y) + ".com";

        var result = await RegisterUser(userCredentials);

        Assert.That(result.IsSuccessStatusCode, Is.True);
    }
}
