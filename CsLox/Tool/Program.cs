
using System.Text;

namespace Shnaramn.Lox
{
    internal class Tool
    {
        public static void Main(string[] args)
        {
            var outputDir = @"/Users/shnara/Code/Lox/CsLox/CsLox";
            DefineAst(
                outputDir, "Expr",
                new string[]
                {
                    "Binary   : Expr Left, Token Operator, Expr Right",
                    "Grouping : Expr Expression",
                    "Literal  : object Value",
                    "Unary    : Token Operator, Expr Right"
                });
        }

        private static void DefineAst(
            string outputDir,
            string baseName,
            IEnumerable<string> types)
        {
            string path = Path.Combine(outputDir, baseName + ".cs");
            StringBuilder output = new StringBuilder();
            int indentCount = 0;

            var prefix = () => new string(' ', indentCount * 4);
            var openBrace = () =>
            {
                output.AppendLine(prefix() + "{");
                ++indentCount;
            };

            var closeBrace = () =>
            {
                --indentCount;
                output.AppendLine(prefix() + "}");
            };

            var defineVisitor = (string baseName, IEnumerable<string> types) =>
            {
                output.AppendLine(prefix() + "public interface IVisitor<R>");
                openBrace();

                foreach (var type in types)
                {
                    var typeName = type.Split(":")[0].Trim();
                    output.AppendLine(prefix() + "R Visit" + typeName + baseName + "(" +
                        typeName + " " + baseName.ToLower() + ");");
                }

                closeBrace();
                output.AppendLine();

                 // The base Accept() method.
                output.AppendLine(prefix() + "public abstract R Accept<R>(IVisitor<R> visitor);");
                output.AppendLine();
            };

            var defineType = (string className, string fieldList) =>
            {
                output.AppendLine(prefix() + "public class " + className + " : " + baseName);
                openBrace();

                // Constructor.
                output.AppendLine(prefix() + "public " + className + "(" + fieldList + ")");
                openBrace();

                // Store parameters in fields.
                string[] fields = fieldList.Split(", ");
                foreach (var field in fields)
                {
                    string name = field.Split(" ")[1];
                    output.AppendLine(prefix() + "this." + name + " = " + name + ";");
                }

                closeBrace();

                // Visitor pattern.
                output.AppendLine();
                output.AppendLine(prefix() + "override public R Accept<R>(IVisitor<R> visitor) =>");
                output.AppendLine(prefix() + "    visitor.Visit" + className + baseName + "(this);");


                // Fields.
                output.AppendLine();
                foreach (string field in fields)
                {
                    output.AppendLine(prefix() + "public readonly " + field + ";");
                }

                closeBrace();
                output.AppendLine();
            };

            output.AppendLine("namespace Shnaramn.Lox");
            openBrace(); // Namespace

            output.AppendLine(prefix() + "public abstract class " + baseName);
            openBrace(); // Class

            defineVisitor(baseName, types);

            // The AST classes.
            foreach (var type in types)
            {
                var className = type.Split(":")[0].Trim();
                var fields = type.Split(":")[1].Trim();
                defineType(className, fields);
            }

            // Remove the additional new line.
            output.Remove(output.Length - 1, 1);

            closeBrace(); // Class
            closeBrace(); // Namespace

            File.WriteAllText(path, output.ToString());
        }
    }
}