namespace Shnaramn.Lox
{
    public class Token
    {
        public readonly TokenType Type;
        public readonly string Lexeme;
        public readonly object? Literal;
        public readonly int Line;

        public Token(
            TokenType tokenType,
            string lexeme,
            object? value,
            int line)
        {
            Type = tokenType;
            Lexeme = lexeme;
            Literal = value;
            Line = line;
        }

        public override string ToString()
        {
            return $"[{Type} {Lexeme}]";
        }
    }
}