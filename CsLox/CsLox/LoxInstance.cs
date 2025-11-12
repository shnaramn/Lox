namespace Shnaramn.Lox;

internal class LoxInstance
{
    private readonly LoxClass _class;
    private readonly Dictionary<string, object> _fields;

    public LoxInstance(LoxClass loxClass)
    {
        _class = loxClass;
        _fields = new Dictionary<string, object>();
    }

    public object Get(Token name)
    {
        if (_fields.ContainsKey(name.Lexeme))
        {
            return _fields[name.Lexeme];
        }

        var method = _class.FindMethod(name.Lexeme);
        if (method != null)
        {
            return method;
        }

        throw new RuntimeError(name, $"Undefined property '{name.Lexeme}'.");
    }

    public void Set(Token name, object value)
    {
        _fields.Add(name.Lexeme, value);
    }

    public override string ToString() => _class.Name + " instance";
}