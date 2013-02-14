using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;
using AST;
using ErrorManagement;

namespace StaDynLanguage.Visitors
{
    class VisitorFindCast : VisitorAdapter
    {
        public Stack<AstNode> castStack = new Stack<AstNode>();
        public override object Visit(CastExpression node, object obj)
        {
            if (node.Location > (Location)obj)
                return null;
            castStack.Push(node);
            return base.Visit(node, obj);
        }
    }
}
