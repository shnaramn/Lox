
namespace Shnaramn.Lox;

internal class LoxFunction : ILoxCallable
{
    private readonly Stmt.Function _function;
    private readonly Environment _closure;
    private readonly bool _isInitializer;

    public LoxFunction(Stmt.Function function, Environment closure, bool isInitializer)
    {
        _function = function;
        _closure = closure;
        _isInitializer = isInitializer;
    }

    public LoxFunction Bind(LoxInstance instance)
    {
        Environment environment = new Environment(_closure);
        environment.DefineVariable("this", instance);
        return new LoxFunction(_function, environment, _isInitializer);
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
            if (_isInitializer) return _closure.GetAt(0, "this");

            return result.Value;
        }

        if (_isInitializer) return _closure.GetAt(0, "this");

        return null;
    }
}