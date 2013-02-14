//////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- 
// Project rROTOR                                                             
// -------------------------------------------------------------------------- 
// File: UnionType.cs                                                      
// Authors: Francisco Ortin - francisco.ortin@gmail.com                       
// Description:                                                               
//    Represents a disjunction set type.                                      
//    Inheritance: TypeExpression.                                            
//    Implements Composite pattern [Composite].                               
// -------------------------------------------------------------------------- 
// Create date: 05-04-2007                                                    
// Modification date: 05-04-2007                                              
//////////////////////////////////////////////////////////////////////////////
//visto
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using AST;
using ErrorManagement;
using Tools;
using TypeSystem.Constraints;
using TypeSystem.Operations;

namespace TypeSystem {
    /// <summary>
    /// Representa a union type.
    /// </summary>
    /// <remarks>
    /// Inheritance: TypeExpression.
    /// Implements Composite pattern [Composite].
    /// </remarks>
    public class UnionType : TypeExpression {
      
        #region Fields
        /// <summary>
        /// Stores a set of type expression
        /// </summary>
        private List<TypeExpression> typeSet = new List<TypeExpression>();
        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of type expressions
        /// </summary>
        public int Count {
            get { return this.typeSet.Count; }
        }

        /// <summary>
        /// Gets the list of type expressions
        /// </summary>
        public IList<TypeExpression> TypeSet {
            get { return this.typeSet; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of Intersection
        /// </summary>
        public UnionType() { }
        public UnionType(TypeExpression te) {
            this.AddType(te);
            this.BuildFullName();
        }
        #endregion

        #region AddType
        /// <summary>
        /// Adds a new type expression.
        /// </summary>
        /// <param name="type">WriteType expression to add.</param>
        /// <returns>If the type has been added</returns>
        public bool AddType(TypeExpression type) {
            bool added = true;
            UnionType union = type as UnionType;
            if (union != null) {
                foreach (TypeExpression t in union.typeSet)
                    added = added && this.AddType(t);
                this.ValidTypeExpression = false;
                return added;
            }
            TypeExpression existingType = typeSet.Find(type.Equals);
            if (existingType == null) {
                this.typeSet.Add(type);
                this.ValidTypeExpression = false;
                return true;
            }
            return false;
        }
        #endregion

        #region ToString()

        /// <summary>
        /// Returns the type expression like a string
        /// </summary>
        /// <returns>string with the type expression associated.</returns>
        public override string BuildTypeExpressionString(int depthLevel) {
            if (this.ValidTypeExpression) return this.typeExpression;
            if (depthLevel <= 0) return this.FullName;

            StringBuilder aux = new StringBuilder();
            aux.Append("\\/(");
            for (int i = 0; i < this.typeSet.Count; i++) {
                aux.AppendFormat("{0}", this.typeSet[i].BuildTypeExpressionString(depthLevel - 1));
                if (i < this.typeSet.Count - 1)
                    aux.AppendFormat(" ,");
            }
            aux.Append(")");
            this.typeExpression = aux.ToString();
            this.ValidTypeExpression = true;
            return this.typeExpression;
        }
        #endregion

        #region BuildFullName()
        /// <summary>
        /// Creates/Updates the full name of the type expression
        /// </summary>
        public override void BuildFullName() {
            StringBuilder aux = new StringBuilder();
            aux.Append("\\/(");
            for (int i = 0; i < this.typeSet.Count; i++) {
                aux.AppendFormat("{0}", this.typeSet[i].FullName);
                if (i < this.typeSet.Count - 1)
                    aux.AppendFormat(" ,");
            }
            aux.Append(")");
            this.fullName = aux.ToString();
        }
        #endregion

        // WriteType inference

        #region Dispatcher
        public override object AcceptOperation(TypeSystemOperation op, object arg) { return op.Exec(this, arg); }
        #endregion

        #region Assignment ANULADA
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
        //public override TypeExpression Assignment(TypeExpression operand, AssignmentOperator op, MethodType methodAnalyzed, SortOfUnification unification, TypeExpression actualImplicitObject, Location location) {
        //    // * If the unification is incremental, we must add it to the union typeset
        //    if (unification == SortOfUnification.Incremental) {
        //        this.typeSet.Add(operand);
        //        return this;
        //    }
        //    foreach (TypeExpression type in this.typeSet)
        //        if (operand.PromotionLevel(type) == -1) {
        //            ErrorManager.Instance.NotifyError(new AssignmentError(operand.FullName, this.fullName, location));
        //            return null;
        //        }
        //    return this;
        //}
        #endregion

        #region Equivalent() ANULADA
        /// <summary>
        ///// WriteType equivalence. Tells us if two types are the same 
        ///// </summary>
        ///// <param name="typeParam">The other type</param>
        ///// <returns>True if the represent the same type</returns>
        //public override bool Equivalent(TypeExpression typeParam) {
        //    foreach (TypeExpression type in this.typeSet)
        //        if (!type.Equivalent(typeParam))
        //            return false;
        //    return true;
        //}
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
        //    // * Infinite loop detection
        //    if (previousDot.Contains(this))
        //        return new UnionType();
        //    previousDot.Add(this);

        //    // * If all the types in typeset generate a constraint, we simply generate one constraint using the whole union type
        //    if (this.IsFreshVariable() && methodAnalyzed != null) {
        //        // * A constraint is added to the method analyzed
        //        DotConstraint constraint = new DotConstraint(this, field, loc);
        //        methodAnalyzed.AddConstraint(constraint);
        //        return constraint.ReturnType;
        //    }
        //    TypeExpression returnType = null;
        //    foreach (TypeExpression type in this.typeSet) {
        //        TypeExpression ret;
        //        if (!this.IsDynamic) {
        //            // * Static Behaviour: All the types must accept the attribute access
        //            ret = type.Dot(field, methodAnalyzed, previousDot, loc);
        //            if (ret == null) {
        //                return null;
        //            }
        //        }
        //        else
        //            // * Dynamic Behaviour: Only one type must accept the attribute access
        //            ret = type.Dot(field, previousDot);
        //        if (ret != null)
        //            returnType = UnionType.collect(returnType, ret);
        //    }
        //    // * If it is a dynamic union, one type must accept the attribute access
        //    if (returnType == null)
        //        ErrorManager.Instance.NotifyError(new NoTypeHasMember(this.FullName, field, loc));
        //    return returnType;
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
        //    // * Infinite loop detection
        //    if (previousDot.Contains(this))
        //        return null;
        //    previousDot.Add(this);

        //    TypeExpression returnType = null;
        //    foreach (TypeExpression type in this.typeSet) {
        //        TypeExpression ret = type.Dot(memberName, previousDot);
        //        if (ret == null && !this.isDynamic)
        //            return null;
        //        if (ret != null)
        //            returnType = UnionType.collect(returnType, ret);
        //    }
        //    return returnType;
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
        //public override TypeExpression Bracket(TypeExpression index, MethodType methodAnalyzed, bool showErrorMessage, Location loc) {
        //    // * If all the types in typeset generate a constraint, we simply generate one constraint using the whole union type
        //    if (this.IsFreshVariable() && methodAnalyzed != null) {
        //        // * A constraint is added to the method analyzed
        //        SquareBracketConstraint constraint = new SquareBracketConstraint(this, index, loc);
        //        methodAnalyzed.AddConstraint(constraint);
        //        return constraint.ReturnType;
        //    }
        //    TypeExpression returnType = null;
        //    foreach (TypeExpression type in this.typeSet) {
        //        TypeExpression ret = type.Bracket(index, methodAnalyzed, !this.IsDynamic && showErrorMessage, loc);
        //        if (ret == null && !this.IsDynamic)
        //            return null;
        //        if (ret != null)
        //            returnType = UnionType.collect(returnType, ret);
        //    }
        //    return returnType;
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
        //                    SortOfUnification activeSortOfUnification, Location loc) {
        //    // * If all the types in typeset generate a constraint, we simply generate one constraint using the whole union type
        //    if (this.IsFreshVariable() && methodAnalyzed != null) {
        //        // * A constraint is added to the method analyzed
        //        ParenthesisConstraint constraint = new ParenthesisConstraint(this, actualImplicitObject, arguments, activeSortOfUnification, loc);
        //        methodAnalyzed.AddConstraint(constraint);
        //        return constraint.ReturnType;
        //    }
        //    TypeExpression returnType = null;
        //    foreach (TypeExpression type in this.typeSet) {
        //        TypeExpression ret = type.Parenthesis(actualImplicitObject, arguments, methodAnalyzed, activeSortOfUnification, loc);
        //        if (ret == null && this.isDynamic)
        //            return null;
        //        returnType = UnionType.collect(returnType, ret);
        //    }
        //    return returnType;
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
            // * If all the types in typeset generate a constraint, we simply generate one constraint using the whole union type
            if (this.IsFreshVariable() && methodAnalyzed != null) {
                // * A constraint is added to the method analyzed
                ArithmeticConstraint constraint = new ArithmeticConstraint(this, operand, op, loc);
                methodAnalyzed.AddConstraint(constraint);
                return constraint.ReturnType;
            }
            TypeExpression returnType = null;
            foreach (TypeExpression type in this.typeSet) {
                TypeExpression ret = type.Arithmetic(operand, op, methodAnalyzed, !this.IsDynamic && showErrorMessage, loc);
                if (ret == null && !this.IsDynamic)
                    return null;
                if (ret != null)
                    returnType = UnionType.collect(returnType, ret);
            }
            // * If there has been some errors, they have not been shown because the type is dynamic, we show it
            if (showErrorMessage && this.IsDynamic && returnType == null)
                ErrorManager.Instance.NotifyError(new NoTypeAcceptsOperation(this.FullName, op.ToString(), operand.FullName, loc));
            return returnType;
        }
        */
        /* ANULADA
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
            // * If all the types in typeset generate a constraint, we simply generate one constraint using the whole union type
            if (this.IsFreshVariable() && methodAnalyzed != null) {
                // * A constraint is added to the method analyzed
                ArithmeticConstraint constraint = new ArithmeticConstraint(this, op, loc);
                methodAnalyzed.AddConstraint(constraint);
                return constraint.ReturnType;
            }
            TypeExpression returnType = null;
            foreach (TypeExpression type in this.typeSet) {
                TypeExpression ret = type.Arithmetic(op, methodAnalyzed, !this.IsDynamic && showErrorMessage, loc);
                if (ret == null && !this.IsDynamic)
                    return null;
                if (ret != null)
                    returnType = UnionType.collect(returnType, ret);
            }
            // * If there has been some errors, they have not been shown because the type is dynamic, we show it
            if (showErrorMessage && this.IsDynamic && returnType == null)
                ErrorManager.Instance.NotifyError(new NoTypeAcceptsOperation(this.FullName, op.ToString(), loc));
            return returnType;
        } */
        #endregion

        #region Relational()ANULADA
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
            // * If all the types in typeset generate a constraint, we simply generate one constraint using the whole union type
            if (this.IsFreshVariable() && methodAnalyzed != null) {
                // * A constraint is added to the method analyzed
                RelationalConstraint constraint = new RelationalConstraint(this, operand, op, loc);
                methodAnalyzed.AddConstraint(constraint);
                return constraint.ReturnType;
            }
            bool oneCorrectType = false;
            foreach (TypeExpression type in this.typeSet) {
                TypeExpression ret = type.Relational(operand, op, methodAnalyzed, !this.IsDynamic && showErrorMessage, loc);
                if (ret == null && !this.IsDynamic)
                    return null;
                if (ret != null)
                    oneCorrectType = true;
            }
            // * If there has been some errors, they have not been shown because the type is dynamic, we show it
            if (showErrorMessage && this.IsDynamic && !oneCorrectType)
                ErrorManager.Instance.NotifyError(new NoTypeAcceptsOperation(this.FullName, op.ToString(), operand.FullName, loc));
            return BoolType.Instance;
        }*/

