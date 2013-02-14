using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using StaDynLanguage.Visitors;
using AST;
using StaDynLanguage.Intellisense;
using TypeSystem;
using System.Windows.Forms;
using System.Diagnostics;
using StaDynLanguage.TypeSystem;

namespace StaDynLanguage.Utils
{
    public static class SourceHelper
    {

        public static bool FindMatchingCloseChar(SnapshotPoint startPoint, char open, char close, int maxLines, out SnapshotSpan pairSpan)
        {
            pairSpan = new SnapshotSpan(startPoint.Snapshot, 1, 1);
            ITextSnapshotLine line = startPoint.GetContainingLine();
            string lineText = line.GetText();
            int lineNumber = line.LineNumber;
            int offset = startPoint.Position - line.Start.Position + 1;

            int stopLineNumber = startPoint.Snapshot.LineCount - 1;
            if (maxLines > 0)
                stopLineNumber = Math.Min(stopLineNumber, lineNumber + maxLines);

            int openCount = 0;
            while (true)
            {
                //walk the entire line
                while (offset < line.Length)
                {
                    char currentChar = lineText[offset];
                    if (currentChar == close) //found the close character
                    {
                        if (openCount > 0)
                        {
                            openCount--;
                        }
                        else    //found the matching close
                        {
                            pairSpan = new SnapshotSpan(startPoint.Snapshot, line.Start + offset, 1);
                            return true;
                        }
                    }
                    else if (currentChar == open) // this is another open
                    {
                        openCount++;
                    }
                    offset++;
                }

                //move on to the next line
                if (++lineNumber > stopLineNumber)
                    break;

                line = line.Snapshot.GetLineFromLineNumber(lineNumber);
                lineText = line.GetText();
                offset = 0;
            }

            return false;
        }
        public static bool FindMatchingOpenChar(SnapshotPoint startPoint, char open, char close, int maxLines, out SnapshotSpan pairSpan)
        {
            pairSpan = new SnapshotSpan(startPoint, startPoint);

            ITextSnapshotLine line = startPoint.GetContainingLine();

            int lineNumber = line.LineNumber;
            int offset = startPoint - line.Start - 1; //move the offset to the character before this one

            //if the offset is negative, move to the previous line
            if (offset < 0)
            {
                line = line.Snapshot.GetLineFromLineNumber(--lineNumber);
                offset = line.Length - 1;
            }

            string lineText = line.GetText();

            int stopLineNumber = 0;
            if (maxLines > 0)
                stopLineNumber = Math.Max(stopLineNumber, lineNumber - maxLines);

            int closeCount = 0;

            while (true)
            {
                // Walk the entire line
                while (offset >= 0)
                {
                    char currentChar = lineText[offset];

                    if (currentChar == open)
                    {
                        if (closeCount > 0)
                        {
                            closeCount--;
                        }
                        else // We've found the open character
                        {
                            pairSpan = new SnapshotSpan(line.Start + offset, 1); //we just want the character itself
                            return true;
                        }
                    }
                    else if (currentChar == close)
                    {
                        closeCount++;
                    }
                    offset--;
                }

                // Move to the previous line
                if (--lineNumber < stopLineNumber)
                    break;

                line = line.Snapshot.GetLineFromLineNumber(lineNumber);
                lineText = line.GetText();
                offset = line.Length - 1;
            }
            return false;
        }
        public static int countCharAppearances(ITextSnapshot snapshot, int start, int lineEnd, int columnEnd, char c)
        {
            //int length=snapshot.TextBuffer.GetReadOnlyExtents(new Span(0, snapshot.Length)).Max(region => region.End);
            int cont = 0, currentLine;
            var line = snapshot.GetLineFromPosition(start);

            currentLine = line.LineNumber;

            string textLine;

            while (currentLine >= lineEnd)
            {
                line = snapshot.GetLineFromLineNumber(currentLine);
                textLine = line.GetText();
                for (int i = line.Length - 1; i >= 0; i--)
                {
                    if (textLine[i] == c)
                        cont++;
                }
                currentLine -= 1;
                //line = snapshot.GetLineFromLineNumber(currentLine);
            }
            return cont;
        }
        public static bool FindFirstAppearanceOf(SnapshotPoint startPoint, char c, out SnapshotSpan pairSpan)
        {
            pairSpan = new SnapshotSpan(startPoint, startPoint);

            ITextSnapshotLine line = startPoint.GetContainingLine();

            string lineText = line.GetText();

            for (int i = 0; i < lineText.Length; i++)
            {
                // Walk the entire line

                char currentChar = lineText[i];

                if (currentChar == c)
                {
                    pairSpan = new SnapshotSpan(line.Start + i, 1); //we just want the character itself
                    return true;

                }

                //// Move to the previous line
                //if (--lineNumber < stopLineNumber)
                //    break;

                //line = line.Snapshot.GetLineFromLineNumber(lineNumber);
                //lineText = line.GetText();
                //offset = line.Length - 1;
            }
            return false;
        }

        public static string getCurrentWord()
        {
            //var currentTextView = FileUtilities.Instance.getCurrentTextView();
            var currentTextSnapShot = FileUtilities.Instance.getCurrentTextSnapShot();
            var currentTextSpan = FileUtilities.Instance.getCurrentTextSpan();

            //CaretPosition caretPosition = currentTextView.Caret.Position;    
            //SnapshotPoint? point = caretPosition.Point.GetPoint(currentTextSnapShot.TextBuffer, caretPosition.Affinity);

            var line = currentTextSnapShot.GetLineFromLineNumber(currentTextSpan.iStartLine);
            var word = line.GetText().Substring(currentTextSpan.iStartIndex, currentTextSpan.iEndIndex - currentTextSpan.iStartIndex);

            return word;

        }

