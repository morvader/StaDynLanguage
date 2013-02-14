////////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- //
// Project rROTOR                                                             //
// -------------------------------------------------------------------------- //
// File: InterfaceType.cs                                                     //
// Authors: Cristina Gonzalez Muñoz  -  cristi.gm@gmail.com                   //
//          Francisco Ortin - francisco.ortin@gmail.com                       //
// Description:                                                               //
//    Represents a interface type.                                            //
//    Inheritance: UserType.                                                  //
//    Implements Composite pattern [Composite].                               //
//    Implements Template Method pattern.                                     //
// -------------------------------------------------------------------------- //
// Create date: 22-10-2006                                                    //
// Modification date: 09-03-2007                                              //
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using ErrorManagement;
using AST;
using Tools;
using TypeSystem.Operations;

namespace TypeSystem {
    /// <summary>
    /// Represents a interface type.
    /// </summary>
    /// <remarks>
    /// Inheritance: UserType.
    /// Implements Composite pattern [Composite].
    /// </remarks>
    //visto
    /// 
    public class InterfaceType : UserType {
        #region Constructor

        /// <summary>
        /// Constructor of InterfaceType
        /// </summary>
        /// <param name="identifier">Class identifier.</param>
        /// <param name="fullName">Class full identifier.</param>
        /// <param name="modifiers">Modifiers of the class type</param>
        public InterfaceType(string identifier, string fullName, List<Modifier> modifiers)
            : base(fullName) {
            this.name = identifier;
            this.Modifiers = modifiers;
        }

        /// <summary>
        /// Constructor of InterfaceType
        /// </summary>
        /// <param name="name">Class identifier.</param>
        public InterfaceType(string name)
            : base(name) {
            this.name = name;
            this.Modifiers = new List<Modifier>();
        }
        #endregion
        
        #region AddBaseClass()
        /// <summary>
        /// Adds a new inherited type
        /// </summary>
        /// <param name="inheritedClass">Information about inherited type.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        public override void AddBaseClass(ClassType inheritedClass, Location location) {
            System.Diagnostics.Debug.Assert(false, "A base class cannot be added to an interface.");
        }
        #endregion
        
        #region BuiltTypeExpression()

        /// <summary>
        /// Creates the type expression string.
        /// </summary>
        public override string BuildTypeExpressionString(int depthLevel) {
            if (this.ValidTypeExpression) return this.typeExpression;
            if (depthLevel <= 0) return this.FullName;

            StringBuilder tE = new StringBuilder();
            // tE: Interface(id, modifiers, interfaces, members)
            tE.AppendFormat("Interface({0},", this.fullName);
            // modifiers
            if (this.modifierList.Count != 0) {
                for (int i = 0; i < this.modifierList.Count - 1; i++) {
                    tE.AppendFormat(" {0} x", this.modifierList[i]);
                }
                tE.AppendFormat(" {0}", this.modifierList[this.modifierList.Count - 1]);
            }
            tE.Append(", ");

            // interfaces
            if (this.interfaceList.Count != 0) {
                for (int i = 0; i < this.interfaceList.Count - 1; i++) {
                    tE.AppendFormat(" {0} x", this.interfaceList[i].FullName);
                }
                tE.AppendFormat(" {0}", this.interfaceList[this.interfaceList.Count - 1].FullName);
            }
            tE.Append(", ");

            // members
            if (this.Members.Count != 0) {
                Dictionary<string, AccessModifier>.KeyCollection keys = this.Members.Keys;
                int i = 0;
                foreach (string key in keys) {
                    tE.Append(this.Members[key].Type.BuildTypeExpressionString(depthLevel - 1));
                    if (i < keys.Count - 1)
                        tE.Append(" x");
                    i++;
                }
            }
            tE.Append(")");
            this.ValidTypeExpression = true;
            return this.typeExpression = tE.ToString();
        }

        #endregion

        // WriteType Inference
               #region Dispatcher
        public override object AcceptOperation(TypeSystemOperation op, object arg) { return op.Exec(this, arg); }
        #endregion

        #region Dot() ANULADA
        ///// <summary>
        ///// Template method (subclasses only need to override the dot method with one parameter)
        ///// Check if the type can make an operation of field access.
        ///// Generates an error if the attribute does not exist.
        ///// Generates a constraint in case it is applied to a free variable. 
        ///// </summary>
        ///// <param name="memberName">The name of the attribute</param>
        ///// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        ///// <param name="fileName">input file</param>
        ///// <param name="previousDot">To detect infinite loops. The types that have been previously passed the dot message. Used for union types.</param>
        ///// <param name="line">source code line</param>
        ///// <param name="column">source code column</param>
        ///// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Dot(string memberName, MethodType methodAnalyzed, IList<TypeExpression> previousDot, Location loc) {
        //    // Try to find the appropriate attribute
        //    TypeExpression member = this.Dot(memberName, previousDot);
        //    if (member == null)
        //        // * Otherwise, error
        //        ErrorManager.Instance.NotifyError(new UnknownMemberError(memberName, loc));
        //    return member;
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
        //    if (this.Members.ContainsKey(memberName))
        //        return this.Members[memberName].WriteType;
        //    foreach (InterfaceType interfaze in this.interfaceList) {
        //        // * Does this interface support this attribute?
        //        TypeExpression member = interfaze.Dot(memberName, previousDot);
        //        if (member != null)
        //            return member;
        //    }
        //    // * not found
        //    return null;
        //}
        #endregion

