            //////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- 
// Project rROTOR                                                             
// -------------------------------------------------------------------------- 
// File: BCLClassType.cs                                                      
// Authors: Francisco Ortin - francisco.ortin@gmail.com                       
// Description:                                                               
//    Represents a class that is part of the BCL. We obtain its behavior by
//       using an intropection object.
//    Inheritance: ClassType.                                            
//    Implements Composite pattern [Leaf].                               
//    Implements Adapter pattern [Adapter].                               
// -------------------------------------------------------------------------- 
// Create date: 07-04-2007                                                    
// Modification date: 05-06-2007                                              
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

using ErrorManagement;
using TypeSystem.Operations;

namespace TypeSystem {
    public class BCLClassType : ClassType, IBCLUserType {
        #region Fields
        /// <summary>
        /// To delegate all the functionalities
        /// </summary>
        private Introspection introspection;

        /// <summary>
        /// Returns the real introspective type
        /// </summary>
        public Type TypeInfo {
            get { return this.introspection.TypeInfo; }
        }

        public TypeExpression FindConstructor(Location location) {
            return introspection.FindConstructor(location);
        }
        
        public TypeExpression FindMember(string memberName) {
            return this.introspection.FindMember(memberName);
        }

        /// <summary>
        /// To know the type equivalence between BCL types and builtin types
        /// </summary>
        public static IDictionary<string, TypeExpression> BCLtoTypeSystemMapping = new Dictionary<string, TypeExpression>();

        #endregion

        #region Constructors
        /// <summary>
        /// Constructor that creates the inheritance tree by means of introspection
        /// </summary>
        /// <param name="name">The name of the class</param>
        /// <param name="introspectiveType">The real introspective type</param>
        public BCLClassType(string name, Type introspectiveType)
            :
            base(name) {
            introspection = new Introspection(this, introspectiveType);
            introspection.createBaseClassAndInterfacesTree();
        }

        static BCLClassType() {
            BCLtoTypeSystemMapping["System.Boolean"] = BoolType.Instance;
            BCLtoTypeSystemMapping["System.Char"] = CharType.Instance;
            BCLtoTypeSystemMapping["System.Int32"] = IntType.Instance;
            BCLtoTypeSystemMapping["System.Double"] = DoubleType.Instance;
            BCLtoTypeSystemMapping["System.String"] = StringType.Instance;
            BCLtoTypeSystemMapping["TypeSystem.BoolType"] = BoolType.Instance;
            BCLtoTypeSystemMapping["TypeSystem.CharType"] = CharType.Instance;
            BCLtoTypeSystemMapping["TypeSystem.IntType"] = IntType.Instance;
            BCLtoTypeSystemMapping["TypeSystem.DoubleType"] = DoubleType.Instance;
            BCLtoTypeSystemMapping["TypeSystem.StringType"] = StringType.Instance;
        }
        #endregion

        #region UpdateConstructors()

        /// <summary>
        /// Makes sure the constructors has been loaded
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// </summary>
        public void UpdateConstructors(Location location) {
            // * Load the constructors
            if (this.Constructors == null)
                this.introspection.FindConstructor(location);
          }

        #endregion

          // WriteType Inference
        #region Dispatcher
        public override object AcceptOperation(TypeSystemOperation op, object arg) { return op.Exec(this, arg); }
        #endregion
       
        #region Dot() CONSTRAINED ANULADA
        /// <summary>
        /// Check if the type can make an operation of field access.
        /// Generates an error if the attribute does not exist.
        /// Generates a constraint in case it is applied to a free variable. 
        /// </summary>
        /// <param name="field">Field to access.</param>
        /// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        /// <param name="previousDot">To detect infinite loops. The types that have been previously passed the dot message. Used for union types.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Dot(string memberName, MethodType methodAnalyzed, IList<TypeExpression> previousDot, Location loc) {
        //        TypeExpression member = this.Dot(memberName, previousDot);
        //        if (member == null)
        //            ErrorManager.Instance.NotifyError(new UnknownMemberError(memberName, loc));
        //        return member;
        //}
        ////UNCONSTRAINED ANULADA
        /// <summary>
        /// Tries to find a attribute. 
        /// No error is generated if the attribute does not exist.
        /// It does not generate a constraint in case it is applied to a free variable.
        /// </summary>
        /// <param name="memberName">Member to access.</param>
        /// <param name="previousDot">To detect infinite loops. The types that have been previously passed the dot message. Used for union types.</param>
        /// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Dot(string memberName, IList<TypeExpression> previousDot) {
        //    // * Has the attribute been previosly found?
        //    if (this.Members.ContainsKey(memberName))
        //        return this.Members[memberName].WriteType;

        //    // * Let's try introspection
        //    TypeExpression member = this.introspection.FindMember(memberName);
        //    if (member != null)
        //        return member;

        //    // * Search in base class
        //    if (this.baseClass != null)
        //        return this.baseClass.Dot(memberName, previousDot);

        //    return null;
        //}
        #endregion

