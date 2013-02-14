using System.Collections.Generic;
using System.Text;
using TypeSystem;
using AST;
using antlr;
using ErrorManagement;
using StaDynLanguage.Utils;
using StaDynLanguage.StaDynAST;
using StaDynLanguage.Intellisense;

namespace StaDynLanguage.StaDynTypeTable
{
    class StaDynTypeTableFunctions
    {
        //Implements Singleton
        static StaDynTypeTableFunctions instance = null;

        StaDynTypeTableFunctions()
        {
        }

        public static StaDynTypeTableFunctions Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StaDynTypeTableFunctions();
                }
                return instance;
            }
        }

        /// <summary>
        /// Checks if a token its a contained in the TypeTable
        /// First find on the TypeTable as it
        /// Second find appending the "Usings" of the source file
        /// Third find appendid the namespaces of the source file
        /// </summary>
        /// <param name="token">IToken with correct line number and colum</param>
        /// <returns>True if TypeTable Contains the token</returns>
        public bool CheckTokenTypeOnCurrentDocument(IToken token)
        {

            //Get the currect open file name
            string currentDocumentFileName = FileUtilities.Instance.getCurrentOpenDocumentFilePath();
            //Refresh the fileName
           // token.setFilename(currentDocumentFileName);

           // Location location = ITokenToLocation.Instance.convertToLocation(token);

            //Location location = new Location(Path.GetFileName(currentDocumentFileName), token.getLine(), token.getColumn());
            Location location = new Location(currentDocumentFileName, token.getLine(), token.getColumn());

            TypeExpression tokenType = null;

            //Simple search on typeTable
            tokenType = TypeTable.Instance.GetType(token.getText(), location);

            if (tokenType != null) return true;

            //Get the current AST
            StaDynSourceFileAST currentSourceFile = ProjectFileAST.Instance.getAstFile(currentDocumentFileName);

            if (currentSourceFile == null || currentSourceFile.Ast == null) return false;
            
          //if (currentSourceFile.Ast == null) return false;

            tokenType = this.findTypeAtUsings(token.getText(), location, currentSourceFile.Ast.Usings);

            if (tokenType != null) return true;

            tokenType = this.findTypeAtCurrentNameSpace(token.getText(), location, currentSourceFile.Ast);

            if (tokenType != null) return true;

            return false;
        }

        /// <summary>
        /// Finds a type in the TypeTable appending "usings"
        /// </summary>
        /// <param name="typeName">Type to find</param>
        /// <param name="location">Locaiton of the token</param>
        /// <param name="fileUsings">List of using to append</param>
        /// <returns>TypeExpression of the matching type, null in other case</returns>
        private TypeExpression findTypeAtUsings(string typeName, Location location, List<string> fileUsings)
        {
            TypeExpression t;
            //Try to apend File namespaces
            foreach (string usings in fileUsings)
            {
                StringBuilder str = new StringBuilder();
                str.Append(usings);
                str.Append(".");
                str.Append(typeName);
                t = TypeTable.Instance.GetType(str.ToString(), location);
                if (t != null)
                    return t;
            }

            return null;
        }

        /// <summary>
        /// Finds a type appending the current NameSpace
        /// </summary>
        /// <param name="typeName">Type to find</param>
        /// <param name="location">Location of the token type</param>
        /// <param name="node">AST of the current type</param>
        /// <returns>TypeExpression of the matching type, null in other case</returns>
        private TypeExpression findTypeAtCurrentNameSpace(string typeName, Location location, SourceFile node)
        {
            TypeExpression t = null;
            //Namespace currentNameSpace = this.getCurrentNameSpace(location, node);
            Namespace currentNameSpace = StaDynIntellisenseHelper.Instance.getCurrentNameSpace(location, node);

            if (currentNameSpace == null) return null;

            string name = currentNameSpace.Identifier.Identifier;

            StringBuilder str = new StringBuilder();
            str.Append(name);
            str.Append(".");
            str.Append(typeName);

            t = TypeTable.Instance.GetType(str.ToString(), location);

            return t;
        }

        /// <summary>
        /// Gets the current namespace according to a given Location
        /// </summary>
        /// <param name="location">Location</param>
        /// <param name="node">AST</param>
        /// <returns>The current namespace or null</returns>
        //private Namespace getCurrentNameSpace(Location location, SourceFile node)
        //{
        //    Location currentNamespaceLocation;

        //    Namespace currentNamespace = null, iternamespace = null;
        //    foreach (string key in node.Namespacekeys)
        //    {
        //        int count = node.GetNamespaceDefinitionCount(key);
        //        for (int i = 0; i < count; i++)
        //        {
        //            iternamespace = node.GetNamespaceDeclarationElement(key, i);
        //            currentNamespaceLocation = iternamespace.Location;

        //            if (location > currentNamespaceLocation)
        //                currentNamespace = iternamespace;
        //        }
        //    }
        //    return currentNamespace;
        //}

        //public ClassDefinition getCurrentClass(Location location, SourceFile node)
        //{
        //    Location currentClassLocation;

        //    ClassDefinition currentClass = null;
        //    Declaration iterClass = null;
        //    Namespace currentNameSpace = this.getCurrentNameSpace(location, node);

        //    if (currentNameSpace == null)
        //        return null;

        //    int count = currentNameSpace.NamespaceMembersCount;
        //    for (int i = 0; i < count; i++)
        //    {
        //        iterClass = currentNameSpace.GetDeclarationElement(i);
        //        currentClassLocation = iterClass.Location;

        //        if (location > currentClassLocation && iterClass is ClassDefinition)
        //            currentClass = (ClassDefinition)iterClass;
        //    }

        //    return currentClass;
        //}

        //public Declaration getCurrentMethod(Location location, SourceFile node)
        //{
        //    ClassDefinition currentClass = this.getCurrentClass(location, node);

        //    int memberCount = currentClass.MemberCount;
        //    Declaration currentClassMember=null;
        //    Declaration returnMember = null;
      
        //    for (int i = 0; i < memberCount; i++)
        //    {
        //        currentClassMember = currentClass.GetMemberElement(i);

        //        if (location >= currentClassMember.Location && currentClassMember is MethodDefinition)
        //            returnMember= currentClassMember;

        //    }

        //    return returnMember;
        //}

        public List<TypeExpression> getTypesAtNameSpace(string nameSpace)
        {
            Dictionary<string, TypeExpression> TableTypes = TypeTable.Instance.getAllTypes();
            List<TypeExpression> foundTypes = new List<TypeExpression>();

            foreach (KeyValuePair<string, TypeExpression> pair in TableTypes)
            {
                if (pair.Key.Contains(nameSpace))
                    foundTypes.Add(pair.Value);
            }

            return foundTypes;
        }

    }
}
