using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using System.Collections;
using Parser;
using System.Windows.Media;
using TypeSystem;
using StaDynLanguage.StaDynAST;
using StaDynLanguage.Utils;
using StaDynLanguage.Visitors;
using AST;
using ErrorManagement;
using System.IO;
using TypeSystem.Operations;
using StaDynLanguage.TypeSystem;
using StaDynLanguage.Intellisense;

namespace StaDynLanguage
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("StaDynLanguage")]
    [Name("StaDynCompletion")]
    class StaDynCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        private IGlyphService GlyphService { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new StaDynCompletionSource(textBuffer, GlyphService);
        }
    }

    class StaDynCompletionSource : ICompletionSource
    {
        private ITextBuffer _buffer;
        private bool _disposed = false;
        private IGlyphService _glyphService;

        public StaDynCompletionSource(ITextBuffer buffer, IGlyphService glyphService)
        {
            _buffer = buffer;
            this._glyphService = glyphService;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (_disposed)
                throw new ObjectDisposedException("StaDynCompletionSource");

            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(snapshot);

            if (triggerPoint == null)
            {
                session.Dismiss();
                return;
            }

            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = triggerPoint;

            System.Drawing.Point caretPosition = FileUtilities.Instance.getCurrentCaretPoint();

            int column = caretPosition.Y;

            while (start > line.Start && !char.IsWhiteSpace((start - 1).GetChar()) && !char.IsPunctuation((start - 1).GetChar()))
            {
                start -= 1;
            }

            var applicableTo = snapshot.CreateTrackingSpan(new SnapshotSpan(start, triggerPoint), SpanTrackingMode.EdgeInclusive);

            char previousChar = snapshot.GetText(triggerPoint.Position - 1, 1)[0];

            List<Completion> allCompletions = new List<Completion>();

            if (previousChar == '.')
            {
                
                List<Completion> memberCompletion = this.memberCompletion(snapshot, triggerPoint.Position - 1, line.LineNumber + 1, column);


                //applicableTo = snapshot.CreateTrackingSpan(new SnapshotSpan(start , 1), SpanTrackingMode.EdgeInclusive);

                if (memberCompletion != null && memberCompletion.Count > 0)
                {
                    completionSets.Add(new CompletionSet("Members", "Members", applicableTo, memberCompletion, Enumerable.Empty<Completion>()));
                    allCompletions.AddRange(memberCompletion);
                }
            }
            else
            {

                List<Completion> keywords = this.getKeyWordsCompletionList();
                List<Completion> types = this.getTypesCompletionList();
                List<Completion> variablesInScope = this.getVariablesInScope(line.LineNumber, column, snapshot, start);
                List<Completion> currentClassMembers = this.getCurrentClassMemebers(line.LineNumber, column);

                if (keywords.Count > 0)
                {
                    completionSets.Add(new CompletionSet("Keywords", "Keywords", applicableTo, keywords, Enumerable.Empty<Completion>()));
                    allCompletions.AddRange(keywords);
                }

                if (types.Count > 0)
                {
                    completionSets.Add(new CompletionSet("Types", "Types", applicableTo, types, Enumerable.Empty<Completion>()));
                    allCompletions.AddRange(types);
                }

                if (variablesInScope != null && variablesInScope.Count > 0)
                {
                    completionSets.Add(new CompletionSet("VariablesInScope", "VariablesInScope", applicableTo, variablesInScope, Enumerable.Empty<Completion>()));
                    allCompletions.AddRange(variablesInScope);
                }

                if (currentClassMembers != null && currentClassMembers.Count > 0)
                {
                    completionSets.Add(new CompletionSet("ClassMembers", "ClassMembers", applicableTo, currentClassMembers, Enumerable.Empty<Completion>()));
                    allCompletions.AddRange(currentClassMembers);
                }
            }

            if (allCompletions.Count > 0)
            {
                allCompletions.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText));

                //Remove duplicates
                List<Completion> unique = new List<Completion>(allCompletions.Distinct<Completion>(new CompletionComparer()));
                //Insert at first
                completionSets.Insert(0, new CompletionSet("All", "All", applicableTo, unique, Enumerable.Empty<Completion>()));
            }
            else
            {
                session.Dismiss();
                session = null;
            }

        }


        private List<Completion> getKeyWordsCompletionList()
        {
            List<Completion> keywords = new List<Completion>();

            ICollection valueColl = CSharpLexer.keywordsTable.Values;

            //Image icon = CompletionGlyph.Instance.getIcon(Accessibility.Public, Element.Property);

            ImageSource image = CompletionGlyph.Instance.getImage(StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.GlyphItemPublic, this._glyphService);

            foreach (string command in valueColl)
            {
                keywords.Add(new Completion(command, command, command, image, command));
            }

            //Sort the elements
            keywords.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText));

            return keywords;
        }

        private List<Completion> getTypesCompletionList()
        {
            List<Completion> typeList = new List<Completion>();

            Dictionary<string, TypeExpression> types = TypeTable.Instance.getAllTypes();

            //Image icon = CompletionGlyph.Instance.getIcon(Accessibility.Public, Element.Class);

            ImageSource image = CompletionGlyph.Instance.getImage(StandardGlyphGroup.GlyphGroupClass, StandardGlyphItem.GlyphItemPublic, this._glyphService);

            foreach (KeyValuePair<string, TypeExpression> type in types)
            {
                string[] typeParts = type.Key.Split('.');
                typeList.Add(new Completion(typeParts[typeParts.Length - 1], typeParts[typeParts.Length - 1], type.Value.ToString(), image, type.Key));
            }

            //Sort the elements
            typeList.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText));

            return typeList;
        }

        private List<Completion> getVariablesInScope(int lineNumber, int column, ITextSnapshot snapshot, int start)
        {
            List<Completion> variablesInScope = new List<Completion>();

            ///*
            // * The foundNode is the next node that we actually are looking for.
            // * So, we must pop as many groups of variables as closing braces we found
            // * between foundeNode location and the location where the completion was triggered.
            // */

            //int blocksToPop = SourceHelper.countCharAppearances(snapshot, start, foundNode.Location.Line, foundNode.Location.Column, '}');

            StaDynVariablesInScopeTable infoVariablesInScope = StaDynIntellisenseHelper.Instance.getVariablesInCurrentScope(lineNumber, column, snapshot, start, false);

            if (infoVariablesInScope == null) return variablesInScope;
            //for (int i = 0; i < infoVariablesInScope.Table.Count; i++)
            //{
            //    if (infoVariablesInScope.Table[i].Count > 0)
            //    {
            //        foreach (KeyValuePair<string, TypeExpression> variable in infoVariablesInScope.Table[i])
            //        {
            //            //icon = CompletionGlyph.Instance.getIcon(variable.Value);
            //            ImageSource image = CompletionGlyph.Instance.getImage(variable.Value, this._glyphService);
            //            variablesInScope.Add(new Completion(variable.Key, variable.Key, variable.Value.FullName, image, variable.Key));
            //        }
            //    }
            //}
            string description;
            for (int i = 0; i < infoVariablesInScope.Table.Count; i++)
            {
                if (infoVariablesInScope.Table[i].Count > 0)
                {
                    foreach (KeyValuePair<string, IdDeclaration> variable in infoVariablesInScope.Table[i])
                    {
                        //icon = CompletionGlyph.Instance.getIcon(variable.Value);
                        description = variable.Value.TypeExpr != null ? variable.Value.TypeExpr.FullName : "";

                        ImageSource image = CompletionGlyph.Instance.getImage(variable.Value.TypeExpr, this._glyphService);
                        variablesInScope.Add(new Completion(variable.Key, variable.Key, description, image, variable.Key));
                    }
                }
            }

            //for (int i = 0; i < infoVariablesInScope.Table.Count - blocksToPop; i++)
            //{
            //    if (infoVariablesInScope.Table[i].Count > 0)
            //    {
            //        foreach (KeyValuePair<string, TypeExpression> variable in infoVariablesInScope.Table[i])
            //        {
            //            //icon = CompletionGlyph.Instance.getIcon(variable.Value);
            //            ImageSource image = CompletionGlyph.Instance.getImage(variable.Value, this._glyphService);
            //            variablesInScope.Add(new Completion(variable.Key, variable.Key, variable.Value.FullName, image, variable.Key));
            //        }
            //    }
            //}

            //Sort the elements
            variablesInScope.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText));

            return variablesInScope;

        }

        private List<Completion> getCurrentClassMemebers(int line, int column)
        {
            List<Completion> currentClassMembers = new List<Completion>();

            StaDynSourceFileAST file = ProjectFileAST.Instance.getAstFile(FileUtilities.Instance.getCurrentOpenDocumentFilePath());
            if (file==null || file.Ast == null) return null;

            //Location currentLocation = new Location(Path.GetFileName(file.FileName), line + 1, column);

            Location currentLocation = new Location(file.FileName, line + 1, column);

            ClassDefinition currentClass = StaDynIntellisenseHelper.Instance.getCurrentClass(currentLocation, file.Ast);

            if (currentClass == null) return currentClassMembers;

            Declaration member;
            string name;
            for (int i = 0; i < currentClass.MemberCount; i++)
            {
                member = currentClass.GetMemberElement(i);
                name = member.FullName;
                if (!String.IsNullOrEmpty(name))
                {
                    name = name.Substring(name.LastIndexOf(".") + 1);
                    ImageSource image = CompletionGlyph.Instance.getImage(member.TypeExpr, this._glyphService);
                    currentClassMembers.Add(new Completion(name, name, member.TypeExpr.FullName, image, name));
                }
            }

            //Sort the elements
            currentClassMembers.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText));

            return currentClassMembers;
        }



        private List<Completion> memberCompletion(ITextSnapshot snapshot, int start, int line, int column)
        {

            List<Completion> memberCompletion = new List<Completion>();
            AstNode foundNode = null;

            //Replace '.' with ';' to avoid parse errors
            Span dotSpan = new Span(start, 1);

            snapshot = snapshot.TextBuffer.Delete(dotSpan);

            snapshot = snapshot.TextBuffer.Insert(dotSpan.Start, ";");

            //StaDynParser parser = new StaDynParser(snapshot.TextBuffer, FileUtilities.Instance.getCurrentOpenDocumentFilePath());

            //Parse source
            StaDynSourceFileAST parseResult;// = parser.parseSource();
            StaDynParser parser = new StaDynParser();
            parser.parseAll();
            parseResult = ProjectFileAST.Instance.getAstFile(FileUtilities.Instance.getCurrentOpenDocumentFilePath());

            //Replace ';' with '.'
            snapshot = snapshot.TextBuffer.Delete(dotSpan);
            snapshot = snapshot.TextBuffer.Insert(dotSpan.Start, ".");

            //parseResult = DecorateAST.Instance.completeDecorateAndUpdate(parseResult);
            if (parseResult == null || parseResult.Ast == null)
                return memberCompletion;

            char previousChar = snapshot.GetText(start - 1, 1)[0];

            column += 1;
            //Previous node is a MthodCall or Cast
            if (previousChar == ')')
            {
                //Start point is the ')', not the '.', so start-1
                //foundNode = this.getCurrentInvocation(parseResult, snapshot, start - 1, line, column);
                foundNode = StaDynIntellisenseHelper.Instance.getCurrentInvocation(parseResult, snapshot, start - 1, line, column);
                if (!(foundNode is InvocationExpression) && !(foundNode is NewExpression))
                    foundNode = this.getCurrentCast(parseResult, snapshot, start - 1, line, column);
            }
            //Previous node is ArrayAcces
            else if (previousChar == ']')
            {
                foundNode = this.getCurrentArray(parseResult, snapshot, start, line, column);
            }
            else
            {
                //Node search
                //foundNode = (AstNode)parseResult.Ast.Accept(new VisitorFindNode(), new Location(Path.GetFileName(parseResult.FileName), line, column));
                foundNode = (AstNode)parseResult.Ast.Accept(new VisitorFindNode(), new Location(parseResult.FileName, line, column));
            }


            if (foundNode == null) return null;

            TypeExpression type = (TypeExpression)foundNode.AcceptOperation(new GetNodeTypeOperation(), null);
            if (type == null) return null;

            //Get the type members
            //double dispathcer pattern
            AccessModifier[] members = (AccessModifier[])type.AcceptOperation(new GetMembersOperation(), null);

            string displayName, description;
            bool duplicate = false;
            foreach (AccessModifier member in members)
            {
                duplicate = false;
                displayName = member.MemberIdentifier;
                description = displayName;

                if (member.Type != null)
                {
                    //description = member.Type.FullName;
                    description = member.Type.AcceptOperation(new GetTypeSystemName(), null) as string;
                    if (String.IsNullOrEmpty(description))
                        description = displayName;

                }
                ImageSource image = CompletionGlyph.Instance.getImage(member.Type, this._glyphService);

                //Completion element = new Completion("." + displayName, "." + displayName, description, image, displayName);
                
                Completion element = new Completion(displayName, "." + displayName, description, image, displayName);

                //Avoid adding duplicate members
                foreach (Completion completion in memberCompletion)
                {
                    if (completion.DisplayText == element.DisplayText)
                    {
                        if (completion.Description != description)
                            completion.Description += @" \/ " + description;
                        duplicate = true;
                        break;
                    }

                }
                if (!duplicate)
                    memberCompletion.Add(element);
                //memberCompletion.Add(element);
            }

            //Sort the elements
            memberCompletion.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText));

            //Remove duplicates
            //List<Completion> unique = new List<Completion>(memberCompletion.Distinct<Completion>(new CompletionComparer()));

            return memberCompletion;
        }


        private AstNode getCurrentArray(StaDynSourceFileAST parseResult, ITextSnapshot snapshot, int start, int line, int column)
        {
            VisitorFindArray vfa = new VisitorFindArray();
            //parseResult.Ast.Accept(vfa, new Location(Path.GetFileName(parseResult.FileName), line, column));
            parseResult.Ast.Accept(vfa, new Location(parseResult.FileName, line, column));
            Stack<AstNode> arrayStack = vfa.arrayStack;

            if (arrayStack == null) return null;

            int contCloseBracket = SourceHelper.countCharAppearances(snapshot, start, line - 1, 0, ']');

            AstNode currentArray = null;
            for (int i = 0; i < contCloseBracket; i++)
                currentArray = arrayStack.Pop();

            return currentArray;

        }

        private AstNode getCurrentCast(StaDynSourceFileAST parseResult, ITextSnapshot snapshot, int start, int line, int column)
        {
            VisitorFindCast vfc = new VisitorFindCast();
            //parseResult.Ast.Accept(vfc, new Location(Path.GetFileName(parseResult.FileName), line, column));
            parseResult.Ast.Accept(vfc, new Location(parseResult.FileName, line, column));
            Stack<AstNode> castStack = vfc.castStack;

            if (castStack == null || castStack.Count == 0) return null;

            int contCloseParent = SourceHelper.countCharAppearances(snapshot, start, line - 1, 0, ')');

            //SnapshotPoint externalCloseParenPosition = new SnapshotPoint(snapshot, start);
            //SnapshotSpan externalOpenParenPosition = new SnapshotSpan();

            //SourceHelper.FindMatchingOpenChar(externalCloseParenPosition, '(', ')', 0, out externalOpenParenPosition);
            //SnapshotPoint firtsOpenParentPosition = new SnapshotPoint(snapshot, externalOpenParenPosition.Start);
            //SnapshotSpan firtsCloseParenPosition = new SnapshotSpan();

            //SourceHelper.FindFirstAppearanceOf(firtsOpenParentPosition, ')', out firtsCloseParenPosition);

            //int columnDiference = start - firtsCloseParenPosition.Start;

            //AstNode foundNode = (AstNode)parseResult.Ast.Accept(new VisitorFindNode(), new Location(Path.GetFileName(parseResult.FileName), line, column - columnDiference));

            //return foundNode;
            contCloseParent--;
            if (contCloseParent > castStack.Count)
                return null;

            AstNode currentMethod = null;
            for (int i = 0; i < contCloseParent; i++)
                currentMethod = castStack.Pop();

            return currentMethod;

        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}

