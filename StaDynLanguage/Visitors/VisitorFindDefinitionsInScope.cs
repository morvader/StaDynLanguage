﻿using System;
using System.Collections.Generic;
using System.Text;

using AST;
using ErrorManagement;
using Symbols;
using Tools;
using TypeSystem;

namespace StaDynLanguage.Visitors
{

    public class StaDynVariablesInScopeTable
    {
        //private List<Dictionary<string, TypeExpression>> table;

        private List<Dictionary<string, IdDeclaration>> table;

        //public List<Dictionary<string, TypeExpression>> Table
        //{
        //    get { return table; }
        //    set { table = value; }
        //}

        public List<Dictionary<string, IdDeclaration>> Table
        {
            get { return table; }
            set { table = value; }
        }      

        #region Constructors

      /// <summary>
      /// Constructor of table.
      /// </summary>
        public StaDynVariablesInScopeTable()
      {
          //this.table = new List<Dictionary<string, TypeExpression>>();
          this.table = new List<Dictionary<string, IdDeclaration>>();
      }

      #endregion

      #region Set()

      /// <summary>
      /// Add a new scope
      /// </summary>
      public void Set()
      {
         //scope++;
          //this.table.Add(new Dictionary<string, TypeExpression>());
          this.table.Add(new Dictionary<string, IdDeclaration>());
      }

      #endregion

      #region Reset()

      /// <summary>
      /// Removes the last scope
      /// </summary>
      public void Reset()
      {
         //scope--;
         this.table.RemoveAt(this.table.Count - 1);
      }

      #endregion

      #region Insert()

      /// <summary>
      /// Insert a new symbol in the current scope.
      /// </summary>
      /// <param name="id">Symbol identifier.</param>
      /// <param name="type">Symbol type.</param>
      /// <param name="isDynamic">True if the symbol is dynamic, false otherwise.</param>
      /// <returns>The symbol inserted, null otherwise.</returns>
      //public string Insert(string id, TypeExpression type, bool isDynamic)
      //{
      //   if (this.table[this.table.Count - 1].ContainsKey(id))
      //       return null;
      //   //Symbol s = new Symbol(id, this.table.Count - 1, type, isDynamic);
      //   this.table[this.table.Count - 1].Add(id, type);
      //   //Console.WriteLine(s.ToString());
      //   return id;
      //}

      public string Insert(string id, IdDeclaration node)
      {
          if (this.table[this.table.Count - 1].ContainsKey(id))
              return null;
          //Symbol s = new Symbol(id, this.table.Count - 1, type, isDynamic);
          //this.table[this.table.Count - 1].Add(id, type);
          this.table[this.table.Count - 1].Add(id, node);
          //Console.WriteLine(s.ToString());
          return id;
      }

      #endregion
    }
    /// <summary>
    /// This class visits the AST to assign symbol information to identifier
    /// expression.
    /// </summary>
    /// <remarks>
    /// Inheritance: VisitorAdapter.
    /// Implements Visitor pattern [Concrete Visitor].
    /// </remarks>
    public class VisitorFindDefinitionsInScope : VisitorAdapter
    {
        #region Fields

        /// <summary>
        /// References to the symbol table.
        /// </summary>
        public StaDynVariablesInScopeTable table;

        bool includeSSAVars = false;

        /// <summary>
        /// Stores scope identifiers
        /// </summary>
        private List<string> usings;

        ///// <summary>
        ///// Represents the type of the current class (this).
        ///// </summary>
        //private UserType thisType;

        ///// <summary>
        ///// Represents the identifier of the current Namespace.
        ///// </summary>
        //private string currentNamespace;

        ///// <summary>
        ///// Represents the identifier of the current Class.
        ///// </summary>
        //private string currentClass;

        ///// <summary>
        ///// Represents the identifier of the current Method.
        ///// </summary>
        //private string currentMethod;

        ///// <summary>
        ///// Represents the base type of the current class (base).
        ///// </summary>
        //private UserType baseType;

        /// <summary>
        /// References to the dynamic variables manager.
        /// </summary>
        //private DynVarManagement.DynVarManager manager;

        ///// <summary>
        ///// Stores the current block to use in DynVarManager.
        ///// </summary>
        //private List<int> blockList;

        ///// <summary>
        ///// Index for block list;
        ///// </summary>
        //private int indexBlockList = 0;

        ///// <summary>
        ///// A mapping between each sort file name and its full directory name
        ///// </summary>
        //private IDictionary<string, string> directories;


