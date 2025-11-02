
namespace Shnaramn.Lox;

public class Return: Exception
{
    public readonly object Value;

    public Return(object value)
    {
        Value = value;
    }
}