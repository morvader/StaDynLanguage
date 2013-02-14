using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaDynLanguage.StaDynAST {

	public class ProjectFileAST {

#region Singleton pattern
				//Implements Singleton
				static ProjectFileAST instance = null;

				ProjectFileAST() {
					FilesAST = new List<StaDynSourceFileAST>();
				}

				public static ProjectFileAST Instance
				{

				    get {
						    if (instance == null) {
								    instance = new ProjectFileAST();
							    }

						    return instance;
					    }
			}
#endregion

				private List<StaDynSourceFileAST> FilesAST;

				public StaDynSourceFileAST[] getFilesAST() {
					return this.FilesAST.ToArray();
				}

				/// <summary>
				/// Gets the index of the filename required. If it doesent exist returns -1
				/// </summary>
				/// <param name="fileName">File Name of AST requested</param>
				/// <returns>index if its exists, -1 if not</returns>
				private int getIndexOfFile(string fileName) {
					for (int i = 0; i < this.FilesAST.Count; i++) {
							if (this.FilesAST[i].FileName.Equals(fileName))
								return i;
						}

					return -1;
				}

				/// <summary>
				/// Adds a new StaDynSourceFileAST to the list
				/// </summary>
				/// <param name="newAST"></param>
				/// <returns>False if the AST exits already</returns>
				public bool addSourceFile(StaDynSourceFileAST newAST) {
					//fasle if already exists

					if (this.getIndexOfFile(newAST.FileName) != -1)
						return false;

					this.FilesAST.Add(newAST);

					return true;
				}

				/// <summary>
				/// Add the new StaDynSourceFile if the fileName doesnt exist or replace the existing AST is already exits
				/// </summary>
				/// <param name="newAst">new StaDynSourceFileAst</param>
				public void addOrReplaceAst(StaDynSourceFileAST newAst) {
					int index = this.getIndexOfFile(newAst.FileName);

					if (index < 0)
						this.addSourceFile(newAst);
					else
						this.FilesAST[index].Ast = newAst.Ast;
				}

				/// <summary>
				/// Gets the StaDynSourceFile for the requested fileName
				/// </summary>
				/// <param name="fileName">FileName</param>
				/// <returns>Null if it doesent exists</returns>
				public StaDynSourceFileAST getAstFile(string fileName) {
					int index = this.getIndexOfFile(fileName);

					if (index < 0)
						return null;

					return this.FilesAST[index];
				}

				public void clear() {
					this.FilesAST.Clear();

				}
		}
}
