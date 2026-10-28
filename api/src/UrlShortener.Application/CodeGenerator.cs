using System.Security.Cryptography;

namespace UrlShortener.Application;

public class CodeGenerator : ICodeGenerator
{
    public const int Length = 7;
    public const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public string Generate() => RandomNumberGenerator.GetString(Alphabet, Length);
}
