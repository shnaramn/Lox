namespace Shnaramn.Lox;

public class Environment
{
    public Environment Enclosing { get; set; }
    private readonly Dictionary<string, object> values = new Dictionary<string, object>();

    public Environment() { }

    public Environment(Environment enclosing)
    {
        this.Enclosing = enclosing;
    }

    public object Get(Token name)
    {
        if (values.ContainsKey(name.Lexeme))
        {
            return values[name.Lexeme];
        }

        if (Enclosing != null)
        {
            return Enclosing.Get(name);
        }

        throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }

    public void DefineVariable(string name, object value)
    {
        values[name] = value;
    }

    public void Assign(Token name, object val)
    {
        if (values.ContainsKey(name.Lexeme))
        {
            values[name.Lexeme] = val;
            return;
        }

        if (Enclosing != null)
        {
            Enclosing.Assign(name, val);
            return;
        }

        throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }
}