        #endregion

        // WriteType Promotion

        #region PromotionLevel() ANULADA
        ///// <summary>
        ///// Returns a value that indicates a promotion level.
        ///// The static behaviour is
        /////     T1\/T2 <= X    iff   T1<=X and T2<=X
        ///// The dynamic behaviour is
        /////     T1\/T2 <= X    iff   T1<=X or T2<=X
        ///// </summary>
        ///// <param name="superType">WriteType to promote.</param>
        ///// <returns>Returns a promotion value.</returns>
        //public override int PromotionLevel(TypeExpression superType) {
        //    // * Checks recursion
        //    if (recursivePromotionLevelDetection.Contains(this))
        //        return 0;
        //    recursivePromotionLevelDetection.Add(this);
        //    // * Static Behaviour: Takes the addition of all the promotion level
        //    // * Dynamic Behaviour: Takes the addition of all the promotion level
        //    int promotionLevel = this.IsDynamic ? Int32.MaxValue : 0;
        //    foreach (TypeExpression subType in this.typeSet) {
        //        int aux = subType.PromotionLevel(superType);
        //        if (this.IsDynamic && aux != -1) {
        //            // * Dynamic
        //            promotionLevel = Math.Min(promotionLevel, aux);
        //            if (promotionLevel == 0)
        //                break;
        //        }
        //        if (!this.IsDynamic) {
        //            // * Static
        //            if (aux == -1) {
        //                promotionLevel = -1;
        //                break;
        //            }
        //            promotionLevel += aux;
        //        }
        //    }
        //    if (this.IsDynamic && promotionLevel == Int32.MaxValue)
        //        // * No promotion at all
        //        promotionLevel = -1;
        //    // * Clears the loop detection for futures method calls
        //    recursivePromotionLevelDetection.Clear();
        //    return promotionLevel;
        //}
        //// * To detect recursive promotion level calls of recursive type variable expressions
        //private static IList<UnionType> recursivePromotionLevelDetection = new List<UnionType>();
        #endregion

