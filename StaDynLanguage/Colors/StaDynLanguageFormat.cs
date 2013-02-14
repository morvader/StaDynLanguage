using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace StaDynLanguage
{
    #region Format definition

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "LineComment")]
    [Name("LineComment")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class lineComment : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "StaDynLanguage" classification type
        /// </summary>
        public lineComment()
        {
            this.DisplayName = "StaDynLineComment"; //human readable version of the name
            this.ForegroundColor = Colors.Green;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TypeDefinition")]
    [Name("TypeDefinition")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class TypeDefinition : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "StaDynLanguage" classification type
        /// </summary>
        public TypeDefinition()
        {
            this.DisplayName = "StaDynTypeDefinition"; //human readable version of the name
            this.ForegroundColor = Color.FromRgb(43, 145, 175);//"#2B91AF";
            //this.ForegroundColor = Colors.HotPink;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "DynamicVar")]
    [Name("DynamicVar")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class DynamicVar : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "StaDynLanguage" classification type
        /// </summary>
        public DynamicVar()
        {
            this.DisplayName = "StaDynDynamicVar"; //human readable version of the name
            this.ForegroundColor = Colors.Red;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Delimiter")]
    [Name("StaDynDelimiter")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class StaDynDelimiter : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "StaDynLanguage" classification type
        /// </summary>
        public StaDynDelimiter()
        {
            this.DisplayName = "StaDynDelimiter"; //human readable version of the name
            this.ForegroundColor = Colors.Black;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Operator")]
    [Name("StaDynOperator")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class StaDynOperator : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "StaDynLanguage" classification type
        /// </summary>
        public StaDynOperator()
        {
            this.DisplayName = "StaDynOperator"; //human readable version of the name
            this.ForegroundColor = Colors.Black;
        }
    }
    #endregion //Format definition

}
