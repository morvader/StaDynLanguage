////////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- //
// Project rROTOR                                                             //
// -------------------------------------------------------------------------- //
// File: VisitorCodeGeneration.cs                                             //
// Author: Cristina Gonzalez Muñoz  -  cristi.gm@gmail.com                    //
//         Francisco Ortin - francisco.ortin@gmail.com                        //
//         Daniel Zapico Rodríguez - daniel.zapico.rodriguez@gmail.com        //
// Description:                                                               //
//    This class walks the AST to obtain the IL code.                         //
//    Inheritance: VisitorAdapter                                             //
//    Implements Visitor pattern [Concrete Visitor].                          //
//    Implements Factory method (the constructor) [Creator].                  //
// -------------------------------------------------------------------------- //
// Create date: 28-05-2007                                                    //
// Modification date: 21-08-2007                                              //
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using AST;
using TypeSystem;
using CodeGeneration.Operations;
using TypeSystem.Operations;

namespace CodeGeneration {
    /// <summary>
    /// This class walks the AST to obtain the IL code.
    /// </summary>
    /// <remarks>
    /// Inheritance: VisitorAdapter.
    /// Implements Visitor pattern [Concrete Visitor].
    /// </remarks>
    abstract class VisitorILCodeGeneration<T> : VisitorCodeGeneration<T> where T : ILCodeGenerator {
        #region Fields

        /// <summary>
        /// Stores the field declarations. Index 0: non static fields. Index1: static fields.
        /// </summary>
        private List<FieldDefinition>[] fields;

        /// <summary>
        /// Represents the indentation to write in the code file.
        /// </summary>
        protected int indent;

        /// <summary>
        /// Stores the initialization of the constants.
        /// </summary>
        private ConstantTable constantsTable;

        /// <summary>
        /// Identifier used to create auxiliar local variables for fields.
        /// </summary>
        private const string auxFieldVar = "v_local_field_temp_";

        /// <summary>
        /// Identifier used to create an auxiliar local variable used in switch conditions.
        /// </summary>
        private const string auxSwitchCond = "v_local_condition_temp_";

        /// <summary>
        /// Number used to create the identifier of auxiliar variables.
        /// </summary>
        private int currentNumber;

        #endregion

        private int namespaceDepth = 0;

        //int indent = 0;
        #region Constructor

        /// <summary>
        /// Constructor of VisitorCodeGeneration
        /// </summary>
        /// <param name="nameModule">WriteModule name.</param>
        /// <param name="targetPlatform">The target platform</param>
        public VisitorILCodeGeneration(string moduleName, T codeGenerator) :
            base(codeGenerator) {
            this.codeGenerator.WriteHeader(moduleName);
            this.indent = 0;
            this.constantsTable = new ConstantTable();
            this.currentNumber = 0;
        }

        #endregion

        #region Close()

        /// <summary>
        /// Closes the code generator.
        /// </summary>
        public override void Close() {
            this.codeGenerator.Close();
        }

        #endregion

        #region AddExceptionCode()

        /// <summary>
        /// Adds the intermediate code for exception to include in the file.
        /// </summary>
        public override void AddExceptionCode() {
            this.codeGenerator.WriteCodeOfExceptions();
        }

        #endregion

        // Declaration

        #region Visit(SourceFile node, Object obj)

        public override Object Visit(SourceFile node, Object obj) {
            this.codeGenerator.InitialComment();

            foreach (string key in node.Namespacekeys) {
                int count = node.GetNamespaceDefinitionCount(key);
                for (int i = 0; i < count; i++)
                    node.GetNamespaceDeclarationElement(key, i).Accept(this, obj);
            }
            for (int i = 0; i < node.DeclarationCount; i++)
                node.GetDeclarationElement(i).Accept(this, obj);

            return null;
        }

        #endregion

        #region Visit(Namespace node, Object obj)

        public override Object Visit(Namespace node, Object obj) {
            this.namespaceDepth++;
            this.codeGenerator.WriteNamespaceHeader(this.indent++, node.Identifier.Identifier);
            for (int i = 0; i < node.NamespaceMembersCount; i++)
                node.GetDeclarationElement(i).Accept(this, obj);

            this.codeGenerator.WriteEndOfBlock(--this.indent);
            this.namespaceDepth--;
            return null;
        }

        #endregion

        #region Visit(ClassDefinition node, Object obj)

        public override Object Visit(ClassDefinition node, Object obj) {
            this.constantsTable.Set();
            this.fields = (List<FieldDefinition>[])node.Accept(new VisitorCodeGeneration2(), null);

            node.TypeExpr.AcceptOperation(new CGClassDefinitionStartOperation<T>(this.codeGenerator, this.indent, node), null);
            this.indent++;

            for (int i = 0; i < node.MemberCount; i++)
                node.GetMemberElement(i).Accept(this, obj);

            if (this.fields[0].Count != 0)
                this.AddConstructor((ClassType)node.TypeExpr, new InheritedAttributes(null, false, null, false, null, false));

            if (this.fields[1].Count != 0)
                this.AddStaticConstructor((ClassType)node.TypeExpr, new InheritedAttributes(null, false, null, false, null, false));

            this.codeGenerator.WriteEndOfClass(--this.indent, node.FullName);
            this.constantsTable.Reset();

            return null;
        }
        #endregion

        #region Visit(InterfaceDefinition node, Object obj)

        public override Object Visit(InterfaceDefinition node, Object obj) {
            node.TypeExpr.AcceptOperation(new CGInterfaceDefinitionStartOperation<T>(this.codeGenerator, this.indent, node), null);
            this.indent++;

            for (int i = 0; i < node.MemberCount; i++)
                node.GetMemberElement(i).Accept(this, obj);

            this.codeGenerator.WriteEndOfInterface(--this.indent, node.FullName);

            return null;
        }

        #endregion

        #region Visit(FieldDeclaration node, Object obj)

        public override Object Visit(FieldDeclaration node, Object obj) {
            return node.TypeExpr.AcceptOperation(new CGProcessFieldOperation<T>(this.codeGenerator, this.indent, node, false, obj), null);
        }

        #endregion

        #region Visit(FieldDefinition node, Object obj)

        public override Object Visit(FieldDefinition node, Object obj) {
            return node.TypeExpr.AcceptOperation(new CGProcessFieldOperation<T>(this.codeGenerator, this.indent, node, false, obj), null);
        }

        #endregion

        #region Visit(ConstantFieldDefinition node, Object obj)

        public override Object Visit(ConstantFieldDefinition node, Object obj) {
            node.TypeExpr.AcceptOperation(new CGProcessFieldOperation<T>(this.codeGenerator, this.indent, node, true, obj), null);
            node.Init.ILTypeExpression.AcceptOperation(new CGConstantFieldDefinitionInitializationOperation<T>(this.codeGenerator, node, obj), null);
            this.constantsTable.Insert(node.Identifier, node.Init);

            return null;
        }

        #endregion

        #region Visit(MethodDeclaration node, Object obj)

        public override Object Visit(MethodDeclaration node, Object obj) {
            node.TypeExpr.AcceptOperation(new CGProcessMethodOperation<T>(this.codeGenerator, this.indent, node, obj), null);
            // as the method is not defined we generate code for an empty block
            this.codeGenerator.WriteStartBlock(this.indent);
            this.codeGenerator.WriteEndOfBlock(this.indent);
            return null;
        }

        #endregion

        #region Visit(MethodDefinition node, Object obj)

        public override Object Visit(MethodDefinition node, Object obj) {
            // if the method is abstract generate an empty body and return
            if (node.ModifiersInfo.Contains(Modifier.Abstract)) {
                node.TypeExpr.AcceptOperation(new CGProcessMethodOperation<T>(this.codeGenerator, this.indent, node, obj), null);
                this.codeGenerator.WriteStartBlock(this.indent++);
                this.codeGenerator.WriteEndOfMethod(--this.indent, ((MethodType)node.TypeExpr).MemberInfo.MemberIdentifier);
                return null;
            }

            this.constantsTable.Set();
            this.currentNumber = 0;

            node.TypeExpr.AcceptOperation(new CGProcessMethodOperation<T>(this.codeGenerator, this.indent, node, obj), null);
            this.codeGenerator.WriteStartBlock(this.indent++);
            if (node.IsEntryPoint)
                this.codeGenerator.WriteEntryPoint(this.indent);

            // First step. Search the information of local variables.
            List<IdDeclaration> decls = (List<IdDeclaration>)node.Accept(new VisitorCodeGeneration2(), null);
            this.ProcessLocalVars(decls);
            this.codeGenerator.WriteLocalVariable(this.indent);

            // Second step. Visits the method body.
            node.Body.Accept(this, new InheritedAttributes(node, false, null, false, null, false));

            // Returns always ...
            //if (node.ReturnTypeInfo.Equals(VoidType.Instance.FullName))
            this.codeGenerator.ret(this.indent--);
            this.codeGenerator.WriteEndOfMethod(this.indent, ((MethodType)node.TypeExpr).MemberInfo.MemberIdentifier);
            this.constantsTable.Reset();

            return null;
        }

        #endregion

        #region Visit(ConstructorDefinition node, Object obj)