        #region Parenthesis() ANULADA
        ///// <summary>
        ///// Check if the type can make a method operation.
        ///// </summary>
        ///// <param name="actualImplicitObject">The actual implicit object employed to pass the message</param>
        ///// <param name="arguments">Arguments of the method.</param>
        ///// <param name="methodAnalyzed">The method that is being analyzed when the operation is performed.</param>
        ///// <param name="showErrorMessage">Indicates if an error message should be shown (used for dynamic types)</param>
        ///// <param name="activeSortOfUnification">The active sort of unification used (Equivalent is the default
        ///// one and Incremental is used in the SSA bodies of the while, for and do statements)</param>
        ///// <param name="fileName">File name.</param>
        ///// <param name="line">Line number.</param>
        ///// <param name="column">Column number.</param>
        ///// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Parenthesis(TypeExpression actualImplicitObject, TypeExpression[] arguments, MethodType methodAnalyzed, 
        //                SortOfUnification activeSortOfUnification, Location loc) {
        //    // * Load the constructors
        //    if (this.Constructors == null)
        //        this.introspection.FindConstructor(loc);

        //    // * Follows the superclass behaviour
        //    return base.Parenthesis(actualImplicitObject, arguments, methodAnalyzed, activeSortOfUnification, loc);
        //}
        #endregion

        #region Bracket() Anulada
        /// <summary>
        /// Check if the type can make an array operation.
        /// </summary>
        /// <param name="index">TypeExpression of the index.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="line">Line number.</param>
        /// <param name="column">Column number.</param>
        /// <returns>WriteType obtained with the operation.</returns>
        //public override TypeExpression Bracket(TypeExpression index, MethodType methodAnalyzed, bool showErrorMessage, Location loc) {
        //    // * Brackets are allowed if it is an array
        //    if (this.introspection.TypeInfo.IsArray)
        //        return TypeTable.Instance.GetType(this.introspection.TypeInfo.GetElementType().FullName, loc);
        //    // * Brackets are allowed if it is an indexer
        //    if (this.Methods.ContainsKey("get_Item")) {
        //        MethodType method = this.Methods["get_Item"].WriteType as MethodType;
        //        if (method != null && method.ParameterListCount == 1) // && method.GetParameter(0).Equivalent(IntType.Instance))
        //            return method.Return;
        //    }
        //    if (showErrorMessage)
        //        ErrorManager.Instance.NotifyError(new OperationNotAllowedError("[]", this.fullName, loc));
        //    return null;
        //}
       #endregion

        #region Equivalent() ANULADA
        ///// <summary>
        ///// WriteType equivalence. Tells us if two types are the same.
        ///// Special care is taken with BCL types because they have differnt mappings (e.g. int == Int32)
        ///// </summary>
        ///// <param name="type">The other type</param>
        ///// <returns>True if the represent the same type</returns>
        //public override bool Equivalent(TypeExpression type) {
        //    if (type == null)
        //        return false;
        //    if (this.fullName.Equals(type.fullName))
        //        return true;
        //    if (BCLtoTypeSystemMapping.ContainsKey(this.fullName) && BCLtoTypeSystemMapping[this.fullName].mEquivalent(type))
        //        return true;
        //    if (this.introspection.TypeInfo.IsArray) {
        //        WriteType elementType = this.introspection.TypeInfo.GetElementType();
        //        if (type is ArrayType)
        //            return new BCLClassType(elementType.FullName, elementType).Equivalent(((ArrayType)type).ArrayTypeExpression);
        //        BCLClassType bclType = TypeExpression.As<BCLClassType>(type);
        //        if (bclType != null && bclType.TypeInfo.IsArray) {
        //            TypeExpression thisArrayType = TypeTable.Instance.GetType(this.TypeInfo.GetElementType().FullName, new Location("", 0, 0)),
        //                paramArrayType = TypeTable.Instance.GetType(bclType.TypeInfo.GetElementType().FullName, new Location("", 0, 0));
        //            return thisArrayType.Equivalent(paramArrayType);
        //        }
        //    }
        //    return false;
        //}
        #endregion

        // Code Generation

        #region ILType()

        /// <summary>
        /// Gets the string type to use in IL code.
        /// </summary>
        /// <returns>Returns the string type to use in IL code.</returns>
        public override string ILType()
        {
           StringBuilder aux = new StringBuilder();
           if (!this.TypeInfo.IsValueType)
              aux.Append("class ");
           else
              aux.Append("valuetype ");

           aux.AppendFormat("[mscorlib]{0}", this.fullName);
           return aux.ToString();
        }

        #endregion

        #region IsValueType()

        /// <summary>
        /// True if type expression is a ValueType. Otherwise, false.
        /// </summary>
        /// <returns>Returns true if the type expression is a ValueType. Otherwise, false.</returns>
        public override bool IsValueType()
        {
           return this.TypeInfo.IsValueType;
           //return this.introspection.GetType().IsValueType;
        }

        #endregion

    }
}
