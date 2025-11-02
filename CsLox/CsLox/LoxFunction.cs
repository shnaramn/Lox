
namespace Shnaramn.Lox;

public class LoxFunction : ILoxCallable
{
    private readonly Stmt.Function _function;
    private readonly Environment _closure;

    public LoxFunction(Stmt.Function function, Environment closure)
    {
        _function = function;
        _closure = closure;
    }

    public int Arity() => _function.Params.Count;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        var environment = new Environment(_closure);

        for (int i = 0; i < arguments.Count; ++i)
        {
            environment.DefineVariable(_function.Params[i].Lexeme, arguments[i]);
        }

        try
        {
            interpreter.ExecuteBlock(_function.Body, environment);
        }
        catch (Return result)
        {
            return result.Value;
        }

        return null;
    }
}