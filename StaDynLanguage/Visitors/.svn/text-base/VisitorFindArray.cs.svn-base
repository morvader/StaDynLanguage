using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;
using AST;
using ErrorManagement;

namespace StaDynLanguage.Visitors
{
    class VisitorFindArray : VisitorAdapter
    {
        public Stack<AstNode> arrayStack = new Stack<AstNode>();

        public override object Visit(ArrayAccessExpression node, object obj)
        {
            if (node.Location > (Location)obj)
                return null;
            arrayStack.Push(node);

            return base.Visit(node, obj);
        }
    }
}