        public override Object Visit(ConstructorDefinition node, Object obj) {
            this.constantsTable.Set();
            this.currentNumber = 0;

            node.TypeExpr.AcceptOperation(new CGProcessMethodOperation<T>(this.codeGenerator, this.indent, node, obj), null);
            this.codeGenerator.WriteStartBlock(this.indent++);

            // First of all, writes the IL code of fields initialization.
            if ((((MethodType)node.TypeExpr).MemberInfo.ModifierMask & Modifier.Static) != 0)
                this.WriteInitStaticFields(new InheritedAttributes(node, false, null, false, null, false));
            else {
                this.WriteInitFields(new InheritedAttributes(node, false, null, false, null, false));
                this.codeGenerator.ldarg(this.indent, 0);
                if (node.Initialization != null)
                    node.Initialization.Accept(this, new InheritedAttributes(node, false, null, false, null, false));
                else
                    // calls the default base constructor
                    this.codeGenerator.constructorCall(this.indent, ((ClassType)((MethodType)node.TypeExpr).MemberInfo.Class).BaseClass, ".ctor");
            }

            // Second step. Search the information of local variables.
            List<IdDeclaration> decls = (List<IdDeclaration>)node.Accept(new VisitorCodeGeneration2(), null);
            this.ProcessLocalVars(decls);
            this.codeGenerator.WriteLocalVariable(this.indent);

            // Third step. Visits the method body
            node.Body.Accept(this, new InheritedAttributes(node, false, null, false, null, false));
            this.codeGenerator.ret(this.indent--);
            this.codeGenerator.WriteEndOfMethod(this.indent, ((MethodType)node.TypeExpr).MemberInfo.MemberIdentifier);

            this.constantsTable.Reset();

            return null;
        }

        #endregion

        #region WriteInitFields()

        private void WriteInitFields(Object obj) {
            Object o;
            for (int i = 0; i < this.fields[0].Count; i++) {
                o = this.fields[0][i].Init.Accept(new VisitorCodeGeneration2(), null);
                if (o is List<IdDeclaration>)
                    this.ProcessLocalVars((List<IdDeclaration>)o);
            }

            this.codeGenerator.WriteLocalVariable(this.indent);

            for (int i = 0; i < this.fields[0].Count; i++) {
                this.codeGenerator.ldarg(this.indent, 0);
                this.fields[0][i].Init.Accept(this, obj);
                this.codeGenerator.stfld(this.indent, this.fields[0][i].TypeExpr, ((FieldType)this.fields[0][i].TypeExpr).MemberInfo.Class.FullName, this.fields[0][i].Identifier);
            }
            this.fields[0].Clear();
        }

        #endregion

        #region WriteInitStaticFields()

        private void WriteInitStaticFields(Object obj) {
            Object o;
            for (int i = 0; i < this.fields[1].Count; i++) {
                o = this.fields[1][i].Init.Accept(new VisitorCodeGeneration2(), null);
                if (o is List<IdDeclaration>)
                    this.ProcessLocalVars((List<IdDeclaration>)o);
            }

            this.codeGenerator.WriteLocalVariable(this.indent);

            for (int i = 0; i < this.fields[1].Count; i++) {
                this.fields[1][i].Init.Accept(this, obj);
                this.codeGenerator.stsfld(this.indent, this.fields[1][i].TypeExpr, ((FieldType)this.fields[1][i].TypeExpr).MemberInfo.Class.FullName, this.fields[1][i].Identifier);
            }
            this.fields[1].Clear();
        }

        #endregion

        #region AddConstructor()

        private void AddConstructor(ClassType classType, Object obj) {
            this.codeGenerator.WriteConstructorHeader(this.indent);
            this.codeGenerator.WriteStartBlock(this.indent++);
            this.WriteInitFields(obj);
            this.codeGenerator.ldarg(this.indent, 0);
            this.codeGenerator.constructorCall(this.indent, classType.BaseClass, ".ctor");
            this.codeGenerator.ret(this.indent--);

            this.codeGenerator.WriteEndOfMethod(this.indent, classType.Name);
        }

        #endregion

        #region AddStaticConstructor()

        private void AddStaticConstructor(ClassType classType, Object obj) {
            this.codeGenerator.WriteStaticConstructorHeader(this.indent);
            this.codeGenerator.WriteStartBlock(this.indent++);
            this.WriteInitStaticFields(obj);
            this.codeGenerator.ret(this.indent--);
            this.codeGenerator.WriteEndOfMethod(this.indent, classType.Name);
        }

        #endregion

        //#region ProcessMethod()

        //private void ProcessMethod(MethodDeclaration node, Object obj) {
        //    node.TypeExpr.AcceptOperation(new CGProcessMethodOperation<T>(this.codeGenerator, this.indent, node, obj));
        //}

        //#endregion

        //#region ProcessField()

        //private void ProcessField(FieldDeclaration node, Object obj, bool constantField) {
        //    node.TypeExpr.AcceptOperation(new CGProcessFieldOperation<T>(this.codeGenerator, this.indent, node, constantField, obj));
        //}

        //#endregion

        #region CheckMakeAnUnbox()

        internal bool CheckMakeAnUnbox(Expression exp) {
            //// For BaseCallExpression, InvocationExpression y NewExpression.
            //// 
            //MethodType mt = null;
            //InvocationExpression ie;
            //BaseCallExpression bc;
            //NewExpression ne;
            //TypeVariable tVar;
            //FieldType field;

            //if (exp is ArgumentExpression)
            //    exp = ((ArgumentExpression)exp).Argument;

            //if ((ie = exp as InvocationExpression) != null) {
            //    // ActualMethodCalled can be a MethodType or a TypeVariable with a method like substitution.
            //    mt = ie.ActualMethodCalled as MethodType;
            //    if (mt == null) {  // ActualMethodCalled is a TypeVariable or UnionType

            //        if ((tVar = ie.ActualMethodCalled as TypeVariable) != null)
            //            mt = tVar.Substitution as MethodType;
            //    }
            //} else {
            //    if ((bc = exp as BaseCallExpression) != null) {
            //        mt = bc.ActualMethodCalled as MethodType;
            //        if (mt == null) { // ActualMethodCalled is a TypeVariable or UnionType

            //            if ((tVar = bc.ActualMethodCalled as TypeVariable) != null)
            //                mt = tVar.Substitution as MethodType;
            //        }
            //    } else {
            //        if ((ne = exp as NewExpression) != null) {
            //            mt = ne.ActualMethodCalled as MethodType;
            //            if (mt == null) {// ActualMethodCalled is a TypeVariable or UnionType
            //                if ((tVar = ne.ActualMethodCalled as TypeVariable) != null)
            //                    mt = tVar.Substitution as MethodType;
            //            }
            //        }
            //    }
            //}

            //if (mt != null)
            //    return !(mt.Return.IsValueType());

            //if ((field = exp.ExpressionType as FieldType) != null)
            //    return (field.FieldTypeExpression is TypeVariable);

            //return false;

            if (exp is ArgumentExpression)
                exp = ((ArgumentExpression)exp).Argument;

            return (bool)exp.AcceptOperation(new Ast.Operations.CheckMakeAnUnboxOperation(), null);
        }

        #endregion

        #region Visit(Definition node, Object obj)
        // no hago mas qe una operación pues me saldrían jerarquias de clase distintas
        public override Object Visit(Definition node, Object obj) {
            //node.Init.Accept(this, obj);
            //InvocationExpression i;
            //MethodType mt;
            //if ((i = node.Init as InvocationExpression) != null) {
            //    if ((mt = i.ActualMethodCalled as MethodType) != null) {
            //        // Both elements of condition must be the same, but it is not true, the promotion is different.
            //        if (node.Init.ILTypeExpression.IsValueType() && !mt.Return.IsValueType())
            //            this.codeGenerator.Promotion(this.indent, mt.Return, mt.Return, node.TypeExpr, node.ILTypeExpression, true, CheckMakeAnUnbox(node.Init));
            //        else
            //            this.codeGenerator.Promotion(this.indent, node.Init.ExpressionType, node.Init.ILTypeExpression, node.TypeExpr, node.ILTypeExpression, true, CheckMakeAnUnbox(node.Init));
            //    }
            //} else
            //    this.codeGenerator.Promotion(this.indent, node.Init.ExpressionType, node.Init.ILTypeExpression, node.TypeExpr, node.ILTypeExpression, true, CheckMakeAnUnbox(node.Init));

            //this.codeGenerator.stloc(this.indent, node.ILName);
            //this.codeGenerator.WriteLine();
            //return null;
            return node.AcceptOperation(new CGVisitDefinitionNodeOperation<T>(this, node, this.codeGenerator, this.indent, obj), null);
        }


        #endregion

        #region Visit(ConstantDefinition node, Object obj)

        public override Object Visit(ConstantDefinition node, Object obj) {
            return this.constantsTable.Insert(node.Identifier, node.Init);
        }

        #endregion

        #region ProcessLocalVars()

        private void ProcessLocalVars(List<IdDeclaration> decls) {
            //this.codeGenerator.LocalVariableIndex = 0;
            for (int i = 0; i < decls.Count; i++)
                this.codeGenerator.AddLocalVariable(decls[i].ILName, decls[i].ILTypeExpression);
        }

        #endregion

        // obj param stores a code generation helper for each statement or expression.
        //  - stores current method information.
        //  - stores information about assignment expressions.
        //    True if it is necessary to create a store instruction. Otherwise, false.
        //  - stores reference information of a concrete identifier.
        //  - stores information about array access.
        //    True if array access expression found. Otherwise, false.
        //  - stores the type expression of the current invocation. 
        //    If not exists, their value is null.
        //  - stores information about the parent node.
        //    True if the parent node is an InvocationExpression. Otherwise, false.

        // Expressions

