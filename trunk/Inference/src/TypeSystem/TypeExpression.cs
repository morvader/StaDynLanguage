////////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- //
// Project rROTOR                                                             //
// -------------------------------------------------------------------------- //
// File: TypeExpression.cs                                                    //
// Authors: Cristina Gonzalez Muñoz  -  cristi.gm@gmail.com                   //
//          Francisco Ortin - francisco.ortin@gmail.com                       //
// Description:                                                               //
//    Abstract class that represents all different types.                     //
//    Implements Composite pattern [Component].                               //
// -------------------------------------------------------------------------- //
// Create date: 15-10-2006                                                    //
// Modification date: 05-06-2007                                              //
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using AST;
using ErrorManagement;
using Tools;
using TypeSystem.Operations;

namespace TypeSystem {
    /// <summary>
    /// Abstract class that represents all different types.
    /// </summary>
    /// <remarks>
    /// Implements Composite pattern [Component].
    /// </remarks>
    /// //visto
    public abstract class TypeExpression {
        
        #region Fields

        /// <summary>
        /// Represents the type by a debug string
        /// Note: WriteType expression is the longest recursive representation of a type expression. 
        /// Fullname is the sortest one.
        /// </summary>
        internal protected string typeExpression;

        /// <summary>
        /// Represents the full name of the type
        /// Note: WriteType expression is the longest recursive representation of a type expression. 
        /// Fullname is the sortest one.
        /// </summary>
        internal protected string fullName;

        /// <summary>
        /// To implement a type expression cache
        /// </summary>
        protected bool validTypeExpression = false;

        /// <summary>
        /// The cached value ot the HasTypeVariables method
        /// </summary>
        protected bool hasTypeVariablesCache;

        /// <summary>
        /// To cache the result of the HasTypeVariables method
        /// </summary>
        protected bool validHasTypeVariables = false;

        /// <summary>
        /// In order to avoid stack overflow in the construction of typeexpression
        /// string (ToString), we set a maximum level of depth
        /// </summary>
        public const int MAX_DEPTH_LEVEL_TYPE_EXPRESSION = 1;

        /// <summary>
        /// Indicates if the type has been set as dynamic
        /// </summary>
        protected bool isDynamic;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the full name of the type
        /// Note: WriteType expression is the longest recursive representation of a type expression. 
        /// Fullname is the sortest one.
        /// </summary>
        public virtual string FullName {
            get { return this.fullName; }
            set { this.fullName = value; }
        }

        /// <summary>
        /// To implement a type expression cache
        /// </summary>
        internal virtual bool ValidTypeExpression {
            get { return validTypeExpression; }
            set {
                validTypeExpression = value;
                if (!value) {
                    // * Updates the full name
                    this.BuildFullName();
                    this.typeExpression = this.fullName;
                }
            }
        }

        /// <summary>
        /// Indicates if the type has been set as dynamic
        /// </summary>
        public virtual bool IsDynamic {
            get { return this.isDynamic; }
            set { this.isDynamic = value; }
        }
        #endregion

        // * Constructors

        #region Construtors
        public TypeExpression() {
            this.isDynamic = false;
        }
        public TypeExpression(bool isDynamic) {
            this.isDynamic = isDynamic;
        }
        #endregion

        // * Debug

        #region ToString()

        /// <summary>
        /// Returns the type expression cached in the typeExpression field.
        /// Note: WriteType expression is the longest recursive representation of a type expression. 
        /// Fullname is the sortest one.
        /// </summary>
        /// <returns>string with the type expression associated.</returns>
        public override string ToString() {
            if (!this.ValidTypeExpression) {
                this.typeExpression = this.BuildTypeExpressionString(MAX_DEPTH_LEVEL_TYPE_EXPRESSION);
                this.ValidTypeExpression = true;
            }
            return this.typeExpression;
        }

        #endregion

        #region BuildTypeExpressionString()
        /// <summary>
        /// Returns the type expression 
        /// <param name="depthLevel">The maximum depth of recursion to construct type expressions</param>
        /// </summary>
        public virtual string BuildTypeExpressionString(int depthLevel) {
            return this.typeExpression;
        }
        #endregion

        #region BuildFullName()
        /// <summary>
        /// Creates/Updates the full name of the type expression
        /// </summary>
        public virtual void BuildFullName() { }
        #endregion

        #region Dispatcher
        public virtual object AcceptOperation(TypeSystemOperation op, object arg) { return op.Exec(this, arg); }
        #endregion
        // WriteType Inference

