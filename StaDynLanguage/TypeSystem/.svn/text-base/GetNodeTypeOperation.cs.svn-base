using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ast.Operations;
using AST;
using TypeSystem;

namespace StaDynLanguage.TypeSystem
{
    class GetNodeTypeOperation : AstOperation
    {
        public GetNodeTypeOperation() { }

        public override object Exec(AstNode a, object arg)
        {
            return null;
        }
        public override object Exec(BaseCallExpression b, object arg)
        {
            return b.ExpressionType;
        }
        public override object Exec(BaseExpression b, object arg)
        {
            return b.ExpressionType;
        }
        public override object Exec(Declaration d, object arg)
        {
            return d.TypeExpr;
        }
        public override object Exec(Definition d, object arg)
        {
            return d.TypeExpr;
        }
        public override object Exec(Expression e, object arg)
        {
            TypeExpression type = e.ExpressionType;
            if (e is CastExpression)
                type = ((CastExpression)e).CastType;

            return type;
        }
        public override object Exec(FieldAccessExpression f, object arg)
        {
            return f.ExpressionType;
        }
        public override object Exec(IdentifierExpression i, object arg)
        {
            return i.ExpressionType;
        }
        public override object Exec(InvocationExpression i, object arg)
        {
            return i.ExpressionType;
        }
        public override object Exec(NewExpression n, object arg)
        {
            return n.ExpressionType;
        }
        public override object Exec(SingleIdentifierExpression s, object arg)
        {
            return s.ExpressionType;
        }
        public override object Exec(Statement s, object arg)
        {
            return null;
        }


    }
}
