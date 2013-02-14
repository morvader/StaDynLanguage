using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeSystem.Operations;

namespace StaDynLanguage.TypeSystem
{
    class GetTypeSystemName : TypeSystemOperation
    {
        public override object Exec(global::TypeSystem.StringType s, object arg)
        {
            return s.FullName;
        }
        public override object Exec(global::TypeSystem.MethodType m, object arg)
        {
            //return m.FullName;
            string methodBaseClass= m.MemberInfo.Class.AcceptOperation(this, null) as string;
            string returnType = m.Return.AcceptOperation(this, null) as string;

            return methodBaseClass + ":" + returnType;
        }
        public override object Exec(global::TypeSystem.IntersectionType i, object arg)
        {
            List<string> names=new List<string>();
            foreach (var item in i.TypeSet)
            {
                names.Add(item.AcceptOperation(this, null) as string);
            }

            return names.First();
        }
        public override object Exec(global::TypeSystem.ClassType c, object arg)
        {
            return c.FullName;
            
        }
        public override object Exec(global::TypeSystem.PropertyType p, object arg)
        {
            string propertyBaseClass = p.MemberInfo.Class.AcceptOperation(this, null) as string;

            string propertyType = p.PropertyTypeExpression.AcceptOperation(this, null) as string;

            return propertyBaseClass + ":" + propertyType;
           
        }
        public override object Exec(global::TypeSystem.FieldType g, object arg)
        {
            string fieldBaseClass = g.MemberInfo.Class.AcceptOperation(this, null) as string;

            string fieldType = g.FieldTypeExpression.AcceptOperation(this, null) as string;

            return fieldBaseClass + ":" + fieldType;
        }
        public override object Exec(global::TypeSystem.BoolType b, object arg)
        {
            return b.FullName;
        }
        public override object Exec(global::TypeSystem.CharType c, object arg)
        {
            return c.FullName;
        }
        public override object Exec(global::TypeSystem.DoubleType d, object arg)
        {
            return d.FullName;
        }
        public override object Exec(global::TypeSystem.IntType i, object arg)
        {
            return i.FullName;
        }
        public override object Exec(global::TypeSystem.NullType n, object arg)
        {
            return n.FullName;
        }
        public override object Exec(global::TypeSystem.VoidType v, object arg)
        {
            return v.FullName;
        }
        public override object Exec(global::TypeSystem.TypeVariable t, object arg)
        {
            //return t.FullName;
          string type;
          if (t.Substitution != null)
            type = t.Substitution.AcceptOperation(this, null) as string;
          else
            type = t.FullName;
          return type;
        }
        public override object Exec(global::TypeSystem.UserType u, object arg)
        {
            return u.FullName;
        }

        public override object Exec(global::TypeSystem.ArrayType a, object arg)
        {
            return a.ArrayTypeExpression.AcceptOperation(this, null) as string;
        }
        public override object Exec(global::TypeSystem.BCLClassType b, object arg)
        {
            return b.FullName;
        }
        
    }
}
