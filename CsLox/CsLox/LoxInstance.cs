namespace Shnaramn.Lox;

internal class LoxInstance
{
    private readonly LoxClass _class;

    public LoxInstance(LoxClass loxClass)
    {
        _class = loxClass;
    }

    public override string ToString() => _class.Name + " instance";
}