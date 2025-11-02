
namespace Shnaramn.Lox;

public class Parser
{
    private class ParseError : Exception { }

    private IList<Token> _tokens;
    private int _current = 0;

    public Parser(IList<Token> tokens)
    {
        _tokens = tokens;
    }

    public List<Stmt> Parse()
    {
        var statements = new List<Stmt>();

        while (!IsAtEnd())
        {
            statements.Add(ParseDeclaration());
        }

        return statements;
    }

    private Stmt ParseDeclaration()
    {
        try
        {
            if (Match(TokenType.VAR))
                return ParseVarDeclaration();

            return ParseStatement();
        }
        catch (ParseError)
        {
            Synchronize();
            return null;
        }
    }

    private Stmt ParseVarDeclaration()
    {
        var name = Consume(TokenType.IDENTIFIER, "Expect variable name.");
        Expr initializer = null;

        if (Match(TokenType.EQUAL))
        {
            initializer = ParseExpression();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
        return new Stmt.Var(name, initializer);
    }

    private Stmt ParseStatement()
    {
        if (Match(TokenType.FOR)) return ParseForStatement();
        if (Match(TokenType.IF)) return ParseIfStatement();
        if (Match(TokenType.PRINT)) return ParsePrintStatement();
        if (Match(TokenType.BRACE_LEFT)) return ParseBlockStatement();
        if (Match(TokenType.WHILE)) return ParseWhileStatement();
        return ParseExpressionStatement();
    }

    private Stmt ParseForStatement()
    {
        Consume(TokenType.PAREN_LEFT, "Expect '(' after keyword 'for'.");

        Stmt initializer;
        if (Match(TokenType.SEMICOLON))
        {
            initializer = null;
        }
        else if (Match(TokenType.VAR))
        {
            initializer = ParseVarDeclaration();
        }
        else
        {
            initializer = ParseExpressionStatement();
        }

        Expr condition = null;
        if (!Check(TokenType.SEMICOLON))
        {
            condition = ParseExpression();
        }
        Consume(TokenType.SEMICOLON, "Expect ';' after condition.");

        Expr incrementor = null;
        if (!Check(TokenType.PAREN_RIGHT))
        {
            incrementor = ParseExpression();
        }
        Consume(TokenType.PAREN_RIGHT, "Expect ')' after for clauses.");

        var body = ParseStatement();

        if (incrementor != null)
        {
            body = new Stmt.Block(
                new List<Stmt>()
                {
                    body,
                    new Stmt.Expression(incrementor)
                });
        }

        if (condition == null) condition = new Expr.Literal(true);
        body = new Stmt.While(condition, body);

        if (initializer != null)
        {
            body = new Stmt.Block(
                new List<Stmt>()
                {
                    initializer,
                    body
                });
        }

        return body;
    }

    private Stmt ParseIfStatement()
    {
        Consume(TokenType.PAREN_LEFT, "Expect '(' after keyword 'if'.");
        var condition = ParseExpression();
        Consume(TokenType.PAREN_RIGHT, "Expect ')' after condition.");

        var thenBranch = ParseStatement();

        Stmt elseBranch = null;

        if (Match(TokenType.ELSE))
        {
            elseBranch = ParseStatement();
        }

        return new Stmt.If(condition, thenBranch, elseBranch);
    }

    private Stmt ParsePrintStatement()
    {
        Expr value = ParseExpression();
        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new Stmt.Print(value);
    }

    private Stmt ParseBlockStatement()
    {
        List<Stmt> statements = new List<Stmt>();
        while (!Check(TokenType.BRACE_RIGHT) && !IsAtEnd())
        {
            statements.Add(ParseDeclaration());
        }

        Consume(TokenType.BRACE_RIGHT, "Expect '}' after block.");

        return new Stmt.Block(statements);
    }

    private Stmt ParseWhileStatement()
    {
        Consume(TokenType.PAREN_LEFT, "Expect '( after 'while'.");
        var condition = ParseExpression();
        Consume(TokenType.PAREN_RIGHT, "Expect ')' after condition.");
        var body = ParseStatement();

        return new Stmt.While(condition, body);
    }
    private Stmt ParseExpressionStatement()
    {
        Expr value = ParseExpression();
        Consume(TokenType.SEMICOLON, "Expect ';' after value.");
        return new Stmt.Expression(value);
    }

    private Expr ParseExpression() => ParseAssignment();

    private Expr ParseAssignment()
    {
        var expr = ParseOrExpression();

        if (Match(TokenType.EQUAL))
        {
            var equals = Previous();
            var value = ParseAssignment();
            if (expr is Expr.Var)
            {
                Token name = ((Expr.Var)expr).Name;
                return new Expr.Assign(name, value);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private Expr ParseOrExpression()
    {
        var expr = ParseAndExpression();

        while (Match(TokenType.OR))
        {
            var @operator = Previous();
            var right = ParseAndExpression();
            expr = new Expr.Logical(expr, @operator, right);
        }

        return expr;
    }

    private Expr ParseAndExpression()
    {
        var expr = ParseEquality();

        while (Match(TokenType.AND))
        {
            var @operator = Previous();
            var right = ParseEquality();
            expr = new Expr.Logical(expr, @operator, right);
        }

        return expr;
    }

    private Expr ParseEquality()
    {
        var expr = ParseComparison();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            var @operator = Previous();
            var right = ParseComparison();
            expr = new Expr.Binary(expr, @operator, right);
        }

        return expr;
    }

    private Expr ParseComparison()
    {
        var expr = ParseTerm();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESSER, TokenType.LESSER_EQUAL))
        {
            var @operator = Previous();
            var right = ParseTerm();
            expr = new Expr.Binary(expr, @operator, right);
        }

        return expr;
    }

    private Expr ParseTerm()
    {
        var expr = ParseFactor();

        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            var @operator = Previous();
            var right = ParseFactor();
            expr = new Expr.Binary(expr, @operator, right);
        }

        return expr;
    }

