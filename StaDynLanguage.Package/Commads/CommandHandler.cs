using System;
using StaDynLanguage.Utils;
using StaDynLanguage.StaDynAST;
using AST;
using StaDynLanguage.Visitors;
using StaDynLanguage.StaDynDynamic;
using ErrorManagement;
using StaDynLanguage;
using TypeSystem;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using StaDynLanguage_Project;
using Microsoft.VisualStudio;
using StaDyn.StaDynProject;
using EnvDTE;


namespace Microsoft.StaDynLanguage_Package.Commads
{
    class CommandHandler
    {
        #region Fields

        private AstNode foundNode = null;
        private StaDynSourceFileAST currentFile = null;
        private System.Drawing.Point currentCaretPosition;

        #endregion

        //Command event-handlers

        #region MakeVarStaticCommand

        /// <summary>
        /// Set static var reference at the cursor's location.
        /// Cause check parse and window repaint.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        public void MakeVarStaticCommand(object sender, EventArgs e)
        {

            //MakeVarDynamicBeforeQueryStatus have found the current node
            if (foundNode == null) return;

            if (foundNode is Declaration)
            {
                //Declaration dec = foundNode as Declaration;

                IdDeclaration idDec = foundNode as IdDeclaration;
                //Must be a TypeVariable
                if (!(idDec.TypeExpr is TypeVariable))
                {
                    MessageBox.Show(
                            "Variable \"" + idDec.Identifier + "\" its not TypeVariable",
                            "Cannot make Static",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string varName = idDec.Identifier;

                int column = currentCaretPosition.Y;
                int line = currentCaretPosition.X;
                //Change the .dyn file
                StaDynDynamicHelper.Instance.makeVarStatic(varName, line, column, currentFile.Ast);

                this.parseAndRefreshHighlingting();

            }

        }

        #endregion

        #region MakeVarDynamicCommand

        /// <summary>
        /// Set dynamic var reference at the cursor's location.
        /// Cause check parse and window repaint.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        public void MakeVarDynamicCommand(object sender, EventArgs e)
        {

            //MakeVarDynamicBeforeQueryStatus have found the current node
            if (foundNode == null) return;

            if (foundNode is Declaration)
            {
                //Declaration dec = foundNode as Declaration;

                string currentWord = SourceHelper.getCurrentWord();

                IdDeclaration idDec = foundNode as IdDeclaration;

                if (currentWord != idDec.Identifier)
                {
                    MessageBox.Show(
                            "Variable \"" + idDec.Identifier + "\" selected: Precission problem",
                            "Cannot make dynamic",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                bool isDynamic = idDec.TypeExpr.IsDynamic;

                //Must be a TypeVariable
                if (!(idDec.TypeExpr is TypeVariable))
                {
                    MessageBox.Show(
                            "Variable \"" + idDec.Identifier + "\" its not TypeVariable",
                            "Cannot make dynamic",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string varName = idDec.Identifier;

                int column = currentCaretPosition.Y;
                int line = currentCaretPosition.X;

                //Change the .dyn file
                StaDynDynamicHelper.Instance.makeVarDynamic(varName, line, column, currentFile.Ast);

                this.parseAndRefreshHighlingting();

            }

        }

        #endregion

        #region MakeEverythingStaticCommand

        /// <summary>
        /// Set static every var reference in the active document.
        /// Cause check parse and window repaint.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        public void MakeEverythingStaticCommand(object sender, EventArgs e)
        {

            currentFile.Ast.Accept(new VisitorDynamicFileGenerator(currentFile.FileName, DynamicBehaviour.EVERYTHINGSTATIC), null);

            this.parseAndRefreshHighlingting();

            //DTE dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            //StaDynLanguage lang = Package.GetGlobalService(typeof(StaDynLanguage)) as StaDynLanguage;
            //if (dte != null && lang!=null)
            //{
            //    StaDynScope scope = lang.GetParseResult(dte.ActiveDocument.FullName);
            //    DynFileGenerationVisitor dynFileGenerator = new DynFileGenerationVisitor(dte.ActiveDocument.FullName);
            //    DynFileGenerationInfo info = new DynFileGenerationInfo(DynFileGenerationReason.MakeEverythingStatic);
            //    scope.Ast.Accept(dynFileGenerator, info);
            //    dynFileGenerator.Save();
            //    causeCheckParse();
            //    causeWindowRepaint();
            //}
        }

        #endregion

        #region MakeEverythingDynamicCommand

        /// <summary>
        /// Set dynamic every var reference in the active document.
        /// Cause check parse and window repaint.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        public void MakeEverythingDynamicCommand(object sender, EventArgs e)
        {
            currentFile.Ast.Accept(new VisitorDynamicFileGenerator(currentFile.FileName, DynamicBehaviour.EVERYTHINGDYNAMIC), null);

            this.parseAndRefreshHighlingting();

            //DTE dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            //StaDynLanguage lang = Package.GetGlobalService(typeof(StaDynLanguage)) as StaDynLanguage;
            //if (dte != null && lang != null)
            //{
            //    StaDynScope scope = lang.GetParseResult(dte.ActiveDocument.FullName);
            //    DynFileGenerationVisitor dynFileGenerator = new DynFileGenerationVisitor(dte.ActiveDocument.FullName);
            //    DynFileGenerationInfo info = new DynFileGenerationInfo(DynFileGenerationReason.MakeEverythingDynamic);
            //    scope.Ast.Accept(dynFileGenerator, info);
            //    dynFileGenerator.Save();
            //    causeCheckParse();
            //    causeWindowRepaint();
            //}
        }

        #endregion

        #region DeclareExplicitCommand

        /// <summary>
        /// Modify var reference at cursor's location declaration changing from "var" to its actual type if possible.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        public void DeclareExplicitCommand(object sender, EventArgs e)
        {

            if (!(foundNode is IdDeclaration)) return;


            SourceHelper.DeclareExplicit(foundNode, true);

            //BeforeQueryStatus handler should have called this, just in case:
            //if (lastSearchInfo == null)
            //    lastSearchInfo = searchDynVarInfo();
            //if (lastSearchInfo != null && lastSearchInfo.VarPath != null && lastSearchInfo.Successful)
            //{
            //    DTE dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            //    StaDynLanguage lang = Package.GetGlobalService(typeof(StaDynLanguage)) as StaDynLanguage;
            //    IVsTextManager txtMgr = Package.GetGlobalService(typeof(SVsTextManager)) as IVsTextManager;
            //    if (dte != null && lang != null && txtMgr != null)
            //    {
            //        IVsTextView textView;
            //        IVsTextLines buffer = null;
            //        txtMgr.GetActiveView(1, null, out textView);
            //        if(textView!=null)
            //            textView.GetBuffer(out buffer);

            //        if (!lastSearchInfo.Successful)
            //            return;

            //        IdDeclaration id = lastSearchInfo.NodesPath.Node as IdDeclaration;
            //        MethodDefinition method = lastSearchInfo.NodesPath.MethodNode;

            //        HashSet<TypeExpression> types = GetAllTypes(id.Identifier, method);

            //        if (types.Count != 1)
            //        {
            //            string message = "The var reference named '" + lastSearchInfo.VarPath.VarName +
            //                "' cannot be declared explicitly since it has more than one type within its scope:\n";
            //            foreach (TypeExpression type in types)
            //                message += " - " + type.FullName + "\n";
            //            message += "To be able to declare explicitly this reference, create a new type from which "
            //                + "all this types will inherit";

            //            MessageBox.Show(
            //                message,
            //                "Cannot declare explicit", 
            //                MessageBoxButtons.OK, MessageBoxIcon.Information);
            //            return;
            //        }
            //        TextDocument doc = dte.ActiveDocument.Object("TextDocument") as TextDocument;
            //        if(buffer!=null && doc!=null)
            //        {
            //            DeclareExplicit(id, method, buffer, doc);
            //        }

            //        causeWindowRepaint();
            //    }
            //}
        }

        #endregion

        #region DeclareEverythingExplicitCommand

        /// <summary>
        /// Modify every var reference in the active document declarations changing from "var" to their actual type if possible.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        public void DeclareEverythingExplicitCommand(object sender, EventArgs e)
        {
            if (currentFile != null)
                currentFile.Ast.Accept(new VisitorDeclareEverythingExplicit(), null);

            //DTE dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            //StaDynLanguage lang = Package.GetGlobalService(typeof(StaDynLanguage)) as StaDynLanguage;
            //IVsTextManager txtMgr = Package.GetGlobalService(typeof(SVsTextManager)) as IVsTextManager;
            //if (dte != null && lang != null && txtMgr != null)
            //{
            //    IVsTextView textView;
            //    IVsTextLines buffer = null;
            //    txtMgr.GetActiveView(1, null, out textView);
            //    if(textView!=null)
            //        textView.GetBuffer(out buffer);

            //    TextDocument doc = dte.ActiveDocument.Object("TextDocument") as TextDocument;
            //    StaDynScope scope = lang.GetParseResult(dte.ActiveDocument.FullName);

            //    if (buffer!=null && doc != null && scope!=null)
            //    {
            //        DeclareEverythingExplicitVisitor visitor = new DeclareEverythingExplicitVisitor(this, doc, buffer);
            //        scope.Ast.Accept(visitor, null);
            //        causeWindowRepaint();
            //    }
            //}
        }

        #endregion

        #region BuildEverythingStaticCommand

        /// <summary>
        /// Build active project as if project's DynVarOption property was set to EverythingStatic.
        /// For this, following steps are taken:
        /// -Set active project's DynVarOption property to EverythingStatic,
        /// -Build the project 
        /// -Restore old property value.
        /// Uses a <see cref="SolutionBuildListener"/> object for the property changes to take effect.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        public void BuildEverythingStaticCommand(object sender, EventArgs e)
        {
            IVsSolutionBuildManager buildManager =
                Package.GetGlobalService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager;
            if (buildManager != null)
            {
                string dynOptionKey = PropertyTag.DynVarOption.ToString();
                string everythingStatic = DynVarOption.EverythingStatic.ToString();

                ProjectConfiguration.Instance.SetProperty(dynOptionKey, everythingStatic);

                //SolutionBuildListener object will change project's DynVarOption property value before and after the build
                //(Constructor handles Advising, events handle Unadvising)
                //new SolutionBuildListener(buildManager, dynOptionKey, everythingStatic);


               
                var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                var sb = (DTEObj.Solution.SolutionBuild as SolutionBuild);
                sb.Build();


                //check the projectIcon
                ProjectConfiguration.Instance.GetActiveProjectNode().checkProjectIcon();

                SourceHelper.refreshHighlighting();


            }
        }

        #endregion

        /// <summary>
        /// Build active project as if project's DynVarOption property was set to EverythingDynamic.
        /// For this, following steps are taken:
        /// -Set active project's DynVarOption property to EverythingDynamic,
        /// -Build the project 
        /// -Restore old property value.
        /// Uses a <see cref="SolutionBuildListener"/> object for the property changes to take effect.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        public void BuildManagedCommand(object sender, EventArgs e) {
          IVsSolutionBuildManager buildManager =
               Package.GetGlobalService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager;
          if (buildManager != null) {
            string dynOptionKey = PropertyTag.DynVarOption.ToString();
            string managed = DynVarOption.Managed.ToString();

            ProjectConfiguration.Instance.SetProperty(dynOptionKey, managed);

            //SolutionBuildListener object will change project's DynVarOption property value before and after the build
            //(Constructor handles Advising, events handle Unadvising)
            //new SolutionBuildListener(buildManager, dynOptionKey, managed);



            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            var sb = (DTEObj.Solution.SolutionBuild as SolutionBuild);
            sb.Build();
              
            //check the projectIcon
            ProjectConfiguration.Instance.GetActiveProjectNode().checkProjectIcon();

            SourceHelper.refreshHighlighting();
          }
        }

        #region BuildEverythingDynamicCommand



        /// <summary>
        /// Build active project as if project's DynVarOption property was set to EverythingDynamic.
        /// For this, following steps are taken:
        /// -Set active project's DynVarOption property to EverythingDynamic,
        /// -Build the project 
        /// -Restore old property value.
        /// Uses a <see cref="SolutionBuildListener"/> object for the property changes to take effect.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        public void BuildEverythingDynamicCommand(object sender, EventArgs e)
        {
            IVsSolutionBuildManager buildManager =
                Package.GetGlobalService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager;
            if (buildManager != null)
            {
                string dynOptionKey = PropertyTag.DynVarOption.ToString();
                string everythingDynamic = DynVarOption.EverythingDynamic.ToString();

                ProjectConfiguration.Instance.SetProperty(dynOptionKey, everythingDynamic);

                //SolutionBuildListener object will change project's DynVarOption property value before and after the build
                //(Constructor handles Advising, events handle Unadvising)
                //new SolutionBuildListener(buildManager, dynOptionKey, everythingDynamic);

               

                var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                var sb = (DTEObj.Solution.SolutionBuild as SolutionBuild);
                sb.Build();

                //check the projectIcon
                ProjectConfiguration.Instance.GetActiveProjectNode().checkProjectIcon();

                SourceHelper.refreshHighlighting();
           
            }
        }

        #endregion

        //BeforeQueryStatus handlers

        #region MakeVarStaticBeforeQueryStatus

        /// <summary>
        /// Determines whether MakeVarStatic command must be visible and/or enabled:
        /// -Visible if active document is a ".stadyn" file.
        /// -Enabled if token at cursor's location is a dynamic var reference.
        /// </summary>
        /// <param name="sender">OleMenuCommand object.</param>
        /// <param name="e">Not used.</param>
        public void MakeVarStaticBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = sender as OleMenuCommand;
            
            if (command != null)
            {
                bool isStaDynFile = false;
                bool isVar = false;
                bool isDynamic = false;
                beforeQueryStatusCommonTasks(out isStaDynFile, out isVar, out isDynamic);
                command.Visible = isStaDynFile;
                command.Enabled = isVar && isDynamic;
            }
        }

        #endregion

        #region MakeVarDynamicBeforeQueryStatus

        /// <summary>
        /// Determines whether MakeVarDynamic command must be visible and/or enabled:
        /// -Visible if active document is a ".stadyn" file.
        /// -Enabled if token at cursor's location is a static var reference.
        /// </summary>
        /// <param name="sender">OleMenuCommand object.</param>
        /// <param name="e">Not used.</param>
        public void MakeVarDynamicBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = sender as OleMenuCommand;
            if (command != null)
            {
                bool isVar = false;
                bool isDynamic = false;
                bool isStaDynFile = false;

                beforeQueryStatusCommonTasks(out isStaDynFile, out isVar, out isDynamic);
                command.Visible = isStaDynFile;
                command.Enabled = isVar && !isDynamic;
            }
        }

        #endregion

        #region MakeEverythingStaticBeforeQueryStatus

        /// <summary>
        /// Determines whether MakeEverythingStatic command must be visible and/or enabled:
        /// -Visible if active document is a ".stadyn" file.
        /// -Enabled if active document is a ".stadyn" file.
        /// </summary>
        /// <param name="sender">OleMenuCommand object.</param>
        /// <param name="e">Not used.</param>
        public void MakeEverythingStaticBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = sender as OleMenuCommand;
            if (command != null)
            {
                bool isStaDynFile = isActiveDocumentStaDynFile();
                command.Visible = isStaDynFile;
                command.Enabled = isStaDynFile;
            }
        }

        #endregion

        #region MakeEverythingDynamicBeforeQueryStatus

        /// <summary>
        /// Determines whether MakeEverythingDynamic command must be visible and/or enabled:
        /// -Visible if active document is a ".stadyn" file.
        /// -Enabled if active document is a ".stadyn" file.
        /// </summary>
        /// <param name="sender">OleMenuCommand object.</param>
        /// <param name="e">Not used.</param>
        public void MakeEverythingDynamicBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = sender as OleMenuCommand;
            if (command != null)
            {
                bool isStaDynFile = isActiveDocumentStaDynFile();
                command.Visible = isStaDynFile;
                command.Enabled = isStaDynFile;
            }
        }

        #endregion

        #region DeclareExplicitBeforeQueryStatus

        /// <summary>
        /// Determines whether DeclareExplicit command must be visible and/or enabled:
        /// -Visible if active document is a ".stadyn" file.
        /// -Enabled if token at cursor's location is a var reference.
        /// </summary>
        /// <param name="sender">OleMenuCommand object.</param>
        /// <param name="e">Not used.</param>
        public void DeclareExplicitBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = sender as OleMenuCommand;
            if (command != null)
            {
                bool isStaDynFile = false;
                bool isVar = false;
                bool isDynamic = false;
                beforeQueryStatusCommonTasks(out isStaDynFile, out isVar, out isDynamic);
                command.Visible = isStaDynFile;

                command.Enabled = isVar;
            }
        }

