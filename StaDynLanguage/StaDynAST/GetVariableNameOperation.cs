﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ast.Operations;

namespace StaDynLanguage.StaDynAST {

	class GetVariableNameOperation: AstOperation {
				public override object Exec(AST.Declaration d, object arg) {

					return base.Exec(d, arg);
				}
		}
}
