
namespace Shnaramn.Lox;

internal class LoxFunction : ILoxCallable
{
    private readonly Stmt.Function _function;
    private readonly Environment _closure;

    public LoxFunction(Stmt.Function function, Environment closure)
    {
        _function = function;
        _closure = closure;
    }

    public LoxFunction Bind(LoxInstance instance)
    {
        Environment environment = new Environment(_closure);
        environment.DefineVariable("this", instance);
        return new LoxFunction(_function, environment);
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