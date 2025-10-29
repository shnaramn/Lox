namespace Shnaramn.Lox;

public class Environment
{
    private readonly Dictionary<string, object> values = new Dictionary<string, object>();

    public object Get(Token name)
    {
        if (values.ContainsKey(name.Lexeme))
        {
            return values[name.Lexeme];
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

        throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }
}