﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Semantic.SSAAlgorithm;
using Semantic;
using TypeSystem;
using System.IO;
using ErrorManagement;

namespace StaDynLanguage.StaDynAST {

	public class DecorateAST {

				//Implemets Singleton

#region Singleton patern
				static DecorateAST instance = null;

				private DecorateAST() {}

				public static DecorateAST Instance
				{

				    get {
						    if (instance == null) {
								    instance = new DecorateAST();
							    }

						    return instance;
					    }
			}
#endregion

				public void VisitorSSA(string fileName) {
					StaDynSourceFileAST file = ProjectFileAST.Instance.getAstFile(fileName);

					if (file.Ast == null) return ;

					file.Ast.Accept(new VisitorSSA(), null);

					//replace AST
					ProjectFileAST.Instance.addOrReplaceAst(file);
				}

				public void VisitorTypeLoad(string fileName, bool clearTypeTable) {
					StaDynSourceFileAST file = ProjectFileAST.Instance.getAstFile(fileName);

					if (file.Ast == null) return ;

					if (clearTypeTable) {
							TypeTable.Instance.Clear();
						}

					file.Ast.Accept(new VisitorTypeLoad(), null);

					//replace AST
					ProjectFileAST.Instance.addOrReplaceAst(file);
				}

				public void VisitorTypeDefinition(string fileName) {
					StaDynSourceFileAST file = ProjectFileAST.Instance.getAstFile(fileName);

					if (file.Ast == null) return ;

					file.Ast.Accept(new VisitorTypeDefinition(), null);

					//replace AST
					ProjectFileAST.Instance.addOrReplaceAst(file);
				}

				public void VisitorSymbolIdentification(string fileName) {
					StaDynSourceFileAST file = ProjectFileAST.Instance.getAstFile(fileName);

					if (file.Ast == null) return ;

					string directory = Path.GetDirectoryName(fileName);

					//req.FileName.Remove(req.FileName.Length - ast.Location.FileName.Length);

					IDictionary<string, string> name = new Dictionary<string, string>();

					name.Add(file.Ast.Location.FileName, directory);

					VisitorSymbolIdentification vsi = new VisitorSymbolIdentification(name);
                    
					file.Ast.Accept(vsi, null);

					//replace AST
					ProjectFileAST.Instance.addOrReplaceAst(file);
				}

				public void VisitorTypeInference(string fileName) {
					StaDynSourceFileAST file = ProjectFileAST.Instance.getAstFile(fileName);

					if (file.Ast == null) return ;

					file.Ast.Accept(new VisitorTypeInference(), null);

					//replace AST
					ProjectFileAST.Instance.addOrReplaceAst(file);
				}

				public StaDynSourceFileAST completeDecorateAndUpdate(StaDynSourceFileAST file) {
					//StaDynSourceFileAST file = ProjectFileAST.Instance.getAstFile(fileName);

					try {
							file = this.completeDecorateAST(file);

						} catch (Exception e) {
							ErrorManager.Instance.NotifyError(new ParserError(new Location(file.FileName, 0, 0), e.Message));
						}

					//replace AST
					ProjectFileAST.Instance.addOrReplaceAst(file);

					return file;
				}

				public StaDynSourceFileAST completeDecorateAndUpdate(string fileName, bool clearTypeTable) {
					StaDynSourceFileAST file = ProjectFileAST.Instance.getAstFile(fileName);

					try {
							this.completeDecorateAST(file);

						} catch (Exception e) {
							ErrorManager.Instance.NotifyError(new ParserError(new Location(fileName, 0, 0), e.Message));
						}

					//replace AST
					ProjectFileAST.Instance.addOrReplaceAst(file);

					return file;

					//try
					//{
					//    this.VisitorSSA(fileName);
					//    this.VisitorTypeLoad(fileName, clearTypeTable);
					//    this.VisitorTypeDefinition(fileName);
					//    this.VisitorSymbolIdentification(fileName);
					//    this.VisitorTypeInference(fileName);
					//}
					//catch (Exception e)
					//{
					//    ErrorManager.Instance.NotifyError(new ParserError(new Location(fileName, 0, 0), e.Message));
					//}

					//return ProjectFileAST.Instance.getAstFile(fileName);
				}

				public StaDynSourceFileAST completeDecorateAST(StaDynSourceFileAST file) {
					try {

							if (file.Ast == null) return file;

							//TypeTable.Instance.Clear();

							file.Ast.Accept(new VisitorSSA(), null);

							file.Ast.Accept(new VisitorTypeLoad(), null);

							file.Ast.Accept(new VisitorTypeDefinition(), null);

							string directory = Path.GetDirectoryName(file.FileName);

							//req.FileName.Remove(req.FileName.Length - ast.Location.FileName.Length);

							IDictionary<string, string> name = new Dictionary<string, string>();

							name.Add(file.Ast.Location.FileName, directory);

							VisitorSymbolIdentification vsi = new VisitorSymbolIdentification(name);

							file.Ast.Accept(vsi, null);

							file.Ast.Accept(new VisitorTypeInference(), null);

						} catch (Exception e) {
							ErrorManager.Instance.NotifyError(new ParserError(new Location(file.FileName, 0, 0), e.Message));
						}

					//Update refernces
					//ProjectFileAST.Instance.addOrReplaceAst(file);

					return file;
				}
		}
}
