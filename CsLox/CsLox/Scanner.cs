

namespace Shnaramn.Lox
{
    public class Scanner
    {
        private string _text;
        private IList<Token> _tokens= new List<Token>();
        private int _line = 1;
        private int _current = 0;
        private int _start = 0;

        public Scanner(string text)
        {
            _text = text;
        }

        public IList<Token> GetTokens()
        {
            while (!IsEndOfText())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, string.Empty, null, _line));

            return _tokens;
        }

        private bool IsEndOfText()
        {
            return _current >= _text.Length;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '{': AddToken(TokenType.BRACE_LEFT); break;
                case '}': AddToken(TokenType.BRACE_RIGHT); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '(': AddToken(TokenType.PAREN_LEFT); break;
                case ')': AddToken(TokenType.PAREN_RIGHT); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;

                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL:TokenType.BANG); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL: TokenType.EQUAL); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case '<': AddToken(Match('=') ? TokenType.LESSER_EQUAL : TokenType.LESSER); break;

                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !IsEndOfText())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;

                // Ignore whitespace.
                case ' ':
                case '\t':
                case '\r':
                    break;

                case '\n':
                    ++_line;
                    break;

                case '"': GetStringLiteral(); break;

                default:
                    if (char.IsDigit(c))
                    {
                        GetNumericLiteral();
                    }
                    else if (char.IsAsciiLetter(c))
                    {
                        GetIdentifierOrKeyword();
                    }
                    else
                    {
                        CsLox.Error(_line, "Unexpected character.");
                    }
                break;
            }
        }

        private char Advance() => _text[_current++];

        private bool Match(char expected)
        {
            if (IsEndOfText() || _text[_current] != expected)
            {
                return false;
            }

            // Consume the match.
            ++_current;
            return true;
        }

        private char Peek() =>
            IsEndOfText() ? '\0' : _text[_current];

        private char PeekNext() =>
            (_current + 1) < _text.Length ? _text[_current + 1] : '\0';

        private void AddToken(TokenType tokenType) =>
            AddToken(tokenType, null);

        private void AddToken(TokenType tokenType, object? literal)
        {
            var text = _text.Substring(_start, _current - _start);
            _tokens.Add(new Token(tokenType, text, literal, _line));
        }

        private void GetStringLiteral()
        {
            while (!IsEndOfText() && Peek() != '"')
            {
                if (Peek() == '\n')
                {
                    ++_line;
                }
                Advance();
            }

            if (IsEndOfText())
            {
                CsLox.Error(_line, "Unterminated string.");
                return;
            }

            // Skip starting '"'
            var value = _text.Substring(_start + 1, _current - _start - 1);

            // Consume closing '"'.
            Advance();

            AddToken(TokenType.STRING, value);
        }

        private void GetNumericLiteral()
        {
            while (char.IsDigit(Peek()))
            {
                Advance();
            }

            if (Peek() == '.' && char.IsDigit(PeekNext()))
            {
                // Consume '.'
                Advance();

                while (char.IsDigit(Peek()))
                {
                    Advance();
                }
            }

            var lexeme = _text.Substring(_start, _current - _start);

            AddToken(TokenType.NUMBER, double.Parse(lexeme));
        }

        private void GetIdentifierOrKeyword()
        {
            while (char.IsAsciiLetterOrDigit(Peek()))
            {
                Advance();
            }

            var text = _text.Substring(_start, _current - _start);
            var tokenType = keywordMap.ContainsKey(text) ? keywordMap[text] : TokenType.IDENTIFIER;

            AddToken(tokenType);
        }

        private Dictionary<string, TokenType> keywordMap = new Dictionary<string, TokenType>
        {
            { "and", TokenType.AND },
            { "class",  TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "fun", TokenType.FUN },
            { "for", TokenType.FOR },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE }
        };
    }
}