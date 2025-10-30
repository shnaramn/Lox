namespace Shnaramn.Lox
{
    public abstract class Stmt
    {
        public interface IVisitor<R>
        {
            R VisitBlockStmt(Block stmt);
            R VisitExpressionStmt(Expression stmt);
            R VisitIfStmt(If stmt);
            R VisitPrintStmt(Print stmt);
            R VisitVarStmt(Var stmt);
        }

        public abstract R Accept<R>(IVisitor<R> visitor);

        public class Block : Stmt
        {
            public Block(List<Stmt> Statements)
            {
                this.Statements = Statements;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitBlockStmt(this);

            public readonly List<Stmt> Statements;
        }

        public class Expression : Stmt
        {
            public Expression(Expr ExpressionStmt)
            {
                this.ExpressionStmt = ExpressionStmt;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitExpressionStmt(this);

            public readonly Expr ExpressionStmt;
        }

        public class If : Stmt
        {
            public If(Expr Condition, Stmt ThenBranch, Stmt ElseBranch)
            {
                this.Condition = Condition;
                this.ThenBranch = ThenBranch;
                this.ElseBranch = ElseBranch;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitIfStmt(this);

            public readonly Expr Condition;
            public readonly Stmt ThenBranch;
            public readonly Stmt ElseBranch;
        }

        public class Print : Stmt
        {
            public Print(Expr ExpressionPrint)
            {
                this.ExpressionPrint = ExpressionPrint;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitPrintStmt(this);

            public readonly Expr ExpressionPrint;
        }

        public class Var : Stmt
        {
            public Var(Token Name, Expr Initializer)
            {
                this.Name = Name;
                this.Initializer = Initializer;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitVarStmt(this);

            public readonly Token Name;
            public readonly Expr Initializer;
        }
    }
}