        #region Promotion()
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
        //public override TypeExpression Promotion(TypeExpression type, MethodType methodAnalyzed, Location loc) {
        //    return this.Promotion(type, AssignmentOperator.Assign, methodAnalyzed, loc);
        //}
        //public override TypeExpression Promotion(TypeExpression type, Enum op, MethodType methodAnalyzed, Location loc) {
        //    return this.InternalPromotion(type, op, methodAnalyzed, loc);
        //}
        //private TypeExpression InternalPromotion(TypeExpression superType, Enum op, MethodType methodAnalyzed, Location loc) {
        //    if (this.IsFreshVariable() && methodAnalyzed != null) {
        //        // * A constraint is added to the method analyzed
        //        PromotionConstraint constraint = new PromotionConstraint(this, superType, op, loc);
        //        methodAnalyzed.AddConstraint(constraint);
        //        return superType;
        //    }
        //    // * Static Behaviour: All the types in typeset must promote 
        //    // * Dynamic Behaviour: One of the types in typeset must promote
        //    int aux = 0;
        //    UnionType dynamicUnionType = new UnionType();
        //    dynamicUnionType.isDynamic = true;
        //    foreach (TypeExpression subType in this.typeSet) {
        //        if (this.IsDynamic) {
        //            // * Dynamic
        //            if (subType.IsFreshVariable())
        //                dynamicUnionType.AddType(subType);
        //            else {
        //                aux = (int) subType.AcceptOperation(new PromotionLevelOperation(superType));
        //                if (aux != -1)
        //                    return superType;
        //            }
        //        }
        //        if (!this.IsDynamic) {
        //            // * Static
        //            aux = (int)subType.AcceptOperation(new PromotionLevelOperation(superType));
        //            if (aux == -1) {
        //                ErrorManager.Instance.NotifyError(new TypePromotionError(this.FullName, superType.FullName, op.ToString(), loc));
        //                return null;
        //            }
        //        }
        //    }
        //    if (dynamicUnionType.Count != 0) {
        //        // * If the union type is dynamic and no type in the type set promotes, then we generate a constraint with one promotion grouping the fresh types in the type set
        //        PromotionConstraint constraint = new PromotionConstraint(dynamicUnionType, superType, op, loc);
        //        methodAnalyzed.AddConstraint(constraint);
        //        return superType;
        //    }
        //    if (this.IsDynamic && aux == -1) {
        //        // * No promotion at all
        //        ErrorManager.Instance.NotifyError(new TypePromotionError(this.FullName, superType.FullName, op.ToString(), loc));
        //        return null;
        //    }
        //    return superType;
        //}
        #endregion

