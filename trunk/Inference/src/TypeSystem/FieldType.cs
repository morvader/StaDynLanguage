////////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- //
// Project rROTOR                                                             //
// -------------------------------------------------------------------------- //
// File: FieldType.cs                                                         //
// Authors: Cristina Gonzalez Mu�oz  -  cristi.gm@gmail.com                   //
//          Francisco Ortin - francisco.ortin@gmail.com                       //
// Description:                                                               //
//    Represents a field type.                                                //
//    Inheritance: MemberType.                                                //
//    Implements Composite pattern [Composite].                               //
// -------------------------------------------------------------------------- //
// Create date: 27-01-2007                                                    //
// Modification date: 11-06-2007                                              //
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using AST;
using ErrorManagement;
using TypeSystem.Constraints;
using Tools;
using DynVarManagement;
using TypeSystem.Operations;
//visto
namespace TypeSystem {
    /// <summary>
    /// Representa a field type.
    /// </summary>
    /// <remarks>
    /// Inheritance: MemberType.
    /// Implements Composite pattern [Composite].
    /// </remarks>
    public class FieldType : TypeExpression, IMemberType {
        #region Fields

        /// <summary>
        /// Represents the field type expression.
        /// </summary>
        private TypeExpression fieldType;

        /// <summary>
        /// Links to attribute information (modifiers and its class type)
        /// </summary>
        private AccessModifier memberInfo;


        #endregion

        #region Properties

        /// <summary>
        /// Gets the field type.
        /// </summary>
        public TypeExpression FieldTypeExpression {
            get { return this.fieldType; }
        }

        /// <summary>
        /// Gets or sets the attribute information of method type
        /// </summary>
        public AccessModifier MemberInfo {
            get { return this.memberInfo; }
            set {
                if (this.memberInfo == null) {
                    this.memberInfo = value;
                }
                else
                    ErrorManager.Instance.NotifyError(new ClassTypeInfoError(this.memberInfo.MemberIdentifier, this.memberInfo.Class.FullName, value.Class.FullName));
            }
        }

