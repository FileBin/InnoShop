using Microsoft.AspNetCore.Identity;

namespace InnoShop.Tests;

public static class TestGenerator {

    const string uppercaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string lowercaseCharacters = "abcdefghijklmnopqrstuvwxyz";

    const string digitsCharacters = "0123456789";

    const string specialCharacters = "~!@#$%^&()_=-+*/.";

    const string allChars = uppercaseCharacters + lowercaseCharacters + digitsCharacters + specialCharacters;

    /// <summary>
    /// Generates a Random Password
    /// respecting the given strength requirements.
    /// </summary>
    /// <param name="passwordOptions">A valid PasswordOptions object
    /// containing the password strength requirements.</param>
    /// <returns>A random password</returns>
    public static string GenerateRandomPassword(PasswordOptions? passwordOptions = null) {
        if (passwordOptions is null) {
            passwordOptions = new PasswordOptions {
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = false,
                RequireUppercase = true,
                RequiredLength = 8,
                RequiredUniqueChars = 1,
            };
        }

        Random rand = Random.Shared;
        List<char> chars = new List<char>();

        Action<string> insertRandomFromSet =
        (string set) => {
            chars.Insert(rand.Next(chars.Count), SelectRandom(set));
        };

        if (passwordOptions.RequireUppercase) {
            insertRandomFromSet(uppercaseCharacters);
        }
        if (passwordOptions.RequireLowercase) {
            insertRandomFromSet(lowercaseCharacters);
        }
        if (passwordOptions.RequireDigit) {
            insertRandomFromSet(digitsCharacters);
        }
        if (passwordOptions.RequireNonAlphanumeric) {
            insertRandomFromSet(specialCharacters);
        }

        for (int i = chars.Count; i < passwordOptions.RequiredLength
        || chars.Distinct().Count() < passwordOptions.RequiredUniqueChars; i++) {
            insertRandomFromSet(allChars);
        }

        return new string(chars.ToArray());
    }

    public static string GenerateRandomString(int length) {
        var bytes = new byte[length];

        Random.Shared.NextBytes(bytes);

        return Convert.ToBase64String(bytes).Substring(0, length);
    }

    public static string GenerateRandomInvalidString(int length) {

        return Enumerable.Range(1, length)
            .Select(i => Random.Shared.Next())
            .Select(r => char.ConvertFromUtf32(r%0x00d800))
            .Aggregate((x, y) => $"{x}{y}");
    }

    private static char SelectRandom(string set) {
        return set[Random.Shared.Next(set.Length)];
    }
}