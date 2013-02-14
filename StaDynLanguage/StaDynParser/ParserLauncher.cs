using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using StaDynLanguage.StaDynAST;
using StaDynLanguage.Errors;
using StaDynLanguage.Utils;
using StaDynLanguage;

namespace StaDynLanguage
{
    /// <summary>
    /// Executes the parsing when the text buffer changes.
    /// </summary>
    internal sealed class ParserLauncher
    {
        //Parse complete statements only
        private string[] parseTriggers = {";","}","\n","\r\n"};
        private string fileName;

        private ITextBuffer textBuffer;
        internal StaDynParser Parser { get; private set; }

        internal ParserLauncher(ITextBuffer textBuffer, string fileName)
        {
            this.fileName = fileName;
            this.textBuffer = textBuffer;
            //this.Parser = new StaDynParser(textBuffer, fileName);
            this.Parser = new StaDynParser();
            textBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(TextBuffer_Changed);
        }

        private void TextBuffer_Changed(object source, TextContentChangedEventArgs e)
        {
            //Try to parse "correct" code only
            //Its more probably when user ends an statement
            string lastChar = e.Changes.Last().NewText;
            if (this.parseTriggers.Contains(lastChar)) 
                Parse();
        }

        private void Parse()
        {
            this.Parser.parseAll();

            //Refresh highligting if no erros
            if (StaDynErrorList.Instance.errorCount() == 0) {
              SourceHelper.refreshHighlighting();
            }
          
           
        }
    }
}