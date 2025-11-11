namespace Shnaramn.Lox;

internal class LoxClass : ILoxCallable {
  public readonly string Name;

  public LoxClass(string name) {
    this.Name = name;
  }

    public int Arity() => 0;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        LoxInstance instance = new LoxInstance(this);
        return instance;
    }

    public override string ToString() => Name;
}