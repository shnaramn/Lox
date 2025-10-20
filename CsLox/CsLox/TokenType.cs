
namespace Shnaramn.Lox
{
    public enum TokenType
    {
        // Single character tokens
        BRACE_LEFT,
        BRACE_RIGHT,
        COMMA,
        DOT,
        MINUS,
        PAREN_LEFT,
        PAREN_RIGHT,
        PLUS,
        SEMICOLON,
        SLASH,
        STAR,

        // One or two character tokens
        BANG,
        BANG_EQUAL,
        EQUAL,
        EQUAL_EQUAL,
        GREATER,
        GREATER_EQUAL,
        LESSER,
        LESSER_EQUAL,

        // Literals
        IDENTIFIER,
        STRING,
        NUMBER,

        // Keywords
        AND,
        CLASS,
        ELSE,
        FALSE,
        FUN,
        FOR,
        IF,
        NIL,
        OR,
        PRINT,
        RETURN,
        SUPER,
        THIS,
        TRUE,
        VAR,
        WHILE,

        // End of file/input.
        EOF
    }
}