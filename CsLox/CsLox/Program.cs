
namespace Shnaramn.Lox
{
    internal class CsLox
    {
        private static readonly Interpreter _interpreter = new Interpreter();
        private static bool _hadError = false;
        private static bool _hadRuntimeError = false;

        public static int Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                return 0;
            }
            else if (args.Length == 1)
            {
                return RunFile(args[0]);
            }
            else if (args.Length == 0)
            {
                RunPrompt();
            }

            return 0;
        }

        internal static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
            _hadError = true;
        }

        internal static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, " at '" + token.Lexeme + "'", message);
            }
        }

        internal static void RuntimeError(RuntimeError error)
        {
            Console.WriteLine(error.Message + "\n[line " + error.Token.Line + "]");
            _hadRuntimeError = true;
        }

        private static int RunFile(string filePath)
        {
            Run(File.ReadAllText(filePath));

            // Indicate an error in the exit code.
            if (_hadError) return -1; // TODO: Fix.
            if (_hadRuntimeError) return -1; // TODO: Fix.

            return 0;
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                var text = Console.ReadLine();
                if (text is null)
                {
                    return;
                }

                Run(text);
                _hadError = false;
            }
        }

        private static void Run(string inputText)
        {
            var scanner = new Scanner(inputText);
            var tokens = scanner.GetTokens();
            var parser = new Parser(tokens);
            var statements = parser.Parse();

            // Stop if there was a syntax error.
            if (_hadError) return;

            Resolver resolver = new Resolver(_interpreter);
            resolver.Resolve(statements);

            // Stop if there was a resolution error.
            if (_hadError) return;

            _interpreter.Interpret(statements);
        }
    }
}