        #region Dot() ANULADA
        ///// <summary>
        ///// Check if the type can make an operation of field access.
        ///// Generates an error if the attribute does not exist.
        ///// Generates a constraint in case it is applied to a free variable. 
        ///// </summary>
        ///// <param name="attribute">Member to access.</param>
        ///// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        ///// <param name="previousDot">To detect infinite loops. The types that have been previously passed the dot message. Used for union types.</param>
        ///// <param name="fileName">File name.</param>
        ///// <param name="line">Line number.</param>
        ///// <param name="column">Column number.</param>
        ///// <returns>WriteType obtained with the operation.</returns>
        //public virtual TypeExpression Dot(string member, MethodType methodAnalyzed, IList<TypeExpression> previousDot, Location loc) {
        //    ErrorManager.Instance.NotifyError(new OperationNotAllowedError(".", this.fullName, loc));
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
        //public virtual TypeExpression Dot(string memberName, IList<TypeExpression> previousDot) {
        //    return null;
        //}
        #endregion

        #region Parenthesis() ANULADA
        /// <summary>
        /// Check if the type can make a method operation.
        /// </summary>
        /// <param name="actualImplicitObject">The actual implicit object employed to pass the message</param>
        /// <param name="arguments">Arguments of the method.</param>
        /// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        /// <param name="activeSortOfUnification">The active sort of unification used (Equivalent is the default
        /// one and Incremental is used in the SSA bodies of the while, for and do statements)</param>
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <returns>WriteType obtained with the operation.</returns>
        //public virtual TypeExpression Parenthesis(TypeExpression actualImplicitObject, TypeExpression[] arguments, MethodType methodAnalyzed, SortOfUnification activeSortOfUnification, Location loc) {
        //    ErrorManager.Instance.NotifyError(new OperationNotAllowedError("()", this.fullName, loc));
        //    return null;
        //}
        #endregion

        #region Bracket() ANULADA
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
        //public virtual TypeExpression Bracket(TypeExpression index, MethodType methodAnalized, bool showErrorMessage, Location loc) {
        //    if (showErrorMessage)
        //        ErrorManager.Instance.NotifyError(new OperationNotAllowedError("[]", this.fullName, loc));
        //    return null;
        //}

        #endregion

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
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <returns>WriteType obtained with the operation.</returns>
        //public virtual TypeExpression Assignment(TypeExpression operand, AssignmentOperator op, MethodType methodAnalyzed, SortOfUnification unification,
        //                TypeExpression actualImplicitObject, Location location) {
        //    ErrorManager.Instance.NotifyError(new AssignmentError(operand.FullName, this.fullName, location));
        //    return null;
        //}
        #endregion

        #region Arithmetic() ANULADA
        /*
         * /// <summary>
        /// Check if the type can make an arithmetic or bitwise operation.
        /// </summary>
        /// <param name="operand">WriteType expression of the operand of binary expression.</param>
        /// <param name="op">Operator.</param>
        /// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        /// <param name="showErrorMessage">Indicates if an error message should be shown (used for dynamic types)</param>
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <returns>WriteType obtained with the operation.</returns>
        public virtual TypeExpression Arithmetic(TypeExpression operand, Enum op, MethodType methodAnalyzed, bool showErrorMessage, Location loc) {
            if (showErrorMessage)
                ErrorManager.Instance.NotifyError(new OperationNotAllowedError(op.ToString(), this.fullName, operand.fullName, loc));
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
        public virtual TypeExpression Arithmetic(UnaryOperator op, MethodType methodAnalyzed, bool showErrorMessage, Location loc) {
            if (showErrorMessage)
                ErrorManager.Instance.NotifyError(new OperationNotAllowedError(op.ToString(), this.fullName, loc));
            return null;
        }
         */
        #endregion

        #region Relational() ANULADA
        /*       /// <summary>
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
        public virtual TypeExpression Relational(TypeExpression operand, RelationalOperator op, MethodType methodAnalyzed, bool showErrorMessage, Location loc) {
            if (showErrorMessage)
                ErrorManager.Instance.NotifyError(new OperationNotAllowedError(op.ToString(), this.fullName, operand.fullName, loc));
            return null;
        }
        
        */
        #endregion

        #region Equivalent() ANULADA
        ///// <summary>
        ///// WriteType equivalence. Tells us if two types are the same (equals is used for object identity)
        ///// </summary>
        ///// <param name="type">The other type</param>
        ///// <returns>True if the represent the same type</returns>
        //public virtual bool Equivalent(TypeExpression type) {
        //    if (this == type)
        //        return true;
        //    TypeVariable typeVariable = type as TypeVariable;
        //    if (typeVariable != null)
        //        return typeVariable.Equivalent(this);
        //    return this.fullName.Equals(type.fullName);
        //}
        #endregion

