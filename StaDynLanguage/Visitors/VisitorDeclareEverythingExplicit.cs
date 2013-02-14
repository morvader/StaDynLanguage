﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;
using TypeSystem;
using StaDynLanguage.Utils;

namespace StaDynLanguage.Visitors {
  public class VisitorDeclareEverythingExplicit: VisitorAdapter {
    public override object Visit(AST.Definition node, object obj) {
      if (node.IdentifierExp.IndexOfSSA != 0) return null;

      if (node.TypeExpr is TypeVariable)
        SourceHelper.DeclareExplicit(node, false);

      return null;
    }
    public override object Visit(AST.IdDeclaration node, object obj) {
      if (node.IdentifierExp.IndexOfSSA != 0) return null;

      if (node.TypeExpr is TypeVariable)
        SourceHelper.DeclareExplicit(node, false);

      return null;
    }
  }
}