        #region Visit(ArithmeticExpression node, Object obj)

        public override Object Visit(ArithmeticExpression node, Object obj) {
            node.FirstOperand.Accept(this, obj);
            // Checks if it is necessary to make a implicit conversion
            this.codeGenerator.Promotion(this.indent, node.FirstOperand.ExpressionType, node.FirstOperand.ILTypeExpression, node.ExpressionType, node.ExpressionType, true, CheckMakeAnUnbox(node.FirstOperand));

            node.SecondOperand.Accept(this, obj);
            // Checks if it is necessary to make a implicit conversion
            this.codeGenerator.Promotion(this.indent, node.SecondOperand.ExpressionType, node.SecondOperand.ILTypeExpression, node.ExpressionType, node.ExpressionType, true, CheckMakeAnUnbox(node.SecondOperand));

            switch (node.Operator) {
                case ArithmeticOperator.Minus:
                    this.codeGenerator.sub(this.indent);
                    break;
                case ArithmeticOperator.Plus:
                    if (node.ExpressionType is StringType)
                        this.codeGenerator.concat(this.indent);
                    else
                        this.codeGenerator.add(this.indent);
                    break;
                case ArithmeticOperator.Mult:
                    this.codeGenerator.mul(this.indent);
                    break;
                case ArithmeticOperator.Div:
                    this.codeGenerator.div(this.indent);
                    break;
                case ArithmeticOperator.Mod:
                    this.codeGenerator.rem(this.indent);
                    break;
                default:
                    break;
            }
            return null;
        }

        #endregion

        #region Visit(BitwiseExpression node, Object obj)

        public override Object Visit(BitwiseExpression node, Object obj) {
            node.FirstOperand.Accept(this, obj);
            // Checks if it is necessary to make a implicit conversion
            this.codeGenerator.Promotion(this.indent, node.FirstOperand.ExpressionType, node.FirstOperand.ILTypeExpression, node.ExpressionType, node.ExpressionType, true, CheckMakeAnUnbox(node.FirstOperand));

            node.SecondOperand.Accept(this, obj);
            // Checks if it is necessary to make a implicit conversion
            this.codeGenerator.Promotion(this.indent, node.SecondOperand.ExpressionType, node.SecondOperand.ILTypeExpression, node.ExpressionType, node.ExpressionType, true, CheckMakeAnUnbox(node.SecondOperand));

            switch (node.Operator) {
                case BitwiseOperator.BitwiseOr:
                    this.codeGenerator.or(this.indent);
                    break;
                case BitwiseOperator.BitwiseAnd:
                    this.codeGenerator.and(this.indent);
                    break;
                case BitwiseOperator.BitwiseXOr:
                    this.codeGenerator.xor(this.indent);
                    break;
                case BitwiseOperator.ShiftLeft:
                    this.codeGenerator.shl(this.indent);
                    break;
                case BitwiseOperator.ShiftRight:
                    this.codeGenerator.shr(this.indent);
                    break;
                default:
                    break;
            }
            return null;
        }

        #endregion

        #region Visit(LogicalExpression node, Object obj)

        public override Object Visit(LogicalExpression node, Object obj) {
            string label0 = this.codeGenerator.NewLabel;
            string label1 = this.codeGenerator.NewLabel;

            node.FirstOperand.Accept(this, obj);
            // Checks if it is necessary to make a implicit conversion
            this.codeGenerator.Promotion(this.indent, node.FirstOperand.ExpressionType, node.FirstOperand.ILTypeExpression, node.ExpressionType, node.ExpressionType, true, CheckMakeAnUnbox(node.FirstOperand));

            switch (node.Operator) {
                case LogicalOperator.Or:
                    this.codeGenerator.brtrue(this.indent, label0);
                    break;
                case LogicalOperator.And:
                    this.codeGenerator.brfalse(this.indent, label0);
                    break;
                default:
                    break;
            }

            node.SecondOperand.Accept(this, obj);
            // Checks if it is necessary to make a implicit conversion
            this.codeGenerator.Promotion(this.indent, node.SecondOperand.ExpressionType, node.SecondOperand.ILTypeExpression, node.ExpressionType, node.ExpressionType, true, CheckMakeAnUnbox(node.SecondOperand));

            this.codeGenerator.br(this.indent, label1);
            this.codeGenerator.WriteLabel(this.indent, label0);

            switch (node.Operator) {
                case LogicalOperator.Or:
                    this.codeGenerator.ldci4(this.indent, 1);
                    break;
                case LogicalOperator.And:
                    this.codeGenerator.ldci4(this.indent, 0);
                    break;
                default:
                    break;
            }

            this.codeGenerator.WriteLabel(this.indent, label1);
            // FALTA HACER EL STORE
            return null;
        }

        #endregion

        #region Visit(RelationalExpression node, Object obj)

        public override Object Visit(RelationalExpression node, Object obj) {
            node.FirstOperand.Accept(this, obj);
            this.codeGenerator.Promotion(this.indent, node.FirstOperand.ExpressionType, node.FirstOperand.ILTypeExpression, node.SecondOperand.ExpressionType, node.SecondOperand.ILTypeExpression, false, CheckMakeAnUnbox(node.FirstOperand));

            node.SecondOperand.Accept(this, obj);
            this.codeGenerator.Promotion(this.indent, node.SecondOperand.ExpressionType, node.SecondOperand.ILTypeExpression, node.FirstOperand.ExpressionType, node.FirstOperand.ILTypeExpression, false, CheckMakeAnUnbox(node.SecondOperand));

            switch (node.Operator) {
                case RelationalOperator.NotEqual:
                    this.codeGenerator.ceq(this.indent);
                    this.codeGenerator.ldc(this.indent, false);
                    this.codeGenerator.ceq(this.indent);
                    break;
                case RelationalOperator.Equal:
                    this.codeGenerator.ceq(this.indent);
                    break;
                case RelationalOperator.LessThan:
                    this.codeGenerator.clt(this.indent);
                    break;
                case RelationalOperator.GreaterThan:
                    this.codeGenerator.cgt(this.indent);
                    break;
                case RelationalOperator.LessThanOrEqual:
                    this.codeGenerator.cgt(this.indent);
                    this.codeGenerator.ldc(this.indent, false);
                    this.codeGenerator.ceq(this.indent);
                    break;
                case RelationalOperator.GreaterThanOrEqual:
                    this.codeGenerator.clt(this.indent);
                    this.codeGenerator.ldc(this.indent, false);
                    this.codeGenerator.ceq(this.indent);
                    break;
                default:
                    break;
            }
            return null;
        }

        #endregion

        #region Visit(TernaryExpression node, Object obj)

        //FirstOperand                                         //FirstOperand  
        //bgt.s      IL_0    // Uses the result to jump        //SecondOpernd
        //ThirdOperand                                         //cgt               // Stores the result in the stack
        //br.s       IL_1                                      //stloc.2
        //IL_0:  SecondOperand                                 
        //IL_1:  stloc.2                                       // c = op1 > op2;

        // c = op1.1 > op1.2 ? op2 : op3;

        //FirstOperand
        //cgt
        //brtrue  IL_0
        //ThirdOperand
        //br      IL_1
        //IL_0: SecondOperand
        //IL_1: stloc.2

        // c = op1.1 > op1.2 ? op2 : op3;

        public override Object Visit(TernaryExpression node, Object obj) {
            string label0 = this.codeGenerator.NewLabel;
            string label1 = this.codeGenerator.NewLabel;

            node.FirstOperand.Accept(this, obj);
            this.codeGenerator.Promotion(this.indent, node.FirstOperand.ExpressionType, node.FirstOperand.ILTypeExpression, BoolType.Instance, BoolType.Instance, true, CheckMakeAnUnbox(node.FirstOperand));

            this.codeGenerator.brtrue(this.indent, label0);

            node.ThirdOperand.Accept(this, obj);
            this.codeGenerator.Promotion(this.indent, node.ThirdOperand.ExpressionType, node.ThirdOperand.ILTypeExpression, node.ExpressionType, node.ExpressionType, true, CheckMakeAnUnbox(node.ThirdOperand));
            this.codeGenerator.br(this.indent, label1);

            this.codeGenerator.WriteLabel(this.indent, label0);
            node.SecondOperand.Accept(this, obj);
            this.codeGenerator.Promotion(this.indent, node.SecondOperand.ExpressionType, node.SecondOperand.ILTypeExpression, node.ExpressionType, node.ExpressionType, true, CheckMakeAnUnbox(node.SecondOperand));

            this.codeGenerator.WriteLabel(this.indent, label1);

            return null;
        }

        #endregion

        #region Visit(UnaryExpression node, Object obj)

