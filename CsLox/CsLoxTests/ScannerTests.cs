
namespace Shnaramn.Lox.UnitTests;

[TestClass]
public class ScannerTests
{
    [TestMethod]
    public void TestScanMathSymbols()
    {
        var scanner = new Lox.Scanner("+-*/");
        var result = scanner.GetTokens();

        foreach (var token in result)
        {
            Console.WriteLine(token);
        }

        CompareWithExpectedTokens(
            new Lox.TokenType[]
            {
                Lox.TokenType.PLUS,
                Lox.TokenType.MINUS,
                Lox.TokenType.STAR,
                Lox.TokenType.SLASH,
                Lox.TokenType.EOF
            },
            result);
    }

    [TestMethod]
    public void TestScanBraces()
    {
        var scanner = new Shnaramn.Lox.Scanner("(){}");
        var result = scanner.GetTokens();

        foreach (var token in result)
        {
            Console.WriteLine(token);
        }

        CompareWithExpectedTokens(
            new Lox.TokenType[]
            {
                Lox.TokenType.PAREN_LEFT,
                Lox.TokenType.PAREN_RIGHT,
                Lox.TokenType.BRACE_LEFT,
                Lox.TokenType.BRACE_RIGHT,
                Lox.TokenType.EOF
            },
            result);
    }

    [TestMethod]
    public void TestScanIdentifiers()
    {
        var scanner = new Lox.Scanner("var i = 0;");
        var result = scanner.GetTokens();

        foreach (var token in result)
        {
            Console.WriteLine(token);
        }

        CompareWithExpectedTokens(
            new Lox.TokenType[]
            {
                Lox.TokenType.VAR,
                Lox.TokenType.IDENTIFIER,
                Lox.TokenType.EQUAL,
                Lox.TokenType.NUMBER,
                Lox.TokenType.SEMICOLON,
                Lox.TokenType.EOF
            },
            result);
    }

    [TestMethod]
    public void TestScanKeywords()
    {
        var scanner = new Lox.Scanner("while true or false");
        var result = scanner.GetTokens();

        foreach (var token in result)
        {
            Console.WriteLine(token);
        }

        CompareWithExpectedTokens(
            new Lox.TokenType[]
            {
                Lox.TokenType.WHILE,
                Lox.TokenType.TRUE,
                Lox.TokenType.OR,
                Lox.TokenType.FALSE,
                Lox.TokenType.EOF
            },
            result);
    }

    [TestMethod]
    public void TestScanNumbers()
    {
        var scanner = new Lox.Scanner("100 99.9");
        var result = scanner.GetTokens();

        foreach (var token in result)
        {
            Console.WriteLine(token);
        }

        CompareWithExpectedTokens(
            new Lox.TokenType[]
            {
                Lox.TokenType.NUMBER,
                Lox.TokenType.NUMBER,
                Lox.TokenType.EOF
            },
            result);
    }

    [TestMethod]
    public void TestScanStringLiterals()
    {
        var scanner = new Lox.Scanner("stringLiteral another literal");
        var result = scanner.GetTokens();

        foreach (var token in result)
        {
            Console.WriteLine(token);
        }

        CompareWithExpectedTokens(
            new Lox.TokenType[]
            {
                Lox.TokenType.IDENTIFIER,
                Lox.TokenType.IDENTIFIER,
                Lox.TokenType.IDENTIFIER,
                Lox.TokenType.EOF
            },
            result);
    }

    private static void CompareWithExpectedTokens(
        IEnumerable<Lox.TokenType> expectedTokens,
        IEnumerable<Token> actualTokens)
    {
        Assert.AreEqual(expectedTokens.Count(), actualTokens.Count());

        for (int i = 0; i < expectedTokens.Count(); ++i)
        {
            Assert.AreEqual(expectedTokens.ElementAt(i), actualTokens.ElementAt(i).Type);
        }
    }
}