        #region SuperType()
        /// <summary>
        /// The opposite of the promotion method. Indicates if the parameter
        /// promotes to the implicit object
        /// T1 <= T1\/T2
        /// T2 <= T1\/T2
        /// </summary>
        /// <param name="type">WriteType to promotion.</param>
        /// <returns>Returns a promotion value.</returns>
        public int SuperType(TypeExpression subType) {
            // * Takes the lower promotion level (except -1)
            int lowerLevelOfPromotion = Int32.MaxValue;
            foreach (TypeExpression superType in this.typeSet) {
                int aux = (int) subType.AcceptOperation(new PromotionLevelOperation(superType), null);
                if (aux != -1 && aux < lowerLevelOfPromotion)
                    lowerLevelOfPromotion = aux;
            }
            return lowerLevelOfPromotion == Int32.MaxValue ? -1 : lowerLevelOfPromotion;
        }

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
            // * Infinite recursion detection
            Pair<TypeExpression, TypeExpression> pair = new Pair<TypeExpression, TypeExpression>(this, te);
            if (previouslyUnified.Contains(pair))
                return true;
            previouslyUnified.Add(pair);

            // * First checks that all the expression are equivalent. Otherwise substitutions could be
            //   partially applied to type variables that should finally not be unified
            bool equivalent = true;
            foreach (TypeExpression type in this.typeSet)
                if (!(equivalent = (bool)type.AcceptOperation(new EquivalentOperation(te), null)))
                    break;
            if (!equivalent)
                return false;
            // * Lets unify, incrementing type variables with union types
            foreach (TypeExpression type in this.typeSet)
                // * Unification of union types is incremental
                te.Unify(type, SortOfUnification.Incremental, previouslyUnified);
            // * Clears the type expression cache
            this.ValidTypeExpression = false;
            te.ValidTypeExpression = false;
            return true;
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
            bool toReturn = false;
            foreach (TypeExpression type in this.typeSet)
                if (type.HasTypeVariables()) {
                    toReturn = true;
                    break;
                }
            this.validHasTypeVariables = true;
            return this.hasTypeVariablesCache = toReturn;
        }
        #endregion

