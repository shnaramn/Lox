namespace Shnaramn.Lox
{
    public abstract class Expr
    {
        public interface IVisitor<R>
        {
            R VisitAssignExpr(Assign expr);
            R VisitBinaryExpr(Binary expr);
            R VisitGroupingExpr(Grouping expr);
            R VisitLiteralExpr(Literal expr);
            R VisitUnaryExpr(Unary expr);
            R VisitVarExpr(Var expr);
        }

        public abstract R Accept<R>(IVisitor<R> visitor);

        public class Assign : Expr
        {
            public Assign(Token Name, Expr Value)
            {
                this.Name = Name;
                this.Value = Value;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitAssignExpr(this);

            public readonly Token Name;
            public readonly Expr Value;
        }

        public class Binary : Expr
        {
            public Binary(Expr Left, Token Operator, Expr Right)
            {
                this.Left = Left;
                this.Operator = Operator;
                this.Right = Right;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitBinaryExpr(this);

            public readonly Expr Left;
            public readonly Token Operator;
            public readonly Expr Right;
        }

        public class Grouping : Expr
        {
            public Grouping(Expr Expression)
            {
                this.Expression = Expression;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitGroupingExpr(this);

            public readonly Expr Expression;
        }

        public class Literal : Expr
        {
            public Literal(object Value)
            {
                this.Value = Value;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitLiteralExpr(this);

            public readonly object Value;
        }

        public class Unary : Expr
        {
            public Unary(Token Operator, Expr Right)
            {
                this.Operator = Operator;
                this.Right = Right;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitUnaryExpr(this);

            public readonly Token Operator;
            public readonly Expr Right;
        }

        public class Var : Expr
        {
            public Var(Token Name)
            {
                this.Name = Name;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitVarExpr(this);

            public readonly Token Name;
        }
    }
}
