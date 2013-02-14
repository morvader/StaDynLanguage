﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using antlr;
using StaDynLanguage.Utils;
using StaDynLanguage.StaDynTypeTable;
using StaDynLanguage.StaDynDynamic;
using StaDynLanguage.StaDynAST;
using StaDynLanguage_Project;

namespace StaDynLanguage
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("StaDynLanguage")]
    [TagType(typeof(StaDynTokenTag))]
    internal sealed class StaDynTokenTagProvider : ITaggerProvider
    {

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new StaDynTokenTagger(buffer) as ITagger<T>;
        }
    }

    public class StaDynTokenTag : ITag
    {
        public StaDynTokenTypes type { get; private set; }
        public IToken StaDynToken { get; private set; }

        public StaDynTokenTag(StaDynTokenTypes type, IToken token)
        {
            this.type = type;
            this.StaDynToken = token;
        }
    }

    internal sealed class StaDynTokenTagger : ITagger<StaDynTokenTag>
    {

        ITextBuffer _buffer;
        StaDynLexer lexer;

        internal StaDynTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            //_buffer.Changed += _buffer_Changed;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged = delegate { };


        //void _buffer_Changed(object sender, TextContentChangedEventArgs e)
        //{
        //    if (e.After != _buffer.CurrentSnapshot)
        //        return;

        //    OnTagsChanged(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length));
        //}

        //void OnTagsChanged(SnapshotSpan span)
        //{

        //    if (TagsChanged != null)
        //        TagsChanged(this, new SnapshotSpanEventArgs(span));
        //}

        public IEnumerable<ITagSpan<StaDynTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {

            var currentFile = ProjectFileAST.Instance.getAstFile(FileUtilities.Instance.getCurrentOpenDocumentFilePath());

            //Get the compileMode
            string dynOption = StaDyn.StaDynProject.ProjectConfiguration.Instance.GetProperty(PropertyTag.DynVarOption.ToString());
            DynVarOption compileMode = (DynVarOption)Enum.Parse(typeof(DynVarOption), dynOption);

            foreach (SnapshotSpan curSpan in spans)
            {
                ITextSnapshotLine containingLine = curSpan.Start.GetContainingLine();
                int curLoc = containingLine.Start.Position;
                int charsMatched = 0;
                int currentPositionInLine = 1;
                string tokenText = null;
                lexer = new StaDynLexer(containingLine.GetText());

                while (curLoc < containingLine.End)
                {
                    IToken StaDynToken = null;
                    try
                    {
                        StaDynToken = lexer.nextToken();
                    }
                    catch (Exception e)
                    {

                    }
                    if (StaDynToken == null)
                        break;
                    tokenText = StaDynToken.getText();

                    if (!string.IsNullOrEmpty(tokenText))
                    {
                        charsMatched = StaDynToken.getText().Length;
                        currentPositionInLine += StaDynToken.getText().Length;
                    }
                    else
                        charsMatched = 0;


                    //Sets the line number
                    StaDynToken.setLine(containingLine.LineNumber);
                    StaDynToken.setColumn(currentPositionInLine);

                    StaDynTokenTypes tokenType = lexer.getTokenType(StaDynToken.Type);


                    //Check if the current token its a type
                    //Needed for highlighting
                    if (tokenType == StaDynTokenTypes.Identifier && StaDynTypeTableFunctions.Instance.CheckTokenTypeOnCurrentDocument(StaDynToken))
                    {
                        tokenType = StaDynTokenTypes.TypeDefinition;
                    }

                    //If the compile mode is set to EverythingDynamic
                    //We hace to check wich AST identifires are varibales declared by user
                    //And then check if can ve dynamic
                    else if (tokenType == StaDynTokenTypes.Identifier && compileMode == DynVarOption.EverythingDynamic)
                    {
                       
                        if (currentFile == null) break;

                        StaDynLanguage.Visitors.StaDynVariablesInScopeTable infoVariablesInScope = StaDynLanguage.Intellisense.StaDynIntellisenseHelper.Instance.getVariablesInCurrentScope(StaDynToken.getLine(), StaDynToken.getColumn(), _buffer.CurrentSnapshot, 0, false);

                        for (int i = 0; i < infoVariablesInScope.Table.Count; i++)
                        {
                            if (infoVariablesInScope.Table[i].Count > 0)
                            {
                                foreach (KeyValuePair<string, AST.IdDeclaration> variable in infoVariablesInScope.Table[i])
                                {
                                    if (variable.Key == tokenText && variable.Value.Symbol != null)
                                    {
                                        //Only identifiers declared as var
                                        //HACK: Harcoded the "[Var
                                        if (variable.Value.Symbol.SymbolType.FullName.StartsWith("[Var"))
                                        {
                                            tokenType = StaDynTokenTypes.DynamicVar;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //Atmanaged compile mode
                    //Check if the current token its a dynamicVar
                    //Needed for highlighting
                    else if (tokenType == StaDynTokenTypes.Identifier && StaDynDynamicHelper.Instance.checkDynamicVar(StaDynToken.getText(), StaDynToken.getLine(), StaDynToken.getColumn(), currentFile) && compileMode==DynVarOption.Managed)
                    {
                        tokenType = StaDynTokenTypes.DynamicVar;
                    }

                    var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, charsMatched));
                    if (tokenSpan.IntersectsWith(curSpan))
                        yield return new TagSpan<StaDynTokenTag>(tokenSpan,
                                                              new StaDynTokenTag(tokenType, StaDynToken));

                    curLoc += charsMatched;
                }
            }
        }

        //public void refreshHighlighting(ITextBuffer textBuffer)
        //{

        //    //FileUtilities.Instance.refreshView();


        //    //string bck = textBuffer.CurrentSnapshot.GetText();
        //    //int cont = 0;
        //    //////SnapshotSpan span = new SnapshotSpan(textBuffer.CurrentSnapshot, 0, textBuffer.CurrentSnapshot.Length);
        //    //////NormalizedSnapshotSpanCollection spansCollection = new NormalizedSnapshotSpanCollection(span);
        //    //System.Drawing.Point caretPosition = FileUtilities.Instance.getCurrentCaretPosition();


        //    //_buffer.Delete(new Span(0, textBuffer.CurrentSnapshot.Length));
        //    //_buffer.Insert(0, bck);

        //    ////FileUtilities.Instance.setCaretPosition(caretPosition.X, caretPosition.Y);
        //    //_buffer.PostChanged += (s, e) =>
        //    //{
        //    //    //while (_buffer.EditInProgress) { }
        //    //    if (cont == 0)
        //    //        FileUtilities.Instance.setCaretPosition(caretPosition.X, caretPosition.Y);
        //    //    cont++;
        //    //};


        //    ITextBuffer currentBuffer = FileUtilities.Instance.getCurrentTextSnapShot().TextBuffer;

        //    foreach (ITextSnapshotLine line in currentBuffer.CurrentSnapshot.Lines)
        //    {
        //        currentBuffer.Insert(line.Start.Position, " ");
        //        currentBuffer.Delete(new Span(line.Start.Position, 1));
        //        // line.Snapshot.TextBuffer.Delete(new Span(0, 1));
        //    }

        //    //foreach (ITextSnapshotLine line in textBuffer.CurrentSnapshot.Lines)
        //    //{
        //    //    textBuffer.Insert(line.Start.Position," ");
        //    //    textBuffer.Delete(new Span(line.Start.Position, 1));
        //    //   // line.Snapshot.TextBuffer.Delete(new Span(0, 1));
        //    //}

        //}


    }
}