        private bool found;

        #endregion

        #region Constructor

        public VisitorFindDefinitionsInScope(bool includeSSAVars) {
            this.table = new StaDynVariablesInScopeTable();
            this.includeSSAVars = includeSSAVars;
            this.usings = new List<string>();
            //this.manager = new DynVarManagement.DynVarManager();
            //this.currentNamespace = "";
            //this.currentClass = "";
            //this.currentMethod = "";
            //this.found = false;
            //this.directories = directories;
        }
        ///// <summary>
        ///// Constructor of VisitorSymbolIdentification
        ///// <param name="directories">A mapping between each sort file name and its full directory name</param>
        ///// </summary>
        //public VisitorSymbolIdentification(IDictionary<string, string> directories)
        //{
        //    this.table = new StaDynVariablesInScopeTable();
        //    this.usings = new List<string>();
        //    this.manager = new DynVarManagement.DynVarManager();
        //    this.currentNamespace = "";
        //    this.currentClass = "";
        //    this.currentMethod = "";
        //    this.directories = directories;
        //}

        #endregion

        //#region loadDynVars

        ///// <summary>
        ///// Loads the info in the DynVarManager
        ///// </summary>
        //private void loadDynVars()
        //{
        //    string dynFile = Path.ChangeExtension(this.directories[this.currentFile] + "\\" + this.currentFile, DynVarManagement.DynVarManager.DynVarFileExt);

        //    if (File.Exists(dynFile))
        //    {
        //        try
        //        {
        //            manager.Load(dynFile);
        //        }
        //        catch (DynVarManagement.DynVarException e)
        //        {
        //            ErrorManager.Instance.NotifyError(new LoadingDynVarsError(e.Message));
        //        }
        //    }
        //}

        //#endregion

        //#region searchDynInfo()

        ///// <summary>
        ///// Searches a dynamic reference to the specified identifier.
        ///// </summary>
        ///// <param name="id">variable identifier.</param>
        ///// <returns>True if the identifier is a dynamic variable, false otherwise.</returns>
        //private bool searchDynInfo(string id)
        //{
        //    if (manager.Ready)
        //    {
        //        if (this.currentMethod.Length == 0)
        //            return manager.SearchDynVar(this.currentNamespace, this.currentClass, id);
        //        if ((this.blockList != null) && (this.blockList.Count != 0) && (this.indexBlockList > 0))
        //            return manager.SearchDynVar(this.currentNamespace, this.currentClass, this.currentMethod, this.blockList.GetRange(0, this.indexBlockList), id);
        //        return manager.SearchDynVar(this.currentNamespace, this.currentClass, this.currentMethod, id);
        //    }
        //    return false;
        //}

        //#endregion

        #region Visit(SourceFile node, Object obj)

        public override Object Visit(SourceFile node, Object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }

            this.currentFile = node.Location.FileName;

            //this.loadDynVars();

            for (int i = 0; i < node.Usings.Count; i++)
            {
                usings.Add(node.Usings[i]);
            }

            foreach (string key in node.Namespacekeys)
            {
                int count = node.GetNamespaceDefinitionCount(key);
                for (int i = 0; i < count; i++)
                {
                    if (found) return this.table;
                    node.GetNamespaceDeclarationElement(key, i).Accept(this, obj);
                }
            }

            for (int i = 0; i < node.DeclarationCount; i++)
            {
                if (found) return this.table;
                node.GetDeclarationElement(i).Accept(this, obj);
            }

            return this.table;
        }

        #endregion

        #region Visit(Namespace node, Object obj)

        public override Object Visit(Namespace node, Object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }

            this.table.Set();
            //this.currentNamespace = node.Identifier.Identifier;

            usings.Add(node.Identifier.Identifier);

            for (int i = 0; i < node.NamespaceMembersCount; i++)
            {
                if (found) return this.table;
                node.GetDeclarationElement(i).Accept(this, obj);
            }
            if (found) return this.table;
            usings.Remove(node.Identifier.Identifier);

