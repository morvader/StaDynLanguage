using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Parser;
using Antlr.Runtime;
using System.IO;

    class Program {

        // * Testing program
        const string program = @"
/* Sample application
   to tests tokens */

using System;

namespace Prueba {
    class Hello {
        // One line comment
        public static void Main(String[] args) {
            double a = 3*2.34;
            Console.Write(Format(""Hello World"");
        }
    }
}
";

        static void Main(string[] args) {
            CSharpLexer lexer = new CSharpLexer(new StringReader(program));
            antlr.IToken token=null;
            while ((token = lexer.nextToken()).Type != CSharpLexer.EOF)
                Console.WriteLine("Token: '{0}', Type: {1}.", 
                    token.getText(), 
                    TokenClassification.Instance.getTokenType(token.Type)
                    );
        }
    }
