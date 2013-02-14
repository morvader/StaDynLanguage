using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeSystem;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using System.ComponentModel.Composition;

namespace StaDynLanguage
{
    [Obsolete]
    public enum Accessibility
    {
        Public = 0,
        Internal = 1,
        Friend = 2,
        Protected = 3,
        Private = 4,
        Shortcut = 5,
    }
    [Obsolete]
    public enum Element
    {
        Class = 0,
        Constant = 1,
        Delegate = 2,
        Enum = 3,
        Enummember = 4,
        Event = 5,
        Exception = 6,
        Field = 7,
        Interface = 8,
        Macro = 9,
        Map = 10,
        Mapitem = 11,
        Method = 12,
        Overload = 13,
        Module = 14,
        Namespace = 15,
        Operator = 16,
        Property = 17,
        Struct = 18,
        Template = 19,
        Typedef = 20,
        Type = 21,
        Union = 22,
        Variable = 23,
        Valuetype = 24,
        Intrinsic = 25,
    }

    public class CompletionGlyph
    {
        //Implements Singleton
        private static CompletionGlyph instance = new CompletionGlyph();

        private CompletionGlyph()
        {
            publicAccess = (int)Modifier.Public;
            publicStaticAccess = publicAccess + (int)Modifier.Static;
            publicAbstractAccess = publicAccess + (int)Modifier.Abstract;
            publicVirtualAccess = publicAccess + (int)Modifier.Virtual;

        }
        [Obsolete]
        public System.Windows.Controls.Image getIcon(AccessModifier acces)
        {
            System.Windows.Controls.Image simpleImage = new System.Windows.Controls.Image();
            BitmapImage bi = new BitmapImage();

            bi.BeginInit();
            bi.SourceRect = new Int32Rect(16 * this.GetIndexFor(acces), 0, 16, 16);
            bi.UriSource = new Uri(StaDynLanguage.Properties.Resources.CompletionSetPath, UriKind.RelativeOrAbsolute);
            bi.EndInit();

            simpleImage.Source = bi;
            return simpleImage;
        }
        [Obsolete]
        public System.Windows.Controls.Image getIcon(Accessibility acces, Element element)
        {
            System.Windows.Controls.Image simpleImage = new System.Windows.Controls.Image();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();

            bi.SourceRect = new Int32Rect(16 * this.GetIndexFor(acces, element), 0, 16, 16);
            bi.UriSource = new Uri(StaDynLanguage.Properties.Resources.CompletionSetPath, UriKind.RelativeOrAbsolute);
            bi.EndInit();

            simpleImage.Source = bi;
            return simpleImage;
        }
        [Obsolete]
        public System.Windows.Controls.Image getIcon(TypeExpression type)
        {
            System.Windows.Controls.Image simpleImage = new System.Windows.Controls.Image();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();

            if (type is IMemberType)
            {
                IMemberType memberType = type as IMemberType;
                bi.SourceRect = new Int32Rect(16 * this.GetIndexFor(memberType.MemberInfo), 0, 16, 16);
            }
            else
                //Public accessibility because VS2010 does it
                bi.SourceRect = new Int32Rect(16 * this.GetIndexFor(Accessibility.Public, this.getElementVSValue(type)), 0, 16, 16);

            bi.UriSource = new Uri(StaDynLanguage.Properties.Resources.CompletionSetPath, UriKind.RelativeOrAbsolute);
            bi.EndInit();

            simpleImage.Source = bi;
            return simpleImage;
        }

        public ImageSource getImageSource(TypeExpression type, IGlyphService glyphService)
        {

            return this.getImage(type, glyphService);
        }

        public static CompletionGlyph Instance
        {
            get { return instance; }
        }

        private int publicAccess;// = (int)Modifier.Public;
        private int publicStaticAccess;// = publicAccess + (int)Modifier.Static;
        private int publicAbstractAccess;// = publicAccess + (int)Modifier.Abstract;
        private int publicVirtualAccess;// = publicAccess + (int)Modifier.Virtual;

        [Obsolete]
        public int GetIndexFor(Accessibility accessibility, Element element)
        {
            return (int)accessibility + (int)element * 6;
        }

        [Obsolete]
        public int GetIndexFor(AccessModifier modifier)
        {
            int accessibility = getAccesibilityVSValue(modifier);
            Element element = getElementVSValue(modifier);
            return (int)accessibility + (int)element * 6;
        }

