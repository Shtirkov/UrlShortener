using System.Security.Cryptography;

namespace UrlShortener.Application;

public static class CodeGenerator
{
    public const int Length = 7;
    public const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public static string Generate() => RandomNumberGenerator.GetString(Alphabet, Length);
}
