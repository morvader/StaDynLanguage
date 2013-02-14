using TypeSystem.Operations;
using TypeSystem;
using ErrorManagement;
using AST;
using Tools;
using System.Collections.Generic;
using CodeGeneration.ExceptionManagement;
namespace CodeGeneration.Operations {
    /// <summary>
       ///  It typechecks the runtime arguments, embeded in a method call, with the parametes of this method.
       ///  </summary>       
    internal class CGCastOperation <T>:TypeSystemOperation where T: ILCodeGenerator {

        /// <summary>
        /// streamto to write to
        /// </summary>
        private T codeGenerator;
        /// <summary>
        /// indentation to use
        /// </summary>
        private int indent;

 
        public CGCastOperation(T codeGenerator, int indent) {
            this.codeGenerator = codeGenerator;
            this.indent = indent;
          }

        public override object Exec(ClassType ct, object arg) {
            this.codeGenerator.castclass(this.indent, ct);
            return null;
        }
        public override object Exec(IntType it, object arg) {
            this.codeGenerator.convToInt(this.indent);
            return null;
        }

        public override object Exec(DoubleType dt, object arg) {
            this.codeGenerator.convToDouble(this.indent);
            return null;
        }
  
        public override object ReportError(TypeExpression tE) {
            ErrorManager.Instance.NotifyError(new CodeGenerationError("No se ha definido la operación solicitada"));
            return null;
        }
    }
}