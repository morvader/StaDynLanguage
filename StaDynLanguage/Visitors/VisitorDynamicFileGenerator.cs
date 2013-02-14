using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;
using DynVarManagement;
using System.IO;
using TypeSystem;
using StaDynLanguage.StaDynDynamic;

namespace StaDynLanguage.Visitors
{
    public enum DynamicBehaviour { EVERYTHINGSTATIC, EVERYTHINGDYNAMIC };

    public class VisitorDynamicFileGenerator : VisitorAdapter
    {
        private string filename;
        private DynamicBehaviour dynamicBehaviour;
        private string currentNamespace = null;
        private string currentClassName = null;
        private string currentInterfaceName = null;
        private string currentMethodName = null;
        DynVarManager dynVarManager;

        public VisitorDynamicFileGenerator(string fileName,DynamicBehaviour behaviour)
        {
            this.filename = fileName;
            this.dynamicBehaviour = behaviour;
        }

        public override object Visit(AST.SourceFile node, object obj)
        {
            //Prepare DynFile and DynVarManager
            this.dynVarManager = new DynVarManager();
            string dynFilename = Path.ChangeExtension(this.filename, DynVarManagement.DynVarManager.DynVarFileExt);
            dynVarManager.LoadOrCreate(dynFilename);

            //Star visiting nodes
            base.Visit(node, obj);
          
            //Save the results when all its done
            dynVarManager.Save();

            return null;
        }
        public override object Visit(AST.ClassDefinition node, object obj)
        {
            currentClassName = node.Identifier;
            return base.Visit(node, obj);
        }

        public override object Visit(AST.MethodDefinition node, object obj)
        {
            currentMethodName = node.Identifier;
            return base.Visit(node, obj);
        }
        public override object Visit(AST.Namespace node, object obj)
        {
            currentNamespace = node.Identifier.Identifier;
            return base.Visit(node, obj);
        }
        public override object Visit(AST.InterfaceDefinition node, object obj)
        {
            currentInterfaceName = node.Identifier;
            return base.Visit(node, obj);
        }

        public override object Visit(AST.FieldDeclaration node, object obj) {     
          //var fieldType = node.TypeExpr as FieldType;
          //if (!(fieldType.FieldTypeExpression is TypeVariable)) return base.Visit(node, obj);

          //this.setVarDynamism(node);

          return base.Visit(node, obj);
        }
      
        public override object Visit(AST.Definition node, object obj) {
          if (!(node.TypeExpr is TypeVariable)) return base.Visit(node, obj);

          this.setVarDynamism(node);

          return base.Visit(node, obj);
        }
        public override object Visit(AST.IdDeclaration node, object obj)
        {
            if (!(node.TypeExpr is TypeVariable)) return base.Visit(node, obj);

            this.setVarDynamism(node);

            return base.Visit(node, obj);
            
        }

        private void setVarDynamism(AST.AstNode node) {
          var IdNode = node as AST.IdDeclaration;

          if(IdNode==null) return;

          var varpath = new VarPath();
          varpath.VarName = IdNode.Identifier;
          varpath.NamespaceName = currentNamespace==null ? "": currentNamespace;
          varpath.ClassName = currentClassName;
          varpath.MethodName = currentMethodName;
          varpath.InterfaceName = currentInterfaceName;

          if (this.dynamicBehaviour == DynamicBehaviour.EVERYTHINGDYNAMIC) {
            //Set dynamic only if its not dynamic allready
            if (!this.dynVarManager.IsDynamic(varpath))
              dynVarManager.SetDynamic(varpath);
          }
          else {
            if (this.dynVarManager.IsDynamic(varpath))
              dynVarManager.SetStatic(varpath);
          }
        }
    }
}
