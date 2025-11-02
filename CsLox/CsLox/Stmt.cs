namespace Shnaramn.Lox
{
    public abstract class Stmt
    {
        public interface IVisitor<R>
        {
            R VisitBlockStmt(Block stmt);
            R VisitExpressionStmt(Expression stmt);
            R VisitFunctionStmt(Function stmt);
            R VisitIfStmt(If stmt);
            R VisitPrintStmt(Print stmt);
            R VisitReturnStmt(Return stmt);
            R VisitVarStmt(Var stmt);
            R VisitWhileStmt(While stmt);
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

        public class Function : Stmt
        {
            public Function(Token Name, List<Token> Params, List<Stmt> Body)
            {
                this.Name = Name;
                this.Params = Params;
                this.Body = Body;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitFunctionStmt(this);

            public readonly Token Name;
            public readonly List<Token> Params;
            public readonly List<Stmt> Body;
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

        public class Return : Stmt
        {
            public Return(Token Keyword, Expr Value)
            {
                this.Keyword = Keyword;
                this.Value = Value;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitReturnStmt(this);

            public readonly Token Keyword;
            public readonly Expr Value;
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

        public class While : Stmt
        {
            public While(Expr Condition, Stmt Body)
            {
                this.Condition = Condition;
                this.Body = Body;
            }

            override public R Accept<R>(IVisitor<R> visitor) =>
                visitor.VisitWhileStmt(this);

            public readonly Expr Condition;
            public readonly Stmt Body;
        }
    }
}
