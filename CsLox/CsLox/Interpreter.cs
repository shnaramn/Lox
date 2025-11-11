
namespace Shnaramn.Lox;

public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
    private readonly Dictionary<Expr, int> _locals = new Dictionary<Expr, int>();
    public readonly Environment Globals = new Environment();
    public Environment Environment { get; set; }

    public Interpreter()
    {
        Globals.DefineVariable("clock", new Clock());
        Environment = Globals;
    }

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError error)
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

    public void Resolve(Expr expr, int depth)
    {
        _locals[expr] = depth;
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
                return !IsTrue(right);

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

    private static bool IsTrue(object val)
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
        LookUpVariable(expr.Name, expr);

    public object VisitVarStmt(Stmt.Var stmt)
    {
        var value = stmt.Initializer != null ? Evaluate(stmt.Initializer) : null;
        Environment.DefineVariable(stmt.Name.Lexeme, value);
        return null;
    }

    public object VisitAssignExpr(Expr.Assign expr)
    {
        var val = Evaluate(expr.Value);
        var distance = _locals[expr];

        if (distance != null)
        {
            Environment.AssignAt(distance, expr.Name, val);
        }
        else
        {
            Globals.Assign(expr.Name, val);
        }

        return null;
    }

    public object VisitBlockStmt(Stmt.Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(this.Environment));
        return null;
    }

    public void ExecuteBlock(List<Stmt> statements, Environment environment)
    {
        Environment previous = this.Environment;
        try
        {
            this.Environment = environment;

            foreach (Stmt statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            this.Environment = previous;
        }
    }

    public object VisitIfStmt(Stmt.If stmt)
    {
        var value = Evaluate(stmt.Condition);

        if (IsTrue(value))
        {
            Execute(stmt.ThenBranch);
        }
        else if (stmt.ElseBranch != null)
        {
            Execute(stmt.ElseBranch);
        }

        return null;
    }

    public object VisitLogicalExpr(Expr.Logical expr)
    {
        var left = Evaluate(expr.Left);

        // Short cirtuit?
        if (expr.Operator.Type == TokenType.OR)
        {
            if (IsTrue(left)) return left;
        }
        else // And Operation
        {
            if (!IsTrue(left)) return left;
        }

        return Evaluate(expr.Right);
    }

    public object VisitWhileStmt(Stmt.While stmt)
    {
        while (IsTrue(Evaluate(stmt.Condition)))
        {
            Execute(stmt.Body);
        }

        return null;
    }

    public object VisitCallExpr(Expr.Call expr)
    {
        var callee = Evaluate(expr.Callee);

        var args = new List<object>();
        foreach (var arg in expr.Arguments)
        {
            args.Add(Evaluate(arg));
        }

        if (!(callee is ILoxCallable))
        {
            throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
        }

        ILoxCallable function = (ILoxCallable)callee;

        if (args.Count != function.Arity())
        {
            throw new RuntimeError(expr.Paren, $"Expected {function.Arity()} arguments but got {args.Count}.");
        }
        return function.Call(this, args);
    }

    public object VisitFunctionStmt(Stmt.Function stmt)
    {
        var function = new LoxFunction(stmt, Environment);
        Environment.DefineVariable(stmt.Name.Lexeme, function);
        return null;
    }

    public object VisitReturnStmt(Stmt.Return stmt)
    {
        var value = (stmt.Value == null) ? null : Evaluate(stmt.Value);
        throw new Return(value);
    }

    public object VisitClassStmt(Stmt.Class stmt)
    {
        Environment.DefineVariable(stmt.Name.Lexeme, null);
        LoxClass klass = new LoxClass(stmt.Name.Lexeme);
        Environment.Assign(stmt.Name, klass);
        return null;
    }

    private object LookUpVariable(Token name, Expr expr)
    {
        if (_locals.ContainsKey(expr))
        {
            var distance = _locals[expr];
            return Environment.GetAt(distance, name.Lexeme);
        }
        else
        {
            return Globals.Get(name);
        }
    }

    public object VisitGetExpr(Expr.Get expr)
    {
        var obj = Evaluate(expr.Object);
        if (obj is LoxInstance)
        {
            (obj as LoxInstance).Get(expr.Name);
        }

        throw new RuntimeError(expr.Name, "Only instances can have properties.");
    }
}
