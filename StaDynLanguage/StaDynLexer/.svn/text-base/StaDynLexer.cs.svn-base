using Parser;
using System.IO;
using antlr;

namespace StaDynLanguage
{
    class StaDynLexer
    {
        //private static readonly StaDynLexer instance = new StaDynLexer();

        private CSharpLexer lexer;
        private StaDynTokenResolver tokenClassification = StaDynTokenResolver.Instance;
        public const int parameterListStartChar = StaDynTokenResolver.parameterListStartChar;
        public const int parameterListEndChar = StaDynTokenResolver.parameterListEndChar;
        public const int parameterNextChar = StaDynTokenResolver.parameterNextChar;
        public const int parameterAccesMemberChar = StaDynTokenResolver.parameterAccesMemberChar;

        public StaDynLexer(string source)
        {
            lexer = new CSharpLexer(new StringReader(source));
        }

        public IToken nextToken()
        {
            //string s = lexer.getFilename();
           
            return lexer.nextToken();
          
        }

        public StaDynTokenTypes getTokenType(int tokenType)
        {
            return this.tokenClassification.getTokenType(tokenType);
        }

    }
}