        #region AsClassType()
        /// <summary>
        /// Represent a type as a class. It is mainly used to obtain the BCL representation of types
        /// (string=String, int=Int32, []=Array...)
        /// </summary>
        /// <returns>The class type is there is a map, null otherwise</returns>
        public virtual ClassType AsClassType() {
            return null;
        }

        #endregion

        // WriteType Promotion

        #region PromotionLevel() ANULADA
        //public virtual int PromotionLevel(TypeExpression type) {
        //    return -1;
        //}
        #endregion

        #region Promotion() ANULADA
        /// <summary>
        /// Requires the implicit object to be a subtype of the type parameter
        /// </summary>
        /// <param name="type">WriteType to promotion.</param>
        /// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        /// <param name="op">An optional operator to report error messages.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <returns>The supertype; null if there has been some error.</returns>
        //public virtual TypeExpression Promotion(TypeExpression type, MethodType methodAnalyzed, Location location) {
        //    if ((int) this.AcceptOperation(new PromotionLevelOperation(type)) == -1) {
        //        ErrorManager.Instance.NotifyError(new TypePromotionError(this.FullName, type.FullName, location));
        //        return null;
        //    }
        //    return type;
        //}
        //public virtual TypeExpression Promotion(TypeExpression type, Enum op, MethodType methodAnalyzed, Location location) {
        //    if ((int)this.AcceptOperation( new PromotionLevelOperation(type)) == -1) {
        //        ErrorManager.Instance.NotifyError(new TypePromotionError(this.FullName, type.FullName, op.ToString(), location));
        //        return null;
        //    }
        //    return type;
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
        /// <returns>The returned type expression</returns>
        //public virtual TypeExpression Cast(TypeExpression castType, MethodType methodAnalyzed, Location loc) {
        //    if (castType == null)
        //        return null;
        //    if (((int)castType.AcceptOperation(new PromotionLevelOperation(this)) != -1) || ((int)this.AcceptOperation(new PromotionLevelOperation(castType)) != -1))
        //        return castType;
        //    ErrorManager.Instance.NotifyError(new TypeCastError(this.FullName, castType.FullName, loc));
        //    return null;
        //}
        #endregion

        #region EqualsForOverload() ANULADA
        /// <summary>
        /// Used to not repeat methods in overload
        /// </summary>
        /// <param name="typeExpression">The other type expression</param>
        /// <returns>If both represent the same type</returns>
        //public virtual bool EqualsForOverload(object typeExpression) {
        //    // * By default, we use the equals comparison
        //    return this.Equals(typeExpression);
        //}
        #endregion

        // WriteType Unification

        #region Unify()
        /// <summary>
        /// Tries to unify the type expression of this and the parameter
        /// </summary>
        /// <param name="te">The type expression to be unified together with this</param>
        /// <param name="unification">Indicates if the kind of unification (equivalent, incremental or override).</param>
        /// <param name="previouslyUnified">To detect infinite loops. The previously unified pairs of type expressions.</param>
        /// <returns>If both type expressionas could be unfied</returns>
        public abstract bool Unify(TypeExpression te, SortOfUnification unification, IList<Pair<TypeExpression, TypeExpression>> previouslyUnified);
        #endregion

        #region HasTypeVariables()
        /// <summary>
        /// To know if the type expression has some type variables and requieres unification
        /// The default implementation is return false
        /// </summary>
        /// <returns>If it has any type variable</returns>
        public virtual bool HasTypeVariables() {
            return false;
        }
        #endregion

        #region IsFreshVariable()
        /// <summary>
        /// To know if it is a type variable with no substitution
        /// </summary>
        /// <returns>True in case it is a fresh variable with no substitution</returns>
        public virtual bool IsFreshVariable() {
            return false;
        }
        #endregion

        #region HasFreshVariable()
        /// <summary>
        /// To know if it is a type variable with no substitution
        /// </summary>
        /// <returns>True in case it is a fresh variable with no substitution. This method has a different behaviour than IsFreshVariable with UnionTypes, the rest of types have the same.</returns>
        public virtual bool HasFreshVariable() {
            return IsFreshVariable();
        }
        #endregion

        #region HasIntersectionVariable()
        /// <summary>
        /// To know if it is a type variable with no substitution
        /// </summary>
        /// <returns>True in case it is a fresh variable with no substitution. This method has a different behaviour than IsFreshVariable with UnionTypes, the rest of types have the same.</returns>
        public virtual bool HasIntersectionVariable() {
            return false;
        }
        #endregion


