////////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- //
// Project rROTOR                                                             //
// -------------------------------------------------------------------------- //
// File: NullType.cs                                                          //
// Authors: Cristina Gonzalez Muñoz  -  cristi.gm@gmail.com                   //
//          Francisco Ortin - francisco.ortin@gmail.com                       //
// Description:                                                               //
//    Represent a null type.                                                  //
//    Inheritance: TypeExpression.                                            //
//    Implements Composite pattern [Leaf].                                    //
//    Implements Singleton pattern.                                           //
// -------------------------------------------------------------------------- //
// Create date: 05-04-2007                                                    //
// Modification date: 05-04-2007                                              //
////////////////////////////////////////////////////////////////////////////////
//VISTO
using System;
using System.Collections.Generic;
using System.Text;

using AST;
using ErrorManagement;
using Tools;
using TypeSystem.Operations;

namespace TypeSystem {
    /// <summary>
    /// Represent a null type.
    /// </summary>
    /// <remarks>
    /// Inheritance: TypeExpression.
    /// Implements Composite pattern [Leaf].
    /// Implements Singleton pattern.
    /// </remarks>
    public class NullType : TypeExpression {
        
        #region Fields

        /// <summary>
        /// Instance of class NullType. (unique)
        /// </summary>
        private static readonly NullType instance = new NullType();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the unique instance of NullType
        /// </summary>
        public static NullType Instance {
            get { return instance; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor of NullType.
        /// </summary>
        private NullType() {
            this.typeExpression = "null";
            this.fullName = "null";
        }

        /// <summary>
        /// Private and static constructor of NullType.
        /// </summary>
        static NullType() {
        }

        #endregion

        // WriteType Inference

        #region Dispatcher
        public override object AcceptOperation(TypeSystemOperation op, object arg) { return op.Exec(this, arg); }
        #endregion

        #region Assignment() ANULADA

        /// <summary>
        ///// Check if the type can make an assignment operation.
        ///// </summary>
        ///// <param name="operand">WriteType expression of the operand of binary expression.</param>
        ///// <param name="op">Operator.</param>
        ///// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        ///// <param name="unification">Indicates if the kind of unification (equivalent, incremental or override).</param>
        ///// <param name="actualImplicitObject">Only suitable when the assignment is executed as a constraint of a method call. In that case,
        ///// this parameter represents the actual object used to pass the message; null otherwise.</param>
        ///// <param name="fileName">File name.</param>
        ///// <param name="line">Line number.</param>
        ///// <param name="column">Column number.</param>
        ///// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Assignment(TypeExpression operand, AssignmentOperator op, MethodType methodAnalyzed, SortOfUnification unification,
        //            TypeExpression actualImplicitObject, Location location) {
        //    if (op == AssignmentOperator.Assign)
        //        return operand;
        //    else
        //        ErrorManager.Instance.NotifyError(new AssignmentError(operand.FullName, this.FullName, location));
        //    return null;
        //}
        #endregion

        #region Arithmetic() ANULADA
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
        } */
        #endregion

        // WriteType Promotion

        #region PromotionLevel() ANULADA

        /// <summary>
        /// Returns a value that indicates a promotion level.
        /// </summary>
        /// <param name="type">WriteType to promotion.</param>
        /// <returns>Returns a promotion value.</returns>
        //public override int PromotionLevel(TypeExpression type) {
        //    // * Built-in types: no promotion, except string
        //    if (type is BoolType || type is CharType || type is DoubleType || type is IntType || type is VoidType)
        //        return -1;
        //    // * BCL Value Types (structs): No promotion
        //    BCLClassType bclClass = TypeExpression.As<BCLClassType>(type);
        //    if (bclClass != null) {
        //        if (bclClass.TypeInfo.IsValueType)
        //            return -1;
        //        // * Correct promotion to classes that are not value types
        //        return 0;
        //    }
        //    // * WriteType variable
        //    TypeVariable typeVariable = type as TypeVariable;
        //    if (typeVariable != null) {
        //        if (typeVariable.Substitution != null)
        //            // * If the variable is bounded, the promotion is the one of its substitution
        //            return this.PromotionLevel(typeVariable.EquivalenceClass.Substitution);
        //        // * A free variable is complete promotion
        //        return 0;
        //    }
        //    // * Union type
        //    UnionType unionType = TypeExpression.As<UnionType>(type);
        //    if (unionType != null)
        //        return unionType.SuperType(this);
        //    // * Field type and bounded type variable
        //    FieldType fieldType = TypeExpression.As<FieldType>(type);
        //    if (fieldType != null)
        //        return this.PromotionLevel(fieldType.FieldTypeExpression);
        //    // * Correct Promotion
        //    return 0;
        //}

        #endregion

        // WriteType Unification

        #region Unify
        /// <summary>
        /// This method unifies two type expressions (this and te)
        /// </summary>
        /// <param name="unification">Indicates if the kind of unification (equivalent, incremental or override).</param>
        /// <param name="te">The expression to be unfied with this</param>
        /// <param name="previouslyUnified">To detect infinite loops. The previously unified pairs of type expressions.</param>
        /// <returns>If the unification was successful</returns>
        public override bool Unify(TypeExpression te, SortOfUnification unification, IList<Pair<TypeExpression, TypeExpression>> previouslyUnified) {

            throw new NotImplementedException("NullType.Unify() Not implemented");
        }
        #endregion

        #region IsValueType()

        /// <summary>
        /// True if type expression is a ValueType. Otherwise, false.
        /// </summary>
        /// <returns>Returns true if the type expression is a ValueType. Otherwise, false.</returns>
        public override bool IsValueType()
        {
           return false;
        }

        #endregion

    }
}
