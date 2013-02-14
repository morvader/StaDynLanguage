using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynVarManagement;
using ErrorManagement;
using AST;
using StaDynLanguage.StaDynAST;
using System.IO;
using StaDynLanguage.Intellisense;

namespace StaDynLanguage.StaDynDynamic
{
    public class StaDynDynamicHelper
    {
        //Implements Singleton
        static StaDynDynamicHelper instance = null;

        StaDynDynamicHelper()
        {
        }

        public static StaDynDynamicHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StaDynDynamicHelper();
                }
                return instance;
            }
        }

        public VarPath getVarPath(string varName, int line, int column, SourceFile ast) {
          if (ast == null ) return null;

          //Location currentLocation = new Location(Path.GetFileName(file.FileName), line + 1, column);
          Location currentLocation = new Location(ast.Location.FileName, line + 1, column);

          Namespace currentNamespace = StaDynIntellisenseHelper.Instance.getCurrentNameSpace(currentLocation, ast);
          ClassDefinition currentClass = StaDynIntellisenseHelper.Instance.getCurrentClass(currentLocation, ast);
          MethodDefinition currentMethod = StaDynIntellisenseHelper.Instance.getCurrentMethod(currentLocation, ast);

          VarPath varpath = new VarPath();
          varpath.VarName = varName;

          varpath.NamespaceName = currentNamespace != null ? currentNamespace.Identifier.Identifier : "";
          varpath.ClassName = currentClass != null ? currentClass.Identifier : null;
          varpath.MethodName = currentMethod != null ? currentMethod.Identifier : null;

          return varpath;
        }
        public VarPath getVarPath(string varName, int line, int column, StaDynSourceFileAST file)
        {
            if (file == null || file.Ast == null) return null;

            return this.getVarPath(varName, line, column, file.Ast);

            ////Location currentLocation = new Location(Path.GetFileName(file.FileName), line + 1, column);
            //Location currentLocation = new Location(file.FileName, line + 1, column);

            //Namespace currentNamespace = StaDynIntellisenseHelper.Instance.getCurrentNameSpace(currentLocation, file.Ast);
            //ClassDefinition currentClass = StaDynIntellisenseHelper.Instance.getCurrentClass(currentLocation, file.Ast);
            //MethodDefinition currentMethod = StaDynIntellisenseHelper.Instance.getCurrentMethod(currentLocation, file.Ast);

            //VarPath varpath = new VarPath();
            //varpath.VarName = varName;

            //varpath.NamespaceName = currentNamespace != null ? currentNamespace.Identifier.Identifier : "";
            //varpath.ClassName = currentClass != null ? currentClass.Identifier : null;
            //varpath.MethodName = currentMethod != null ? currentMethod.Identifier : null;

            //return varpath;
        }
        //public bool checkDynamicVar(IToken token)
        public bool checkDynamicVar(string varName, int line, int column, StaDynSourceFileAST file)
        {
           
            VarPath varpath = this.getVarPath(varName, line, column, file);

            if (varpath == null)
                return false;

            DynVarManager dynVarManager = new DynVarManager();

            string filename = Path.ChangeExtension(file.FileName, DynVarManagement.DynVarManager.DynVarFileExt);
            dynVarManager.LoadOrCreate(filename);

            return dynVarManager.IsDynamic(varpath);

        }

        public bool checkDynamicVar(VarPath varpath,string fileName)
        {
            DynVarManager dynVarManager = new DynVarManager();

            string filename = Path.ChangeExtension(fileName, DynVarManagement.DynVarManager.DynVarFileExt);
            dynVarManager.LoadOrCreate(filename);

            return dynVarManager.IsDynamic(varpath);
        }

        public void makeVarDynamic(string varName, int line, int column, SourceFile ast)
        {
          VarPath varpath = this.getVarPath(varName, line, column, ast);

            DynVarManager dynVarManager = new DynVarManager();

            string filename = Path.ChangeExtension(ast.Location.FileName, DynVarManagement.DynVarManager.DynVarFileExt);
            dynVarManager.LoadOrCreate(filename);
            dynVarManager.SetDynamic(varpath);
            dynVarManager.Save();

        }

        public void makeVarStatic(string varName, int line, int column, SourceFile ast) {
          VarPath varpath = this.getVarPath(varName, line, column, ast);

          DynVarManager dynVarManager = new DynVarManager();

          string filename = Path.ChangeExtension(ast.Location.FileName, DynVarManagement.DynVarManager.DynVarFileExt);
          dynVarManager.LoadOrCreate(filename);
          dynVarManager.SetStatic(varpath);
          dynVarManager.Save();

        }
    }
}
