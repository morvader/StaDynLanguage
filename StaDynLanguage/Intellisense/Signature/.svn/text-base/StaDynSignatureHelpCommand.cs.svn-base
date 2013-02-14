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
    internal sealed class StaDynSignatureHelpCommandHandler : IOleCommandTarget
    {
        IOleCommandTarget m_nextCommandHandler;
        ITextView m_textView;
        ISignatureHelpBroker m_broker;
        ISignatureHelpSession m_session;
        ITextStructureNavigator m_navigator;

        internal StaDynSignatureHelpCommandHandler(IVsTextView textViewAdapter, ITextView textView, ITextStructureNavigator nav, ISignatureHelpBroker broker)
        {
            this.m_textView = textView;
            this.m_broker = broker;
            this.m_navigator = nav;

            //add this to the filter chain
            textViewAdapter.AddCommandFilter(this, out m_nextCommandHandler);
        }
        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            char typedChar = char.MinValue;

            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
            {
                typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                if (typedChar.Equals('('))
                {
                    //move the point back so it's in the preceding word
                    SnapshotPoint point = m_textView.Caret.Position.BufferPosition - 1;
                    TextExtent extent = m_navigator.GetExtentOfWord(point);
                    string word = extent.Span.GetText();
                    /******************************************************************/
                    //if (word.Equals("add"))
                    if(!String.IsNullOrEmpty(word))
                        m_session = m_broker.TriggerSignatureHelp(m_textView);

                }
                else if (typedChar.Equals(')') && m_session != null)
                {
                    m_session.Dismiss();
                    m_session = null;
                }
            }
            return m_nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }
        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return m_nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }


    }
}
