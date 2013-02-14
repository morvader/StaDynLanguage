using System;
using System.Text;

using antlr;
using Parser;
using AST;
using System.IO;
using System.Diagnostics;

using ErrorManagement;
using Microsoft.VisualStudio.Text;
using StaDynLanguage.StaDynAST;
using StaDynLanguage.Errors;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using TypeSystem;
using StaDyn.StaDynProject;
using Semantic.SSAAlgorithm;
using Semantic;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using StaDynLanguage_Project;
using StaDynLanguage.Utils;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using DynVarManagement;




namespace StaDynLanguage
{
    public class StaDynParser
    {
        //private ITextBuffer source;
        private string fileName;
        //private string sourceCode;

        public StaDynParser(ITextBuffer source, string fileName)
        {
            //this.source = source;
            //this.fileName = fileName;

        }

        public StaDynParser(string fileName)
        {
            this.fileName = fileName;
            //Stream s = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }

        public StaDynParser()
        {
            // TODO: Complete member initialization
        }

        public StaDynSourceFileAST parseSource( Stream source)
        {
            //ErrorManager.Instance.Clear();
            //TypeTable.Instance.Clear();

            # region LexerConfiguration
            TokenStream lexer;

            //Stream s = new MemoryStream(ASCIIEncoding.Default.GetBytes(source.CurrentSnapshot.GetText()));
            //Stream s = new MemoryStream(ASCIIEncoding.Default.GetBytes(sourceCode));
            // Create a scanner that reads from the input stream passed to us

            //Stream s = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            CSharpLexer antlrLexer = new CSharpLexer(new StreamReader(source));

            // Define a selector that can switch from the C# codelexer to the C# preprocessor lexer
            TokenStreamSelector selector = new TokenStreamSelector();
            antlrLexer.Selector = selector;
            antlrLexer.setFilename(fileName);

            CSharpPreprocessorLexer preproLexer = new CSharpPreprocessorLexer(antlrLexer.getInputState());
            preproLexer.Selector = selector;
            CSharpPreprocessorHooverLexer hooverLexer = new CSharpPreprocessorHooverLexer(antlrLexer.getInputState());
            hooverLexer.Selector = selector;

            // use the special token object class
            antlrLexer.setTokenCreator(new CustomHiddenStreamToken.CustomHiddenStreamTokenCreator());
            antlrLexer.setTabSize(1);
            preproLexer.setTokenCreator(new CustomHiddenStreamToken.CustomHiddenStreamTokenCreator());
            preproLexer.setTabSize(1);
            hooverLexer.setTokenCreator(new CustomHiddenStreamToken.CustomHiddenStreamTokenCreator());
            hooverLexer.setTabSize(1);

            // notify selector about various lexers; name them for convenient reference later
            selector.addInputStream(antlrLexer, "codeLexer");
            selector.addInputStream(preproLexer, "directivesLexer");
            selector.addInputStream(hooverLexer, "hooverLexer");
            selector.select("codeLexer"); // start with main the CSharp code lexer
            lexer = selector;

            // create the stream filter; hide WS and SL_COMMENT
            TokenStreamHiddenTokenFilter filter = new TokenStreamHiddenTokenFilter(lexer);
            filter.hide(CSharpTokenTypes.WHITESPACE);
            filter.hide(CSharpTokenTypes.NEWLINE);
            filter.hide(CSharpTokenTypes.ML_COMMENT);
            filter.hide(CSharpTokenTypes.SL_COMMENT);

            //------------------------------------------------------------------

            #endregion

            // Create a parser that reads from the scanner
            CSharpParser parser = new CSharpParser(filter);

            parser.setFilename(fileName);

            StaDynSourceFileAST AstFile = new StaDynSourceFileAST();
            AstFile.FileName = fileName;



            try
            {
                AstFile.Ast = parser.compilationUnit();

            }
            catch (Exception e)
            {
                //Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Parser exception", e.Message));
                Trace.WriteLine("Parser exception " + e.Message);
                ErrorManager.Instance.NotifyError(new ParserError(new Location(fileName, 0, 0), e.Message));

                //StaDynErrorList.Instance.addError(ValidationErrorSeverity.Error, ValidationErrorType.Semantic, e.Message, fileName, 1, 3);

                ErrorPresenter.Instance.createErrors();
                //TextSpan t = new TextSpan();

                // t.iStartLine = t.iEndLine = ((antlr.ANTLRException)e).Data. - 1;
                //t.iStartIndex = ((antlr.RecognitionException)e).getColumn() - 1;
                //t.iEndIndex = t.iStartIndex + ((antlr.RecognitionException)e).token.getText().Length;

                // //ParserError error = new ParserError(source.GetFilePath(), t.iStartLine, t.iStartIndex, ((antlr.RecognitionException)e).Message);
                // ErrorAdapter error=new ErrorAdapter(new Location(source.GetFilePath(),0,0));

                // error.Description = e.Message;


                //StaDynErrorList.Instance.addError(TaskCategory.BuildCompile, TaskErrorCategory.Error, e.Message, e.Message, e.Message, 0, 0);

            }
            finally
            {
                source.Close();
            }

            return AstFile;

        }

        public void parseAll()
        {
            string staDynPath=ProjectConfiguration.Instance.GetProperty(PropertyTag.StaDynPath.ToString());
            if (String.IsNullOrEmpty(staDynPath)) return;

            ErrorManager.Instance.LogFileName = Path.Combine(staDynPath, "error.log");
            ProjectFileAST.Instance.clear();
            ErrorManager.Instance.Clear();
            TypeTable.Instance.Clear();
            ErrorPresenter.Instance.ClearErrors();

            //Parse current project source file
            foreach (ProjectItem item in FileUtilities.Instance.GetActiveProjectCompileItems())
            {
                this.fileName = Path.GetFullPath(item.get_FileNames(1));

                Stream str;
                //If ProjectItem is curretly open we have to get the window code
                if (item.IsOpen)
                {
                    //var souceCode = FileUtilities.Instance.GetIVsTextView(fileName);
                    TextDocument doc = item.Document.Object("TextDocument") as TextDocument;
                    EditPoint start = doc.StartPoint.CreateEditPoint();
                    EditPoint end = doc.EndPoint.CreateEditPoint();
                    string sourceCode = start.GetText(end);
                    str = new MemoryStream(ASCIIEncoding.Default.GetBytes(sourceCode));

                }
                //Get the sourceCode directly from the file
                else
                {
                    str = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                }

                ProjectFileAST.Instance.addOrReplaceAst(this.parseSource(str));
                
            }

            string DynVarOption= ProjectConfiguration.Instance.GetProperty(PropertyTag.DynVarOption.ToString());
            DynVarOption option = (DynVarOption)Enum.Parse(typeof(DynVarOption), DynVarOption);

            DynVarOptions.Instance.EverythingDynamic = option == StaDynLanguage_Project.DynVarOption.EverythingDynamic;
            DynVarOptions.Instance.EverythingStatic = option == StaDynLanguage_Project.DynVarOption.EverythingStatic;
            

            StaDynSourceFileAST[] listFile= ProjectFileAST.Instance.getFilesAST();

            try
            {
                for (int i = 0; i < listFile.Length; i++)
                {
                    if (listFile[i].Ast == null)
                        return;
                    listFile[i].Ast.Accept(new VisitorSSA(), null);
                }

                for (int i = 0; i < listFile.Length; i++)
                {
                    listFile[i].Ast.Accept(new VisitorTypeLoad(), null);
                }

                for (int i = 0; i < listFile.Length; i++)
                {
                    listFile[i].Ast.Accept(new VisitorTypeDefinition(), null);
                }

                for (int i = 0; i < listFile.Length; i++)
                {
                    string directory = Path.GetDirectoryName(listFile[i].FileName);
                    //req.FileName.Remove(req.FileName.Length - ast.Location.FileName.Length);

                    IDictionary<string, string> name = new Dictionary<string, string>();
                    name.Add(listFile[i].Ast.Location.FileName, directory);
                    VisitorSymbolIdentification vsi = new VisitorSymbolIdentification(name);

                    listFile[i].Ast.Accept(vsi, null);
                    //listFile[i].Ast.Accept(new VisitorSymbolIdentification(directories), null);
                }

                for (int i = 0; i < listFile.Length; i++)
                {
                    listFile[i].Ast.Accept(new VisitorTypeInference(), null);
                }

                for (int i = 0; i < listFile.Length; i++)
                {

                    ProjectFileAST.Instance.addOrReplaceAst(listFile[i]);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Parser exception " + e.Message);
                ErrorManager.Instance.NotifyError(new ParserError(new Location(fileName, 0, 0), e.Message));
                //StaDynErrorList.Instance.addError(ValidationErrorSeverity.Error, ValidationErrorType.Semantic, e.Message,fileName, 0, 0);
            }

            if (ErrorManager.Instance.ErrorCount > 0)
            {             
                ErrorPresenter.Instance.createErrors();
            }
            else{
                ErrorPresenter.Instance.ClearErrors();
                //ErrorPresenter.Instance.showErrorList();
            }

            //SourceHelper.refreshHighlighting();

        }
    }
}