            //this.currentNamespace = "";
            this.table.Reset();
            return this.table;
        }

        #endregion

        #region Visit(ClassDefinition node, Object obj)

        public override Object Visit(ClassDefinition node, Object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }

            this.table.Set();
            //this.currentClass = node.Identifier;
            //this.thisType = (UserType)node.TypeExpr;
            //this.baseType = ((ClassType)node.TypeExpr).BaseClass;

            // first step
            for (int i = 0; i < node.MemberCount; i++)
            {
                if (node.GetMemberElement(i) is FieldDeclaration)
                    this.fieldDeclarationSymbol((FieldDeclaration)node.GetMemberElement(i));
            }

            // second step
            for (int i = 0; i < node.MemberCount; i++)
            {
                if (found) return this.table;
                node.GetMemberElement(i).Accept(this, obj);
            }
            if (found) return this.table;
            //this.currentClass = "";
            //this.thisType = null;
            //this.baseType = null;
            this.table.Reset();
            return this.table;
        }

        #endregion

        #region Visit(IdDeclaration node, Object obj)

        public override Object Visit(IdDeclaration node, Object obj)
        {
            declarationSymbol(node);

            //if (node.Location == ((AstNode)obj).Location || found)
            //{
            //    found = true;
            //    return this.table;
            //}
            //node.Symbol = declarationSymbol(node);
            return this.table;
        }

        #endregion

        #region Visit(Definition node, Object obj)

        public override Object Visit(Definition node, Object obj)
        {
            declarationSymbol(node);

            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            node.Init.Accept(this, obj);
            //node.Symbol = declarationSymbol(node);
            return this.table;
        }

        #endregion

        #region Visit(ConstantDefinition node, Object obj)

        public override Object Visit(ConstantDefinition node, Object obj)
        {
            declarationSymbol(node);

            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            node.Init.Accept(this, obj);
            //node.Symbol = declarationSymbol(node);
            return this.table;
        }

        #endregion

        #region declarationSymbol()

        /// <summary>
        /// Inserts the symbol associated to the declaration
        /// </summary>
        /// <param name="node">Declaration information.</param>
        /// <returns>The symbol inserted</returns>
        private Symbol declarationSymbol(IdDeclaration node)
        {
            string id = node.Identifier;
            TypeExpression te = this.searchType(node.FullName, node.Location.Line, node.Location.Column);
            if (node.IdentifierExp.IndexOfSSA != -1 && this.includeSSAVars)
                id += node.IdentifierExp.IndexOfSSA;

            //this.table.Insert(id, te,false);
            this.table.Insert(id, node);

            //TypeExpression te = this.searchType(node.FullName, node.Location.Line, node.Location.Column);
            //if (te != null)
            //{
            //    //node.TypeExpr = te;
            //    string id = node.Identifier;
            //    //if (node.IdentifierExp.IndexOfSSA != -1)
            //    //    id += node.IdentifierExp.IndexOfSSA;
            //    if (node.IdentifierExp.IndexOfSSA != -1 && this.includeSSAVars)
            //        id += node.IdentifierExp.IndexOfSSA;

            //    //this.table.Insert(id, te,false);
            //    this.table.Insert(id,node);
               
            //}
            //ErrorManager.Instance.NotifyError(new UnknownTypeError(node.FullName, new Location(this.currentFile, node.Location.Line, node.Location.Column)));
            return null;
        }

        #endregion

        #region fieldDeclarationSymbol()

        /// <summary>
        /// Inserts the symbol associated to the field
        /// </summary>
        /// <param name="node">Field information.</param>
        private void fieldDeclarationSymbol(FieldDeclaration node)
        {
            this.table.Insert(node.Identifier, node);
            //this.table.Insert(node.Identifier, node.TypeExpr, false);

            //if (node.TypeExpr == null)
            //{
            //    ErrorManager.Instance.NotifyError(new UnknownTypeError(node.FullName, new Location(this.currentFile, node.Location.Line, node.Location.Column)));
            //    return;
            //}
            ////bool isDynamic = searchDynInfo(node.Identifier);
            //if (this.table.Insert(node.Identifier, node.TypeExpr, false) == null)
            //{
            //    ErrorManager.Instance.NotifyError(new DeclarationFoundError(node.Identifier, new Location(this.currentFile, node.Location.Line, node.Location.Column)));
            //    return;
            //}
            //DynVarOptions.Instance.AssignDynamism(this.thisType.Fields[node.Identifier].Type, isDynamic);
        }

        #endregion

        #region Visit(MethodDefinition node, Object obj)

        public override Object Visit(MethodDefinition node, Object obj)
        {
            //if (node.Location == ((AstNode)obj).Location || found)
            //{
            //    found = true;
            //    return this.table;
            //}

            //this.blockList = new List<int>();
            //this.indexBlockList = 0;

            this.table.Set();
            //this.currentMethod = node.Identifier;
            //node.IsReturnDynamic = this.searchDynInfo("");
            this.parameterSymbol(node);

            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }

            Location paramLocation;
            foreach (var param in node.ParametersInfo)
            {
                paramLocation = new Location(node.Location.FileName, param.Line, param.Column);

                if (paramLocation == ((AstNode)obj).Location)
                {
                    found = true;
                    return this.table;
                }

            }


            if (found) return this.table;
            node.Body.Accept(this, obj);
            if (found) return this.table;
            //this.currentMethod = "";
            this.table.Reset();
            return this.table;
        }

        #endregion

        #region Visit(ConstructorDefinition node, Object obj)

        public override Object Visit(ConstructorDefinition node, Object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            //this.blockList = new List<int>();
            //this.indexBlockList = 0;

            this.table.Set();
            //this.currentMethod = node.Identifier;
            this.parameterSymbol(node);

            if (node.Initialization != null)
                node.Initialization.Accept(this, obj);

            if (found) return this.table;

            node.Body.Accept(this, obj);
            if (found) return this.table;
            //this.currentMethod = "";
            this.table.Reset();
            return this.table;
        }

        #endregion

        #region parameterSymbol()

        private void parameterSymbol(MethodDefinition node)
        {
            var method = node as MethodDefinition;
            if (method == null || method.TypeExpr== null) return;
            for (int i = 0; i < ((MethodType)node.TypeExpr).ParameterListCount; i++)
            {
                string id = node.ParametersInfo[i].Identifier;
                var exp = new SingleIdentifierExpression(id, node.Location);
                var type = ((MethodType)node.TypeExpr).GetParameter(i);
                IdDeclaration dec=new IdDeclaration(exp,type.FullName,node.Location);
                dec.TypeExpr = type;
               

                this.table.Insert(id, dec);
                //this.table.Insert(id, ((MethodType)node.TypeExpr).GetParameter(i), false);

                //if (!(((MethodType)node.TypeExpr).GetParameter(i) is ArrayType))
                //    id += ": (parameter)";
                ////if (this.table.Insert(id, ((MethodType)node.TypeExpr).GetParameter(i), searchDynInfo(node.ParametersInfo[i].Identifier)) == null)
                //if (this.table.Insert(id, ((MethodType)node.TypeExpr).GetParameter(i), false) == null)
                //    ErrorManager.Instance.NotifyError(new DeclarationFoundError(node.ParametersInfo[i].Identifier, new Location(this.currentFile, node.Location.Line, node.Location.Column)));
            }
        }

        #endregion

        // Literals

        #region Visit(SingleIdentifierExpression node, Object obj)

        public override Object Visit(SingleIdentifierExpression node, Object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            //Symbol sym = null;
            //if (node.IndexOfSSA != -1)
            //    sym = this.table.Search(node.Identifier + node.IndexOfSSA);
            //else
            //    sym = this.table.Search(node.Identifier);

            //if (sym != null)
            //    node.IdSymbol = (Symbol)sym;

            return this.table;
        }

        #endregion

        #region searchType()

        /// <summary>
        /// Searches a type expression associated to the specified name.
        /// </summary>
        /// <param name="typeIdentifier">WriteType name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <returns>WriteType expression associated to the name.</returns>
        private TypeExpression searchType(string typeIdentifier, int line, int column)
        {
            TypeExpression te = null;
            bool found = false;
            int rank = 0;

            while ((typeIdentifier.Contains("[]")) && (!found))
            {
                te = TypeTable.Instance.GetType(typeIdentifier, new Location(this.currentFile, line, column));
                if (te == null)
                {
                    typeIdentifier = typeIdentifier.Substring(0, typeIdentifier.Length - 2);
                    rank++;
                }
                else
                    found = true;
            }

            if (!found)
                te = TypeTable.Instance.GetType(typeIdentifier, new Location(this.currentFile, line, column));

            if (te == null)
            {
                for (int i = 0; i < usings.Count; i++)
                {
                    StringBuilder str = new StringBuilder();
                    str.Append(usings[i]);
                    str.Append(".");
                    str.Append(typeIdentifier);

                    te = TypeTable.Instance.GetType(str.ToString(), new Location(this.currentFile, line, column));
                    if (te != null)
                        break;
                }
            }

            if (te != null)
            {
                if (rank != 0)
                {
                    for (int i = 0; i < rank; i++)
                    {
                        te = new ArrayType(te);
                        if (!TypeTable.Instance.ContainsType(te.FullName))
                            TypeTable.Instance.AddType(te.FullName, te, new Location(this.currentFile, line, column));
                    }
                }
            }

            return te;
        }

        #endregion

        #region compoundExpressionToArray()

        /// <summary>
        /// Gets the type expression of the arguments.
        /// </summary>
        /// <param name="args">Arguments information.</param>
        /// <returns>Returns the argument type expressions </returns>
        private TypeExpression[] compoundExpressionToArray(CompoundExpression args)
        {
            TypeExpression[] aux = new TypeExpression[args.ExpressionCount];
            TypeExpression te;

            for (int i = 0; i < args.ExpressionCount; i++)
            {
                if ((te = args.GetExpressionElement(i).ExpressionType) != null)
                    aux[i] = te;
                else
                    return null;
            }
            return aux;
        }

        #endregion

        // Statements

        #region Visit(Block node, Object obj)

        public override Object Visit(Block node, Object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }

            if (node.StatementCount != 0)
            {
                //if (!(obj is MethodDefinition)) // Set block
                //{
                //    if (this.indexBlockList >= this.blockList.Count)
                //        this.blockList.Add(0);
                //    else
                //        this.blockList[indexBlockList] = this.blockList[indexBlockList] + 1;
                //    this.indexBlockList++;
                //}

                for (int i = 0; i < node.StatementCount; i++)
                {
                    if (found) return this.table;
                    node.GetStatementElement(i).Accept(this, obj);
                }

                //if (!(obj is MethodDefinition))
                //{
                //    for (int i = this.indexBlockList; i < this.blockList.Count; i++)
                //        this.blockList.RemoveAt(i);
                //    this.indexBlockList--;
                //}
            }

            return this.table;
        }

        #endregion

        #region Visit(DoStatement node, Object obj)

        public override Object Visit(DoStatement node, Object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }

            for (int i = 0; i < node.InitDo.Count; i++)
            {
                if (found) return this.table;
                node.InitDo[i].Accept(this, obj);
            }
            if (found) return this.table;
            this.table.Set();
            for (int i = 0; i < node.BeforeBody.Count; i++)
            {
                if (found) return this.table;
                node.BeforeBody[i].Accept(this, obj);
            }

            if (found) return this.table;
            node.Statements.Accept(this, obj);
            if (found) return this.table;
            this.table.Reset();

            if (found) return this.table;
            node.Condition.Accept(this, obj);
            if (found) return this.table;
            return this.table;
        }

        #endregion

        #region Visit(ForStatement node, Object obj)

        public override Object Visit(ForStatement node, Object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }

            this.table.Set();
            for (int i = 0; i < node.InitializerCount; i++)
            {
                if (found) return this.table;
                node.GetInitializerElement(i).Accept(this, obj);
            }
            for (int i = 0; i < node.AfterInit.Count; i++)
            {
                if (found) return this.table;
                node.AfterInit[i].Accept(this, obj);
            }
            for (int i = 0; i < node.BeforeCondition.Count; i++)
            {
                if (found) return this.table;
                node.BeforeCondition[i].Accept(this, obj);
            }
            if (found) return this.table;
            node.Condition.Accept(this, obj);
            if (found) return this.table;
            for (int i = 0; i < node.AfterCondition.Count; i++)
            {
                if (found) return this.table;
                node.AfterCondition[i].Accept(this, obj);
            }
            if (found) return this.table;
            node.Statements.Accept(this, obj);
            if (found) return this.table;
            for (int i = 0; i < node.IteratorCount; i++)
            {
                if (found) return this.table;
                node.GetIteratorElement(i).Accept(this, obj);
            }
            if (found) return this.table;
            this.table.Reset();
            return this.table;
        }

        #endregion

        #region Visit(IfElseStatement node, Object obj)

        public override Object Visit(IfElseStatement node, Object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            if (found) return this.table;
            node.Condition.Accept(this, obj);
            if (found) return this.table;
            for (int i = 0; i < node.AfterCondition.Count; i++)
            {
                if (found) return this.table;
                node.AfterCondition[i].Accept(this, obj);
                if (found) return this.table;
            }
            this.table.Set();
            if (found) return this.table;
            node.TrueBranch.Accept(this, obj);
            if (found) return this.table;
            this.table.Reset();
            this.table.Set();
            if (found) return this.table;
            node.FalseBranch.Accept(this, obj);
            if (found) return this.table;
            this.table.Reset();
            for (int i = 0; i < node.ThetaStatements.Count; i++)
            {
                if (found) return this.table;
                node.ThetaStatements[i].Accept(this, obj);
            }
            return this.table;
        }

        #endregion

        #region Visit(WhileStatement node, Object obj)

        public override Object Visit(WhileStatement node, Object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }

            for (int i = 0; i < node.InitWhile.Count; i++)
            {
                if (found) return this.table;
                node.InitWhile[i].Accept(this, obj);
            }
            for (int i = 0; i < node.BeforeCondition.Count; i++)
            {
                if (found) return this.table;
                node.BeforeCondition[i].Accept(this, obj);
            }
            if (found) return this.table;
            node.Condition.Accept(this, obj);
            for (int i = 0; i < node.AfterCondition.Count; i++)
            {
                if (found) return this.table;
                node.AfterCondition[i].Accept(this, obj);
            }
            this.table.Set();
            if (found) return this.table;
            node.Statements.Accept(this, obj);
            if (found) return this.table;
            this.table.Reset();
            return this.table;
        }

        #endregion

        #region Visit(SwitchStatement node, Object obj)

        public override Object Visit(SwitchStatement node, Object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }

            node.Condition.Accept(this, obj);
            for (int i = 0; i < node.AfterCondition.Count; i++)
            {
                if (found) return this.table;
                node.AfterCondition[i].Accept(this, obj);
            }

            for (int i = 0; i < node.SwitchBlockCount; i++)
            {
                this.table.Set();
                if (found) return this.table;
                node.GetSwitchSectionElement(i).Accept(this, obj);
                if (found) return this.table;
                this.table.Reset();
            }
            for (int i = 0; i < node.ThetaStatements.Count; i++)
            {
                if (found) return this.table;
                node.ThetaStatements[i].Accept(this, obj);
            }
            return this.table;
        }

        #endregion

        public override object Visit(IntLiteralExpression node, object obj)
        {
            base.Visit(node, obj);

            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;

            }
            return this.table;
        }

        public override object Visit(ArgumentExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }

            return base.Visit(node, obj);
        }

        public override object Visit(ArithmeticExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }

            return base.Visit(node, obj);
        }

        public override object Visit(ArrayAccessExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }
        public override object Visit(AssertStatement node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }
        public override object Visit(AssignmentExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(BaseCallExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(BaseExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }
        public override object Visit(BinaryExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(BitwiseExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(BoolLiteralExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(BreakStatement node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(CastExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(CatchStatement node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(CharLiteralExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(CompoundExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(ConstantFieldDefinition node, object obj)
        {
            base.Visit(node, obj);

            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;

            }
            return this.table;
        }

        public override object Visit(ContinueStatement node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(DeclarationSet node, object obj)
        {
            base.Visit(node, obj);

            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                
            }
            return this.table;
        }

        public override object Visit(DoubleLiteralExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(ExceptionManagementStatement node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(FieldAccessExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(FieldDeclaration node, object obj)
        {
            base.Visit(node, obj);

            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;

            }
            return this.table;
        }

        public override object Visit(FieldDeclarationSet node, object obj)
        {
            base.Visit(node, obj);

            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;

            }
            return this.table;
        }

        public override object Visit(FieldDefinition node, object obj)
        {
            base.Visit(node, obj);

            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;

            }
            return this.table;
        }

        public override object Visit(ForeachStatement node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(InterfaceDefinition node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(InvocationExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(IsExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(LogicalExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(MethodDeclaration node, object obj)
        {
            base.Visit(node, obj);

            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;

            }
            return this.table;
        }

        public override object Visit(MoveStatement node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(NewArrayExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(NewExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(NullExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(PropertyDefinition node, object obj)
        {
            base.Visit(node, obj);

            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;

            }
            return this.table;
        }

        public override object Visit(QualifiedIdentifierExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(RelationalExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(ReturnStatement node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(StringLiteralExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(SwitchLabel node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(SwitchSection node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(TernaryExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(ThetaStatement node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(ThisExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(ThrowStatement node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }

        public override object Visit(UnaryExpression node, object obj)
        {
            if (node.Location == ((AstNode)obj).Location || found)
            {
                found = true;
                return this.table;
            }
            return base.Visit(node, obj);
        }
    }
}
