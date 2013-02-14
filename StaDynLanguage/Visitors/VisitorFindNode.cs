using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;
using AST;
using TypeSystem;
using Symbols;
using System.Diagnostics;
using ErrorManagement;

namespace StaDynLanguage.Visitors
{
    public class VisitorFindNode : Visitor
    {
        AstNode previousNode, foundNode = null;

        #region Visit(SourceFile node, Object obj)

        public override Object Visit(SourceFile node, Object obj)
        {
            try
            {
                previousNode = node;
                foreach (string key in node.Namespacekeys)
                {
                    int count = node.GetNamespaceDefinitionCount(key);
                    for (int i = 0; i < count; i++)
                    {
                        if (foundNode != null) return foundNode;
                        node.GetNamespaceDeclarationElement(key, i).Accept(this, obj);
                    }
                }

                for (int i = 0; i < node.DeclarationCount; i++)
                {
                    if (foundNode != null) return foundNode;
                    node.GetDeclarationElement(i).Accept(this, obj);
                }
                if (foundNode == null)
                    foundNode = previousNode;

            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return null;
            }
            return foundNode;
        }

        #endregion

        #region Visit(Namespace node, Object obj)

        public override Object Visit(Namespace node, Object obj)
        {
            if (foundNode != null) return foundNode;

            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }

            if (node.Location >= previousNode.Location) previousNode = node;

            node.Identifier.Accept(this, obj);

            for (int i = 0; i < node.NamespaceMembersCount; i++)
            {
                if (foundNode != null) return foundNode;
                node.GetDeclarationElement(i).Accept(this, obj);
            }

            return null;
        }

        #endregion

        #region Visit(DeclarationSet node, Object obj)

        public override Object Visit(DeclarationSet node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;

            for (int i = 0; i < node.Count; i++)
            {
                if (foundNode != null) return foundNode;
                node.GetDeclarationElement(i).Accept(this, obj);
            }
            return null;
        }

        #endregion

        #region Visit(FieldDeclarationSet node, Object obj)

        public override Object Visit(FieldDeclarationSet node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }

