namespace Shnaramn.Lox;

internal class LoxClass : ILoxCallable
{
    public readonly string Name;

    private Dictionary<string, LoxFunction> _methods;

    public LoxClass(string name, Dictionary<string, LoxFunction> methods)
    {
        this.Name = name;
        this._methods = methods;
    }

    public int Arity()
    {
        var initializer = FindMethod("init");
        return initializer == null ? 0 : initializer.Arity();
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        LoxInstance instance = new LoxInstance(this);

        LoxFunction initializer = FindMethod("init");
        if (initializer != null)
        {
            initializer.Bind(instance).Call(interpreter, arguments);
        }

        return instance;
    }

    public LoxFunction FindMethod(string name) =>
        _methods.ContainsKey(name) ? _methods[name] : null;

    public override string ToString() => Name;
}