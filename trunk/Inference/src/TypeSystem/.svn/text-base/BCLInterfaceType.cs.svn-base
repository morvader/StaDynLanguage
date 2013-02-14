//////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- 
// Project rROTOR                                                             
// -------------------------------------------------------------------------- 
// File: BCLInterfaceType.cs                                                      
// Authors: Francisco Ortin - francisco.ortin@gmail.com                       
// Description:                                                               
//    Represents an interface that is part of the BCL. We obtain its 
//       behavior by using an intropection object.
//    Inheritance: ClassType.                                            
//    Implements Composite pattern [Leaf].                               
//    Implements Adapter pattern [Adapter].                               
// -------------------------------------------------------------------------- 
// Create date: 07-04-2007                                                    
// Modification date: 07-04-2007                                              
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using TypeSystem.Operations;
//vISTO
namespace TypeSystem
{
   public class BCLInterfaceType : InterfaceType, IBCLUserType
   {
      #region Fields
      /// <summary>
      /// To delegate all the functionalities
      /// </summary>
      private Introspection introspection;

      #endregion


      #region Constructors
      /// <summary>
      /// Constructor that creates the inheritance tree by means of introspection
      /// </summary>
      /// <param name="name">The name of the class</param>
      /// <param name="introspectiveType">The real introspective type</param>
      public BCLInterfaceType(string name, Type introspectiveType)
         :
          base(name)
      {
         introspection = new Introspection(this, introspectiveType);
         introspection.createBaseClassAndInterfacesTree();
      }
      #endregion

      #region Properties
      /// <summary>
      /// Returns the real introspective type
      /// </summary>
      public Type TypeInfo
      {
         get { return this.introspection.TypeInfo; }
      }

      #endregion


      public TypeExpression FindMember(string memberName) {
          return this.introspection.FindMember(memberName);
      }
      // WriteType Inference

      #region Dispatcher
      public override object AcceptOperation(TypeSystemOperation op, object arg) { return op.Exec(this, arg); }
      #endregion

      #region Dot()UNCONSTRAINED ANULADA
      /// <summary>
      /// Tries to find a attribute. 
      /// No error is generated if the attribute does not exist.
      /// It does not generate a constraint in case it is applied to a free variable.
      /// </summary>
      /// <param name="memberName">The attribute's name</param>
      /// <param name="previousDot">To detect infinite loops. The types that have been previously passed the dot message. Used for union types.</param>
      /// <returns>The type expression of the attribute; null if it has not been found</returns>
      //public override TypeExpression Dot(string memberName, IList<TypeExpression> previousDot)
      //{
      //   // * Has the attribute previously found?
      //   if (this.Members.ContainsKey(memberName))
      //      return this.Members[memberName].WriteType;

      //   // * Lets use introspection
      //   TypeExpression memberType = introspection.FindMember(memberName);
      //   if (memberType != null)
      //      return memberType;

      //   // * Search in the inhirtance tree
      //   foreach (BCLInterfaceType interfaze in this.interfaceList)
      //   {
      //      // * Does this interface support this attribute?
      //      TypeExpression member = interfaze.Dot(memberName, previousDot);
      //      if (member != null)
      //         return member;
      //   }
      //   // * not found
      //   return null;
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
         aux.AppendFormat("class [mscorlib]{0}", this.fullName);
         return aux.ToString();
      }

      #endregion


   }
}
