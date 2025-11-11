namespace Shnaramn.Lox
{
    public abstract class Expr
    {
        public interface IVisitor<R>
        {
            R VisitAssignExpr(Assign expr);
            R VisitBinaryExpr(Binary expr);
            R VisitCallExpr(Call expr);
            R VisitGetExpr(Get expr);
            R VisitGroupingExpr(Grouping expr);
            R VisitLiteralExpr(Literal expr);
            R VisitLogicalExpr(Logical expr);
            R VisitSetExpr(Set expr);
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

        public class Call : Expr
        {
            public Call(Expr Callee, Token Paren, List<Expr> Arguments)
            {
                this.Callee = Callee;
                this.Paren = Paren;
                this.Arguments = Arguments;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitCallExpr(this);

            public readonly Expr Callee;
            public readonly Token Paren;
            public readonly List<Expr> Arguments;
        }

        public class Get : Expr
        {
            public Get(Expr Object, Token Name)
            {
                this.Object = Object;
                this.Name = Name;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitGetExpr(this);

            public readonly Expr Object;
            public readonly Token Name;
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

        public class Logical : Expr
        {
            public Logical(Expr Left, Token Operator, Expr Right)
            {
                this.Left = Left;
                this.Operator = Operator;
                this.Right = Right;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitLogicalExpr(this);

            public readonly Expr Left;
            public readonly Token Operator;
            public readonly Expr Right;
        }

        public class Set : Expr
        {
            public Set(Expr Object, Token Name, Expr Value)
            {
                this.Object = Object;
                this.Name = Name;
                this.Value = Value;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitSetExpr(this);

            public readonly Expr Object;
            public readonly Token Name;
            public readonly Expr Value;
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
