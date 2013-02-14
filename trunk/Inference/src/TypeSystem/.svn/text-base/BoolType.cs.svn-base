////////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- //
// Project rROTOR                                                             //
// -------------------------------------------------------------------------- //
// File: BoolType.cs                                                          //
// Authors: Cristina Gonzalez Muñoz  -  cristi.gm@gmail.com                   //
//          Francisco Ortin - francisco.ortin@gmail.com                       //
// Description:                                                               //
//    Represent a bool type.                                                  //
//    Inheritance: TypeExpression.                                            //
//    Implements Composite pattern [Leaf].                                    //
//    Implements Singleton pattern.                                           //
// -------------------------------------------------------------------------- //
// Create date: 09-02-2007                                                    //
// Modification date: 22-03-2007                                              //
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using AST;
using ErrorManagement;
using Tools;
using TypeSystem.Operations;
//VISTO
namespace TypeSystem {
    /// <summary>
    /// Represent a bool type.
    /// </summary>
    /// <remarks>
    /// Inheritance: TypeExpression.
    /// Implements Composite pattern [Leaf].
    /// Implements Singleton pattern.
    /// </remarks>
   public class BoolType : TypeExpression {
        #region Fields

        /// <summary>
        /// Instance of class BoolType. (unique)
        /// </summary>
        private static readonly BoolType instance = new BoolType();

        /// <summary>
        /// To delegate all the object oriented behaviour that the built-in type does not offer
        /// </summary>
        private BCLClassType BCLType = new BCLClassType("System.Boolean", Type.GetType("System.Boolean"));


        #endregion

        #region Properties

