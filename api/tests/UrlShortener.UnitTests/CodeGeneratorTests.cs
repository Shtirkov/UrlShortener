using UrlShortener.Application;

namespace UrlShortener.UnitTests;

public class CodeGeneratorTests
{
    private readonly CodeGenerator _generator = new();

    [Fact]
    public void Generate_ReturnsBase62CodeOfFixedLength()
    {
        var code = _generator.Generate();

        Assert.Equal(CodeGenerator.Length, code.Length);
        Assert.All(code, c => Assert.Contains(c, CodeGenerator.Alphabet));
    }

    [Fact]
    public void Generate_ProducesDistinctCodes()
    {
        var codes = Enumerable.Range(0, 10_000).Select(_ => _generator.Generate()).ToHashSet();

        Assert.Equal(10_000, codes.Count);
    }
}
