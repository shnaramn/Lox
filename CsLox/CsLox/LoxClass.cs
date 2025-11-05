namespace Shnaramn.Lox;

internal class LoxClass {
  public readonly string Name;

  public LoxClass(string name) {
    this.Name = name;
  }

  public override string ToString() => Name;
}