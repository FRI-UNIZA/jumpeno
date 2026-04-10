namespace Jumpeno.Client.Utils;

public class StringGenerator {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Random G = new();

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    private string Generate(
        // Parameters:
        int minLength, int maxLength, string characters = Chars.ALPHA_UPPER,
        // Exceptions:
        string minLengthID = "", string maxLengthID = "", string charactersID = ""
        ) {
        // 1) Check values:
        if (string.IsNullOrEmpty(characters)) throw Exceptions.VALUES.Add(Errors.UNDEFINED.SetID(charactersID));
        Checker.CheckGreaterOrEqualTo(minLength, 0, minLengthID);
        Checker.CheckGreaterOrEqualTo(maxLength, minLength, maxLengthID);
        if (maxLength <= 0) return string.Empty;
        // 2) Generate:
        int length = G.Next(minLength, maxLength + 1);
        // 3) Convert to string:
        return new string([.. Enumerable.Range(0, length).Select(_ => characters[G.Next(characters.Length)])]);
    }

    public string Generate(int minLength, int maxLength, string characters = Chars.ALPHA_UPPER)
    => Generate(
        minLength, maxLength, characters, 
        nameof(minLength), nameof(maxLength), nameof(characters)
    );

    public string Generate(int length, string characters = Chars.ALPHA_UPPER)
    => Generate(
        length, length, characters,
        nameof(length), nameof(length), nameof(characters)
    );

    public string GenerateResetPassword(int minLength, int maxLength)
    {
        // 1) Check values:
        Checker.CheckGreaterOrEqualTo(minLength, UserValidator.PASSWORD_GENERATOR_MIN_LENGTH, nameof(minLength));
        Checker.CheckGreaterOrEqualTo(maxLength, minLength, nameof(maxLength));
        // 2) Generate:
        int length = G.Next(minLength, maxLength + 1);
        int partLength = length / 4;
        string password = Generate(partLength, Chars.ALPHA_LOWER);
        password += Generate(partLength, Chars.ALPHA_UPPER);
        password += Generate(partLength, Chars.NUM);
        password += Generate(length - (partLength * 3), Chars.SPECIAL);
        var passwordArray = password.ToCharArray();
        G.Shuffle(passwordArray);
        // 3) Convert to string:
        return new(passwordArray);
    }
}
