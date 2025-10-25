using System.Text;

namespace Shnaramn.Lox;

public class AstPrinter : Expr.Visitor<string>
{
    public string Print(Expr expr) => expr.Accept(this);

    public string VisitBinaryExpr(Expr.Binary expr) =>
        Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);

    public string VisitGroupingExpr(Expr.Grouping expr) =>
        Parenthesize("group", expr.Expression);

#pragma warning disable CS8603 // Possible null reference return.
    public string VisitLiteralExpr(Expr.Literal expr) =>
        expr?.Value is null ? "nil" : expr.Value.ToString();
#pragma warning restore CS8603 // Possible null reference return.

    public string VisitUnaryExpr(Expr.Unary expr) =>
        Parenthesize(expr.Operator.Lexeme, expr.Right);

    private string Parenthesize(string name, params Expr[] exprs)
    {
        StringBuilder builder = new StringBuilder();

        builder
            .Append("(")
            .Append(name);

        foreach (Expr expr in exprs)
        {
            builder.Append(" ");
            builder.Append(expr.Accept(this));
        }

        builder.Append(")");

        return builder.ToString();
    }
}