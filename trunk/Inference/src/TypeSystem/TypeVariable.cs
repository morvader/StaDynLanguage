////////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- //
// Project rROTOR                                                             //
// -------------------------------------------------------------------------- //
// File: TypeVariable.cs                                                      //
// Authors: Cristina Gonzalez Muñoz  -  cristi.gm@gmail.com                   //
//          Francisco Ortin - francisco.ortin@gmail.com                       //
// Description:                                                               //
//    Represents a generic type expression.                                   //
//    Inheritance: TypeExpression.                                            //
//    Implements Composite pattern [Leaf].                                    //
// -------------------------------------------------------------------------- //
// Create date: 15-10-2006                                                    //
// Modification date: 27-03-2007                                              //
////////////////////////////////////////////////////////////////////////////////
//VISTO
using System;
using System.Collections.Generic;
using System.Text;

using AST;
using ErrorManagement;
using TypeSystem.Constraints;
using Tools;
using DynVarManagement;
using TypeSystem.Operations;

namespace TypeSystem {
    /// <summary>
    /// Represents a generic type expression
    /// </summary>
    /// <remarks>
    /// Inheritance: TypeExpression.
    /// Implements Composite pattern [Leaf].
    /// </remarks>
    public class TypeVariable : TypeExpression {
        
        #region Fields

        /// <summary>
        /// Sequence of values to identify several types of variables.
        /// </summary>
        private static int lastVariable = 0;

        /// <summary>
        /// Saves the last type variable created
        /// </summary>
        private static TypeVariable lastTypeVariable;

        /// <summary>
        /// Concrete value for the variable type
        /// </summary>
        private int variable;

        /// <summary>
        /// The equivalence class of the type variable (see the dragon book)
        /// </summary>
        private EquivalenceClass equivalenceClass;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a new identify to the variable type
        /// </summary>
        public static TypeVariable NewTypeVariable {
            get {
                // * Creates a new type variable
                TypeVariable typeVariable = new TypeVariable(lastVariable++);
                // * And registers it into the type table
                TypeTable.Instance.AddVarType(typeVariable);
                TypeVariable.lastTypeVariable = typeVariable;
                return typeVariable;
            }
        }

        /// <summary>
        /// Gets the last type variable created
        /// </summary>
        public static TypeVariable LastTypeVariable {
            get { return lastTypeVariable; }
        }

        /// <summary>
        /// The equivalence class of the type variable (see the dragon book)
        /// </summary>
        public EquivalenceClass EquivalenceClass {
            get { return equivalenceClass; }
            set { equivalenceClass = value; }
        }

        /// <summary>
        /// Concrete value for the variable type
        /// </summary>
        public int Variable {
            get { return this.variable; }
        }

        /// <summary>
        /// Gets the substitution; null if it does not exist
        /// </summary>
        public TypeExpression Substitution {
            get {
                if (this.equivalenceClass != null)
                    return this.equivalenceClass.Substitution;
                return null;
            }
        }

        /// <summary>
        /// The full name in type variables is calculated just in time
        /// </summary>
        public override string FullName {
            get {
                this.BuildFullName();
                return this.fullName;
            }
        }

