
namespace Shnaramn.Lox;

public class Clock : ILoxCallable
{
    public int Arity() => 0;

    public object Call(Interpreter interpreter, List<object> arguments) =>
        DateTime.Now;
}