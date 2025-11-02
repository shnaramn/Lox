using Shnaramn.Lox;

public interface ILoxCallable
{
    object Call(Interpreter interpreter, List<object> arguments);

    int Arity();
}