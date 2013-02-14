using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AST;
using StaDynLanguage.Visitors;
using StaDynLanguage.StaDynAST;
using Microsoft.VisualStudio.Text;
using StaDynLanguage.Utils;
using ErrorManagement;
using System.IO;
using antlr;
using DynVarManagement;

namespace StaDynLanguage.Intellisense
{
    public class StaDynIntellisenseHelper
    {
        //Implements Singleton
        static StaDynIntellisenseHelper instance = null;

        StaDynIntellisenseHelper()
        {
        }

        public static StaDynIntellisenseHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StaDynIntellisenseHelper();
                }
                return instance;
            }
        }

        public AstNode getCurrentInvocation(StaDynSourceFileAST parseResult, ITextSnapshot snapshot, int start, int line, int column)
        {
            VisitorFindCallMethod vfc = new VisitorFindCallMethod();
            //parseResult.Ast.Accept(vfc, new Location(Path.GetFileName(parseResult.FileName), line, column));
            parseResult.Ast.Accept(vfc, new Location(parseResult.FileName, line, column));
            Stack<AstNode> methodStack = vfc.methodStack;

            if (methodStack == null || methodStack.Count == 0) return null;

            int contCloseParent = SourceHelper.countCharAppearances(snapshot, start, line - 1, 0, ')');

            //SnapshotPoint starPoint = new SnapshotPoint(snapshot, start);
            //SnapshotSpan openParenPosition = new SnapshotSpan();

            //SourceHelper.FindMatchingOpenChar(starPoint, '(', ')', 0, out openParenPosition);

            //int columnDiference = start - openParenPosition.Start;

            //AstNode foundNode = (AstNode)parseResult.Ast.Accept(new VisitorFindNode(), new Location(Path.GetFileName(parseResult.FileName), line, column - columnDiference));

            //return foundNode;

            //Maybe an error or casting
            if (contCloseParent > methodStack.Count)
                return null;

            AstNode currentMethod = null;
            for (int i = 0; i < contCloseParent; i++)
                currentMethod = methodStack.Pop();

            return currentMethod;

        }
        /// <summary>
        /// Gets the current namespace according to a given Location
        /// </summary>
        /// <param name="location">Location</param>
        /// <param name="node">AST</param>
        /// <returns>The current namespace or null</returns>
        public Namespace getCurrentNameSpace(Location location, SourceFile node)
        {
            Location currentNamespaceLocation;

            Namespace currentNamespace = null, iternamespace = null;
            foreach (string key in node.Namespacekeys)
            {
                int count = node.GetNamespaceDefinitionCount(key);
                for (int i = 0; i < count; i++)
                {
                    iternamespace = node.GetNamespaceDeclarationElement(key, i);
                    currentNamespaceLocation = iternamespace.Location;

                    if (location > currentNamespaceLocation)
                        currentNamespace = iternamespace;
                }
            }
            return currentNamespace;
        }

        //public InterfaceDefinition getCurrentInterface(Location location, SourceFile node)
        //{
        //    Location currentInterfaceLocation;

        //    InterfaceDefinition currentInterface = null, iterInterface = null;
        //    foreach (string key in node.Namespacekeys)
        //    {
        //        int count = node.GetNamespaceDefinitionCount(key);
        //        for (int i = 0; i < count; i++)
        //        {
        //            iterInterface = node.GetNamespaceDeclarationElement(key, i);
        //            currentInterfaceLocation = iterInterface.Location;

        //            if (location > currentInterfaceLocation)
        //                currentInterface = iterInterface;
        //        }
        //    }
        //    return currentInterface;
        //}

        public ClassDefinition getCurrentClass(Location location, SourceFile node)
        {
            Location currentClassLocation;

            ClassDefinition currentClass = null;
            Declaration iterClass = null;
            Namespace currentNameSpace = this.getCurrentNameSpace(location, node);

            if (currentNameSpace != null) {
              int count = currentNameSpace.NamespaceMembersCount;
              for (int i = 0; i < count; i++) {
                iterClass = currentNameSpace.GetDeclarationElement(i);
                currentClassLocation = iterClass.Location;

                if (location > currentClassLocation && iterClass is ClassDefinition)
                  currentClass = (ClassDefinition)iterClass;
              }
            }
            else if (node.DeclarationCount > 0) {
              int count = node.DeclarationCount;
              for (int i = 0; i < count; i++) {
                iterClass = node.GetDeclarationElement(i);
                if (iterClass is ClassDefinition) {
                  currentClassLocation = iterClass.Location;

                  if (location > currentClassLocation && iterClass is ClassDefinition)
                    currentClass = (ClassDefinition)iterClass;
                }
              }
            }

            return currentClass;
        }

        public MethodDefinition getCurrentMethod(Location location, SourceFile node)
        {
            ClassDefinition currentClass = this.getCurrentClass(location, node);
            if (currentClass == null) return null;

            int memberCount = currentClass.MemberCount;
            Declaration currentClassMember = null;
            //Always have a contructor
            MethodDefinition returnMember = currentClass.GetMemberElement(memberCount - 1) as MethodDefinition;
            
            //Declaration previousClassMember = currentClass.GetMemberElement(memberCount-1);

            //We need to avoid the constructor
            for (int i = 0; i < memberCount -1; i++)
            {
                currentClassMember = currentClass.GetMemberElement(i);

                if (currentClassMember is MethodDefinition)
                {
                    if (location >= currentClassMember.Location)// && currentClassMember is MethodDefinition)
                        returnMember = currentClassMember as MethodDefinition;

                }

            }

            return returnMember;
        }

        public StaDynVariablesInScopeTable getVariablesInCurrentScope(int line, int column, ITextSnapshot snapshot, int start, bool includeSSAVars)
        {
            var variablesInScope = new StaDynVariablesInScopeTable();

            StaDynSourceFileAST file = ProjectFileAST.Instance.getAstFile(FileUtilities.Instance.getCurrentOpenDocumentFilePath());
            if (file==null || file.Ast == null) return null;

            //AstNode foundNode = (AstNode)file.Ast.Accept(new VisitorFindNode(), new Location(Path.GetFileName(file.FileName), line + 1, column));
            AstNode foundNode = (AstNode)file.Ast.Accept(new VisitorFindNode(), new Location(file.FileName, line + 1, column));

            if (foundNode == null) return null;
            variablesInScope = file.Ast.Accept(new VisitorFindDefinitionsInScope(includeSSAVars), foundNode) as StaDynVariablesInScopeTable;

            if (variablesInScope == null) return variablesInScope;

            /*
             * The foundNode is the next node that we actually are looking for.
             * So, we must pop as many groups of variables as closing braces we found
             * between foundeNode location and the location where the completion was triggered.
             */
            int blocksToPop = SourceHelper.countCharAppearances(snapshot, start, foundNode.Location.Line, foundNode.Location.Column, '}');

            for (int i = 0; i < blocksToPop; i++)
                variablesInScope.Reset();

            return variablesInScope;
        }
    }
}
