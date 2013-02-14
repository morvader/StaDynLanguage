﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AST;

namespace StaDynLanguage.StaDynAST {

	public class StaDynSourceFileAST {
				private SourceFile ast = null;
				string fileName = null;

				public SourceFile Ast
				{

				    get {
						    return ast;
					    }

				    set {
						    ast = value;
					    }
			}
				public string FileName
				{

				    get {
						    return fileName;
					    }

				    set {
						    fileName = value;
					    }
			}

				public StaDynSourceFileAST() {
				}

				public StaDynSourceFileAST(SourceFile AST, string fileName) {
					this.ast = AST;
					this.fileName = fileName;
				}

		}
}
