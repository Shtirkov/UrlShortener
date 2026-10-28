using UrlShortener.Application;

namespace UrlShortener.UnitTests;

public class CodeGeneratorTests
{
    [Fact]
    public void Generate_ReturnsBase62CodeOfFixedLength()
    {
        var code = CodeGenerator.Generate();

        Assert.Equal(CodeGenerator.Length, code.Length);
        Assert.All(code, c => Assert.Contains(c, CodeGenerator.Alphabet));
    }

    [Fact]
    public void Generate_ProducesDistinctCodes()
    {
        var codes = Enumerable.Range(0, 10_000).Select(_ => CodeGenerator.Generate()).ToHashSet();

        Assert.Equal(10_000, codes.Count);
    }
}