        /// <summary>
        /// Indicates if the type has been set as dynamic
        /// </summary>
        public override bool IsDynamic {
            set {
                // * If type variable is dynamic, so it is its substitution
                this.isDynamic = value;
                if (this.Substitution != null)
                    this.Substitution.IsDynamic = value;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of class VariableType.
        /// Use the NetTypeVariable static property instead of using this constructor.
        /// </summary>
        private TypeVariable(int variable) {
            this.variable = variable;
            this.BuildFullName();
            this.BuildTypeExpressionString(TypeExpression.MAX_DEPTH_LEVEL_TYPE_EXPRESSION);
            this.ValidTypeExpression = true;
        }

        #endregion

        #region addToMyEquivalenceClass()
        /// <summary>
        /// Adds a type variable to the object's equivalence class
        /// </summary>
        /// <param name="typeExpression">The type variable to add</param>
        /// <param name="unification">Indicates if the kind of unification (equivalent, incremental or override).</param>
        /// <param name="previouslyUnified">To detect infinite loops. The previously unified pairs of type expressions.</param>
        /// <returns>If the type variable has been actually added</returns>
        /// EN esta clase se le ha cambiado su visibilidad
        internal bool addToMyEquivalenceClass(TypeExpression typeExpression, SortOfUnification unification, IList<Pair<TypeExpression, TypeExpression>> previouslyUnified) {
            // * It the type variable does not have a equivalence class, we create it
            if (this.equivalenceClass == null)
                this.equivalenceClass = new EquivalenceClass(this);
            bool added = this.equivalenceClass.add(typeExpression, unification, previouslyUnified);
            this.ValidTypeExpression = false;
            if (typeExpression is TypeVariable)
                typeExpression.ValidTypeExpression = false;
            return added;
        }

        #endregion

        #region BuildTypeExpressionString()
        /// <summary>
        /// The type expression following the following convention: variable={equivalenceclass}=substitution
        /// </summary>
        /// <returns>variable=equivalence class=substitution</returns>
        public override string BuildTypeExpressionString(int depthLevel) {
            if (this.ValidTypeExpression) return this.typeExpression;
            if (depthLevel <= 0) return this.FullName;

            if (equivalenceClass == null)
                return this.typeExpression = "Var(" + this.variable + ")";
            string substitutionString, eqClassString;
            eqClassString = equivalenceClass.ToString();
            if (equivalenceClass.Substitution == null)
                substitutionString = "fresh variable";
            else
                substitutionString = equivalenceClass.Substitution.BuildTypeExpressionString(depthLevel - 1);
            this.ValidTypeExpression = true;
            return this.typeExpression = String.Format("[Var({0})={1}={2}]", this.variable, eqClassString, substitutionString);
        }
        #endregion

        #region BuildFullName()
        /// <summary>
        /// Creates/Updates the full name of the type expression
        /// </summary>
        public override void BuildFullName() {
            if (equivalenceClass == null) {
                this.fullName = "Var(" + this.variable + ")";
                return;
            }
            string eqClassString = equivalenceClass.ToString();
            if (equivalenceClass.Substitution == null)
                this.fullName = String.Format("[Var({0})={1}]", this.variable, eqClassString);
            else
                this.fullName = String.Format("[Var({0})={1}={2}]", this.variable, eqClassString, equivalenceClass.Substitution.FullName);
        }
        #endregion

        #region ToString()

        public override string ToString() {
            this.validTypeExpression = false;
            return this.typeExpression = this.BuildTypeExpressionString(1);
         }

        #endregion
        
        #region Equivalent() ANULADA
        ////  <summary>
        //// WriteType equivalence. Tells us if two types are the same 
        //// </summary>
        //// <param name="type">The other type</param>
        //// <returns>True if the represent the same type</returns>
        ////public override bool Equivalent(TypeExpression type) {
        ////    if (this.Substitution != null) {
        ////        DynVarOptions.Instance.AssignDynamism(this.Substitution, this.IsDynamic);
        ////         * If the variable is bounded, the equivalence is the one of its substitution
        ////        return this.EquivalenceClass.Substitution.Equivalent(type);
        ////    }
        ////     * A free variable is equivalent to any type
        ////    return true;
        ////}
        #endregion

        // WriteType Inference

        #region Dispatcher
        public override object AcceptOperation(TypeSystemOperation op, object arg) { return op.Exec(this, arg); }
        #endregion

        #region Dot() ANULADA
        ///// <summary>
        ///// Check if the type can make an operation of field access.
        ///// </summary>
        ///// <param name="field">Field to access.</param>
        ///// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        ///// <param name="previousDot">To detect infinite loops. The types that have been previously passed the dot message. Used for union types.</param>
        ///// <param name="fileName">File name.</param>
        ///// <param name="line">Line number.</param>
        ///// <param name="column">Column number.</param>
        ///// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Dot(string field, MethodType methodAnalyzed, IList<TypeExpression> previousDot, Location location) {
        //    if (this.Substitution != null) {
        //        DynVarOptions.Instance.AssignDynamism(this.Substitution, this.IsDynamic);
        //        return this.Substitution.Dot(field, methodAnalyzed, previousDot, location);
        //    }
        //    if (methodAnalyzed != null) {
        //        // * A attribute access constraint is added to the method analyzed
        //        DotConstraint constraint = new DotConstraint(this, field, location);
        //        methodAnalyzed.AddConstraint(constraint);
        //        return constraint.ReturnType;
        //    }
        //    ErrorManager.Instance.NotifyError(new OperationNotAllowedError(".", this.fullName, location));
        //    return null;
        //}
        ///// <summary>
        ///// Tries to find a attribute. No error is generated if the attribute does not exist.
        ///// </summary>
        ///// <param name="memberName">Member to access.</param>
        ///// <param name="previousDot">To detect infinite loops. The types that have been previously passed the dot message. Used for union types.</param>
        ///// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Dot(string memberName, IList<TypeExpression> previousDot) {
        //    if (this.Substitution != null) {
        //        DynVarOptions.Instance.AssignDynamism(this.Substitution, this.IsDynamic);
        //        return this.Substitution.Dot(memberName, previousDot);
        //    }
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
        //public override TypeExpression Parenthesis(TypeExpression actualImplicitObject, TypeExpression[] arguments, MethodType methodAnalyzed,
        //                    SortOfUnification activeSortOfUnification, Location location) {
        //    if (this.Substitution != null) {
        //        DynVarOptions.Instance.AssignDynamism(this.Substitution, this.IsDynamic);
        //        return this.Substitution.Parenthesis(actualImplicitObject, arguments, methodAnalyzed, activeSortOfUnification, location);
        //    }
        //    if (methodAnalyzed != null) {
        //        // * A method invocation constraint is added to the method analyzed
        //        ParenthesisConstraint constraint = new ParenthesisConstraint(this, actualImplicitObject, arguments, activeSortOfUnification, location);
        //        methodAnalyzed.AddConstraint(constraint);
        //        return constraint.ReturnType;
        //    }
        //    ErrorManager.Instance.NotifyError(new OperationNotAllowedError("()", this.fullName, location));
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
        //public override TypeExpression Assignment(TypeExpression operand, AssignmentOperator op, MethodType methodAnalyzed, SortOfUnification unification,
        //            TypeExpression actualImplicitObject, Location location) {
        //    // * Bounded variable?
        //    if (this.Substitution != null && unification == SortOfUnification.Equivalent)
        //        // * Check promotion to its substitution
        //        return operand.Promotion(this, op, methodAnalyzed, location);
        //    // * If the variable its not bounded, we add the parameter to the equivalence list
        //    if (this.addToMyEquivalenceClass(operand, unification, new List<Pair<TypeExpression, TypeExpression>>()))
        //        return this;
        //    // * If it has not been possible, error
        //    ErrorManager.Instance.NotifyError(new OperationNotAllowedError(op.ToString(), this.FullName, operand.FullName, location));
        //    return null;
        //}
        #endregion

        #region Bracket() ANULADA
        /// <summary>
        /// Check if the type can make an array operation.
        /// </summary>
        /// <param name="index">TypeExpression of the index.</param>
        /// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        /// <param name="showErrorMessage">Indicates if an error message should be shown (used for dynamic types)</param>
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Bracket(TypeExpression index, MethodType methodAnalyzed, bool showErrorMessage, Location location) {
        //    if (this.Substitution != null) {
        //        DynVarOptions.Instance.AssignDynamism(this.Substitution, this.IsDynamic);
        //        return this.Substitution.Bracket(index, methodAnalyzed, showErrorMessage, location);
        //    }
        //    if (methodAnalyzed != null) {
        //        // * A bracket constraint is added to the method analyzed
        //        SquareBracketConstraint bracketConstraint = new SquareBracketConstraint(this, index, location);
        //        methodAnalyzed.AddConstraint(bracketConstraint);
        //        // * Also a promotion constriaint of the index to IntType
        //        //index.Promotion(IntType.Instance, ArrayOperator.Indexer, methodAnalyzed, fileName, line, column);
        //        return bracketConstraint.ReturnType;
        //    }
        //    if (showErrorMessage)
        //        ErrorManager.Instance.NotifyError(new OperationNotAllowedError("[]", this.fullName, location));
        //    return null;
        //}
        #endregion

        #region Arithmetic RELATIONQL() ANULADA
/*        /// <summary>
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
        public override TypeExpression Arithmetic(TypeExpression operand, Enum op, MethodType methodAnalyzed, bool showErrorMessage, Location location) {
            if (this.Substitution != null) {
                DynVarOptions.Instance.AssignDynamism(this.Substitution, this.IsDynamic);
                return this.Substitution.Arithmetic(operand, op, methodAnalyzed, showErrorMessage, location);
            }
            if (methodAnalyzed != null) {
                // * A constraint is added to the method analyzed
                ArithmeticConstraint constraint = new ArithmeticConstraint(this, operand, op, location);
                methodAnalyzed.AddConstraint(constraint);
                return constraint.ReturnType;
            }
            if (showErrorMessage)
                ErrorManager.Instance.NotifyError(new OperationNotAllowedError(op.ToString(), this.fullName, operand.FullName, location));
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
        public override TypeExpression Arithmetic(UnaryOperator op, MethodType methodAnalyzed, bool showErrorMessage, Location location) {
            if (this.Substitution != null) {
                DynVarOptions.Instance.AssignDynamism(this.Substitution, this.IsDynamic);
                return this.Substitution.Arithmetic(op, methodAnalyzed, showErrorMessage, location);
            }
            if (methodAnalyzed != null) {
                // * A constraint is added to the method analyzed
                ArithmeticConstraint constraint = new ArithmeticConstraint(this, op, location);
                methodAnalyzed.AddConstraint(constraint);
                return constraint.ReturnType;
            }
            if (showErrorMessage)
                ErrorManager.Instance.NotifyError(new OperationNotAllowedError(op.ToString(), this.fullName, location));
            return null;
        }
        #endregion

        #region Relational()
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
        public override TypeExpression Relational(TypeExpression operand, RelationalOperator op, MethodType methodAnalyzed, bool showErrorMessage, Location location) {
            if (this.Substitution != null) {
                DynVarOptions.Instance.AssignDynamism(this.Substitution, this.IsDynamic);
                return this.Substitution.Relational(operand, op, methodAnalyzed, showErrorMessage, location);
            }
            if (methodAnalyzed != null) {
                // * A constraint is added to the method analyzed
                RelationalConstraint constraint = new RelationalConstraint(this, operand, op, location);
                methodAnalyzed.AddConstraint(constraint);
                return constraint.ReturnType;
            }
            if (showErrorMessage)
                ErrorManager.Instance.NotifyError(new OperationNotAllowedError(op.ToString(), this.fullName, operand.fullName, location));
            return null;
        }
*/
        #endregion
          
        // WriteType Promotion

        #region PromotionLevel() ANULADA
        ///// <summary>
        ///// Returns a value that indicates a promotion level.
        ///// </summary>
        ///// <param name="type">WriteType to promotion.</param>
        ///// <returns>Returns a promotion value.</returns>
        //public override int PromotionLevel(TypeExpression type) {
        //    if (this.Substitution != null) {
        //        DynVarOptions.Instance.AssignDynamism(this.Substitution, this.IsDynamic);
        //        // * If the variable is bounded, the promotion is the one of its substitution
        //        return this.EquivalenceClass.Substitution.PromotionLevel(type);
        //    }
        //    // * A free variable is complete promotion
        //    return 0;
        //}
      
        #endregion
       
        #region Promotion() ANULADA
        ///// <summary>
        ///// Requires the implicit object to be a subtype of the type parameter
        ///// </summary>
        ///// <param name="type">WriteType to promotion.</param>
        ///// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        ///// <param name="op">An optional operator to report error messages.</param>
        ///// <param name="fileName">File name.</param>
        ///// <param name="line">Line number.</param>
        ///// <param name="column">Column number.</param>
        ///// <returns>The supertype; null if there has been some error.</returns>
        //public override TypeExpression Promotion(TypeExpression type, MethodType methodAnalyzed, Location location) {
        //    if (this.Substitution != null) {
        //        DynVarOptions.Instance.AssignDynamism(this.Substitution, this.IsDynamic);
        //        return this.Substitution.Promotion(type, methodAnalyzed, location);
        //    }
        //    if (methodAnalyzed != null) {
        //        // * A constraint is added to the method analyzed
        //        PromotionConstraint constraint = new PromotionConstraint(this, type, location);
        //        methodAnalyzed.AddConstraint(constraint);
        //        return constraint.ReturnType;
        //    }
        //    ErrorManager.Instance.NotifyError(new TypePromotionError(this.FullName, type.FullName, location));
        //    return null;
        //}
        //public override TypeExpression Promotion(TypeExpression type, Enum op, MethodType methodAnalyzed, Location location) {
        //    if (this.Substitution != null) {
        //        DynVarOptions.Instance.AssignDynamism(this.Substitution, this.IsDynamic);
        //        return this.Substitution.Promotion(type, op, methodAnalyzed, location);
        //    }
        //    if (methodAnalyzed != null) {
        //        // * A constraint is added to the method analyzed
        //        PromotionConstraint constraint = new PromotionConstraint(this, type, op, location);
        //        methodAnalyzed.AddConstraint(constraint);
        //        return constraint.ReturnType;
        //    }
        //    ErrorManager.Instance.NotifyError(new TypePromotionError(this.FullName, type.FullName, op.ToString(), location));
        //    return null;
        //}
        #endregion

        #region Cast() ANULADA
        ///// <summary>
        ///// Tells if the type can be cast to the casttype
        ///// </summary>
        ///// <param name="castType">The expected type</param>
        ///// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        ///// <param name="fileName">File name.</param>
        ///// <param name="line">Line number.</param>
        ///// <param name="column">Column number.</param>
        ///// <returns>The returned type expression</returns>
        //public override TypeExpression Cast(TypeExpression castType, MethodType methodAnalyzed, Location location) {
        //    if (this.Substitution != null) {
        //        return this.Substitution.Cast(castType, methodAnalyzed, location);
        //    }
        //    if (methodAnalyzed != null) {
        //        // * A constraint is added to the method analyzed
        //        CastConstraint constraint = new CastConstraint(this, castType, location);
        //        methodAnalyzed.AddConstraint(constraint);
        //        return constraint.ReturnType;
        //    }
        //    ErrorManager.Instance.NotifyError(new TypeCastError(this.FullName, castType.FullName, location));
        //    return null;
        //}
        #endregion

        #region EqualsForOverload() ANULADA
        /// <summary>
        /// Used to check if the overload is possible
        /// </summary>
        /// <param name="typeExpression">The other type expression</param>
        /// <returns>If both represent the same type</returns>
        //public override bool EqualsForOverload(object typeExpression) {
        //    return typeExpression is TypeVariable;
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
            if (te == null)
                return false;
            bool success = this.addToMyEquivalenceClass(te, unification, previouslyUnified);
            // * Clears the type expression cache
            this.ValidTypeExpression = te.ValidTypeExpression = false;
            return success;
        }
        #endregion

        #region HasTypeVariables()
        /// <summary>
        /// To know if the type expression has some type variables and requieres unification
        /// The default implementation is return false
        /// </summary>
        /// <returns>If it has any type variable</returns>
        public override bool HasTypeVariables() {
            return true;
        }
        #endregion

        #region IsFressVariable()
        /// <summary>
        /// To know if it is a type variable with no substitution
        /// </summary>
        /// <returns>True in case it is a fresh variable with no substitution</returns>
        public override bool IsFreshVariable() {
            return this.Substitution == null;
        }
        #endregion

        #region CloneType()
        /// <summary>
        /// This method creates a new type variable.
        /// It these type variables where bounded to types or other
        /// type variables, they are maintained.
        /// </summary>
        /// <param name="typeVariableMappings">Each new type varaiable represent a copy of another existing one.
        /// This parameter is a mapping between them, wher tmpName=old and value=new.</param>
        /// <returns>The new cloned class type</returns>
        public override TypeExpression CloneType(IDictionary<TypeVariable, TypeVariable> typeVariableMappings) {
            // * We clone the type of the elements
            IList<EquivalenceClass> equivalenceClasses = new List<EquivalenceClass>();
            TypeVariable newTypeVariable = (TypeVariable)this.CloneTypeVariables(typeVariableMappings, equivalenceClasses, new List<ClassType>());
            // * For each equivalence class we create a new one, 
            //   substituting the old type variables for the new ones
            // * The substitution is not altered
            // * Since equivalence classes and type variables have a bidirectional association,
            //   the new equivalence classes will make type variables update their new equivalence classes
            foreach (EquivalenceClass equivalenceClass in equivalenceClasses)
                equivalenceClass.UpdateEquivalenceClass(typeVariableMappings);
            newTypeVariable.ValidTypeExpression = false;
            // * The new class type is returned
            return newTypeVariable;
        }
        #endregion

        #region CloneTypeVariables()
        /// <summary>
        /// Method that clones each type variable of a type expression.
        /// Equivalence classes are not cloned (but included in the equivalenceClasses parameter.
        /// The default implementation is do nothing (for built-in types).
        /// </summary>
        /// <param name="typeVariableMappings">Each new type varaiable represent a copy of another existing one.
        /// This parameter is a mapping between them, wher tmpName=old and value=new.
        /// If the variable to be cloned already has a map, we return the latter.</param>
        /// <param name="equivalenceClasses">Each equivalence class of all the type variables.</param>
        /// <param name="clonedClasses">This parameter collects the set of all cloned classes. It is used to detect infinite recursion.</param>
        /// <returns>The new type expression (itself by default)</returns>
        public override TypeExpression CloneTypeVariables(IDictionary<TypeVariable, TypeVariable> typeVariableMappings, IList<EquivalenceClass> equivalenceClasses, IList<ClassType> clonedClasses) {
            // * If the variable to be cloned already has a map, we return the latter
            if (typeVariableMappings.ContainsKey(this))
                return typeVariableMappings[this];
            TypeVariable newOne = TypeVariable.NewTypeVariable;
            newOne.IsDynamic = this.IsDynamic;
            // * Sets the mapping between the old and new one in the typeVariableMappings parameter
            typeVariableMappings[this] = newOne;
            // * Add both equivalence classes to the equivalenceClasses parameter
            if (this.EquivalenceClass != null && !equivalenceClasses.Contains(this.equivalenceClass))
                equivalenceClasses.Add(this.equivalenceClass);
            // * Assigns a clone of the substitution when it previously exists
            if (this.Substitution != null) {
                TypeExpression clonedSubstitution = this.Substitution.CloneTypeVariables(typeVariableMappings, equivalenceClasses, clonedClasses);
                newOne.addToMyEquivalenceClass(clonedSubstitution, SortOfUnification.Equivalent, new List<Pair<TypeExpression, TypeExpression>>());
            }
            else if (this.EquivalenceClass != null)
                newOne.equivalenceClass = this.equivalenceClass;
            newOne.ValidTypeExpression = false;
            // * Returns the new type variable
            return newOne;
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
            if (this.Substitution == null)
                return;
            this.Substitution.UpdateEquivalenceClass(typeVariableMappings, previouslyUpdated);
            this.ValidTypeExpression = false;
        }
        #endregion

        #region Freeze()
        /// <summary>
        /// WriteType variable may change its type's substitution (e.g., field type variables)
        /// This method returns the type in an specific time (frozen).
        /// If this type's substitution changes, the frozen type does not.
        /// <returns>The frozen type</returns>
        /// </summary>
       public override TypeExpression Freeze()
       {
          if (this.Substitution == null)
             return this;
          return this.Substitution.Freeze();
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
            if (clonedTypeVariables.ContainsKey(this.variable))
                // * Already cloned
                return clonedTypeVariables[this.variable];
            if (this.Substitution != null) {
                // * Lets clone it
                TypeVariable newTypeVariable = (TypeVariable)this.MemberwiseClone();
                newTypeVariable.variable = TypeVariable.NewTypeVariable.Variable;
                // * We add it to the list
                clonedTypeVariables[this.variable] = newTypeVariable;
                // * We also clone all the type variables of its class equivalence
                if (newTypeVariable.equivalenceClass != null)
                    newTypeVariable.equivalenceClass.CloneTypeVariables(clonedTypeVariables, equivalenceClasses, methodAnalyzed);
                newTypeVariable.BuildFullName();
                newTypeVariable.BuildTypeExpressionString(2);
                return newTypeVariable;
            }
            if (methodAnalyzed != null) {
                // * A clone constraint is added to the method analyzed
                CloneConstraint constraint = new CloneConstraint(this);
                methodAnalyzed.AddConstraint(constraint);
                // * We add it to the list
                clonedTypeVariables[this.variable] = constraint.ReturnType;
                // * We also clone all the type variables of its class equivalence
                if (this.equivalenceClass != null)
                    this.equivalenceClass.CloneTypeVariables(clonedTypeVariables, equivalenceClasses, methodAnalyzed);
                return constraint.ReturnType;
            }
            return null;
        }
        #endregion

        #region Equals&GetHashCode()
        public override bool Equals(object obj) {
            if (this.GetHashCode() != obj.GetHashCode())
                return false;
            TypeVariable typeVariable = obj as TypeVariable;
            if (typeVariable == null)
                return false;
            return this.variable == typeVariable.variable;
        }
        public override int GetHashCode() {
            return this.variable;
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
        public override bool Remove(TypeVariable toRemove) {
            bool ret = this.equivalenceClass.Remove(toRemove);
            this.ValidTypeExpression = false;
            return ret;
        }
        #endregion

        // Code Generation

        #region ILType()
        /// <summary>
        /// Gets the type name to use in IL code.
        /// </summary>
        /// <returns>Returns the type name to use in IL code.</returns>
        public override string ILType() {
            // * If the type variable has been unified, the IL type is the substitution
            if (this.Substitution != null)
                return this.Substitution.ILType();
            // * Otherwise, object is used
            return "object";
        }
        #endregion

        #region IsValueType()
        /// <summary>
        /// True if type expression is a ValueType. Otherwise, false.
        /// </summary>
        /// <returns>Returns true if the type expression is a ValueType. Otherwise, false.</returns>
        public override bool IsValueType() {
            if (this.Substitution != null)
                return this.Substitution.IsValueType();
            return false;
        }

        #endregion

    }
}

