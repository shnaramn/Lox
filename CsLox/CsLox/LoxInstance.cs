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

        throw new RuntimeError(name, $"Undefined property '{name.Lexeme}'.");
    }

    public override string ToString() => _class.Name + " instance";
}