        #region IsFreshVariable()

        /// <summary>
        /// To know if it is a type variable with no substitution
        /// </summary>
        /// <returns>True in case it is a fresh variable with no substitution</returns>
        /// <remarks>Return true if all of subtypes are fresh variables.</remarks>
        public override bool IsFreshVariable() {
            foreach (TypeExpression type in this.typeSet)
                if (!type.IsFreshVariable())
                    return false;
            return true;
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
            UnionType newType = (UnionType)this.MemberwiseClone();
            IList<TypeExpression> oldTypeSet = this.typeSet;
            newType.typeSet = new List<TypeExpression>();
            foreach (TypeExpression oldType in oldTypeSet)
                newType.typeSet.Add(oldType.CloneTypeVariables(typeVariableMappings, equivalenceClasses, clonedClasses));
            newType.ValidTypeExpression = false;
            return newType;
        }
        #endregion

        // Helper Methods

        #region collect()

        /// <summary>
        /// This static helper method group a newType into a union collection of types.
        /// </summary>
        /// <param name="collection">The collection of type (or simply one type)</param>
        /// <param name="newType">The type to be added</param>
        /// <returns>The union collection of types</returns>
        public static TypeExpression collect(TypeExpression collection, TypeExpression newType) {
            if (newType == null)
                return collection;
            if (collection == null)
                return newType;
            UnionType union = collection as UnionType;
            if (collection != null)
                union = new UnionType(collection);
            if (union.AddType(newType))
                return union;
            // * If two elements are the same, retuns the first
            return collection;
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
            foreach (TypeExpression type in this.typeSet)
                if (type.HasTypeVariables()) {
                    type.UpdateEquivalenceClass(typeVariableMappings, previouslyUpdated);
                    type.ValidTypeExpression = false;
                }
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
            if (!this.HasTypeVariables())
                return;
            for (int i = 0; i < this.typeSet.Count; i++) {
                TypeVariable typeVariable = this.typeSet[i] as TypeVariable;
                if (typeVariable == null) {
                    if (this.typeSet[i].HasTypeVariables())
                        this.typeSet[i].ReplaceTypeVariables(typeVariableMappings);
                }
                else if (typeVariableMappings.ContainsKey(typeVariable))
                    this.typeSet[i] = typeVariableMappings[typeVariable];
                this.ValidTypeExpression = false;
            }
        }
        #endregion