        #region CloneType()
        /// <summary>
        /// This method creates a new type, creating new type variables for
        /// type expression. It these type variables where bounded to types or other
        /// type variables, they are maintained.
        /// </summary>
        /// <param name="typeVariableMappings">Each new type varaiable represent a copy of another existing one.
        /// This parameter is a mapping between them, wher tmpName=old and value=new.</param>
        /// <returns>The new cloned class type</returns>
        public virtual TypeExpression CloneType(IDictionary<TypeVariable, TypeVariable> typeVariableMappings) {
            // * By default, no clone is performed (built-in types)
            return this;
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
        public virtual TypeExpression CloneTypeVariables(IDictionary<TypeVariable, TypeVariable> typeVariableMappings, IList<EquivalenceClass> equivalenceClasses, IList<ClassType> clonedClasses) {
            return this;
        }
        #endregion

        #region UpdateEquivalenceClass()
        /// <summary>
        /// Replaces the equivalence class of type variables substituting the old type variables for the new ones.
        /// </summary>
        /// <param name="typeVariableMappings">Each new type varaiable represent a copy of another existing one.
        /// This parameter is a mapping between them, wher tmpName=old and value=new.</param>
        /// <param name="previouslyUpdated">To detect infinite loops. Previously updated type expressions.</param>
        public virtual void UpdateEquivalenceClass(IDictionary<TypeVariable, TypeVariable> typeVariableMappings, IList<TypeExpression> previouslyUpdated) {
            // * Does nothing (built-in types are not recursive)
        }
        #endregion

        #region ReplaceTypeVariables()
        /// <summary>
        /// Replaces type variables substituting the old type variables for the new ones.
        /// </summary>
        /// <param name="typeVariableMappings">Each new type varaiable represent a copy of another existing one.
        /// This parameter is a mapping between them, wher tmpName=old and value=new.</param>
        public virtual void ReplaceTypeVariables(IDictionary<TypeVariable, TypeVariable> typeVariableMappings) {
            // * Nothing to to (built-in types)
        }
        #endregion

        #region Freeze()
        /// <summary>
        /// WriteType variable may change its type's substitution (e.g., field type variables)
        /// This method returns the type in an specific time (frozen).
        /// If this type's substitution changes, the frozen type does not.
        /// <returns>The frozen type</returns>
        /// </summary>
        public virtual TypeExpression Freeze() {
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
        internal virtual TypeExpression Clone(IDictionary<int, TypeVariable> clonedTypeVariables, IList<EquivalenceClass> equivalenceClasses, MethodType methodAnalyzed) {
            if (this.HasTypeVariables())
                throw new InvalidOperationException("The type should implement a Clone method.");
            // * Default implementation (types with no type variables)
            return this;
        }
        #endregion

        // Loop Detection

        #region Remove()
        /// <summary>
        /// When loops are detected, it is necesary to suppress a new extra variable returned in 
        /// the return type of recursive functions
        /// </summary>
        /// <param name="toRemove">The type variable to remove</param>
        /// <returns>If it has been actually removed</returns>
        public virtual bool Remove(TypeVariable toRemove) {
            return false;
        }
        #endregion

        // Code Generation

        #region ILType()
        /// <summary>
        /// Gets the type name to use in IL code.
        /// </summary>
        /// <returns>Returns the type name to use in IL code.</returns>
        public virtual string ILType() {
            return this.fullName;
        }

        #endregion

        #region IsValueType()
        /// <summary>
        /// True if type expression is a ValueType. Otherwise, false.
        /// </summary>
        /// <returns>Returns true if the type expression is a ValueType. Otherwise, false.</returns>
        public abstract bool IsValueType();
        #endregion

        // Helper Methods

        #region As<T>()
        /// <summary>
        /// Returns a concrete type expression from a general one. It takes into accout that it can
        /// be a type variable and, if so, it does the same with its substitution.
        /// </summary>
        /// <typeparam name="T">A concrete type expression</typeparam>
        /// <param name="type">The general type expression</param>
        /// <returns>The cast type</returns>
        public static T As<T>(TypeExpression type) where T : TypeExpression {
            TypeVariable typeVariable = type as TypeVariable;
            if (typeVariable != null)
                type = typeVariable.Substitution;
            T castType = type as T;
            return castType;
        }
        #endregion
        
        #region Is<T>()
        /// <summary>
        /// Tells if a type expression is a type or a type variable
        /// unified to a type
        /// </summary>
        /// <typeparam name="T">A concrete type expression</typeparam>
        /// <param name="type">The general type expression</param>
        /// <returns>If the type is the expected one</returns>
        public static bool Is<T>(TypeExpression type) where T : TypeExpression {
            return TypeExpression.As<T>(type) != null;
        }
        #endregion

        virtual public BCLClassType getBCLType() {
            System.Diagnostics.Debug.Assert(true, "getBCLType called in type expression inconsistence in the program");
            return null;
        }

    }
}
