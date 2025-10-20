namespace Shnaramn.Lox
{
    abstract class Expr
    {
        class Binary : Expr
        {
            public Binary(Expr Left, Token Operator, Expr Right)
            {
                this.Left = Left;
                this.Operator = Operator;
                this.Right = Right;
            }

            public readonly Expr Left;
            public readonly Token Operator;
            public readonly Expr Right;
        }

        class Grouping : Expr
        {
            public Grouping(Expr Expression)
            {
                this.Expression = Expression;
            }

            public readonly Expr Expression;
        }

        class Literal : Expr
        {
            public Literal(object Value)
            {
                this.Value = Value;
            }

            public readonly object Value;
        }

        class Unary : Expr
        {
            public Unary(Token Operator, Expr Right)
            {
                this.Operator = Operator;
                this.Right = Right;
            }

            public readonly Token Operator;
            public readonly Expr Right;
        }
    }
}