     private Expr ParseFactor()
    {
        var expr = ParseUnary();

        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            var @operator = Previous();
            var right = ParseUnary();
            expr = new Expr.Binary(expr, @operator, right);
        }

        return expr;
    }

    private Expr ParseUnary()
    {
        if (Match(TokenType.MINUS, TokenType.BANG))
        {
            var @operator = Previous();
            var right = ParseUnary();
            return new Expr.Unary(@operator, right);
        }

        return ParseCall();
    }

    private Expr ParseCall()
    {
        var expr = ParsePrimary();

        while (true)
        {
            if (Match(TokenType.PAREN_LEFT))
            {
                expr = FinishCall(expr);
            }
            else
            {
                break;
            }
        }

        return expr;
    }

    private Expr ParsePrimary()
    {
        if (Match(TokenType.FALSE)) return new Expr.Literal(false);
        if (Match(TokenType.TRUE)) return new Expr.Literal(true);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        if (Match(TokenType.NIL)) return new Expr.Literal(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
#pragma warning disable CS8604 // Possible null reference argument.
            return new Expr.Literal(Previous().Literal);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        if (Match(TokenType.IDENTIFIER))
        {
            return new Expr.Var(Previous());
        }

        if (Match(TokenType.PAREN_LEFT))
        {
            Expr expr = ParseExpression();
            Consume(TokenType.PAREN_RIGHT, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }

        throw Error(Peek(), "Expect expression.");
    }

    private Expr FinishCall(Expr callee)
    {
        var arguments = new List<Expr>();

        if (!Check(TokenType.PAREN_RIGHT))
        {
            do
            {
                if (arguments.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 arguments.");
                }
                arguments.Add(ParseExpression());
            } while (Match(TokenType.COMMA));
        }

        Token paren = Consume(TokenType.PAREN_RIGHT, "Expect ')' after arguments.");

        return new Expr.Call(callee, paren, arguments);
    }

    private bool Match(params TokenType[] tokenTypes)
    {
        foreach (var tokenType in tokenTypes)
        {
            if (Check(tokenType))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }

    private bool Check(TokenType tokenType) =>
        IsAtEnd() ? false : (Peek().Type == tokenType);

    private Token Advance()
    {
        if (!IsAtEnd())
        {
            ++_current;
        }
        return Previous();
    }

    private bool IsAtEnd() => Peek().Type == TokenType.EOF;

    private Token Peek() => _tokens[_current];

    private Token Previous() => _tokens[_current - 1];

    private ParseError Error(Token token, String message)
    {
        CsLox.Error(token, message);
        return new ParseError();
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.SEMICOLON) return;

            switch (Peek().Type)
            {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.VAR:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }
}