        /// <summary>
        /// Gets the unique instance of BoolType
        /// </summary>
        public static BoolType Instance {
            get { return instance; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor of BoolType.
        /// </summary>
        private BoolType() {
            this.typeExpression = "bool";
            this.fullName = "bool";
        }

        /// <summary>
        /// Private and static constructor of BoolType.
        /// </summary>
        static BoolType() {
        }

        #endregion

        // WriteType Inference
      
        #region Dispatcher
        public override object AcceptOperation(TypeSystemOperation op, object arg) { return op.Exec(this, arg); }
        #endregion

        #region Assignment() ANULADA

        //// <summary>
        //// Check if the type can make an assignment operation.
        //// </summary>
        //// <param name="operand">WriteType expression of the operand of binary expression.</param>
        //// <param name="op">Operator.</param>
        //// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        //// <param name="unification">Indicates if the kind of unification (equivalent, incremental or override).</param>
        //// <param name="actualImplicitObject">Only suitable when the assignment is executed as a constraint of a method call. In that case,
        //// this parameter represents the actual object used to pass the message; null otherwise.</param>
        //// <param name="fileName">File name.</param>
        //// <param name="line">Line number.</param>
        //// <param name="column">Column number.</param>
        //// <returns>WriteType obtained with the operation.</returns>
        ////public override TypeExpression Assignment(TypeExpression operand, AssignmentOperator op, MethodType methodAnalyzed, SortOfUnification unification,
        ////            TypeExpression actualImplicitObject, Location location) {
        ////    if (op == AssignmentOperator.Assign)
        ////        return operand.Promotion(this, op, methodAnalyzed, location);
        ////    ErrorManager.Instance.NotifyError(new AssignmentError(operand.FullName, this.FullName, location));
        ////    return null;
        ////}

        #endregion

        #region Relational() ANULADA
       /*
        /// <summary>
        /// Check if the type can make an relational operation.
        /// </summary>
        /// <param name="operand">WriteType expression of the operand of binary expression.</param>
        /// <param name="op">Operator.</param>
        /// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        /// <param name="showErrorMessage">Indicates if an error message should be shown (used for dynamic types)</param>
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <returns>WriteType obtained with the operation.</returns>
        public override TypeExpression Relational(TypeExpression operand, RelationalOperator op, MethodType methodAnalyzed, bool showErrorMessage, Location loc) {
            if ((op == RelationalOperator.Equal) || (op == RelationalOperator.NotEqual)) {
                if (operand.PromotionLevel(this) != -1)
                    return BoolType.Instance;
                if (showErrorMessage) {
                    ErrorManager.Instance.NotifyError(new TypePromotionError(operand.FullName, this.FullName, loc));
                    return null;
                }
            }
            if (showErrorMessage)
                ErrorManager.Instance.NotifyError(new OperationNotAllowedError(op.ToString(), this.FullName, operand.FullName, loc));
            return null;
        }
      */
        #endregion

        #region Arithmetic ANULADA
        /*
         /// <summary>
        /// Check if the type can make an arithmetic operation.
        /// </summary>
        /// <param name="operand">WriteType expression of the operand of binary expression.</param>
        /// <param name="op">Operator.</param>
        /// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        /// <param name="showErrorMessage">Indicates if an error message should be shown (used for dynamic types)</param>
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <returns>WriteType obtained with the operation.</returns>
        public override TypeExpression Arithmetic(TypeExpression operand, Enum op, MethodType methodAnalyzed, bool showErrorMessage, Location loc) {
            if (op.Equals(ArithmeticOperator.Plus) && operand.Equivalent(StringType.Instance))
                return StringType.Instance;
            if (showErrorMessage)
                ErrorManager.Instance.NotifyError(new TypePromotionError(operand.FullName, this.FullName, op.ToString(), loc));
            return null;
        }
        * */
        #endregion

        #region Dot() ANULADA
        ///// <summary>
        ///// Check if the type can make an operation of field access.
        ///// Generates an error if the attribute does not exist.
        ///// Generates a constraint in case it is applied to a free variable. 
        ///// </summary>
        ///// <param name="field">Field to access.</param>
        ///// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        ///// <param name="previousDot">To detect infinite loops. The types that have been previously passed the dot message. Used for union types.</param>
        ///// <param name="fileName">File name.</param>
        ///// <param name="line">Line number.</param>
        ///// <param name="column">Column number.</param>
        ///// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Dot(string field, MethodType methodAnalyzed, IList<TypeExpression> previousDot, Location loc) {
        //    return this.BCLType.Dot(field, methodAnalyzed, previousDot, loc);
        //}
        ///// <summary>
        ///// Tries to find a attribute. 
        ///// No error is generated if the attribute does not exist.
        ///// It does not generate a constraint in case it is applied to a free variable.
        ///// </summary>
        ///// <param name="memberName">Member to access.</param>
        ///// <param name="previousDot">To detect infinite loops. The types that have been previously passed the dot message. Used for union types.</param>
        ///// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Dot(string memberName, IList<TypeExpression> previousDot) {
        //    return this.BCLType.Dot(memberName, previousDot);
        //}
        #endregion

        #region AsClassType()
        /// <summary>
        /// Represent a type as a class. It is mainly used to obtain the BCL representation of types
        /// (string=String, int=Int32, []=Array...)
        /// </summary>
        /// <returns>The class type is there is a map, null otherwise</returns>
        public override ClassType AsClassType() {
            return this.BCLType;
        }
        #endregion

        // WriteType Promotion

        #region PromotionLevel() ANULADA
        ///// <summary>
        ///// Returns a value that indicates a promotion level.
        ///// </summary>
        ///// <param name="type">WriteType to promotion.</param>
        ///// <returns>Returns a promotion value.</returns>
        //public override int PromotionLevel(TypeExpression type) {
        //    // * Bool type and type variable
        //    if (TypeExpression.As<BoolType>(type)!=null)
        //        return 0;
        //    // * WriteType variable
        //    TypeVariable typeVariable = type as TypeVariable;
        //    if (typeVariable != null && typeVariable.Substitution==null) 
        //        // * A free variable is complete promotion
        //        return 0;
        //    // * Union type
        //    UnionType unionType = TypeExpression.As<UnionType>(type);
        //    if (unionType != null)
        //        return unionType.SuperType(this);
        //    // * Field type and bounded type variable
        //    FieldType fieldType = TypeExpression.As<FieldType>(type);
        //    if (fieldType != null)
        //        return this.PromotionLevel(fieldType.FieldTypeExpression);
        //    // * Use the BCL object oriented approach
        //    return this.BCLType.PromotionLevel(type);
        //}

        #endregion

        // WriteType Unification

        #region Unify
        /// <summary>
        /// This method unifies two type expressions (this and te)
        /// </summary>
        /// <param name="te">The expression to be unfied with this</param>
        /// <param name="unification">Indicates if the kind of unification (equivalent, incremental or override).</param>
        /// <param name="previouslyUnified">To detect infinite loops. The previously unified pairs of type expressions.</param>
        /// <returns>If the unification was successful</returns>
        public override bool Unify(TypeExpression te, SortOfUnification unification, IList<Pair<TypeExpression, TypeExpression>> previouslyUnified) {
            BoolType bt = te as BoolType;
            if (bt != null)
                return true;
            if (te is TypeVariable && unification != SortOfUnification.Incremental)
                // * No incremental unification is commutative
                return te.Unify(this, unification, previouslyUnified);
            return false;
        }
        #endregion

        #region IsValueType()

        /// <summary>
        /// True if type expression is a ValueType. Otherwise, false.
        /// </summary>
        /// <returns>Returns true if the type expression is a ValueType. Otherwise, false.</returns>
        public override bool IsValueType()
        {
           return true;
        }

        #endregion

    }
}