        public override Object Visit(UnaryExpression node, Object obj) {
            InheritedAttributes ia = (InheritedAttributes)obj;

            switch (node.Operator) {
                #region Increment/Decrement expression had been resolved in parser phase.

                //case UnaryOperator.PrefixIncrement: // ++a -> a = a + 1; a;
                //case UnaryOperator.PrefixDecrement: // --a -> a = a - 1; a;
                //   {
                //      sa = (SynthesizedAttributes)node.Operand.Accept(this, new InheritedAttributes(ia.CurrentMethod, true, ia.Reference, ia.ArrayAccessFound, ia.ActualMethodCalled));

                //      node.Operand.Accept(this, obj);
                //      this.codeGenerator.ldci4(this.indent, 1);
                //      if (node.Operator == UnaryOperator.PrefixIncrement)
                //         this.codeGenerator.add(this.indent);
                //      else
                //         this.codeGenerator.sub(this.indent);

                //      this.codeGenerator.dup(this.indent);

                //      this.WriteStoreInstruction(sa, ia.CurrentMethod);
                //   }
                //   break;

                //case UnaryOperator.PostfixIncrement: // a++ -> a; a = a + 1;
                //case UnaryOperator.PostfixDecrement: // a-- -> a; a = a - 1;
                //   {
                //      // Creates an auxiliar variable to store the old value.
                //      string aux = auxUnaryExpVar + this.CurrentAuxiliarSuffix++;
                //      node.Operand.Accept(this, obj);
                //      this.codeGenerator.WriteAuxiliarLocalVariable(this.indent, aux, node.Operand.ILTypeExpression.ILType());
                //      this.codeGenerator.stloc(this.indent, aux);

                //      // Updates the real variable.
                //      sa = (SynthesizedAttributes)node.Operand.Accept(this, new InheritedAttributes(ia.CurrentMethod, true, ia.Reference, ia.ArrayAccessFound, ia.ActualMethodCalled));

                //      node.Operand.Accept(this, obj);
                //      this.codeGenerator.ldci4(this.indent, 1);
                //      if (node.Operator == UnaryOperator.PostfixIncrement)
                //         this.codeGenerator.add(this.indent);
                //      else
                //         this.codeGenerator.sub(this.indent);

                //      sa.CreateAuxiliarVar = false;
                //      this.WriteStoreInstruction(sa, ia.CurrentMethod);

                //      // Loads auxiliar variable to use it.
                //      this.codeGenerator.ldloc(this.indent, aux);
                //   }
                //   break;

                #endregion

                case UnaryOperator.Not:
                    node.Operand.Accept(this, obj);
                    this.codeGenerator.ldc(this.indent, false);
                    this.codeGenerator.ceq(this.indent);
                    break;
                case UnaryOperator.BitwiseNot:
                    node.Operand.Accept(this, obj);
                    this.codeGenerator.not(this.indent);
                    break;
                case UnaryOperator.Minus:
                    node.Operand.Accept(this, obj);
                    this.codeGenerator.neg(this.indent);
                    break;
                case UnaryOperator.Plus:
                    node.Operand.Accept(this, obj);
                    break;
                default:
                    break;
            }

            return null;
        }

        #endregion

        #region Visit(AssignmentExpression node, Object obj)

        public override Object Visit(AssignmentExpression node, Object obj) {
            InheritedAttributes ia = (InheritedAttributes)obj;
            SynthesizedAttributes sa = (SynthesizedAttributes)node.FirstOperand.Accept(this, new InheritedAttributes(ia.CurrentMethod, true, ia.Reference, ia.ArrayAccessFound, ia.ActualMethodCalled, ia.IsParentNodeAnInvocation));

            node.SecondOperand.Accept(this, obj);
            // Checks if is necessary to make a implicit conversion
            this.codeGenerator.Promotion(this.indent, node.SecondOperand.ExpressionType, node.SecondOperand.ILTypeExpression, node.FirstOperand.ExpressionType, node.FirstOperand.ILTypeExpression, true, CheckMakeAnUnbox(node.SecondOperand));

            #region Assignment expression with other operator, had been resolved in parser phase.
            //switch (node.Operator)
            //{
            //   case AssignmentOperator.PlusAssign:
            //      if (node.ExpressionType is StringType)
            //         this.codeGenerator.concat(this.indent);
            //      else
            //         this.codeGenerator.add(this.indent);
            //      break;
            //   case AssignmentOperator.MinusAssign:
            //      this.codeGenerator.sub(this.indent);
            //      break;
            //   case AssignmentOperator.MultAssign:
            //      this.codeGenerator.mul(this.indent);
            //      break;
            //   case AssignmentOperator.DivAssign:
            //      this.codeGenerator.div(this.indent);
            //      break;
            //   case AssignmentOperator.ModAssign:
            //      this.codeGenerator.rem(this.indent);
            //      break;
            //   case AssignmentOperator.ShiftRightAssign:
            //      this.codeGenerator.shr(this.indent);
            //      break;
            //   case AssignmentOperator.ShiftLeftAssign:
            //      this.codeGenerator.shl(this.indent);
            //      break;
            //   case AssignmentOperator.BitwiseAndAssign:
            //      this.codeGenerator.and(this.indent);
            //      break;
            //   case AssignmentOperator.BitwiseXOrAssign:
            //      this.codeGenerator.xor(this.indent);
            //      break;
            //   case AssignmentOperator.BitwiseOrAssign:
            //      this.codeGenerator.or(this.indent);
            //      break;
            //   default:
            //      break;
            //}

            #endregion

            this.codeGenerator.dup(this.indent);
            this.WriteStoreInstruction(sa, ia.CurrentMethod);

            if (node.MoveStat != null)
                node.MoveStat.Accept(this, obj);

            return null;
        }

        #endregion

        #region WriteStoreInstruction()

        /// <summary>
        /// Writes a store instruction .
        /// </summary>
        /// <param name="sa">Synthesized attributes used to write the store instruction.</param>
        /// <param name="currentMethod">Current method.</param>
        private void WriteStoreInstruction(SynthesizedAttributes sa, MethodDefinition currentMethod) {
            SingleIdentifierExpression idToStore;

            if ((idToStore = sa.Identifier as SingleIdentifierExpression) != null) {
                FieldType fieldType;
                TypeExpression t = null;
                if (idToStore.IdSymbol != null)
                    t = idToStore.IdSymbol.SymbolType;

                if (((fieldType = idToStore.ILTypeExpression as FieldType) != null) || ((fieldType = t as FieldType) != null)) {
                    if ((fieldType.MemberInfo.ModifierMask & Modifier.Static) != 0)
                        this.codeGenerator.stsfld(this.indent, fieldType.FieldTypeExpression, fieldType.MemberInfo.Class.FullName, idToStore.Identifier);
                    else {
                        string id = auxFieldVar + idToStore.Identifier;
                        if (sa.CreateAuxiliarVar) {
                            // Uses a new local variable to store the field value to allow use it later
                            this.codeGenerator.WriteAuxiliarLocalVariable(this.indent, id, fieldType.FieldTypeExpression.ILType());
                            this.codeGenerator.stloc(this.indent, id);
                        }
                        this.codeGenerator.stfld(this.indent, fieldType.FieldTypeExpression, fieldType.MemberInfo.Class.FullName, idToStore.Identifier);

                        if (sa.CreateAuxiliarVar)
                            this.codeGenerator.ldloc(this.indent, id);
                    }
                } else {
                    if (idToStore.ILTypeExpression is PropertyType) {
                        if (sa.IdentifierExpressionMode == IdentifierMode.Instance) {
                            string id = auxFieldVar + idToStore.Identifier;
                            if (sa.CreateAuxiliarVar) {
                                // Uses a new local variable to store the field value to allow use it later
                                this.codeGenerator.WriteAuxiliarLocalVariable(this.indent, id, idToStore.ILTypeExpression.ILType());
                                this.codeGenerator.stloc(this.indent, id);
                            }

                            this.codeGenerator.CallVirt(this.indent, (PropertyType)idToStore.ILTypeExpression, ((PropertyType)idToStore.ILTypeExpression).MemberInfo.Class, idToStore.Identifier, true);

                            if (sa.CreateAuxiliarVar)
                                this.codeGenerator.ldloc(this.indent, id);
                        } else
                            this.codeGenerator.Call(this.indent, (PropertyType)idToStore.ILTypeExpression, ((PropertyType)idToStore.ILTypeExpression).MemberInfo.Class, idToStore.Identifier, true);
                    } else {
                        if ((idToStore.IndexOfSSA <= 0) && (currentMethod.SearchParam(idToStore.Identifier)))
                            this.codeGenerator.starg(this.indent, idToStore.ILName);
                        else {
                            if ((idToStore.IndexOfSSA != -1) || (idToStore.ILTypeExpression is ArrayType))
                                this.codeGenerator.stloc(this.indent, idToStore.ILName);
                        }
                    }
                }
            } else {
                string id = auxFieldVar + this.currentNumber;
                if (sa.CreateAuxiliarVar) {
                    // Uses a new local variable to store the field value to allow use it later
                    this.currentNumber++;
                    this.codeGenerator.WriteAuxiliarLocalVariable(this.indent, id, ((ArrayAccessExpression)sa.Identifier).ExpressionType.ILType());
                    this.codeGenerator.stloc(this.indent, id);
                }

                writeStoreArrayElement(((ArrayAccessExpression)sa.Identifier).FirstOperand.ILTypeExpression, ((ArrayAccessExpression)sa.Identifier).ExpressionType);

                if (sa.CreateAuxiliarVar)
                    this.codeGenerator.ldloc(this.indent, id);
            }
        }

        #endregion

        #region writeStoreArrayElement()

        private void writeStoreArrayElement(TypeExpression reference, TypeExpression type) {
            if (TypeExpression.Is<FieldType>(reference))
                reference = ((FieldType)reference).FieldTypeExpression;

            if (TypeExpression.Is<ArrayType>(reference))
                type.AcceptOperation(new CGStoreArrayElementOperation<T>(this.codeGenerator, this.indent), null);
            else
                // Only BCLClass can be indexer. (Indexer class definition currently does not apply because it is commented in grammar file).
                IndexerCall(reference, "set_Item");
        }

        #endregion

        #region writeLoadArrayElement()

