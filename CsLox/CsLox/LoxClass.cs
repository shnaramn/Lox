namespace Shnaramn.Lox;

internal class LoxClass : ILoxCallable
{
    public readonly string Name;

    private LoxClass _superClass = null;
    private Dictionary<string, LoxFunction> _methods;

    public LoxClass(string name, LoxClass superClass, Dictionary<string, LoxFunction> methods)
    {
        this.Name = name;
        this._superClass = superClass;
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

    public LoxFunction FindMethod(string name)
    {
        if (_methods.ContainsKey(name))
        {
            return _methods[name];
        }

        if (_superClass != null)
        {
            return _superClass.FindMethod(name);
        }

        return null;
    }

    public override string ToString() => Name;
}