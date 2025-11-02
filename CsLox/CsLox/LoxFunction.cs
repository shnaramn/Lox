
namespace Shnaramn.Lox;

public class LoxFunction : ILoxCallable
{
    private readonly Stmt.Function _function;

    public LoxFunction(Stmt.Function function)
    {
        _function = function;
    }

    public int Arity() => _function.Params.Count;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        var environment = new Environment(interpreter.Globals);

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