        [Obsolete]
        private int getAccesibilityVSValue(AccessModifier modifier)
        {
            int currentValue = (int)modifier.ModifierMask;

            switch (modifier.ModifierMask)
            {
                case Modifier.Public:
                    return (int)Accessibility.Public;
                case Modifier.Protected:
                    return (int)Accessibility.Protected;
                case Modifier.Private:
                    return (int)Accessibility.Private;
                case Modifier.Internal:
                    return (int)Accessibility.Internal;
                default:
                    if (currentValue == this.publicStaticAccess || currentValue == this.publicVirtualAccess || currentValue == this.publicAbstractAccess)
                        return (int)Accessibility.Public;
                    return 0;
            }
        }

        [Obsolete]
        private Element getElementVSValue(AccessModifier modifier)
        {
            TypeExpression type = modifier.Type;

            return this.getElementVSValue(type);

        }
        [Obsolete]
        private Element getElementVSValue(TypeExpression type)
        {

            if (type is MethodType)
                return Element.Method;
            else if (type is InterfaceType)
                return Element.Interface;
            else if (type is PropertyType)
                return Element.Property;
            else if (type is UnionType)
                return Element.Union;
            else if (type is ClassType || type is BCLClassType)
                return Element.Class;
            else if (type is FieldType)
                return Element.Field;
            else if (type is TypeVariable)
                return Element.Variable;
            else if (type is UserType || type is IBCLUserType)
                return Element.Type;
            else if (type is IntType || type is BoolType || type is CharType || type is StringType)
                return Element.Variable;
            else if (type is ArrayType || type is TypeTable)
                return Element.Map;
            else if (type is NameSpaceType)
                return Element.Namespace;



            return Element.Variable;

        }

        public ImageSource getImage(TypeExpression type, IGlyphService glyphService)
        {
            //StandardGlyphGroup group;
            StandardGlyphItem item = StandardGlyphItem.GlyphItemPublic;
            if (type is IMemberType)
            {
                //group = this.getGroupVSValue(type);
                if (((IMemberType)type).MemberInfo != null)
                    item = this.getItemVSValue(((IMemberType)type).MemberInfo);
                
            }

            return glyphService.GetGlyph(this.getGroupVSValue(type), item);
        }

        public ImageSource getImage(StandardGlyphGroup group, StandardGlyphItem item, IGlyphService glyphService)
        {
            return glyphService.GetGlyph(group, item);
        }
        private StandardGlyphGroup getGroupVSValue(TypeExpression type)
        {
           
            if (type is MethodType)
                return StandardGlyphGroup.GlyphGroupMethod;
            else if (type is InterfaceType)
                return StandardGlyphGroup.GlyphGroupInterface;
            else if (type is PropertyType)
                return StandardGlyphGroup.GlyphGroupProperty;
            else if (type is UnionType)
                return StandardGlyphGroup.GlyphGroupUnion;
            else if (type is ClassType || type is BCLClassType)
                return StandardGlyphGroup.GlyphGroupClass;
            else if (type is FieldType)
                return StandardGlyphGroup.GlyphGroupField;
            else if (type is TypeVariable)
                return StandardGlyphGroup.GlyphGroupVariable;
            else if (type is UserType || type is IBCLUserType)
                return StandardGlyphGroup.GlyphGroupType;
            else if (type is IntType || type is BoolType || type is CharType || type is StringType)
                return StandardGlyphGroup.GlyphGroupVariable;
            else if (type is ArrayType || type is TypeTable)
                return StandardGlyphGroup.GlyphGroupMap;
            else if (type is NameSpaceType)
                return StandardGlyphGroup.GlyphGroupNamespace;
            else if(type is IntersectionType)
                return StandardGlyphGroup.GlyphGroupMethod;



            return StandardGlyphGroup.GlyphGroupVariable;

        }

        private StandardGlyphItem getItemVSValue(AccessModifier modifier)
        {
            int currentValue = (int)modifier.ModifierMask;

            switch (modifier.ModifierMask)
            {
                case Modifier.Public:
                    return StandardGlyphItem.GlyphItemPublic;
                case Modifier.Protected:
                    return StandardGlyphItem.GlyphItemProtected;
                case Modifier.Private:
                    return StandardGlyphItem.GlyphItemPrivate;
                case Modifier.Internal:
                    return StandardGlyphItem.GlyphItemInternal;
                default:
                    if (currentValue == this.publicStaticAccess || currentValue == this.publicVirtualAccess || currentValue == this.publicAbstractAccess)
                        return StandardGlyphItem.GlyphItemPublic;
                    return 0;
            }
        }
    }
}
