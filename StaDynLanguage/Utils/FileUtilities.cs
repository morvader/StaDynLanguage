using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using OleInterop = Microsoft.VisualStudio.OLE.Interop;
using System.IO;
using System.Drawing;
using Microsoft.VisualStudio.Text;
using EnvDTE;
using System.Collections.Generic;
using Microsoft.VisualStudio.ComponentModelHost;

namespace StaDynLanguage.Utils
{
    //Implements Singleton
    public class FileUtilities
    {
        static FileUtilities instance = null;

        FileUtilities()
        {
          
        }

        public static FileUtilities Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FileUtilities();
                }
                return instance;
            }
        }

        public string getFilePath(Microsoft.VisualStudio.Text.Editor.IWpfTextView wpfTextView)
        {
            Microsoft.VisualStudio.Text.ITextDocument document;
            if ((wpfTextView == null) ||
                    (!wpfTextView.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(Microsoft.VisualStudio.Text.ITextDocument), out document)))
                return String.Empty;

            // If we have no document, just ignore it.
            if ((document == null) || (document.TextBuffer == null))
                return String.Empty;

            return document.FilePath;
        }

        public string getCurrentOpenDocumentFilePath()
        {
            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            EnvDTE.Document currentDocument = DTEObj.ActiveDocument;

            string filePath = null;
            if (currentDocument != null)
                filePath = Path.GetFullPath(currentDocument.FullName);

            return filePath;
        }

        public bool checkCurrentFileExtension(string extension)
        {
            string currentFile = this.getCurrentOpenDocumentFilePath();

            return Path.GetExtension(currentFile) == extension;
        }

        public Point getCurrentCaretPoint()
        {

            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            EnvDTE.Document currentDocument = DTEObj.ActiveDocument;

            IVsTextView activeView = null;

            ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)DTEObj);

            IVsTextManager vsTextMgr = (IVsTextManager)sp.GetService(typeof(SVsTextManager));

            vsTextMgr.GetActiveView(1, null, out activeView);

            int x = 0, y = 0;
            if (activeView != null)
            {
                activeView.GetCaretPos(out x, out y);
            }

            return new Point(x, y);
        }

        public TextSpan getCurrentTextSpan()
        {
            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            EnvDTE.Document currentDocument = DTEObj.ActiveDocument;

            IVsTextView activeView = null;

            ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)DTEObj);

            IVsTextManager vsTextMgr = (IVsTextManager)sp.GetService(typeof(SVsTextManager));

            vsTextMgr.GetActiveView(1, null, out activeView);

            int x = 0, y = 0;
            if (activeView != null)
            {
                activeView.GetCaretPos(out x, out y);
            }

            var tsdeb = new TextSpan[1] { new TextSpan() };

            try
            {
                activeView.GetWordExtent(x, y, (uint)WORDEXTFLAGS.WORDEXT_CURRENT, tsdeb);
            }
            catch (Exception e)
            {

            }

            //Try with previous character
            if (String.IsNullOrEmpty(tsdeb[0].ToString()))
                activeView.GetWordExtent(x - 1, y, (uint)WORDEXTFLAGS.WORDEXT_CURRENT, tsdeb);

            return tsdeb[0];
        }

        public int getCurrentCaretPosition()
        {

            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            EnvDTE.Document currentDocument = DTEObj.ActiveDocument;

            IVsTextView activeView = null;

            ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)DTEObj);

            IVsTextManager vsTextMgr = (IVsTextManager)sp.GetService(typeof(SVsTextManager));

            vsTextMgr.GetActiveView(1, null, out activeView);
            var currentPoint = this.getCurrentCaretPoint();

            int currentPosition, v;

            activeView.GetNearestPosition(currentPoint.X, currentPoint.Y, out currentPosition, out v);

            return currentPosition;

        }
        public int getPositionFromSpan(int line, int column)
        {
            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            EnvDTE.Document currentDocument = DTEObj.ActiveDocument;

            IVsTextView activeView = null;

            ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)DTEObj);

            IVsTextManager vsTextMgr = (IVsTextManager)sp.GetService(typeof(SVsTextManager));

            vsTextMgr.GetActiveView(1, null, out activeView);
            //var currentPoint = this.getCurrentCaretPoint();

            int currentPosition, v;

            activeView.GetNearestPosition(line, column, out currentPosition, out v);

            return currentPosition;

        }

        public void setCaretPosition(int line, int column)
        {
            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            EnvDTE.Document currentDocument = DTEObj.ActiveDocument;

            IVsTextView activeView = null;

            ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)DTEObj);

            IVsTextManager vsTextMgr = (IVsTextManager)sp.GetService(typeof(SVsTextManager));



            vsTextMgr.GetActiveView(1, null, out activeView);
            if (activeView != null)
            {
                // activeView.UpdateViewFrameCaption();
                if (line == 0) line += 1;
                if (column == 0) column += 1;
                activeView.SetCaretPos(line, column);
                //activeView.PositionCaretForEditing(line, 1);
                //activeView.UpdateViewFrameCaption();
            }

        }

        public Point getCurrentCaretPosition(int currentPosition)
        {

            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            EnvDTE.Document currentDocument = DTEObj.ActiveDocument;

            IVsTextView activeView = null;

            ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)DTEObj);

            IVsTextManager vsTextMgr = (IVsTextManager)sp.GetService(typeof(SVsTextManager));

            vsTextMgr.GetActiveView(1, null, out activeView);

            int x, y;

            activeView.GetLineAndColumn(currentPosition, out x, out y);

            return new Point(x, y);
        }

        [Obsolete]
        public void refreshView()
        {

            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            EnvDTE.Document currentDocument = DTEObj.ActiveDocument;

            IVsTextView activeView = null;

            ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)DTEObj);

            IVsTextManager vsTextMgr = (IVsTextManager)sp.GetService(typeof(SVsTextManager));

            vsTextMgr.GetActiveView(1, null, out activeView);
            if (activeView != null)
            {

                //activeView.UpdateViewFrameCaption();

                //currentDocument.Activate();

                //DTEObj.ActiveWindow.Visible=false;
                //DTEObj.ActiveWindow.Visible=true;
                currentDocument.ActiveWindow.Visible = false;
                currentDocument.ActiveWindow.Visible = true;


                activeView.UpdateViewFrameCaption();
                //activeView.CloseView();

            }
        }

        public void OpenDocument(string file)
        {
            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            var item = DTEObj.Solution.FindProjectItem(file);
            if (item != null)
            {
                item.Open();
                item.Document.Activate();
                
            }
        }

        public bool IsDocumentOpen(string file) {
          var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
          var item = DTEObj.Solution.FindProjectItem(file);
          return item.IsOpen;
          
        }

        public void SaveDocument(string file)
        {
            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            var item = DTEObj.Solution.FindProjectItem(file);
            if (item != null)
            {
                item.Save(file);
            }
        }

        public ITextSnapshot getCurrentTextSnapShot()
        {
            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            EnvDTE.Document currentDocument = DTEObj.ActiveDocument;

            IVsTextView activeView = null;

            ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)DTEObj);

            IVsTextManager vsTextMgr = (IVsTextManager)sp.GetService(typeof(SVsTextManager));

            vsTextMgr.GetActiveView(1, null, out activeView);

            var userData = activeView as IVsUserData;
            if (userData == null)// no text view 
            {
                Console.WriteLine("No text view is currently open");
                return null;
            }
            // In the next 4 statments, I am trying to get access to the editor's view 
            object holder;
            Guid guidViewHost = Microsoft.VisualStudio.Editor.DefGuidList.guidIWpfTextViewHost;
            userData.GetData(ref guidViewHost, out holder);
            var viewHost = (IWpfTextViewHost)holder;

            // Get a snapshot of the current editor's text.
            return viewHost.TextView.TextSnapshot;

        }

        //public int getCurrentCaretPosition()
        //{
        //    var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
        //    EnvDTE.Document currentDocument = DTEObj.ActiveDocument;

        //    IVsTextView activeView = null;

        //    ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)DTEObj);

        //    IVsTextManager vsTextMgr = (IVsTextManager)sp.GetService(typeof(SVsTextManager));

        //    vsTextMgr.GetActiveView(1, null, out activeView);

        //    var userData = activeView as IVsUserData;
        //    if (userData == null)// no text view 
        //    {
        //        Console.WriteLine("No text view is currently open");
        //        return null;
        //    }
        //    // In the next 4 statments, I am trying to get access to the editor's view 
        //    object holder;
        //    Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
        //    userData.GetData(ref guidViewHost, out holder);
        //    var viewHost = (IWpfTextViewHost)holder;

        //    // Get a snapshot of the current editor's text.
        //    return viewHost.TextView.Caret.Position;
        //}
        public IWpfTextView getCurrentTextView()
        {
            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            EnvDTE.Document currentDocument = DTEObj.ActiveDocument;

            IVsTextView activeView = null;

            ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)DTEObj);

            IVsTextManager vsTextMgr = (IVsTextManager)sp.GetService(typeof(SVsTextManager));


            vsTextMgr.GetActiveView(1, null, out activeView);

            var userData = activeView as IVsUserData;
            if (userData == null)// no text view 
            {
                Console.WriteLine("No text view is currently open");
                return null;
            }
            // In the next 4 statments, I am trying to get access to the editor's view 
            object holder;
            Guid guidViewHost = Microsoft.VisualStudio.Editor.DefGuidList.guidIWpfTextViewHost;
            userData.GetData(ref guidViewHost, out holder);
            var viewHost = (IWpfTextViewHost)holder;

            // Get a snapshot of the current editor's text.
            return viewHost.TextView;
        }

        public Project getActiveProject()
        {
            var DTEObj = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            Project activeProject = null;

            if (DTEObj != null)
            {

                Array activeSolutionProjects = DTEObj.ActiveSolutionProjects as Array;
                if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
                {
                    activeProject = activeSolutionProjects.GetValue(0) as Project;
                }
            }

            return activeProject;
        }

        public List<ProjectItem> GetActiveProjectCompileItems()
        {
            List<ProjectItem> items = new List<ProjectItem>();
            Project proj = getActiveProject();
            Property prop = null;

            foreach (ProjectItem item in proj.ProjectItems)
            {
                if (item.FileCount <= 0)
                    continue;
                if (!".stadyn".Equals(Path.GetExtension(item.get_FileNames(1))))
                    continue;
                if (item.Properties == null)
                    continue;
                if ((prop = item.Properties.Item("BuildAction")) == null)
                    continue;
                if (prop.Value == null)
                    continue;
                if ("Compile".Equals(prop.Value.ToString()))
                    items.Add(item);

            }
            return items;
        }
        public Microsoft.VisualStudio.TextManager.Interop.IVsTextView GetIVsTextView(string filePath)
        {
            var dte2 = (EnvDTE80.DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            //var dte2 = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte2;
            Microsoft.VisualStudio.Shell.ServiceProvider serviceProvider = new Microsoft.VisualStudio.Shell.ServiceProvider(sp);

            Microsoft.VisualStudio.Shell.Interop.IVsUIHierarchy uiHierarchy;
            uint itemID;
            Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame windowFrame;
            Microsoft.VisualStudio.Text.Editor.IWpfTextView wpfTextView = null;
            if (Microsoft.VisualStudio.Shell.VsShellUtilities.IsDocumentOpen(serviceProvider, filePath, Guid.Empty,
                                            out uiHierarchy, out itemID, out windowFrame))
            {
                // Get the IVsTextView from the windowFrame.
                return Microsoft.VisualStudio.Shell.VsShellUtilities.GetTextView(windowFrame);
            }

            return null;
        }
        public IWpfTextView GetIWpfTextView(string filePath)
        {
            var dte2 = (EnvDTE80.DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            //var dte2 = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte2;
            Microsoft.VisualStudio.Shell.ServiceProvider serviceProvider = new Microsoft.VisualStudio.Shell.ServiceProvider(sp);

            Microsoft.VisualStudio.Shell.Interop.IVsUIHierarchy uiHierarchy;
            uint itemID;
            Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame windowFrame;
            Microsoft.VisualStudio.Text.Editor.IWpfTextView wpfTextView = null;
            IVsTextView textview = null;

            if (Microsoft.VisualStudio.Shell.VsShellUtilities.IsDocumentOpen(serviceProvider, filePath, Guid.Empty,
                                            out uiHierarchy, out itemID, out windowFrame))
            {
                // Get the IVsTextView from the windowFrame.
                textview = Microsoft.VisualStudio.Shell.VsShellUtilities.GetTextView(windowFrame);
            }

            //IComponentModel componentModel = (IComponentModel)GetService(typeof(SComponentModel));
            if (textview == null) return null;
            IComponentModel componentModel = (IComponentModel)serviceProvider.GetService(typeof(SComponentModel));
            wpfTextView = componentModel.GetService<IVsEditorAdaptersFactoryService>().GetWpfTextView(textview);


            return wpfTextView;
        }
    }




}