        private void writeLoadArrayElement(TypeExpression reference, TypeExpression type) {
            if (TypeExpression.Is<FieldType>(reference))
                reference = ((FieldType)reference).FieldTypeExpression;

            if (TypeExpression.Is<ArrayType>(reference))
                type.AcceptOperation(new CGLoadArrayElementOperation<T>(this.codeGenerator, this.indent), null);
            else
                // Only BCLClass can be indexer. (Indexer class definition currently does not apply because it is commented in grammar file).
                IndexerCall(reference, "get_Item");
        }

        #endregion

        #region IndexerCall

        private void IndexerCall(TypeExpression reference, string memberId) {
            BCLClassType bclClass = TypeExpression.As<BCLClassType>(reference);
            if (bclClass != null)
                if (bclClass.Methods.ContainsKey(memberId)) {
                    MethodType method = bclClass.Methods[memberId].Type as MethodType;
                    string[] args = new string[method.ParameterListCount];
                    for (int i = 0; i < method.ParameterListCount; i++)
                        args[i] = method.GetParameter(i).ILType();

                    this.codeGenerator.CallVirt(this.indent, "instance ", method.Return.ILType(), reference.ILType(), memberId, args);
                }
        }

        #endregion

        #region Visit(CastExpression node, Object obj)
        //OPERQCION////
        public override Object Visit(CastExpression node, Object obj) {
            node.Expression.Accept(this, obj);
            ////we are sending a message to this expression
            ////TODO: Pensar si merece la pena refactorizar este trozo de código como carga de variable auxiliar

            InheritedAttributes ia = (InheritedAttributes)obj;

            if (ia.MessagePassed && node.ExpressionType.IsValueType())
                return this.LoadAuxiliarVariable(node.ExpressionType.ILType());

            if (node.CastType.IsValueType() && !node.Expression.ILTypeExpression.IsValueType())
                this.codeGenerator.UnboxAny(indent, node.CastType);
            else
                node.CastType.AcceptOperation(new CGCastOperation<T>(this.codeGenerator, this.indent), null);

            return null;
        }

        #endregion

        #region Visit(IsExpression node, Object obj)

        public override Object Visit(IsExpression node, Object obj) {
            node.Expression.Accept(this, obj);
            this.codeGenerator.isinst(this.indent, node.TypeExpr);
            return null;
        }

        #endregion

        #region Visit(NewExpression node, Object obj)

        public override Object Visit(NewExpression node, Object obj) {
            Object objArgs = obj;
            MethodType actualMethodCalled = node.ActualMethodCalled as MethodType;
            InheritedAttributes ia = (InheritedAttributes)obj;

            if (actualMethodCalled != null)
                objArgs = new InheritedAttributes(ia.CurrentMethod, ia.Assignment, ia.Reference, ia.ArrayAccessFound, actualMethodCalled, ia.IsParentNodeAnInvocation);

            node.Arguments.Accept(this, objArgs);
            this.codeGenerator.newobj(this.indent, actualMethodCalled, node.NewType, ".ctor");
            return null;
        }

        #endregion
        public virtual void ThrowMissingMethodException(string method) {
            this.codeGenerator.WriteThrowMissingMethodException(this.indent, method);
        }

        #region Visit(NewArrayExpression node, Object obj)

        public override Object Visit(NewArrayExpression node, Object obj) {
            InheritedAttributes ia = (InheritedAttributes)obj;
            InheritedAttributes iA = new InheritedAttributes(ia.CurrentMethod, false, ia.Reference, false, ia.ActualMethodCalled, ia.IsParentNodeAnInvocation);

            node.Size.Accept(this, iA);
            this.codeGenerator.newarr(this.indent, ((ArrayType)node.ExpressionType).ArrayTypeExpression);

            // Stores in auxiliar variable
            this.codeGenerator.stloc(this.indent, node.Identifier);

            if (node.Init != null)
                for (int i = 0; i < node.Init.ExpressionCount; i++) {
                    // Loads the auxiliar variable
                    this.codeGenerator.ldloc(this.indent, node.Identifier);
                    this.codeGenerator.ldci4(this.indent, i);
                    node.Init.GetExpressionElement(i).Accept(this, obj);
                    this.codeGenerator.Promotion(this.indent, node.Init.GetExpressionElement(i).ExpressionType, node.Init.GetExpressionElement(i).ILTypeExpression, ((ArrayType)node.ExpressionType).ArrayTypeExpression, ((ArrayType)node.ExpressionType).ArrayTypeExpression, true, CheckMakeAnUnbox(node.Init.GetExpressionElement(i)));
                    this.writeStoreArrayElement(node.ExpressionType, ((ArrayType)node.ExpressionType).ArrayTypeExpression);
                }

            // Stores the auxiliar variable in real variable...
            this.codeGenerator.ldloc(this.indent, node.Identifier);

            return null;
        }

        #endregion

        // Synthesized attributes information
        // - stores the current expression to use in store instruction in assignment node.
        // - stores the identifier mode (Instance, UserType, Namespace)
        // - stores information about the creation of auxiliar variables.
        //   True if it is necessary to create an auxiliar variable. Otherwise, false.

        // Returns Synthesized attributes information
        #region Visit(ArrayAccessExpression node, Object obj)

        public override Object Visit(ArrayAccessExpression node, Object obj) {
            InheritedAttributes ia = (InheritedAttributes)obj;
            Object o = new InheritedAttributes(ia.CurrentMethod, false, ia.Reference, true, ia.ActualMethodCalled, ia.IsParentNodeAnInvocation);

            node.FirstOperand.Accept(this, o);
            node.SecondOperand.Accept(this, o);

            // Checks if it is necessary to promotion the index type expression
            BCLClassType bclClass;
            TypeExpression reference = node.FirstOperand.ILTypeExpression;
            if (TypeExpression.Is<FieldType>(reference))
                reference = ((FieldType)reference).FieldTypeExpression;

            if ((bclClass = TypeExpression.As<BCLClassType>(reference)) != null) {
                if (bclClass.Methods.ContainsKey("get_Item")) {
                    MethodType method = bclClass.Methods["get_Item"].Type as MethodType;
                    if (method.ParameterListCount == 1)
                        this.codeGenerator.Promotion(this.indent, node.SecondOperand.ExpressionType, node.SecondOperand.ILTypeExpression, method.GetParameter(0), method.GetParameter(0), true, CheckMakeAnUnbox(node.SecondOperand));
                }
            }

            if (!((InheritedAttributes)obj).Assignment)
                this.writeLoadArrayElement(node.FirstOperand.ILTypeExpression, node.ExpressionType);

            return new SynthesizedAttributes(node);
        }

        #endregion

        // Returns Synthesized attributes information
        #region Visit(FieldAccessExpression node, Object obj)

        public override Object Visit(FieldAccessExpression node, Object obj) {
            InheritedAttributes ia = (InheritedAttributes)obj;
            InheritedAttributes objLeft = new InheritedAttributes(ia.CurrentMethod, false, ia.Reference, ia.ArrayAccessFound, ia.ActualMethodCalled, false, node, true);
            InheritedAttributes objRight = new InheritedAttributes(ia.CurrentMethod, ia.Assignment, node.Expression, ia.ArrayAccessFound, ia.ActualMethodCalled, false, node, true);
            Object o = node.Expression.Accept(this, objLeft);

            if (!ia.Assignment) {
                // Grammar file has not property definition. We only can call properties had been defined in mscorlib.
                if (node.ILTypeExpression is PropertyType) {
                    PropertyType propertyType = TypeExpression.As<PropertyType>(node.ILTypeExpression);
                    UnionType unionType = TypeExpression.As<UnionType>(node.Expression.ExpressionType);
                    if (unionType != null) {
                        string endLabel = this.codeGenerator.NewLabel;
                        // * 1.2.1.1 The implicit object has unknown type (union types)
                        bool clean = false;
                        for (int i = 0; i < unionType.Count; i++) {
                            TypeExpression actualClass = TypeExpression.As<TypeExpression>(unionType.TypeSet[i]);
                            if (actualClass != null) {
                                string nextPropertyLabel = this.codeGenerator.NewLabel;
                                if ((propertyType.MemberInfo.ModifierMask & Modifier.Static) == 0) {
                                    // check the invocation reference
                                    clean = true;
                                    this.codeGenerator.dup(this.indent);
                                    this.codeGenerator.isinst(this.indent, actualClass);
                                    this.codeGenerator.brfalse(this.indent, nextPropertyLabel);

                                    if (actualClass.IsValueType())
                                        this.codeGenerator.Unbox(this.indent, actualClass);
                                }
                                this.CallProperty(propertyType, actualClass, ia, o);
                                this.codeGenerator.br(this.indent, endLabel);
                                if (clean)
                                    this.codeGenerator.WriteLabel(this.indent, nextPropertyLabel);
                            } // end of actualPropertyCall != null
                        } // end for
                        // There are some mistake. Throws MissingMethodException. 
                        this.ThrowMissingMethodException(propertyType.ILType());
                        // End of invocation.
                        this.codeGenerator.WriteLabel(this.indent, endLabel);

                    } else //is not an UnionType
                        this.CallProperty(node, ia, o);

                    // is not a property type
                } else {
                    if ((node.Expression.ILTypeExpression is TypeVariable) && !ia.IsParentNodeAnInvocation)
                        this.InstrospectiveFieldInvocation(node.Expression, node.FieldName.Identifier, objLeft);
                    else {
                        node.FieldName.Accept(this, objRight);
                        // * If fieldname is a method and the expression is a built in type, we must do boxing
                        MethodType method = TypeExpression.As<MethodType>(node.FieldName.ExpressionType);
                        if (node.Expression.ExpressionType.IsValueType() && method != null)
                            this.codeGenerator.BoxIfNeeded(this.indent, node.Expression.ExpressionType);
                    }
                }

                return o;
            } else {
                Object oAux = node.FieldName.Accept(this, objRight);
                if (node.ILTypeExpression is PropertyType)
                    if (oAux is SynthesizedAttributes && o is SynthesizedAttributes) {
                        SynthesizedAttributes sa = new SynthesizedAttributes(((SynthesizedAttributes)oAux).Identifier);
                        sa.IdentifierExpressionMode = ((SynthesizedAttributes)o).IdentifierExpressionMode;
                        oAux = sa;
                    }

                return oAux;
            }

        }
        /// <summary>
        /// write the the neccesary calls to a property
        /// </summary>
        /// <param name="node">The node with fieldAccess expression</param>
        /// <param name="ia">InheritanceAttributes</param>
        /// <param name="o">Object that can be SynthesizedAttributes instance</param>
        private void CallProperty(FieldAccessExpression node, InheritedAttributes ia, Object o) {
            if (o is SynthesizedAttributes && ((SynthesizedAttributes)o).IdentifierExpressionMode == IdentifierMode.Instance)
                this.codeGenerator.CallVirt(this.indent, (PropertyType)node.ILTypeExpression, node.Expression.ILTypeExpression, node.FieldName.Identifier, ia.Assignment);
            else
                this.codeGenerator.Call(this.indent, (PropertyType)node.ILTypeExpression, node.Expression.ILTypeExpression, node.FieldName.Identifier, ia.Assignment);

            if (ia.MessagePassed && node.ExpressionType.IsValueType())
                this.LoadAuxiliarVariable(node.ExpressionType.ILType());
        }
        private void CallProperty(PropertyType propertyType, TypeExpression klass, InheritedAttributes ia, Object o) {
            if (o is SynthesizedAttributes && ((SynthesizedAttributes)o).IdentifierExpressionMode == IdentifierMode.Instance)
                this.codeGenerator.CallVirt(this.indent, propertyType, klass, propertyType.MemberInfo.MemberIdentifier, ia.Assignment);
            else
                this.codeGenerator.Call(this.indent, propertyType, klass, propertyType.MemberInfo.MemberIdentifier, ia.Assignment);

            if (ia.MessagePassed && propertyType.PropertyTypeExpression.IsValueType())
                this.LoadAuxiliarVariable(propertyType.PropertyTypeExpression.ILType());
        }