            return null;
        }

        #endregion

        #region Visit(IdDeclaration node, Object obj)

        public override Object Visit(IdDeclaration node, Object obj)
        {
            if (node.IdentifierExp.IndexOfSSA != 0) return null;

            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;
            return null;
        }

        #endregion

        #region Visit(Definition node, Object obj)

        public override Object Visit(Definition node, Object obj)
        {
          if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj) {
            foundNode = previousNode;
            return foundNode;
          }
          if (node.Location >= previousNode.Location) previousNode = node;

            return node.Init.Accept(this, obj);
        }

        #endregion

        #region Visit(ConstantDefinition node, Object obj)

        public override Object Visit(ConstantDefinition node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;

            return node.Init.Accept(this, obj);
        }

        #endregion

        #region Visit(PropertyDefinition node, Object obj)

        public override Object Visit(PropertyDefinition node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;

            if (node.GetBlock != null)
                node.GetBlock.Accept(this, obj);

            if (foundNode != null) return foundNode;

            if (node.SetBlock != null)
                node.SetBlock.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(ClassDefinition node, Object obj)

        public override Object Visit(ClassDefinition node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;
            for (int i = 0; i < node.MemberCount; i++)
            {
                if (foundNode != null) return foundNode;

                node.GetMemberElement(i).Accept(this, obj);
            }
            return null;
        }

        #endregion

        #region Visit(InterfaceDefinition node, Object obj)

        public override Object Visit(InterfaceDefinition node, Object obj)
        {
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);

            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;
            for (int i = 0; i < node.MemberCount; i++)
            {
                if (foundNode != null) return foundNode;

                node.GetMemberElement(i).Accept(this, obj);
            }
            return null;
        }

        #endregion

        #region Visit(ConstructorDefinition node, Object obj)

        public override Object Visit(ConstructorDefinition node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;


            if (node.Initialization != null)
                node.Initialization.Accept(this, obj);

            if (foundNode != null) return foundNode;

            return node.Body.Accept(this, obj);
        }

        #endregion

        #region Visit(FieldDeclaration node, Object obj)

        public override Object Visit(FieldDeclaration node, Object obj)
        {
            Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;

            return null;
        }

        #endregion

        #region Visit(FieldDefinition node, Object obj)

        public override Object Visit(FieldDefinition node, Object obj)
        {
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            return node.Init.Accept(this, obj);
        }

        #endregion

        #region Visit(ConstantFieldDefinition node, Object obj)

        public override Object Visit(ConstantFieldDefinition node, Object obj)
        {
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            return node.Init.Accept(this, obj);
        }

        #endregion

        #region Visit(MethodDeclaration node, Object obj)

        public override Object Visit(MethodDeclaration node, Object obj)
        {
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;
            return null;
        }

        #endregion

        #region Visit(MethodDefinition node, Object obj)

        public override Object Visit(MethodDefinition node, Object obj)
        {
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;

            Location paramLocation;
            foreach (var param in node.ParametersInfo)
            {
                paramLocation = new Location(previousNode.Location.FileName, param.Line, param.Column);

                if (this.previousNode.Location < (Location)obj && paramLocation >= (Location)obj)
                {
                    foundNode = previousNode;
                    return foundNode;
                }
                var paramDec = new IdDeclaration(new SingleIdentifierExpression(param.Identifier, paramLocation), param.ParamType, paramLocation);
                var paramTypeExpression = TypeTable.Instance.GetType(param.ParamType, paramLocation);
                paramDec.TypeExpr = paramTypeExpression;
                previousNode = paramDec;

            }

            //if (node.Location >= previousNode.Location) previousNode = node;

            return node.Body.Accept(this, obj);
        }

        #endregion

        // Expressions
        #region Visit(ArgumentExpression node, Object obj)

        public override Object Visit(ArgumentExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;

            return node.Argument.Accept(this, obj);
        }

        #endregion

        #region Visit(ArithmeticExpression node, Object obj)

        public override Object Visit(ArithmeticExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.FirstOperand.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.SecondOperand.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(ArrayAccessExpression node, Object obj)

        public override Object Visit(ArrayAccessExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;

            node.FirstOperand.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.SecondOperand.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(AssignmentExpression node, Object obj)

        public override Object Visit(AssignmentExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;

            node.FirstOperand.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.SecondOperand.Accept(this, obj);

            if (foundNode != null) return foundNode;

            if (node.MoveStat != null)
                node.MoveStat.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(BaseCallExpression node, Object obj)

        public override Object Visit(BaseCallExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;

            return node.Arguments.Accept(this, obj);
        }

        #endregion

        #region Visit(BaseExpression node, Object obj)

        public override Object Visit(BaseExpression node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;

            return null;
        }

        #endregion

        #region Visit(BinaryExpression node, Object obj)

        public override Object Visit(BinaryExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;

            node.FirstOperand.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.SecondOperand.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(BitwiseExpression node, Object obj)

        public override Object Visit(BitwiseExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;

            node.FirstOperand.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.SecondOperand.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(BoolLiteralExpression node, Object obj)

        public override Object Visit(BoolLiteralExpression node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }

            if (node.Location >= previousNode.Location) previousNode = node;
            return null;
        }

        #endregion

        #region Visit(CastExpression node, Object obj)

        public override Object Visit(CastExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;

            return node.Expression.Accept(this, obj);
        }

        #endregion

        #region Visit(CharLiteralExpression node, Object obj)

        public override Object Visit(CharLiteralExpression node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;

            return null;
        }

        #endregion

        #region Visit(CompoundExpression node, Object obj)

        public override Object Visit(CompoundExpression node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            //if (node.Location >= previousNode.Location) previousNode = node;

            for (int i = 0; i < node.ExpressionCount; i++)
            {
                if (foundNode != null) return foundNode;

                node.GetExpressionElement(i).Accept(this, obj);
            }
            return null;
        }

        #endregion

        #region Visit(DoubleLiteralExpression node, Object obj)

        public override Object Visit(DoubleLiteralExpression node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;
            Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            return null;
        }

        #endregion

        #region Visit(FieldAccessExpression node, Object obj)

        public override Object Visit(FieldAccessExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.Expression.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.FieldName.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(IntLiteralExpression node, Object obj)

        public override Object Visit(IntLiteralExpression node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;
            Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            return null;
        }

        #endregion

        #region Visit(InvocationExpression node, Object obj)

        public override Object Visit(InvocationExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);

            node.Identifier.Accept(this, obj);

            if (foundNode != null) return foundNode;

            if (node.Arguments.ExpressionCount > 0)
                node.Arguments.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(IsExpression node, Object obj)

        public override Object Visit(IsExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            return node.Expression.Accept(this, obj);
        }

        #endregion

        #region Visit(LogicalExpression node, Object obj)

        public override Object Visit(LogicalExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.FirstOperand.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.SecondOperand.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(NewArrayExpression node, Object obj)

        public override Object Visit(NewArrayExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            if (node.Size != null)
                node.Size.Accept(this, obj);

            if (foundNode != null) return foundNode;

            if (node.Init != null)
                node.Init.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(NewExpression node, Object obj)

        public override Object Visit(NewExpression node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;
            Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            return node.Arguments.Accept(this, obj);
        }

        #endregion

        #region Visit(NullExpression node, Object obj)

        public override Object Visit(NullExpression node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;
            Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            return null;
        }

        #endregion

        #region Visit(QualifiedIdentifierExpression node, Object obj)

        public override Object Visit(QualifiedIdentifierExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.IdName.Accept(this, obj);


            if (foundNode != null) return foundNode;

            node.IdExpression.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(RelationalExpression node, Object obj)

        public override Object Visit(RelationalExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.FirstOperand.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.SecondOperand.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(SingleIdentifierExpression node, Object obj)

        public override Object Visit(SingleIdentifierExpression node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }

            if (node.Location >= previousNode.Location) previousNode = node;
            Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            return null;
        }

        #endregion

        #region Visit(StringLiteralExpression node, Object obj)

        public override Object Visit(StringLiteralExpression node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;
            Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            return null;
        }

        #endregion

        #region Visit(TernaryExpression node, Object obj)

        public override Object Visit(TernaryExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.FirstOperand.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.SecondOperand.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.ThirdOperand.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(ThisExpression node, Object obj)

        public override Object Visit(ThisExpression node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;

            }
            if (node.Location >= previousNode.Location) previousNode = node;
            Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            return null;
        }

        #endregion

        #region Visit(UnaryExpression node, Object obj)

        public override Object Visit(UnaryExpression node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.Operand.Accept(this, obj);
            return null;
        }

        #endregion

        // Statements

        #region Visit(AssertStatement node, Object obj)

        public override Object Visit(AssertStatement node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.Condition.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.Expression.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(BreakStatement node, Object obj)

        public override Object Visit(BreakStatement node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;
            Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            return null;
        }

        #endregion

        #region Visit(CatchStatement node, Object obj)

        public override Object Visit(CatchStatement node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.Exception.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.Statements.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(Block node, Object obj)

        public override Object Visit(Block node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
           
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            for (int i = 0; i < node.StatementCount; i++)
            {
                if (foundNode != null) return foundNode;

                node.GetStatementElement(i).Accept(this, obj);
            }
            return null;
        }

        #endregion

        #region Visit(ContinueStatement node, Object obj)

        public override Object Visit(ContinueStatement node, Object obj)
        {
            if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            {
                foundNode = previousNode;
                return foundNode;
            }
            if (node.Location >= previousNode.Location) previousNode = node;
            Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            return null;
        }

        #endregion

        #region Visit(DoStatement node, Object obj)

        public override Object Visit(DoStatement node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            for (int i = 0; i < node.InitDo.Count; i++)
            {
                if (foundNode != null) return foundNode;

                node.InitDo[i].Accept(this, obj);
            }
            for (int i = 0; i < node.BeforeBody.Count; i++)
            {
                if (foundNode != null) return foundNode;

                node.BeforeBody[i].Accept(this, obj);
            }

            if (foundNode != null) return foundNode;

            node.Statements.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.Condition.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(ForeachStatement node, Object obj)

        public override Object Visit(ForeachStatement node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.ForEachDeclaration.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.ForeachExp.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.ForeachBlock.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(ForStatement node, Object obj)

        public override Object Visit(ForStatement node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            for (int i = 0; i < node.InitializerCount; i++)
            {
                if (foundNode != null) return foundNode;

                node.GetInitializerElement(i).Accept(this, obj);
            }
            for (int i = 0; i < node.AfterInit.Count; i++)
            {
                if (foundNode != null) return foundNode;

                node.AfterInit[i].Accept(this, obj);
            }
            for (int i = 0; i < node.BeforeCondition.Count; i++)
            {
                if (foundNode != null) return foundNode;

                node.BeforeCondition[i].Accept(this, obj);
            }

            if (foundNode != null) return foundNode;

            node.Condition.Accept(this, obj);
            for (int i = 0; i < node.AfterCondition.Count; i++)
            {
                if (foundNode != null) return foundNode;

                node.AfterCondition[i].Accept(this, obj);
            }

            if (foundNode != null) return foundNode;
            node.Statements.Accept(this, obj);
            for (int i = 0; i < node.IteratorCount; i++)
            {
                if (foundNode != null) return foundNode;
                node.GetIteratorElement(i).Accept(this, obj);
            }

            return null;
        }

        #endregion

        #region Visit(IfElseStatement node, Object obj)

        public override Object Visit(IfElseStatement node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.Condition.Accept(this, obj);
            for (int i = 0; i < node.AfterCondition.Count; i++)
            {
                if (foundNode != null) return foundNode;

                node.AfterCondition[i].Accept(this, obj);
            }

            if (foundNode != null) return foundNode;

            node.TrueBranch.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.FalseBranch.Accept(this, obj);
            for (int i = 0; i < node.ThetaStatements.Count; i++)
            {
                if (foundNode != null) return foundNode;
                node.ThetaStatements[i].Accept(this, obj);
            }
            return null;
        }

        #endregion

        #region Visit(ReturnStatement node, Object obj)

        public override Object Visit(ReturnStatement node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.Assigns.Accept(this, obj);

            if (foundNode != null) return foundNode;

            if (node.ReturnExpression != null)
                return node.ReturnExpression.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(SwitchLabel node, Object obj)

        public override Object Visit(SwitchLabel node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            //if (foundNode != null) return foundNode;


            if (node.SwitchSectionType == SectionType.Case)
                node.Condition.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(SwitchSection node, Object obj)

        public override Object Visit(SwitchSection node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            for (int i = 0; i < node.LabelSection.Count; i++)
            {
                if (foundNode != null) return foundNode;

                node.LabelSection[i].Accept(this, obj);
            }
            for (int i = 0; i < node.SwitchBlock.StatementCount; i++)
            {
                if (foundNode != null) return foundNode;

                node.SwitchBlock.GetStatementElement(i).Accept(this, obj);
            }
            return null;
        }

        #endregion

        #region Visit(SwitchStatement node, Object obj)

        public override Object Visit(SwitchStatement node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.Condition.Accept(this, obj);
            for (int i = 0; i < node.AfterCondition.Count; i++)
            {

                if (foundNode != null) return foundNode;

                node.AfterCondition[i].Accept(this, obj);
            }

            for (int i = 0; i < node.SwitchBlockCount; i++)
            {

                if (foundNode != null) return foundNode;
                node.GetSwitchSectionElement(i).Accept(this, obj);
            }
            for (int i = 0; i < node.ThetaStatements.Count; i++)
            {
                if (foundNode != null) return foundNode;

                node.ThetaStatements[i].Accept(this, obj);
            }
            return null;
        }

        #endregion

        #region Visit(ThrowStatement node, Object obj)

        public override Object Visit(ThrowStatement node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            return node.ThrowExpression.Accept(this, obj);
        }

        #endregion

        //#region Visit(TryBlockStatement node, Object obj)

        //public override Object Visit(TryBlockStatement node, Object obj)
        //{
        //    Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName); 
        //    node.TryBlock.Accept(this, obj);
        //    for (int i = 0; i < node.CatchCount; i++)
        //    {
        //        node.GetCatchElement(i).Accept(this, obj);
        //    }
        //    node.FinallyBlock.Accept(this, obj);
        //    return null;
        //}

        //#endregion

        #region Visit(WhileStatement node, Object obj)

        public override Object Visit(WhileStatement node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            for (int i = 0; i < node.InitWhile.Count; i++)
            {
                if (foundNode != null) return foundNode;

                node.InitWhile[i].Accept(this, obj);
            }
            for (int i = 0; i < node.BeforeCondition.Count; i++)
            {
                if (foundNode != null) return foundNode;

                node.BeforeCondition[i].Accept(this, obj);
            }
            if (foundNode != null) return foundNode;

            node.Condition.Accept(this, obj);
            for (int i = 0; i < node.AfterCondition.Count; i++)
            {
                if (foundNode != null) return foundNode;

                node.AfterCondition[i].Accept(this, obj);
            }
            if (foundNode != null) return foundNode;

            node.Statements.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(MoveStatement node, Object obj)

        public override Object Visit(MoveStatement node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location) previousNode = node;
            //Trace.WriteLine(node.Location.Line + " " + node.Location.Column + " :" + node.GetType().FullName);
            node.LeftExp.Accept(this, obj);

            if (foundNode != null) return foundNode;

            node.RightExp.Accept(this, obj);

            if (foundNode != null) return foundNode;

            if (node.MoveStat != null)
                node.MoveStat.Accept(this, obj);
            return null;
        }

        #endregion

        #region Visit(ThetaStatement node, Object obj)

        public override Object Visit(ThetaStatement node, Object obj)
        {
            //if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
            //{
            //    foundNode = previousNode;
            //    return foundNode;
            //}
            //if (node.Location >= previousNode.Location)
            //    previousNode = node;
          
            node.ThetaId.Accept(this, obj);
            for (int i = 0; i < node.ThetaList.Count; i++)
            {
                if (foundNode != null) return foundNode;

                node.ThetaList[i].Accept(this, obj);
            }
            return null;
        }

        #endregion

        #region Visit(ExceptionManagementStatement node, Object obj)

        public override Object Visit(ExceptionManagementStatement node, Object obj)
        {
        //    if (this.previousNode.Location < (Location)obj && node.Location >= (Location)obj)
        //    {
        //        foundNode = previousNode;
        //        return foundNode;
        //    }
        //    if (node.Location >= previousNode.Location) previousNode = node;

            node.TryBlock.Accept(this, obj);

            if (foundNode != null) return foundNode;

            for (int i = 0; i < node.CatchCount; i++)
            {
                if (foundNode != null) return foundNode;
                node.GetCatchElement(i).Accept(this, obj);
            }

            if (foundNode != null) return foundNode;

            if (node.FinallyBlock != null)
            {
                if (foundNode != null) return foundNode;

                node.FinallyBlock.Accept(this, obj);
            }

            return null;
        }

        #endregion
    }
}