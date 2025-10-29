
namespace Shnaramn.Lox;

public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
    private Environment environment = new Environment();

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        } catch (RuntimeError error)
        {
            CsLox.RuntimeError(error);
        }
    }

    public object Execute(Stmt statement)
    {
        return statement.Accept(this);
    }

    public object Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    public object VisitBinaryExpr(Expr.Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);

            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);

            case TokenType.GREATER:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left > (double)right;

            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left >= (double)right;

            case TokenType.LESSER:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left < (double)right;

            case TokenType.LESSER_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left <= (double)right;

            case TokenType.MINUS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left - (double)right;

            case TokenType.PLUS:
                if (left is double && right is double)
                    return (double)left + (double)right;
                else if (left is string && right is string)
                    return (string)left + (string)right;

                throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings.");

            case TokenType.SLASH:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left / (double)right;

            case TokenType.STAR:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;

            default:
                return null;
        }
    }

    public object VisitGroupingExpr(Expr.Grouping expr) =>
        Evaluate(expr.Expression);

    public object VisitLiteralExpr(Expr.Literal expr) =>
        expr.Value;

    public object VisitUnaryExpr(Expr.Unary expr)
    {
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.BANG:
                return !IsTruthy(right);

            case TokenType.MINUS:
                CheckNumberOperand(expr.Operator, right);
                return -(double)right;

            default:
                return null;
        }
    }

    private void CheckNumberOperand(Token @operator, object operand)
    {
        if (operand is double) return;
        throw new RuntimeError(@operator, "Operand must be a number.");
    }

    private void CheckNumberOperands(Token @operator, object left, object right)
    {
        if (left is double && right is double) return;
        throw new RuntimeError(@operator, "Operand must be a number.");
    }

    private static bool IsTruthy(object val)
    {
        if (val is null) return false;
        if (val is bool) return (bool)val;
        if (val is double) return (double)val != 0;
        return true;
    }

    private static bool IsEqual(object a, object b)
    {
        if (a == null) return b == null;
        if (b == null) return false;
        return a.Equals(b);
    }

    private string Stringify(object obj)
    {
        if (obj == null) return "nil";

        if (obj is double)
        {
            string text = obj.ToString();
            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }

        return obj.ToString();
    }

    public object VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.ExpressionStmt);
        return null;
    }

    public object VisitPrintStmt(Stmt.Print stmt)
    {
        var value = Evaluate(stmt.ExpressionPrint);
        Console.WriteLine(Stringify(value));
        return null;
    }

    public object VisitVarExpr(Expr.Var expr) =>
        environment.Get(expr.Name);

    public object VisitVarStmt(Stmt.Var stmt)
    {
        var value = stmt.Initializer != null ? Evaluate(stmt.Initializer) : null;
        environment.DefineVariable(stmt.Name.Lexeme, value);
        return null;
    }

    public object VisitAssignExpr(Expr.Assign expr)
    {
        var val = Evaluate(expr.Value);
        environment.Assign(expr.Name, val);
        return null;
    }
}