        #endregion

        #region InstrospectiveFieldInvocation()

        /// <summary>
        /// Invokes a field without knowing its type, using introspection.
        /// Code Generation Template:
        ///  * ld       <implicit object>
        ///  * CallVirt   instance class [mscorlib]System.WriteType [mscorlib]System.Object::GetType()
        ///  * ldstr    <member name>
        ///  * ldc.i4   <binding flags options> // 0x434 -> 1076
        ///  * ldnull
        ///  * ld       <implicit object>
        ///  * ldnull
        ///  * CallVirt   instance object [mscorlib]System.WriteType::InvokeMember(string, valuetype [mscorlib]System.Reflection.BindingFlags, class [mscorlib]System.Reflection.Binder, object, object[])
        /// </summary>
        /// <param name="node">The AST expression node</param>
        /// <param name="memberName">The name of the member</param>
        /// <param name="obj">The visitor paramenter</param>
        /// <param name="inheritedAttributes">Inherited attributes</param>
        private void InstrospectiveFieldInvocation(Expression node, string memberName, Object obj) {
            this.codeGenerator.CallVirt(this.indent, "instance class", "[mscorlib]System.Type", "[mscorlib]System.Object", "GetType", null);
            this.codeGenerator.ldstr(this.indent, memberName);
            this.codeGenerator.ldci4(this.indent, 1076);
            this.codeGenerator.ldnull(this.indent);
            node.Accept(this, obj); // ld <implicit object>
            this.codeGenerator.ldnull(this.indent);
            this.codeGenerator.CallVirt(this.indent, "instance", "object", "[mscorlib]System.Type", "InvokeMember", new string[] { "string", "valuetype [mscorlib]System.Reflection.BindingFlags", "class [mscorlib]System.Reflection.Binder", "object", "object[]" });
        }
        #endregion

        #region Visit(CompoundExpression node, Object obj)

        public override Object Visit(CompoundExpression node, Object obj) {
            MethodType mt = ((InheritedAttributes)obj).ActualMethodCalled;

            for (int i = 0; i < node.ExpressionCount; i++) {
                node.GetExpressionElement(i).Accept(this, obj);
                if (mt != null)
                    this.codeGenerator.Promotion(this.indent, node.GetExpressionElement(i).ExpressionType, node.GetExpressionElement(i).ILTypeExpression, mt.GetParameter(i), mt.GetParameter(i), true, CheckMakeAnUnbox(node.GetExpressionElement(i)));
            }
            return null;
        }

        #endregion

        // Literals

        #region Visit(BaseExpression node, Object obj)

        public override Object Visit(BaseExpression node, Object obj) {
            this.codeGenerator.ldarg(this.indent, 0);
            return null;
        }

        #endregion

        #region Visit(BoolLiteralExpression node, Object obj)

        public override Object Visit(BoolLiteralExpression node, Object obj) {
            this.codeGenerator.ldc(this.indent, node.BoolValue);
            return null;
        }

        #endregion

        #region Visit(CharLiteralExpression node, Object obj)

        public override Object Visit(CharLiteralExpression node, Object obj) {
            this.codeGenerator.ldci4(this.indent, Convert.ToUInt16(node.CharValue));
            return null;
        }

        #endregion

        #region Visit(DoubleLiteralExpression node, Object obj)

        public override Object Visit(DoubleLiteralExpression node, Object obj) {
            this.codeGenerator.ldcr8(this.indent, node.ILValue);
            return null;
        }

        #endregion

        #region Visit(IntLiteralExpression node, Object obj)

        public override Object Visit(IntLiteralExpression node, Object obj) {
            this.codeGenerator.ldci4(this.indent, node.IntValue);
            return null;
        }

        #endregion

        #region Visit(StringLiteralExpression node, Object obj)

        public override Object Visit(StringLiteralExpression node, Object obj) {
            this.codeGenerator.ldstr(this.indent, node.StringValue);
            return null;
        }

        #endregion

        #region Visit(NullExpression node, Object obj)

        public override Object Visit(NullExpression node, Object obj) {
            this.codeGenerator.ldnull(this.indent);
            return null;
        }

        #endregion

        // Returns Synthesizedg attributes information

        #region Visit(SingleIdentifierExpression node, Object obj)
        public override Object Visit(SingleIdentifierExpression node, Object obj) {
            InheritedAttributes helper = (InheritedAttributes)obj;
            int scope;
            Expression exp = this.constantsTable.Search(node.Identifier, out scope);

            if (exp == null || node.IdSymbol != null && scope + this.namespaceDepth < node.IdSymbol.Scope)
                exp = null;

            if (exp == null) {
                FieldType field;
                TypeExpression t = null;
                if (node.IdSymbol != null)
                    t = node.IdSymbol.SymbolType;

                field = node.ExpressionType as FieldType;
                if (field == null)
                    field = t as FieldType;

                if (node.IdMode == IdentifierMode.Instance && helper.Reference == null && field != null && (field.MemberInfo.ModifierMask & Modifier.Static) == 0)
                    this.codeGenerator.ldarg(this.indent, 0);

                if (!helper.Assignment) {
                    if (field != null)
                        WriteLoadField(field, node.Identifier, helper);
                    else
                        if (node.IndexOfSSA <= 0 && helper.CurrentMethod.SearchParam(node.Identifier))
                            this.WriteLoadArg(node.ILName, helper);
                        else
                            if (node.IndexOfSSA != -1 || node.ILTypeExpression is ArrayType)
                                this.WriteLoadLocalVar(node.ILName, helper, node.ExpressionType);

                } else {
                    if (helper.ArrayAccessFound) {
                        if (node.ILTypeExpression is ArrayType)
                            this.codeGenerator.ldloc(this.indent, node.ILName);
                        if (node.ILTypeExpression is FieldType && ((FieldType)node.ILTypeExpression).FieldTypeExpression is ArrayType)
                            WriteLoadField((FieldType)node.ILTypeExpression, node.Identifier, helper);
                    }
                }
            } else
                exp.Accept(this, obj);

            return new SynthesizedAttributes(node);
        }

        #endregion

        #region WriteLoadField()

        private void WriteLoadField(FieldType type, string id, InheritedAttributes helper) {
            BCLClassType baseClass = null;
            if (helper.ActualMethodCalled != null)
                baseClass = helper.ActualMethodCalled.MemberInfo.Class as BCLClassType;

            if ((type.MemberInfo.ModifierMask & Modifier.Static) != 0)
                if (baseClass != null && baseClass.IsValueType())
                    this.codeGenerator.ldsflda(this.indent, type, type.MemberInfo.Class.ILType(), id);
                else
                    this.codeGenerator.ldsfld(this.indent, type, type.MemberInfo.Class.ILType(), id);
            else
                if ((baseClass != null) && (baseClass.IsValueType()))
                    this.codeGenerator.ldflda(this.indent, type, type.MemberInfo.Class.ILType(), id);
                else
                    this.codeGenerator.ldfld(this.indent, type, type.MemberInfo.Class.ILType(), id);
        }

