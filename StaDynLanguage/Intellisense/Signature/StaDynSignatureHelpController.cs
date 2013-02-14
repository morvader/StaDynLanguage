using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;

namespace StaDynLanguage.Intellisense.Signature
{
    [Export(typeof(IVsTextViewCreationListener))]
    [Name("StaDyn Signature Help controller")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    [ContentType("StaDynLanguage")]
    internal class TestSignatureHelpCommandProvider : IVsTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterService;

        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import]
        internal ISignatureHelpBroker SignatureHelpBroker;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;

            textView.Properties.GetOrCreateSingletonProperty(
                 () => new StaDynSignatureHelpCommandHandler(textViewAdapter,
                    textView,
                    NavigatorService.GetTextStructureNavigator(textView.TextBuffer),
                    SignatureHelpBroker));
        }

    }


}
