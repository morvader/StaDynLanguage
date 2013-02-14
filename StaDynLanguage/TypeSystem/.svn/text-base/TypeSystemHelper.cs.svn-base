using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeSystem;
using AST;
using Ast.Operations;

namespace StaDynLanguage.TypeSystem
{

    public class TypeSystemHelper
    {
        #region Singleton

        //Implements Singleton
        static TypeSystemHelper instance = null;

        TypeSystemHelper()
        {
        }

        public static TypeSystemHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TypeSystemHelper();
                }
                return instance;
            }
        }
        #endregion

        public HashSet<TypeExpression> getTypesFromUnion(UnionType union)
        {
            HashSet<TypeExpression> types = new HashSet<TypeExpression>();
            foreach (TypeExpression type in union.TypeSet)
            {
                TypeVariable varType = type as TypeVariable;
                TypeExpression substitutionType = varType != null ? varType.Substitution : null;
                if (substitutionType == null)
                    continue;

                if (substitutionType is UnionType)
                {
                    types.UnionWith(getTypesFromUnion(substitutionType as UnionType));
                    continue;
                }
                types.Add(substitutionType);
            }
            return types;
        }

        public HashSet<TypeExpression> getSubstitutionType(TypeExpression type)
        {
            var types = new HashSet<TypeExpression>();

            if (!(type is TypeVariable))
            {
                types.Add(type);
                return types;
            }

            var varType = (TypeVariable)type;

            TypeExpression substitutionType = varType != null ? varType.Substitution : null;
            //if (substitutionType != null)
            //    types.Add(substitutionType);

            if (substitutionType is UnionType)
            {
                types.UnionWith(getTypesFromUnion(substitutionType as UnionType));
            }
            else
            {
                if (substitutionType != null)
                    types.Add(substitutionType);
            }
            return types;
        }
    }


}
