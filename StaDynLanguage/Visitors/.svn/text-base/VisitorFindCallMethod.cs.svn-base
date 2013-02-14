using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;
using AST;
using ErrorManagement;

namespace StaDynLanguage.Visitors
{
    class VisitorFindCallMethod:VisitorAdapter
    {
        public Stack<AstNode> methodStack = new Stack<AstNode>();
        public override object Visit(InvocationExpression node, object obj)
        {
            if (node.Location > (Location)obj)
                return null;
            methodStack.Push(node);
            return base.Visit(node, obj);

            
        }
        public override object Visit(NewExpression node, object obj)
        {
            if (node.Location > (Location)obj)
                return null;
            methodStack.Push(node);
            return base.Visit(node, obj);
        }
    }
}
