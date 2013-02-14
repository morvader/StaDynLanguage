using TypeSystem.Operations;
using TypeSystem;
namespace CodeGeneration.Operations {
    /// <summary>
    ///  Generates the store operation according with the type of the array.
    ///  </summary>       
    internal class CGLoadArrayElementOperation<T> : TypeSystemOperation where T : ILCodeGenerator {

        /// <summary>
        /// stream to write to
        /// </summary>
        private T codeGenerator;
        /// <summary>
        /// indentation to use
        /// </summary>
        private int indent;


        public CGLoadArrayElementOperation(T codeGenerator, int indent) {
            this.codeGenerator = codeGenerator;
            this.indent = indent;
        }
        public override object Exec(ClassType ct, object arg) {
            this.codeGenerator.ldelemRef(this.indent);
            return null;
        }
        public override object Exec(StringType s, object arg) {
            this.codeGenerator.ldelemRef(this.indent);
            return null;
        }
        public override object Exec(ArrayType a, object arg) {
            this.codeGenerator.ldelemRef(this.indent);
            return null;
        }
        public override object Exec(TypeVariable t, object arg) {
            this.codeGenerator.ldelemRef(this.indent);
            return null;
        }

        public override object Exec(UnionType t, object arg) {
            this.codeGenerator.ldelemRef(this.indent);
            return null;
        }
        public override object Exec(DoubleType d, object arg) {
            this.codeGenerator.ldelemDouble(this.indent);
            return null;
        }
        public override object Exec(TypeExpression t, object arg) {
            this.codeGenerator.ldelemInt(this.indent);
            return null;
        }
    }

}