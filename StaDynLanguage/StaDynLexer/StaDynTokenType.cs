using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaDynLanguage
{
    public enum StaDynTokenTypes : int
    {
        Text,
        Keyword,
        Identifier,
        String,
        Literal,
        Operator,
        Delimiter,
        WhiteSpace,
        LineComment,
        Comment,
        Number,
        TypeDefinition,
        DynamicVar,
    };

    public class StaDynTokenTypeResolver
    {
        IDictionary<StaDynTokenTypes, string> _StaDynTypes;

        private StaDynTokenTypeResolver()
        {
            _StaDynTypes = new Dictionary<StaDynTokenTypes, string>{
            {StaDynTokenTypes.Comment,"Comment"},
            {StaDynTokenTypes.Delimiter,"Delimiter"},
            {StaDynTokenTypes.Identifier,"Identifier"},
            {StaDynTokenTypes.Keyword,"Keyword"},
            {StaDynTokenTypes.LineComment,"lineComment"},
            {StaDynTokenTypes.Literal,"Literal"},
            {StaDynTokenTypes.Number,"Number"},
            {StaDynTokenTypes.Operator,"Operator"},
            {StaDynTokenTypes.String,"String"},
            {StaDynTokenTypes.Text,"Text"},
            {StaDynTokenTypes.WhiteSpace,"WhiteSpace"},
            {StaDynTokenTypes.TypeDefinition,"TypeDefinition"},
            {StaDynTokenTypes.DynamicVar,"DynamicVar"}
            };
        }

        private static StaDynTokenTypeResolver instance = new StaDynTokenTypeResolver();
        public static StaDynTokenTypeResolver Instance
        {
            get { return instance; }
        }

        public IDictionary<StaDynTokenTypes, string> getAllTypes()
        {
            return this._StaDynTypes;
        }

        public string getText(StaDynTokenTypes type)
        {
            string textValue=null;
            this._StaDynTypes.TryGetValue(type, out textValue);
            return textValue;
        }
    }
}