        public static TextSpan getCurrentTextSpan()
        {
            return FileUtilities.Instance.getCurrentTextSpan();
        }

        public static void replaceWord(ITextSnapshot snapshot, int lineStart, string oldWord, string newWord)
        {

            var line = snapshot.GetLineFromLineNumber(lineStart);
            int position = line.Start.Position + line.GetText().IndexOf(oldWord);

            Span oldSpan = new Span(position, oldWord.Length);

            snapshot.TextBuffer.Replace(oldSpan, newWord);

        }

        public static void refreshHighlighting()
        {
            ITextSnapshot currentSnapShot = FileUtilities.Instance.getCurrentTextSnapShot();

            if (currentSnapShot != null && currentSnapShot.Length > 0 && currentSnapShot.ContentType.TypeName == "StaDynLanguage")
            {
                ITextBuffer currentBuffer = currentSnapShot.TextBuffer;

                foreach (ITextSnapshotLine line in currentBuffer.CurrentSnapshot.Lines)
                {
                    currentBuffer.Insert(line.Start.Position, " ");
                    currentBuffer.Delete(new Span(line.Start.Position, 1));
                    // line.Snapshot.TextBuffer.Delete(new Span(0, 1));
                }
            }
        }

        public static void refreshHighlighting(string file)
        {
            ITextSnapshot currentSnapShot = FileUtilities.Instance.GetIWpfTextView(file).TextSnapshot;
            
            if (currentSnapShot != null && currentSnapShot.Length > 0 && currentSnapShot.ContentType.TypeName == "StaDynLanguage")
            {
                ITextBuffer currentBuffer = currentSnapShot.TextBuffer;
                
                foreach (ITextSnapshotLine line in currentBuffer.CurrentSnapshot.Lines)
                {
                    currentBuffer.Insert(line.Start.Position, " ");
                    currentBuffer.Delete(new Span(line.Start.Position, 1));
                    // line.Snapshot.TextBuffer.Delete(new Span(0, 1));
                }
            }
        }

        public static void DeclareExplicit(AstNode node, bool showMessageBox)
        {

            if (!(node is IdDeclaration)) return;

            string varName = ((IdDeclaration)node).Identifier;
            int lineNumber = node.Location.Line;
            int column = node.Location.Column;
         
            var snapshot = FileUtilities.Instance.getCurrentTextSnapShot();

            //if (node is Definition) {
            //  if (((Definition)node).FrozenTypeExpression != null) {
            //    SourceHelper.replaceWord(snapshot, lineNumber - 1, "var", ((Definition)node).FrozenTypeExpression.FullName);
            //    return;
            //  }
            //}
           

           // int start = FileUtilities.Instance.getCurrentCaretPosition();
            int start = FileUtilities.Instance.getPositionFromSpan(lineNumber -1, column);
            StaDynVariablesInScopeTable infoVariablesInScope = StaDynIntellisenseHelper.Instance.getVariablesInCurrentScope(lineNumber + 1, column, snapshot, start, true);

            //var SSAVariablesList = new List<Declaration>();
            var substitutionTypes = new HashSet<TypeExpression>();
            //Look for the same variable in the current scope
            for (int i = 0; i < infoVariablesInScope.Table.Count; i++)
            {
                if (infoVariablesInScope.Table[i].Count > 0)
                {
                    foreach (KeyValuePair<string, IdDeclaration> variable in infoVariablesInScope.Table[i])
                    {
                        //string SSAVarName="";
                        //var type = variable.Value.TypeExpr;

                      var type = ((Declaration)variable.Value).ILTypeExpression;
                      
                        //if(variable.Key.Length>varName.Length)
                        //    SSAVarName = variable.Key.Substring(0, varName.Length);
                        //if (variable.Key.StartsWith(varName) && type is TypeVariable && ((TypeVariable)type).Substitution != null)
                       
                        //if (SSAVarName.Equals(varName) && type is TypeVariable)
                        if (variable.Value.Identifier == varName && type != null)
                        {
                            //SSAVariablesList.Add(variable.Value);
                          substitutionTypes.UnionWith(TypeSystemHelper.Instance.getSubstitutionType(type));
                        }

                    }
                }
            }
            //if (SSAVariablesList.Count != 1)
            if (substitutionTypes.Count != 1)
            {
                string message;
                if (substitutionTypes.Count == 0)
                    message = "The var reference named '" + varName +
                    "' cannot be declared explicitly since it has no type yet\n";
                else
                {
                    message = "The var reference named '" + varName +
                        "' cannot be declared explicitly since it has more than one type within its scope:\n";
                    foreach (TypeExpression type in substitutionTypes)
                        message += " - " + type.FullName + "\n";
                    message += "To be able to declare explicitly this reference, create a new type from which "
                        + "all this types will inherit";
                }

                Trace.WriteLine(message);

                if (showMessageBox)
                    MessageBox.Show(
                        message,
                        "Cannot declare explicit",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ////var t = substitutionTypes.ElementAt(0);
            //TypeExpression t= substitutionTypes.ToList()[0];
            //string description = t.AcceptOperation(new GetTypeSystemName(), null) as string;
            //var changeType = description.Split(':');

            //SourceHelper.replaceWord(snapshot, lineNumber - 1, "var", changeType[changeType.Length -1]);
            SourceHelper.replaceWord(snapshot, lineNumber - 1, "var", substitutionTypes.ElementAt(0).FullName);

        }

      
    }
}
