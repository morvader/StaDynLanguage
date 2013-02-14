using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using EnvDTE;

namespace StaDynLanguage.Utils
{
    class VSEventHandler : IVsRunningDocTableEvents
    {

        // RDT
        uint rdtCookie;
        RunningDocumentTable rdt;
        EnvDTE80.DTE2 DTEObj;

        #region Constructor
        /// <summary>
        /// The event explorer user control constructor.
        /// </summary>
        public VSEventHandler()
        {

            // Advise the RDT of this event sink.
            DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
            ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)DTEObj);
            //IOleServiceProvider sp = 
            //    Package.GetGlobalService(typeof(IOleServiceProvider)) as IOleServiceProvider;
            if (sp == null) return;

            rdt = new RunningDocumentTable(sp);
            if (rdt == null) return;

            rdtCookie = rdt.Advise(this);

            
            DTEObj.Events.BuildEvents.OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(BuildEvents_OnBuildDone);
            DTEObj.Events.SolutionEvents.Opened += new _dispSolutionEvents_OpenedEventHandler(SolutionEvents_Opened);
           
        }

        void SolutionEvents_Opened()
        {
            return;
            //throw new NotImplementedException();
        }

        void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            return;
            //throw new NotImplementedException();
        }
        #endregion

        #region IVsRunningDocTableEvents Members

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            //StaDynParser parser = new StaDynParser();
            //parser.parseAll();

            SourceHelper.refreshHighlighting();

            //Must save file again because refreshHighlighting "dirties" the code.
            string file = FileUtilities.Instance.getCurrentOpenDocumentFilePath();
            FileUtilities.Instance.SaveDocument(file);

            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            object document;
            int res = pFrame.GetProperty((int)__VSFPROPID.VSFPROPID_pszMkDocument, out document);
            //int v= pFrame.IsVisible();
            if (document != null)
            {
                //var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                var docEvents = DTEObj.Events.get_DocumentEvents(document as EnvDTE.Document);
                docEvents.DocumentOpened +=
                     new _dispDocumentEvents_DocumentOpenedEventHandler(Connect_DocumentOpened);

            }

            //if (fFirstShow != 0)
            //{
            //    object document;
            //    int res = pFrame.GetProperty((int)__VSFPROPID.VSFPROPID_pszMkDocument, out document);
            //    //int v= pFrame.IsVisible();
            //    if (document != null)
            //    {
            //        var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            //        var docEvents = DTEObj.Events.get_DocumentEvents(document as EnvDTE.Document);

            //        docEvents.DocumentOpened +=
            //             new _dispDocumentEvents_DocumentOpenedEventHandler(Connect_DocumentOpened);
                    
            //        //StaDynParser parser = new StaDynParser();
            //        //parser.parseAll();
            //        //SourceHelper.refreshHighlighting((string)document);

            //    }
            //}

            return VSConstants.S_OK;
        }
        void Connect_DocumentOpened(Document Document)
        {
            StaDynParser parser = new StaDynParser();
            parser.parseAll();
            //Document.Activate();
            SourceHelper.refreshHighlighting(Document.FullName);
            
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        #endregion

    }
}