        #endregion

        #region DeclareEverythingExplicitBeforeQueryStatus

        /// <summary>
        /// Determines whether DeclareEverythingExplicit command must be visible and/or enabled:
        /// -Visible if active document is a ".stadyn" file.
        /// -Enabled if active document is a ".stadyn" file.
        /// </summary>
        /// <param name="sender">OleMenuCommand object.</param>
        /// <param name="e">Not used.</param>
        public void DeclareEverythingExplicitBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = sender as OleMenuCommand;
            if (command != null)
            {
                bool isStaDynFile = isActiveDocumentStaDynFile();
                command.Visible = isStaDynFile;
                command.Enabled = isStaDynFile;
            }
        }

        #endregion

        #region BuildEverythingStaticBeforeQueryStatus

        /// <summary>
        /// Determines whether BuildEverythingStatic command must be visible and/or enabled:
        /// -Visible if active project is a StaDyn project.
        /// -Enabled if active project is a StaDyn project.
        /// </summary>
        /// <param name="sender">OleMenuCommand object.</param>
        /// <param name="e">Not used.</param>
        public void BuildEverythingStaticBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = sender as OleMenuCommand;
            if (command != null)
            {
                bool isStaDynProject = isActiveProjectStaDynProject();
                command.Visible = isStaDynProject;
                command.Enabled = isStaDynProject;
            }
        }

        #endregion

        #region BuildEverythingDynamicBeforeQueryStatus

        /// <summary>
        /// Determines whether BuildEverythingDynamic command must be visible and/or enabled:
        /// -Visible if active project is a StaDyn project.
        /// -Enabled if active project is a StaDyn project.
        /// </summary>
        /// <param name="sender">OleMenuCommand object.</param>
        /// <param name="e">Not used.</param>
        public void BuildEverythingDynamicBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand command = sender as OleMenuCommand;
            if (command != null)
            {
                bool isStaDynProject = isActiveProjectStaDynProject();
                command.Visible = isStaDynProject;
                command.Enabled = isStaDynProject;
            }
        }

        #endregion

        #region BuildManagedBeforeQueryStatus

        /// <summary>
        /// Determines whether BuildEverythingDynamic command must be visible and/or enabled:
        /// -Visible if active project is a StaDyn project.
        /// -Enabled if active project is a StaDyn project.
        /// </summary>
        /// <param name="sender">OleMenuCommand object.</param>
        /// <param name="e">Not used.</param>
        public void BuildManagedBeforeQueryStatus(object sender, EventArgs e) {
          OleMenuCommand command = sender as OleMenuCommand;
          if (command != null) {
            bool isStaDynProject = isActiveProjectStaDynProject();
            command.Visible = isStaDynProject;
            command.Enabled = isStaDynProject;
          }
        }

        #endregion

        //Other public methods

        #region DeclareExcplicit

        /// <summary>
        /// Modify var reference declaration changing from "var" to its actual type if possible.
        /// This method is public in order to be called by <see cref="CommandHandler.DeclareExplicitCommand"/> 
        /// and <see cref="DeclareEverythingExplicitVisitor"/>.
        /// The operation will fail if the declaration has more than one type along its scope (counting as well 
        /// types inside UnionType types).
        /// </summary>
        /// <param name="id">IdDeclaration node for the var reference declaration.</param>
        /// <param name="method">MethodDefinition node containing the var reference declaration.</param>
        /// <param name="buffer">IVsTextLines object containing document's content.</param>
        /// <param name="doc">TextDocument object for the document.</param>
        /// public void DeclareExplicit
        //public void DeclareExplicit(IdDeclaration id, MethodDefinition method, IVsTextLines buffer, TextDocument doc)
        //public void DeclareExplicit()
        //{

        //    int lineNumber = this.currentCaretPosition.X ;
        //    int column = this.currentCaretPosition.Y;
        //    var snapshot = FileUtilities.Instance.getCurrentTextSnapShot();
        //    int start = FileUtilities.Instance.getCurrentCaretPosition();

        //    StaDynVariablesInScopeTable infoVariablesInScope = StaDynIntellisenseHelper.Instance.getVariablesInCurrentScope(lineNumber +1, column, snapshot, start,true);

        //    string varName = ((IdDeclaration)foundNode).Identifier;

        //    var SSAVariablesList = new List<Declaration>();
        //    //Look for the same variable in the current scope
        //    for (int i = 0; i < infoVariablesInScope.Table.Count; i++)
        //    {
        //        if (infoVariablesInScope.Table[i].Count > 0)
        //        {
        //            foreach (KeyValuePair<string, IdDeclaration> variable in infoVariablesInScope.Table[i])
        //            {
        //                var type = variable.Value.TypeExpr;
        //                if (variable.Key.StartsWith(varName) && type is TypeVariable && ((TypeVariable)type).Substitution !=null)
        //                    SSAVariablesList.Add(variable.Value);

        //            }
        //        }
        //    }
        //    if (SSAVariablesList.Count !=1)
        //    {
        //         string message;
        //         if (SSAVariablesList.Count == 0)
        //             message = "The var reference named '" + varName +
        //             "' cannot be declared explicitly since it has no type yet\n";
        //         else
        //         {
        //             message = "The var reference named '" + varName +
        //                 "' cannot be declared explicitly since it has more than one type within its scope:\n";
        //             foreach (Declaration node in SSAVariablesList)
        //                 message += " - " + ((TypeVariable)node.TypeExpr).Substitution.FullName + "\n";
        //             message += "To be able to declare explicitly this reference, create a new type from which "
        //                 + "all this types will inherit";
        //         }
        //        MessageBox.Show(
        //            message,
        //            "Cannot declare explicit",
        //            MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    SourceHelper.replaceWord(snapshot, lineNumber, "var", ((TypeVariable)SSAVariablesList[0].TypeExpr).Substitution.FullName);

        //}

        #endregion

        #region GetAllTypes

        /// <summary>
        /// Get all types a reference has or may have along the method where it is declared.
        /// </summary>
        /// <param name="identifier">Name of the identifier.</param>
        /// <param name="method">Method where the identifier is declared.</param>
        /// <returns>Set of types assigned to the identifier. Types contained in 
        /// UnionTypes are recursively inserted in the set.</returns>
        //public HashSet<TypeExpression> GetAllTypes(string identifier, MethodDefinition method)
        //{
        //    //Verify that the variable has only one type along the block
        //    //(Making use of SSA duplicated IdDeclaration nodes for that)

        //    //SearchInfo info = new SearchInfo(-1, -1, SearchReason.VarIdDeclarationSearch, StoreMode.StoreAll);
        //    //if (method != null)
        //    //    method.Accept(new SearchVisitor(), info);

        //    //HashSet<TypeExpression> types = new HashSet<TypeExpression>();
        //    //IdDeclaration id;
        //    //foreach (AstNode node in info.NodeList)
        //    //{
        //    //    id = node as IdDeclaration;
        //    //    if (id == null | !id.Identifier.Equals(identifier))
        //    //        continue;

        //    //    TypeVariable varType = id.TypeExpr as TypeVariable;
        //    //    TypeExpression substitutionType = varType != null ? varType.Substitution : null;
        //    //    if (substitutionType == null)
        //    //        continue;

        //    //    if (substitutionType is UnionType)
        //    //    {
        //    //        types.UnionWith(getTypesFromUnion(substitutionType as UnionType));
        //    //        continue;
        //    //    }

        //    //    types.Add(substitutionType);
        //    //}

        //    //return types;
        //}

        #endregion

        // Private methods

        #region searchDynVarInfo

        //private SearchInfo searchDynVarInfo()
        //{
        //    TokenInfo token = null;
        //    int line;
        //    int column;
        //    string tokenText = TextTools.GetCurrentTokenInfo(out token, out line, out column);

        //    StaDynLanguage lang = Package.GetGlobalService(typeof(StaDynLanguage)) as StaDynLanguage;

        //    DTE dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SDTE)) as DTE;
        //    StaDynScope scope = lang.GetParseResult(dte.ActiveDocument.FullName);

        //    TextTools.VsToInference(ref line, ref column);
        //    SearchInfo searchInfo = new SearchInfo(line, column, tokenText, SearchReason.AnyVarSearch, StoreMode.NamesAndNodes);
        //    scope.Ast.Accept(new SearchVisitor(), searchInfo);

        //    return searchInfo;
        //}

        #endregion

        #region  beforeQueryStatusCommonTasks

        /// <summary>
        /// Performs several tasks useful for the commands' BeforeQueryStatus handlers.
        /// Stores in lastSearchInfo information retrieved by <see cref="CommandHandler.searchDynVarInfo"/>.
        /// </summary>
        /// <param name="isStaDynFile">Active document is a ".stadyn" file.</param>
        /// <param name="isVar">Token at cursor's location is a var reference.</param>
        /// <param name="isDynamic">Token at cursor's location is a dynamic var reference.</param>
        private void beforeQueryStatusCommonTasks(out bool isStaDynFile, out bool isVar, out bool isDynamic)
        {

            isStaDynFile = isVar = isDynamic = false;

            isStaDynFile = FileUtilities.Instance.checkCurrentFileExtension(".stadyn");

            //Get the span of the current word (caret on)
            var currentWordTextSpan = SourceHelper.getCurrentTextSpan();

            this.currentCaretPosition.X = currentWordTextSpan.iStartLine;
            this.currentCaretPosition.Y = currentWordTextSpan.iEndIndex;

            int line = currentWordTextSpan.iStartLine;
            int column = currentWordTextSpan.iEndIndex + 1;

            StaDynParser parser = new StaDynParser();
            parser.parseAll();
            currentFile = ProjectFileAST.Instance.getAstFile(FileUtilities.Instance.getCurrentOpenDocumentFilePath());

            if (currentFile == null || currentFile.Ast == null) return;
            //string currentWord = SourceHelper.getCurrentWord();
            //this.foundNode = (AstNode)currentFile.Ast.Accept(new VisitorFindNode(), new Location(Path.GetFileName(currentFile.FileName), line + 1, column));
            this.foundNode = (AstNode)currentFile.Ast.Accept(new VisitorFindNode(), new Location(currentFile.FileName, line + 1, column));

            if (foundNode is Declaration)
            {
                //Declaration dec = foundNode as Declaration;

                IdDeclaration idDec = foundNode as IdDeclaration;

                isVar = idDec.TypeExpr is TypeVariable;

                isDynamic = StaDynDynamicHelper.Instance.checkDynamicVar(idDec.Identifier, line, column, currentFile);
                //isDynamic = idDec.TypeExpr.IsDynamic;
            }

        }

        private void parseAndRefreshHighlingting()
        {

            StaDynParser parser = new StaDynParser();
            parser.parseAll();
            currentFile = ProjectFileAST.Instance.getAstFile(FileUtilities.Instance.getCurrentOpenDocumentFilePath());

            // if (parseResult == null || parseResult.Ast == null)

            SourceHelper.refreshHighlighting();
        }

        #endregion

        #region  isActiveDocumentStaDynFile

        /// <summary>
        /// Get if active document is a ".stadyn" file.
        /// </summary>
        /// <returns>true if active document is a ".stadyn" file.</returns>
        private bool isActiveDocumentStaDynFile()
        {
            return FileUtilities.Instance.checkCurrentFileExtension(".stadyn");
        }

        #endregion

        #region  isActiveProjectStaDynProject

        /// <summary>
        /// Get if active project is a StaDyn project.
        /// </summary>
        /// <returns>true if active project is a StaDyn project.</returns>
        private bool isActiveProjectStaDynProject()
        {
            return ProjectConfiguration.Instance.GetActiveProjectNode() is StaDynProjectNode;
        }

        #endregion

       

  
    }

    #region class SolutionBuildListener

    /// <summary>
    /// Implementation of IVsUpdateSolutionEvents which:
    /// -On creation: Registers with a IVsSolutionBuildManager for solution build events.
    /// -Before building: Modifies the value of one project property.
    /// -After building: Restores original value of modified property, and unregisters.
    /// </summary>
    public class SolutionBuildListener : IVsUpdateSolutionEvents
    {
        #region Fields

        private IVsSolutionBuildManager buildManager;
        private uint cookie;

        private string propertyKey;
        private string oldValue;
        private string newValue;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="buildManager">IVsSolutionBuildManager service (for registering this object).</param>
        /// <param name="propertyKey">Key of the property to be modified during build.</param>
        /// <param name="newValue">Value during build for the property to be modified.</param>
        public SolutionBuildListener(IVsSolutionBuildManager buildManager, string propertyKey, string newValue)
        {
            this.buildManager = buildManager;
            this.propertyKey = propertyKey;
            this.newValue = newValue;
            this.oldValue = ProjectConfiguration.Instance.GetProperty(propertyKey);

            buildManager.AdviseUpdateSolutionEvents(this, out cookie);
        }

        #endregion

        #region IVsUpdateSolutionEvents Members

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="pIVsHierarchy">Not used.</param>
        /// <returns>VSConstants.S_OK if successful</returns>
        public int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Modifies the property to the new value.
        /// </summary>
        /// <param name="pfCancelUpdate">Not used.</param>
        /// <returns>VSConstants.S_OK if successful</returns>
        public int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            changeConfiguration();
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Modifies the property to the original value and unregisters this object from the IVsSolutionBuildManager.
        /// </summary>
        /// <returns>VSConstants.S_OK if successful</returns>
        public int UpdateSolution_Cancel()
        {
            restoreConfiguration();
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Modifies the property to the original value and unregisters this object from the IVsSolutionBuildManager.
        /// </summary>
        /// <param name="fSucceeded">Not used.</param>
        /// <param name="fModified">Not used.</param>
        /// <param name="fCancelCommand">Not used.</param>
        /// <returns>VSConstants.S_OK if successful</returns>
        public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            restoreConfiguration();
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="pfCancelUpdate">Not used.</param>
        /// <returns>VSConstants.S_OK if successful</returns>
        public int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region changeConfiguration

        /// <summary>
        /// Modifies the property to the new value.
        /// </summary>
        private void changeConfiguration()
        {
            ProjectConfiguration.Instance.SetProperty(propertyKey, newValue);
        }

        #endregion

        #region restoreConfiguration

        /// <summary>
        /// Modifies the property to the original value and unregisters this object from the IVsSolutionBuildManager.
        /// </summary>
        private void restoreConfiguration()
        {
            ProjectConfiguration.Instance.SetProperty(propertyKey, oldValue);
            buildManager.UnadviseUpdateSolutionEvents(cookie);
        }

        #endregion
    }

    #endregion
}
