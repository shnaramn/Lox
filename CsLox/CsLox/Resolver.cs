namespace Shnaramn.Lox;

public class Resolver : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
    private enum FunctionType
    {
        None,
        Function
    }

    private Stack<Dictionary<string, bool>> _scopes = new Stack<Dictionary<string, bool>>();
    private FunctionType _currentFunction = FunctionType.None;
    private readonly Interpreter _interpreter;

    public Resolver(Interpreter interpreter)
    {
        _interpreter = interpreter;
    }

    public void Resolve(List<Stmt> statements)
    {
        foreach (var statement in statements)
        {
            Resolve(statement);
        }
    }

    public void Resolve(Stmt statement)
    {
        statement.Accept(this);
    }

    public void Resolve(Expr statement)
    {
        statement.Accept(this);
    }

    public object VisitAssignExpr(Expr.Assign expr)
    {
        Resolve(expr.Value);
        ResolveLocal(expr, expr.Name);
        return null;
    }

    public object VisitBinaryExpr(Expr.Binary expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return null;
    }

    public object VisitBlockStmt(Stmt.Block stmt)
    {
        BeginScope();
        Resolve(stmt.Statements);
        EndScope();
        return null;
    }

    public object VisitCallExpr(Expr.Call expr)
    {
        Resolve(expr.Callee);
        foreach (var arg in expr.Arguments)
        {
            Resolve(arg);
        }
        return null;
    }

    public object VisitExpressionStmt(Stmt.Expression stmt)
    {
        Resolve(stmt.ExpressionStmt);
        return null;
    }

    public object VisitFunctionStmt(Stmt.Function stmt)
    {
        Declare(stmt.Name);
        Define(stmt.Name);

        ResolveFunction(stmt, FunctionType.Function);
        return null;
    }

    public object VisitGroupingExpr(Expr.Grouping expr)
    {
        Resolve(expr.Expression);
        return null;
    }

    public object VisitIfStmt(Stmt.If stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.ThenBranch);

        if (stmt.ElseBranch != null)
        {
            Resolve(stmt.ElseBranch);
        }
        return null;
    }

    public object VisitLiteralExpr(Expr.Literal expr)
    {
        return null;
    }

    public object VisitLogicalExpr(Expr.Logical expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return null;
    }

    public object VisitPrintStmt(Stmt.Print stmt)
    {
        Resolve(stmt.ExpressionPrint);
        return null;
    }

    public object VisitReturnStmt(Stmt.Return stmt)
    {
        if (_currentFunction == FunctionType.None)
        {
            CsLox.Error(stmt.Keyword, "Can't return from top-level code.");
        }

        if (stmt.Value != null)
        {
            Resolve(stmt.Value);
        }
        return null;
    }

    public object VisitUnaryExpr(Expr.Unary expr)
    {
        Resolve(expr.Right);
        return null;
    }

    public object VisitVarExpr(Expr.Var expr)
    {
        if (_scopes.Count != 0 && _scopes.Peek()[expr.Name.Lexeme] == false)
        {
            CsLox.Error(expr.Name, "Can't read local variable in its own initializer.");
        }

        ResolveLocal(expr, expr.Name);
        return null;
    }

    public object VisitVarStmt(Stmt.Var stmt)
    {
        Declare(stmt.Name);
        if (stmt.Initializer != null)
        {
            Resolve(stmt.Initializer);
        }
        Define(stmt.Name);
        return null;
    }

    public object VisitWhileStmt(Stmt.While stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.Body);
        return null;
    }

    private void BeginScope()
    {
        _scopes.Push(new Dictionary<string, bool>());
    }

    private void EndScope()
    {
        _scopes.Pop();
    }

    private void ResolveLocal(Expr expr, Token name)
    {
        var array = _scopes.ToArray();

        for (int i = 0; i < array.Count(); ++i)
        {
            if (array[i].ContainsKey(name.Lexeme))
            {
                _interpreter.Resolve(expr, i);
                return;
            }
        }
    }

    private void ResolveFunction(Stmt.Function stmt, FunctionType functionType)
    {
        var enclosingFunction = _currentFunction;
        _currentFunction = functionType;

        BeginScope();
        foreach (var parameter in stmt.Params)
        {
            Declare(parameter);
            Define(parameter);
        }

        Resolve(stmt.Body);
        EndScope();

        _currentFunction = enclosingFunction;
    }

    private void Declare(Token name)
    {
        if (_scopes.Count == 0) return;

        var scope = _scopes.Peek();
        if (scope.ContainsKey(name.Lexeme))
        {
            CsLox.Error(name, "Already a variable with this name in this scope.");
        }

        scope.Add(name.Lexeme, false);
    }

    private void Define(Token name)
    {
        if (_scopes.Count == 0) return;
        _scopes.Peek()[name.Lexeme] = true;
    }

    public object VisitClassStmt(Stmt.Class stmt)
    {
        Declare(stmt.Name);
        Define(stmt.Name);
        return null;
    }
}