        #region Assignment  = ... ANULADA
        /// <summary>
        /// Check if the type can make an assignment operation.
        /// </summary>
        /// <param name="operand">WriteType expression of the operand of binary expression.</param>
        /// <param name="op">Operator.</param>
        /// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        /// <param name="unification">Indicates if the kind of unification (equivalent, incremental or override).</param>
        /// <param name="actualImplicitObject">Only suitable when the assignment is executed as a constraint of a method call. In that case,
        /// this parameter represents the actual object used to pass the message; null otherwise.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Assignment(TypeExpression operand, AssignmentOperator op, MethodType methodAnalyzed, SortOfUnification unification,
        //            TypeExpression actualImplicitObject, Location location) {
        //    if (op == AssignmentOperator.Assign)
        //        return operand.Promotion(this, op, methodAnalyzed, location);
        //    ErrorManager.Instance.NotifyError(new AssignmentError(operand.FullName, this.FullName, location));
        //    return null;
        //}
        #endregion

        // WriteType Promotion

        #region PromotionLevel() ANULADA

        /// <summary>
        /// Returns a value that indicates a promotion level.
        /// </summary>
        /// <param name="type">WriteType to promotion.</param>
        /// <returns>Returns a promotion value.</returns>
            //public override int PromotionLevel(TypeExpression type) {
            //    int aux, less = -1;

            //    // * Equivalent types
            //    if ((bool)this.AcceptOperation(new EquivalentOperation((type))))
            //        less = 0;

            //    // * WriteType variable
            //    TypeVariable typeVariable = type as TypeVariable;
            //    if (typeVariable != null) {
            //        if (typeVariable.Substitution != null)
            //            // * If the variable is bounded, the promotion is the one of its substitution
            //            return this.PromotionLevel(typeVariable.EquivalenceClass.Substitution);
            //        // * A free variable is complete promotion
            //        return 0;
            //    }

            //    // * Field type and bounded type variable
            //    FieldType fieldType = TypeExpression.As<FieldType>(type);
            //    if (fieldType != null)
            //        return this.PromotionLevel(fieldType.FieldTypeExpression);

            //    // * Interface List
            //    if (this.interfaceList.Count != 0) {
            //        for (int i = 0; i < this.interfaceList.Count; i++) {
            //            if ((bool)this.interfaceList[i].AcceptOperation(new EquivalentOperation(type))) {
            //                if ((less > 1) || (less == -1))
            //                    less = 1;
            //            }
            //            else {
            //                aux = this.interfaceList[i].PromotionLevel(type);
            //                if (aux != -1) {
            //                    if ((less > (aux + 1)) || (less == -1))
            //                        less = aux + 1;
            //                }
            //            }
            //        }
            //    }
            //    if (less != -1)
            //        return less;

            //    // * Union type
            //    UnionType unionType = TypeExpression.As<UnionType>(type);
            //    if (unionType != null)
            //        return unionType.SuperType(this);

            //    // * No promotion
            //    return -1;
            //}

        #endregion

        // WriteType Unification

        #region ILType()

        /// <summary>
        /// Gets the string type to use in IL code.
        /// </summary>
        /// <returns>Returns the string type to use in IL code.</returns>
        public override string ILType() {
            StringBuilder aux = new StringBuilder();
            aux.AppendFormat("class {0}", this.fullName);
            return aux.ToString();
        }

        #endregion















        #region Unify
        /// <summary>
        /// This method unifies two type expressions (this and te)
        /// </summary>
        /// <param name="te">The expression to be unfied with this</param>
        /// <param name="unification">Indicates if the kind of unification (equivalent, incremental or override).</param>
        /// <param name="previouslyUnified">To detect infinite loops. The previously unified pairs of type expressions.</param>
        /// <returns>If the unification was successful</returns>
        public override bool Unify(TypeExpression te, SortOfUnification unification, IList<Pair<TypeExpression, TypeExpression>> previouslyUnified) {
            InterfaceType it = TypeExpression.As<InterfaceType>(te);
            if (it != null) {
                bool success = (bool)this.AcceptOperation(new EquivalentOperation(it), null);
                // * Clears the type expression cache
                this.ValidTypeExpression = false;
                te.ValidTypeExpression = false;
                return success;
            }
            if (te is TypeVariable && unification!=SortOfUnification.Incremental)
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
           return false;
        }

        #endregion

    }
}