        #region Freeze()
        /// <summary>
        /// WriteType variable may change its type's substitution (e.g., field type variables)
        /// This method returns the type in an specific time (frozen).
        /// If this type's substitution changes, the frozen type does not.
        /// <returns>The frozen type</returns>
        /// </summary>
        public override TypeExpression Freeze() {
            if (!this.HasTypeVariables())
                return this;
            UnionType newUnionType = new UnionType();
            foreach (TypeExpression type in this.TypeSet)
                newUnionType.AddType(type.Freeze());
            return newUnionType;
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
            UnionType newUnionType = (UnionType)this.MemberwiseClone();
            newUnionType.typeSet = new List<TypeExpression>();
            // * Clones all the types in the union
            foreach (TypeExpression type in this.typeSet)
                newUnionType.typeSet.Add(type.Clone(clonedTypeVariables, equivalenceClasses, methodAnalyzed));
            return newUnionType;
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
            int i;
            for (i = 0; i < this.typeSet.Count; i++) {
                TypeVariable typeVariable = this.typeSet[i] as TypeVariable;
                if (typeVariable != null && typeVariable.Variable == toRemove.Variable) {
                    this.typeSet.RemoveAt(i);
                    this.ValidTypeExpression = false;
                    break;
                }
                if (this.typeSet[i].Remove(toRemove)) {
                    this.typeSet[i].ValidTypeExpression = false;
                    break;
                }
            }
            return i < this.typeSet.Count;
        }
        #endregion

        // Code Generation

        #region ILType()

        /// <summary>
        /// Gets the string type to use in IL code.
        /// </summary>
        /// <returns>Returns the string type to use in IL code.</returns>
        public override string ILType() {
            return "object";
        }

        #endregion

        #region IsValueType()

        /// <summary>
        /// True if type expression is a ValueType. Otherwise, false.
        /// </summary>
        /// <returns>Returns true if the type expression is a ValueType. Otherwise, false.</returns>
        public override bool IsValueType() {
            foreach (TypeExpression type in this.TypeSet)
                // * If Dynamic, a value type is enough
                if (this.IsDynamic) {
                    if (type.IsValueType())
                        return true;
                }
                // * If Static, all types must be value types
                else if (!type.IsValueType())
                    return false;
            if (this.IsDynamic)
                // * None is value type
                return false;
            // * Static: All are value types
            return true;
        }

        #endregion

        #region HasFreshVariable()

        /// <summary>
        /// To know if it is a type variable with no substitution
        /// </summary>
        /// <returns>True in case it is a fresh variable with no substitution</returns>
        /// <remarks>Return true if, at least, one of subtypes is fresh variable.</remarks>
        public override bool HasFreshVariable() {
            foreach (TypeExpression type in this.typeSet)
                if (type.IsFreshVariable())
                    return true;
            return false;
        }
        #endregion

        #region HasIntersectionVariable()

        /// <summary>
        /// To know if it has a element with intersection.
        /// </summary>
        /// <returns>True if has a intersection type.</returns>
        /// <remarks>Return true if, at least, one of subtypes is intersection type.</remarks>
        public override bool HasIntersectionVariable() {
            foreach (TypeExpression type in this.typeSet)
                if (TypeExpression.Is<IntersectionType>(type))
                    return true;
            return false;
        }
        #endregion

        #region ContainsValueType()

        /// <summary>
        /// Returns true if the union type contains the specified value type.
        /// </summary>
        /// <param name="type">WriteType expression to search.</param>
        /// <returns>True if the union type contains the specified type expression. Otherwise, false.</returns>
        public bool ContainsValueType(TypeExpression type) {
            for (int i = 0; i < this.typeSet.Count; i++) {
                if (this.typeSet[i].IsValueType()) {
                    if (this.typeSet[i] == type)
                        return true;
                }
            }
            return false;
        }

        #endregion

        #region ContainsDifferentReturns()

        /// <summary>
        /// Returns true if the union type contains method types and this methods returns differents types. (At least, one valut type).
        /// </summary>
        /// <returns>True if the union type contains method types and this methods returns differents types. Otherwise, false.</returns>
        public bool ContainsDifferentReturns() {
            TypeExpression current = null;
            TypeExpression aux = null;

            for (int i = 0; i < this.typeSet.Count; i++) {
                MethodType mt = null;
                if ((mt = TypeExpression.As<MethodType>(this.typeSet[i])) != null) {
                    current = mt.Return;
                    if (aux != null && aux != current) // This comparation searches differents value types.
                        return true;
                    aux = current;
                }
            }
            return false;
        }

        #endregion

    }
}