        /// <summary>
        /// To implement a type expression cache
        /// </summary>
        internal override bool ValidTypeExpression {
            set {
                validTypeExpression = value;
                if (!value) {
                    // * Updates the full name
                    this.BuildFullName();
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of FieldType
        /// </summary>
        public FieldType(TypeExpression type) {
            if (type != null) {
                this.fieldType = type;
                this.fullName = type.fullName;
            }
        }

        #endregion

        #region BuildTypeExpressionString

        /// <summary>
        /// Creates the type expression string.
        /// </summary>
        public override string BuildTypeExpressionString(int depthLevel) {
            if (this.ValidTypeExpression) return this.typeExpression;
            if (depthLevel <= 0)
                return this.FullName;

            //this.fullName = this.MemberInfo.Class.FullName + "." + this.MemberInfo.MemberIdentifier;
            if (this.fieldType == null)
                return "null";
            this.fullName = this.fieldType.FullName;

            StringBuilder tE = new StringBuilder();
            // tE: Field(IdClass, IdField, mods, type)
            tE.AppendFormat("Field({0}, {1},", this.MemberInfo.Class.BuildTypeExpressionString(depthLevel - 1), this.MemberInfo.MemberIdentifier);
            // modifiers
            if (this.MemberInfo.Modifiers.Count != 0) {
                for (int i = 0; i < this.MemberInfo.Modifiers.Count - 1; i++) {
                    tE.AppendFormat(" {0} x", this.MemberInfo.Modifiers[i]);
                }
                tE.AppendFormat(" {0}", this.MemberInfo.Modifiers[this.MemberInfo.Modifiers.Count - 1]);
            }
            tE.Append(", ");

            // type
            tE.Append(this.fieldType.BuildTypeExpressionString(depthLevel - 1));
            tE.Append(")");
            this.ValidTypeExpression = true;
            return this.typeExpression = tE.ToString();
        }
        #endregion

        #region BuildFullName()
        /// <summary>
        /// Creates/Updates the full name of the type expression
        /// </summary>
        public override void BuildFullName() {
            this.fieldType.BuildFullName();
            this.fullName = this.fieldType.fullName;
        }
        #endregion

        // WriteType inference

        #region Dispatcher
        public override object AcceptOperation(TypeSystemOperation op, object arg) { return op.Exec(this, arg); }
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
        //    if (this.fieldType != null) {
        //        // * If the field type is a dynamic union type, so it is the union
        //        UnionType unionType = As<UnionType>(this.fieldType);
        //        if (unionType != null)
        //            DynVarOptions.Instance.AssignDynamism(this.fieldType, this.IsDynamic);
        //        return this.fieldType.Dot(field, methodAnalyzed, previousDot, loc);
        //    }
        //    return null;
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
        //    if (this.fieldType != null)
        //        return this.fieldType.Dot(memberName, previousDot);
        //    return null;
        //}
        #endregion

        //#region Bracket() anulada

        ///// <summary>
        ///// Check if the type can make an array operation.
        ///// </summary>
        ///// <param name="index">TypeExpression of the index.</param>
        ///// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        ///// <param name="showErrorMessage">Indicates if an error message should be shown (used for dynamic types)</param>
        ///// <param name="fileName">File name.</param>
        ///// <param name="line">Line number.</param>
        ///// <param name="column">Column number.</param>
        ///// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Bracket(TypeExpression index, MethodType methodAnalyzed, bool showErrorMessage, Location loc) {
        //    if (this.fieldType != null)
        //        return this.fieldType.Bracket(index, methodAnalyzed, showErrorMessage, loc);
        //    return null;
        //}

        //#endregion

        #region Assignment() ANULADA
        /// <summary>
        /// Check if the type can make an assignment operation.
        /// </summary>
        /// <param name="operand">WriteType expression of the operand of binary expression.</param>
        /// <param name="op">Operator.</param>
        /// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        /// <param name="unification">Indicates if the kind of unification (equivalent, incremental or override).</param>
        /// <param name="actualImplicitObject">Only suitable when the assignment is executed as a constraint of a method call. In that case,
        /// this parameter represents the actual object used to pass the message; null otherwise.</param>
        /// <param name="location"> Location of the element</param>
        /// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Assignment(TypeExpression operand, AssignmentOperator op, MethodType methodAnalyzed, SortOfUnification unification,
        //            TypeExpression actualImplicitObject, Location location) {
        //    // * We check if a constraint must be generated. Is it an assignment of the implicit object's field?
        //    bool found = false;
        //    // * In case it has free variables and the reference used is this, we add a constraint to the method
        //    if (this.HasTypeVariables() && methodAnalyzed != null && ClassType.IsConcreteType(actualImplicitObject) == null) {
        //        // * They should be the same exact (sub)classes. This represent the same instance, not another instance of the same class.
        //        ClassType methodSuperClass = (ClassType)methodAnalyzed.MemberInfo.Class;
        //        while (!(found = (this.MemberInfo.Class == methodSuperClass)) && methodSuperClass != null)
        //            methodSuperClass = methodSuperClass.BaseClass;
        //        if (found) {
        //            // * An assignment constraint is added, postponing the type inference
        //            // * If an actual implicit object is used, we take its field's type
        //            FieldType fieldType = this;
        //            ClassType thisType = TypeExpression.As<ClassType>(actualImplicitObject);
        //            if (thisType == null) {
        //                FieldType field = TypeExpression.As<FieldType>(actualImplicitObject);
        //                if (field != null)
        //                    thisType = TypeExpression.As<ClassType>(field.FieldTypeExpression);
        //            }
        //            if (thisType != null)
        //                fieldType = (FieldType)thisType.Fields[this.MemberInfo.MemberIdentifier].WriteType;
        //            methodAnalyzed.AddConstraint(new FieldTypeAssignmentConstraint(fieldType, operand, unification));
        //            methodAnalyzed.ValidTypeExpression = false;
        //            return this.fieldType;
        //        }
        //    }
        //    if (!found && this.fieldType != null)
        //        return this.fieldType.Assignment(operand, op, null, unification, actualImplicitObject, location);
        //    return null;
        //}

        #endregion

       #region Arithmetic() >NUL>C>
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
            if (this.fieldType != null)
                return this.fieldType.Arithmetic(operand, op, methodAnalyzed, showErrorMessage, loc);
            return null;
        }

        /// <summary>
        /// Check if the type can make an arithmetic operation.
        /// </summary>
        /// <param name="op">Operator.</param>
        /// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        /// <param name="showErrorMessage">Indicates if an error message should be shown (used for dynamic types)</param>
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <returns>WriteType obtained with the operation.</returns>
        public override TypeExpression Arithmetic(UnaryOperator op, MethodType methodAnalyzed, bool showErrorMessage, Location loc) {
            return this.fieldType.Arithmetic(op, methodAnalyzed, showErrorMessage, loc);
        }
        */
       #endregion

        #region Relational()ANULADA
        /*      /// <summary>
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
            if (this.fieldType != null)
                return this.fieldType.Relational(operand, op, methodAnalyzed, showErrorMessage, loc);
            return null;
        }

        */
        #endregion

        #region PromotionLevl()-->#        /// <summary>
        /// Returns a value that indicates a promotion level.
        /// </summary>
        /// <param name="type">WriteType to promotion.</param>
        /// <returns>Returns a promotion value.</returns>
        //public override int PromotionLevel(TypeExpression type) {
        //    if (this.fieldType != null)
        //        return this.fieldType.PromotionLevel(type);
        //    return -1;
        //}
        /// <summary>
#endregion
        #region Promotion ANULADA
        /// Requires the implicit object to be a subtype of the type parameter
        /// </summary>
        /// <param name="type">WriteType to promotion.</param>
        /// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        /// <param name="op">An optional operator to report error messages.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        ///// <returns>The supertype; null if there has been some error.</returns>
        //public override TypeExpression Promotion(TypeExpression type, Enum op, MethodType methodAnalyzed, Location loc) {
        //    if (this.fieldType != null)
        //        return this.fieldType.Promotion(type, op, methodAnalyzed, loc);
        //    return null;
        //}
        #endregion

        #region Cast() ANULADA
        /// <summary>
        /// Tells if the type can be cast to the casttype
        /// </summary>
        /// <param name="castType">The expected type</param>
        /// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        ///// <returns>The returned type expression</returns>
        //public override TypeExpression Cast(TypeExpression castType, MethodType methodAnalyzed, Location loc) {
        //    if (this.fieldType != null)
        //        return this.fieldType.Cast(castType, methodAnalyzed, loc);
        //    return null;
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
            FieldType ft = te as FieldType;
            if (ft != null) {
                bool success = this.fieldType.Unify(ft.fieldType, unification, previouslyUnified);
                if (success) // * Dynamic type
                    DynVarOptions.Instance.AssignDynamism(this, ft.isDynamic);
                // * Clears the type expression cache
                this.ValidTypeExpression = false;
                te.ValidTypeExpression = false;
                return success;
            }
            if (te is TypeVariable && unification != SortOfUnification.Incremental)
                // * No incremental unification is commutative
                return te.Unify(this, unification, previouslyUnified);
            return false;
        }
        #endregion

        #region HasTypeVariables()
        /// <summary>
        /// To know if the type expression has some type variables and requieres unification
        /// The default implementation is return false
        /// </summary>
        /// <returns>If it has any type variable</returns>
        public override bool HasTypeVariables() {
            if (this.validHasTypeVariables)
                return this.hasTypeVariablesCache;
            bool toReturn = this.fieldType.HasTypeVariables();
            this.validHasTypeVariables = true;
            return this.hasTypeVariablesCache = toReturn;

        }
        #endregion

        #region CloneTypeVariables()
        /// <summary>
        /// Method that clones each type variable of a type expression.
        /// Equivalence classes are not cloned (but included in the equivalenceClasses parameter.
        /// The default implementation is do nothing (for built-in types).
        /// </summary>
        /// <param name="typeVariableMappings">Each new type varaiable represent a copy of another existing one.
        /// This parameter is a mapping between them, wher tmpName=old and value=new</param>
        /// <param name="equivalenceClasses">Each equivalence class of all the type variables.</param>
        /// <param name="clonedClasses">This parameter collects the set of all cloned classes. It is used to detect infinite recursion.</param>
        /// <returns>The new type expression (itself by default)</returns>
        public override TypeExpression CloneTypeVariables(IDictionary<TypeVariable, TypeVariable> typeVariableMappings, IList<EquivalenceClass> equivalenceClasses, IList<ClassType> clonedClasses) {
            if (!this.HasTypeVariables())
                return this;
            AccessModifier accesModifier = (AccessModifier)this.MemberInfo.Clone();
            FieldType fieldType = new FieldType(this.FieldTypeExpression.CloneTypeVariables(typeVariableMappings, equivalenceClasses, clonedClasses));
            accesModifier.Class = this.MemberInfo.Class;
            accesModifier.Type = fieldType;
            fieldType.IsDynamic = this.IsDynamic;
            fieldType.MemberInfo = accesModifier;
            this.ValidTypeExpression = fieldType.ValidTypeExpression = false;
            return fieldType;
        }
        #endregion

        #region UpdateEquivalenceClass()
        /// <summary>
        /// Replaces the equivalence class of type variables substituting the old type variables for the new ones.
        /// </summary>
        /// <param name="typeVariableMappings">Each new type varaiable represent a copy of another existing one.
        /// This parameter is a mapping between them, wher tmpName=old and value=new.</param>
        /// <param name="previouslyUpdated">To detect infinite loops. Previously updated type expressions.</param>
        public override void UpdateEquivalenceClass(IDictionary<TypeVariable, TypeVariable> typeVariableMappings, IList<TypeExpression> previouslyUpdated) {
            // * Checks infinite loops
            if (previouslyUpdated.Contains(this))
                return;
            previouslyUpdated.Add(this);

            // * Updates the equivalence class
            if (!this.HasTypeVariables())
                return;
            this.fieldType.UpdateEquivalenceClass(typeVariableMappings, previouslyUpdated);
            this.ValidTypeExpression = false;
        }
        #endregion

        #region ReplaceTypeVariables()
        /// <summary>
        /// Replaces type variables substituting the old type variables for the new ones.
        /// </summary>
        /// <param name="typeVariableMappings">Each new type varaiable represent a copy of another existing one.
        /// This parameter is a mapping between them, wher tmpName=old and value=new.</param>
        public override void ReplaceTypeVariables(IDictionary<TypeVariable, TypeVariable> typeVariableMappings) {
            TypeVariable fieldTypeVariable = this.fieldType as TypeVariable;
            if (fieldTypeVariable == null) {
                if (this.fieldType.HasTypeVariables())
                    this.fieldType.ReplaceTypeVariables(typeVariableMappings);
                return;
            }
            if (typeVariableMappings.ContainsKey(fieldTypeVariable))
                this.fieldType = typeVariableMappings[fieldTypeVariable];
            this.ValidTypeExpression = false;
        }
        #endregion

        #region Equivalent ANULADA
        /// <summary>
        /// WriteType equivalence. Tells us if two types are the same 
        /// </summary>
        /// <param name="type">The other type</param>
        /// <returns>True if the represent the same type</returns>
        //public override bool Equivalent(TypeExpression type) {
        //    FieldType fieldType = type as FieldType;
        //    if (fieldType != null)
        //        return this.fieldType.Equivalent(fieldType.fieldType);
        //    return this.fieldType.Equivalent(type);
        //}

        #endregion
        #region Freeze()
        /// <summary>
        /// WriteType variable may change its type's substitution (e.g., field type variables)
        /// This method returns the type in an specific time (frozen).
        /// If this type's substitution changes, the frozen type does not.
        /// <returns>The frozen type</returns>
        /// </summary>
        public override TypeExpression Freeze() {
            if (this.FieldTypeExpression is TypeVariable && !this.FieldTypeExpression.IsFreshVariable()) {
                FieldType newFieldType = (FieldType)this.MemberwiseClone();
                newFieldType.fieldType = this.FieldTypeExpression.Freeze();
                newFieldType.ValidTypeExpression = false;
                return newFieldType;
            }
            return this;
        }
        #endregion

        // SSA

        #region Clone()
        /// <summary>
        /// Clones a type to be used in SSA. It must taken into account that:
        /// - In case it has no type variables, no clone is performed
        /// - WriteType variables, equivalence classes and substitutions are cloned
        /// </summary>
        /// <param name="clonedTypeVariables">WriteType variables that have been cloned.</param>
        /// <param name="equivalenceClasses">Equivalence classes of the type cloned variables. These
        /// equivalence classes need to be updated with the new cloned type variables.</param>
        /// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        /// <returns>The cloned type</returns>
        internal override TypeExpression Clone(IDictionary<int, TypeVariable> clonedTypeVariables, IList<EquivalenceClass> equivalenceClasses, MethodType methodAnalyzed) {
            if (!this.HasTypeVariables())
                return this;
            FieldType newFieldType = (FieldType)this.MemberwiseClone();
            if (newFieldType.fieldType != null)
                newFieldType.fieldType = newFieldType.fieldType.Clone(clonedTypeVariables, equivalenceClasses, methodAnalyzed);
            newFieldType.ValidTypeExpression = false;
            return newFieldType;
        }
        #endregion

        // Code Generation

        #region ILType()

        /// <summary>
        /// Gets the string type to use in IL code.
        /// </summary>
        /// <returns>Returns the string type to use in IL code.</returns>
        public override string ILType() {
            return this.fieldType.ILType();
        }

        #endregion

        #region IsValueType()

        /// <summary>
        /// True if type expression is a ValueType. Otherwise, false.
        /// </summary>
        /// <returns>Returns true if the type expression is a ValueType. Otherwise, false.</returns>
        public override bool IsValueType() {
            return this.fieldType.IsValueType();
        }

        #endregion


    }
}
