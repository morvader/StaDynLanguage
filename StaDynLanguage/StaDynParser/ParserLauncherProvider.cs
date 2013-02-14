using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using StaDynLanguage.Utils;

namespace StaDynLanguage
{
    /// <summary>
    /// Saves an intance of <see cref="ParserRunner"/> in the properties of the text buffer when the text view is created.
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("StadynLanguage")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class ParserLauncherProvider : IWpfTextViewCreationListener
    {
        public void TextViewCreated(IWpfTextView textView)
        {
            string fileName=FileUtilities.Instance.getFilePath(textView);
          
            textView.TextBuffer.Properties.GetOrCreateSingletonProperty<ParserLauncher>(() => new ParserLauncher(textView.TextBuffer,fileName));
        }
    }
}