        #endregion

        #region WriteLoadLocalVar()

        private void WriteLoadLocalVar(string id, InheritedAttributes helper, TypeExpression type) {
            // * Base class could be the BCLClass type of the implicit object...
            BCLClassType baseClass = null;
            if (helper.ActualMethodCalled != null)
                // ... in a method call
                baseClass = helper.ActualMethodCalled.MemberInfo.Class as BCLClassType;
            else {
                // ... or property access
                FieldAccessExpression fieldAccess = helper.ParentNode as FieldAccessExpression;
                if (fieldAccess != null) {
                    PropertyType fieldType = fieldAccess.ExpressionType as PropertyType;
                    if (fieldType != null)
                        baseClass = fieldType.MemberInfo.Class as BCLClassType;
                }
            }

            if (baseClass != null && baseClass.IsValueType()) {
                if (!type.ILType().Equals("object"))
                    // * A value type, as an implicit object, generates a ldloca (address)
                    if (helper.ParentNode is FieldAccessExpression)
                        this.codeGenerator.ldloca(this.indent, id);
                    else
                        // * Otherwise, it generates a lodloc (not the address)
                        this.codeGenerator.ldloc(this.indent, id);
                else {
                    this.codeGenerator.ldloc(this.indent, id);
                    this.codeGenerator.Unbox(this.indent, baseClass);
                }
            } else
                this.codeGenerator.ldloc(this.indent, id);
        }

        #endregion

        #region WriteLoadArg()

        private void WriteLoadArg(string id, InheritedAttributes helper) {
            BCLClassType baseClass = null;
            if (helper.ActualMethodCalled != null)
                baseClass = helper.ActualMethodCalled.MemberInfo.Class as BCLClassType;
            // * A value type, as an implicit object, generates a ldarga (address)
            if (baseClass != null && baseClass.IsValueType() && helper.ParentNode is FieldAccessExpression)
                this.codeGenerator.ldarga(this.indent, id);
            else
                this.codeGenerator.ldarg(this.indent, id);
        }

        #endregion

        #region Visit(ThisExpression node, Object obj)

        public override Object Visit(ThisExpression node, Object obj) {
            this.codeGenerator.ldarg(this.indent, 0);
            return null;
        }

        #endregion

        // Statements

        #region Visit(ReturnStatement node, Object obj)

        public override Object Visit(ReturnStatement node, Object obj) {
            node.Assigns.Accept(this, obj);
            if (node.ReturnExpression != null) {
                node.ReturnExpression.Accept(this, obj);
                this.codeGenerator.Promotion(this.indent, node.ReturnExpression.ExpressionType, node.ReturnExpression.ILTypeExpression, node.CurrentMethodType.Return, node.CurrentMethodType.Return, true, CheckMakeAnUnbox(node.ReturnExpression));
            }
            this.codeGenerator.ret(this.indent);
            return null;
        }

        #endregion

        #region Visit(Block node, Object obj)

        public override Object Visit(Block node, Object obj) {
            for (int i = 0; i < node.StatementCount; i++) {
                node.GetStatementElement(i).Accept(this, obj);
                RemovesTopElement(node.GetStatementElement(i));
            }
            return null;
        }

        #endregion

        #region RemovesTopElement()
        /// <summary>
        /// Generates a pop, taking into account if the expression in the top is not used
        /// </summary>
        /// <param name="stat"></param>
        private void RemovesTopElement(Statement stat) {
            // * If it is not an expression, not pop is needed 
            if (!(stat is Expression))
                return;

            InvocationExpression invocation = stat as InvocationExpression;
            bool pop = true; // by default we remove the top

            // * If the method invoked is a procedure, nothing to pop
            if (invocation != null)
                pop = (bool)invocation.ILTypeExpression.AcceptOperation(new CGRemoveTopElementInvocationOperation(), null);

            if (pop)
                this.codeGenerator.pop(this.indent);
        }

        #endregion

        #region Visit(MoveStatement node, Object obj)

        public override Object Visit(MoveStatement node, Object obj) {
            InheritedAttributes ia = (InheritedAttributes)obj;
            SynthesizedAttributes sa = (SynthesizedAttributes)node.LeftExp.Accept(this, new InheritedAttributes(ia.CurrentMethod, true, ia.Reference, ia.ArrayAccessFound, ia.ActualMethodCalled, ia.IsParentNodeAnInvocation));
            node.RightExp.Accept(this, obj);
            sa.CreateAuxiliarVar = false;
            this.codeGenerator.Promotion(this.indent, node.RightExp.ExpressionType, node.RightExp.ILTypeExpression, node.LeftExp.ExpressionType, node.LeftExp.ILTypeExpression, true, CheckMakeAnUnbox(node.RightExp));
            WriteStoreInstruction(sa, ia.CurrentMethod);
            if (node.MoveStat != null)
                node.MoveStat.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(IfElseStatement node, Object obj)

        public override Object Visit(IfElseStatement node, Object obj) {
            string labelElse = this.codeGenerator.NewLabel;
            string labelEnd = this.codeGenerator.NewLabel;

            node.Condition.Accept(this, obj);
            for (int i = 0; i < node.AfterCondition.Count; i++)
                node.AfterCondition[i].Accept(this, obj);

            this.codeGenerator.brfalse(this.indent, labelElse);
            this.constantsTable.Set();
            node.TrueBranch.Accept(this, obj);
            this.RemovesTopElement(node.TrueBranch);
            this.codeGenerator.br(this.indent, labelEnd);
            this.constantsTable.Reset();

            this.constantsTable.Set();
            this.codeGenerator.WriteLabel(this.indent, labelElse);
            node.FalseBranch.Accept(this, obj);
            this.RemovesTopElement(node.FalseBranch);
            this.constantsTable.Reset();

            this.codeGenerator.WriteLabel(this.indent, labelEnd);

            //for (int i = 0; i < node.ThetaStatements.Count; i++)
            //   node.ThetaStatements[i].Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(DoStatement node, Object obj)

        public override Object Visit(DoStatement node, Object obj) {
            string label = this.codeGenerator.NewLabel;

            for (int i = 0; i < node.InitDo.Count; i++)
                node.InitDo[i].Accept(this, obj);

            //for (int i = 0; i < node.BeforeBody.Count; i++)
            //   node.BeforeBody[i].Accept(this, obj);

            this.constantsTable.Set();
            this.codeGenerator.WriteLabel(this.indent, label);
            node.Statements.Accept(this, obj);
            this.RemovesTopElement(node.Statements);
            this.constantsTable.Reset();

            node.Condition.Accept(this, obj);
            this.codeGenerator.brtrue(this.indent, label);
            return null;
        }

        #endregion

        #region Visit(ForStatement node, Object obj)

        public override Object Visit(ForStatement node, Object obj) {
            string labelCondition = this.codeGenerator.NewLabel;
            string labelBody = this.codeGenerator.NewLabel;

            this.constantsTable.Set();
            for (int i = 0; i < node.InitializerCount; i++) {
                node.GetInitializerElement(i).Accept(this, obj);
                this.RemovesTopElement(node.GetInitializerElement(i));
            }
            for (int i = 0; i < node.AfterInit.Count; i++) {
                node.AfterInit[i].Accept(this, obj);
            }
            this.codeGenerator.br(this.indent, labelCondition);

            this.codeGenerator.WriteLabel(this.indent, labelBody);
            node.Statements.Accept(this, obj);
            this.RemovesTopElement(node.Statements);

            for (int i = 0; i < node.IteratorCount; i++) {
                node.GetIteratorElement(i).Accept(this, obj);
                this.RemovesTopElement(node.GetIteratorElement(i));
            }

            this.codeGenerator.WriteLabel(this.indent, labelCondition);
            //for (int i = 0; i < node.BeforeCondition.Count; i++)
            //{
            //   node.BeforeCondition[i].Accept(this, obj);
            //}
            node.Condition.Accept(this, obj);
            for (int i = 0; i < node.AfterCondition.Count; i++) {
                node.AfterCondition[i].Accept(this, obj);
            }
            this.codeGenerator.brtrue(this.indent, labelBody);

            this.constantsTable.Reset();
            return null;
        }

        #endregion

        #region Visit(WhileStatement node, Object obj)

        public override Object Visit(WhileStatement node, Object obj) {
            string labelCondition = this.codeGenerator.NewLabel;
            string labelBody = this.codeGenerator.NewLabel;

            for (int i = 0; i < node.InitWhile.Count; i++)
                node.InitWhile[i].Accept(this, obj);

            this.codeGenerator.br(this.indent, labelCondition);

            this.constantsTable.Set();
            this.codeGenerator.WriteLabel(this.indent, labelBody);
            node.Statements.Accept(this, obj);
            this.RemovesTopElement(node.Statements);
            this.constantsTable.Reset();

            //for (int i = 0; i < node.BeforeCondition.Count; i++)
            //   node.BeforeCondition[i].Accept(this, obj);

            this.codeGenerator.WriteLabel(this.indent, labelCondition);
            node.Condition.Accept(this, obj);
            for (int i = 0; i < node.AfterCondition.Count; i++)
                node.AfterCondition[i].Accept(this, obj);
            this.codeGenerator.brtrue(this.indent, labelBody);
            return null;
        }

        #endregion

        #region Visit(SwitchStatement node, Object obj)

        public override Object Visit(SwitchStatement node, Object obj) {
            List<string> labels = new List<string>();

            for (int i = 0; i < node.LabelCount(); i++)
                labels.Add(this.codeGenerator.NewLabel);

            string endLabel = this.codeGenerator.NewLabel;
            int index = 0;
            string defaultLabel = "";
            string idSwitchCond = auxSwitchCond + this.currentNumber++;

            this.codeGenerator.WriteAuxiliarLocalVariable(this.indent, idSwitchCond, node.Condition.ILTypeExpression.ILType());
            node.Condition.Accept(this, obj);
            this.codeGenerator.stloc(this.indent, idSwitchCond);

            for (int i = 0; i < node.AfterCondition.Count; i++)
                node.AfterCondition[i].Accept(this, obj);

            for (int i = 0; i < node.SwitchBlockCount; i++) {
                this.constantsTable.Set();

                // First, visits the label section (conditional case section or default section)
                for (int j = 0; j < node.GetSwitchSectionElement(i).LabelSection.Count; j++) {
                    SwitchLabel n = node.GetSwitchSectionElement(i).LabelSection[j];
                    if (n.SwitchSectionType == SectionType.Case) {
                        this.codeGenerator.ldloc(this.indent, idSwitchCond);
                        n.Condition.Accept(this, obj);
                        this.codeGenerator.beq(this.indent, labels[index]);
                    } else
                        defaultLabel = labels[index];
                    index++;
                }
            }


            this.codeGenerator.br(this.indent, defaultLabel != String.Empty ? defaultLabel : endLabel);
            index = 0;

            for (int i = 0; i < node.SwitchBlockCount; i++) {
                // Second, visits the statement section for each switch section.
                for (int j = 0; j < node.GetSwitchSectionElement(i).LabelSection.Count; j++)
                    this.codeGenerator.WriteLabel(this.indent, labels[index++]);

                node.GetSwitchSectionElement(i).SwitchBlock.Accept(this, obj);
                this.RemovesTopElement(node.GetSwitchSectionElement(i).SwitchBlock);
                this.codeGenerator.br(this.indent, endLabel);
                this.constantsTable.Reset();
            }

            //for (int i = 0; i < node.ThetaStatements.Count; i++)
            //   node.ThetaStatements[i].Accept(this, obj);

            this.codeGenerator.WriteLabel(this.indent, endLabel);
            return null;
        }

        #endregion

        #region Visit(SwitchLabel node, Object obj)

        public override Object Visit(SwitchLabel node, Object obj) {
            if (node.SwitchSectionType == SectionType.Case)
                node.Condition.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(BreakStatement node, Object obj) // Nothing to do ¿?

        public override Object Visit(BreakStatement node, Object obj) {
            return null;
        }


        #region Visit(ExceptionManagementStatement node, Object obj)

        public override Object Visit(ExceptionManagementStatement node, Object obj) {
            this.codeGenerator.WriteTryDirective(this.indent);   //.try
            this.codeGenerator.WriteOpenBraceTry(this.indent++);   // {

            if (node.CatchCount != 0 && node.FinallyBlock != null) {
                this.codeGenerator.WriteTryDirective(this.indent);   //.try
                this.codeGenerator.WriteOpenBraceTry(this.indent++);   // {
            }
            string firstLeaveJump = this.codeGenerator.NewLabel;
            string secondLeaveJump = this.codeGenerator.NewLabel;

            node.TryBlock.Accept(this, obj);
            this.codeGenerator.WriteLeave(this.indent, firstLeaveJump);
            this.codeGenerator.WriteCloseBraceTry(--this.indent);   // .end  .try{

            if (node.CatchCount != 0) {

                for (int i = 0; i < node.CatchCount; i++) {
                    node.GetCatchElement(i).Accept(this, obj);
                    this.codeGenerator.WriteLeave(this.indent--, firstLeaveJump);
                    this.codeGenerator.WriteCloseBraceCatch(this.indent);
                }

                this.codeGenerator.WriteLabel(this.indent, firstLeaveJump);
                this.codeGenerator.nop(this.indent);

                if (node.FinallyBlock != null) {
                    this.codeGenerator.WriteLeave(this.indent, secondLeaveJump);
                    this.codeGenerator.WriteCloseBraceTry(--this.indent);   // .end  .finally{
                }
            }

            if (node.FinallyBlock != null) {
                this.codeGenerator.WriteFinally(this.indent);
                this.codeGenerator.WriteOpenBraceFinally(this.indent++);
                node.FinallyBlock.Accept(this, obj);
                this.codeGenerator.WriteLine(this.indent--, "endfinally");
                this.codeGenerator.WriteCloseBraceFinally(this.indent); // . end try
                this.codeGenerator.WriteLabel(this.indent, node.CatchCount == 0 ? firstLeaveJump : secondLeaveJump);
                this.codeGenerator.nop(this.indent);
            }

            return null;
        }

        #endregion

        #region CatchStatement // very important the brace is not closed.
        public override Object Visit(CatchStatement node, Object obj) {
            this.codeGenerator.WriteCatch(this.indent, node.Exception.TypeExpr.ILType(), node.Exception.TypeExpr.ILType());
            this.codeGenerator.WriteOpenBraceCatch(this.indent++);
            this.codeGenerator.stloc(this.indent, node.Exception.ILName);//node.Exception.ILName);
            node.Statements.Accept(this, obj);

            return null;
        }
        #endregion

        #endregion
        public override Object Visit(ThrowStatement node, Object obj) {
            if (node.ThrowExpression == null)
                this.codeGenerator.WriteRethrow(this.indent);
            else {
                node.ThrowExpression.Accept(this, obj);
                codeGenerator.WriteThrow(this.indent);
            }
            return null;
        }
        // helpers

        #region LoadAuxiliarVariable

        protected Object LoadAuxiliarVariable(string type) {
            TemporalVariablesTable tmpVars = TemporalVariablesTable.Instance;
            string id = tmpVars.SearchId(type);

            this.codeGenerator.stloc_s(this.indent, id);
            this.codeGenerator.ldloca_s(this.indent, id);
            return null;
        }

        #endregion


        #region RuntimeCheckArguments()
        /// <summary>
        ///  It typechecks the runtime arguments, embeded in a method call, with the parametes of this method.
        ///  </summary>
        /// <param name="node">the ast node we want to generate code to</param>
        /// <param name="objArgs"></param>
        /// <param name="actualMethodCalled">the current method</param>
        /// <param name="nextMethod">a list of labels to jump </param>
        internal abstract void RuntimeCheckArguments(InvocationExpression node, Object objArgs, MethodType actualMethodCalled, List<string> nextMethod);

        #endregion
        // Commented

        #region Commented Code

        //   #region Visit(ContinueStatement node, Object obj)

        //   public override Object Visit(ContinueStatement node, Object obj)
        //   {
        //      return null;
        //   }

        //   #endregion

        //   #region Visit(ForeachStatement node, Object obj)

        //   public override Object Visit(ForeachStatement node, Object obj)
        //   {
        //      node.ForEachDeclaration.Accept(this, obj);
        //      node.ForeachExp.Accept(this, obj);
        //      node.ForeachBlock.Accept(this, obj);
        //      return null;
        //   }

        //   #endregion

        //   #region Visit(ThrowStatement node, Object obj)

        //   public override Object Visit(ThrowStatement node, Object obj)
        //   {
        //      return node.ThrowExpression.Accept(this, obj);
        //   }

        //   #endregion

        //   #region Visit(ExceptionManagementStatement node, Object obj)

        //   public override Object Visit(ExceptionManagementStatement node, Object obj)
        //   {
        //      node.TryBlock.Accept(this, obj);
        //      for (int i = 0; i < node.CatchCount; i++)
        //      {
        //         node.GetCatchElement(i).Accept(this, obj);
        //      }
        //      node.FinallyBlock.Accept(this, obj);
        //      return null;
        //   }

        //   #endregion

        //   #region Visit(CatchStatement node, Object obj)

        //   public override Object Visit(CatchStatement node, Object obj)
        //   {
        //      node.Exception.Accept(this, obj);
        //      node.Statements.Accept(this, obj);
        //      return null;
        //   }

        //   #endregion

        #endregion

        #region Nothing to do...

        //#region Visit(ThetaStatement node, Object obj)

        //public override Object Visit(ThetaStatement node, Object obj)
        //{
        //   node.ThetaId.Accept(this, obj);
        //   for (int i = 0; i < node.ThetaList.Count; i++)
        //   {
        //      node.ThetaList[i].Accept(this, obj);
        //   }
        //   return null;
        //}

        //#endregion

        //#region Visit(SwitchSection node, Object obj)

        //public override Object Visit(SwitchSection node, Object obj)
        //{
        //   for (int i = 0; i < node.LabelSection.Count; i++)
        //   {
        //      node.LabelSection[i].Accept(this, obj);
        //   }
        //   for (int i = 0; i < node.SwitchBlock.StatementCount; i++)
        //   {
        //      node.SwitchBlock.GetStatementElement(i).Accept(this, obj);
        //   }
        //   return null;
        //}

        //#endregion

        //   #region Visit(PropertyDefinition node, Object obj)

        //   public override Object Visit(PropertyDefinition node, Object obj)
        //   {
        //      if (node.GetBlock != null)
        //         node.GetBlock.Accept(this, obj);
        //      if (node.SetBlock != null)
        //         node.SetBlock.Accept(this, obj);
        //      return null;
        //   }

        //   #endregion

        #endregion
    }
}
