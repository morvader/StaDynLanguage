using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.StaDynLanguage_Package.Commads;
using StaDynLanguage;
using StaDynLanguage.Utils;

namespace Microsoft.StaDynLanguage_Package
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidStaDynLanguage_PackagePkgString)]

    //[ProvideMenuResource(1000, 1)]
    [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")]
    public sealed class StaDynLanguage_PackagePackage : Package, IOleComponent
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public StaDynLanguage_PackagePackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        private CommandHandler commandHandler;

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            oleComponentInitialize();

            commandHandler = new CommandHandler();
            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {

                // Create the command for the menu item.
                CommandID makeVarDynamicCmdId = new CommandID(GuidList.guidStaDynLanguage_PackageCmdSet, (int)PkgCmdIDList.cmdidMakeVarDynamic);
                OleMenuCommand makeVarDynamicMenuItem = new OleMenuCommand(commandHandler.MakeVarDynamicCommand, makeVarDynamicCmdId);
                makeVarDynamicMenuItem.BeforeQueryStatus += new EventHandler(commandHandler.MakeVarDynamicBeforeQueryStatus);
                mcs.AddCommand(makeVarDynamicMenuItem);
                CommandID makeVarStaticCmdId = new CommandID(GuidList.guidStaDynLanguage_PackageCmdSet, (int)PkgCmdIDList.cmdidMakeVarStatic);
                OleMenuCommand makeVarStaticMenuItem = new OleMenuCommand(commandHandler.MakeVarStaticCommand, makeVarStaticCmdId);
                makeVarStaticMenuItem.BeforeQueryStatus += new EventHandler(commandHandler.MakeVarStaticBeforeQueryStatus);
                mcs.AddCommand(makeVarStaticMenuItem);

                CommandID makeEverythingStaticCmdId = new CommandID(GuidList.guidStaDynLanguage_PackageCmdSet, (int)PkgCmdIDList.cmdidMakeEverythingStatic);
                OleMenuCommand makeEverythingStaticMenuItem = new OleMenuCommand(commandHandler.MakeEverythingStaticCommand, makeEverythingStaticCmdId);
                makeEverythingStaticMenuItem.BeforeQueryStatus += new EventHandler(commandHandler.MakeEverythingStaticBeforeQueryStatus);
                mcs.AddCommand(makeEverythingStaticMenuItem);
                CommandID makeEverythingDynamicCmdId = new CommandID(GuidList.guidStaDynLanguage_PackageCmdSet, (int)PkgCmdIDList.cmdidMakeEverythingDynamic);
                OleMenuCommand makeEverythingDynamicMenuItem = new OleMenuCommand(commandHandler.MakeEverythingDynamicCommand, makeEverythingDynamicCmdId);
                makeEverythingDynamicMenuItem.BeforeQueryStatus += new EventHandler(commandHandler.MakeEverythingDynamicBeforeQueryStatus);
                mcs.AddCommand(makeEverythingDynamicMenuItem);

                CommandID declareExplicitCmdId = new CommandID(GuidList.guidStaDynLanguage_PackageCmdSet, (int)PkgCmdIDList.cmdidDeclareExplicit);
                OleMenuCommand declareExplicitMenuItem = new OleMenuCommand(commandHandler.DeclareExplicitCommand, declareExplicitCmdId);
                declareExplicitMenuItem.BeforeQueryStatus += new EventHandler(commandHandler.DeclareExplicitBeforeQueryStatus);
                mcs.AddCommand(declareExplicitMenuItem);
                CommandID declareEverythingExplicitCmdId = new CommandID(GuidList.guidStaDynLanguage_PackageCmdSet, (int)PkgCmdIDList.cmdidDeclareEverythingExplicit);
                OleMenuCommand declareEverythingExplicitMenuItem = new OleMenuCommand(commandHandler.DeclareEverythingExplicitCommand, declareEverythingExplicitCmdId);
                declareEverythingExplicitMenuItem.BeforeQueryStatus += new EventHandler(commandHandler.DeclareEverythingExplicitBeforeQueryStatus);
                mcs.AddCommand(declareEverythingExplicitMenuItem);

                CommandID buildEverythingStaticCmdId = new CommandID(GuidList.guidStaDynLanguage_PackageCmdSet, (int)PkgCmdIDList.cmdidBuildEverythingStatic);
                OleMenuCommand buildEverythingStaticMenuItem = new OleMenuCommand(commandHandler.BuildEverythingStaticCommand, buildEverythingStaticCmdId);
                buildEverythingStaticMenuItem.BeforeQueryStatus += new EventHandler(commandHandler.BuildEverythingStaticBeforeQueryStatus);
                mcs.AddCommand(buildEverythingStaticMenuItem);
                CommandID buildEverythingDynamicCmdId = new CommandID(GuidList.guidStaDynLanguage_PackageCmdSet, (int)PkgCmdIDList.cmdidBuildEverythingDynamic);
                OleMenuCommand buildEverythingDynamicMenuItem = new OleMenuCommand(commandHandler.BuildEverythingDynamicCommand, buildEverythingDynamicCmdId);
                buildEverythingDynamicMenuItem.BeforeQueryStatus += new EventHandler(commandHandler.BuildEverythingDynamicBeforeQueryStatus);
                mcs.AddCommand(buildEverythingDynamicMenuItem);

                CommandID buildManagedCmdId = new CommandID(GuidList.guidStaDynLanguage_PackageCmdSet, (int)PkgCmdIDList.cmdidBuildManaged);
                OleMenuCommand buildManagedMenuItem = new OleMenuCommand(commandHandler.BuildManagedCommand, buildManagedCmdId);
                buildManagedMenuItem.BeforeQueryStatus += new EventHandler(commandHandler.BuildManagedBeforeQueryStatus);
                mcs.AddCommand(buildManagedMenuItem);
            }
        }
        #endregion




        #region IOleComponent Members

        private uint m_componentID;

        private void oleComponentInitialize()
        {
            Trace.WriteLine("Registering timer");
            // Register a timer to call our language service during
            // idle periods.
            IOleComponentManager mgr = GetService(typeof(SOleComponentManager))
                                       as IOleComponentManager;
            if (m_componentID == 0 && mgr != null)
            {
                OLECRINFO[] crinfo = new OLECRINFO[1];
                crinfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
                crinfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime |
                                              (uint)_OLECRF.olecrfNeedPeriodicIdleTime;
                crinfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal |
                                              (uint)_OLECADVF.olecadvfRedrawOff |
                                              (uint)_OLECADVF.olecadvfWarningsOff;
                crinfo[0].uIdleTimeInterval = 1000;
                int hr = mgr.FRegisterComponent(this, crinfo, out m_componentID);
            }



        }

        public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return 1;
        }

        public int FDoIdle(uint grfidlef)
        {
            bool bPeriodic = (grfidlef & (uint)_OLEIDLEF.oleidlefPeriodic) != 0;
            IOleComponentManager cmpMgr = (IOleComponentManager)Package.GetGlobalService(typeof(SOleComponentManager));

            if (bPeriodic && cmpMgr != null)
            {
                //var changes = FileUtilities.Instance.getCurrentTextSnapShot().Version.Changes;
                
                ////var changes = FileUtilities.Instance.GetIWpfTextView((string)document).TextSnapshot.Version.Changes;
                //if (changes != null)
                //{
                //    StaDynParser parser = new StaDynParser();
                //    parser.parseAll();
                //    SourceHelper.refreshHighlighting();
                //}

              Trace.WriteLine("Entering timer");

                StaDynParser parser = new StaDynParser();
                parser.parseAll();
                SourceHelper.refreshHighlighting();

                cmpMgr.FRevokeComponent(m_componentID);

            }

            return 0;
        }

        public int FPreTranslateMessage(MSG[] pMsg)
        {
            return 0;
        }

        public int FQueryTerminate(int fPromptUser)
        {
            return 1;
        }

        public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return 0;
        }

        public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }

        public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved)
        {

        }

        public void OnAppActivate(int fActive, uint dwOtherThreadID)
        {

        }

        public void OnEnterState(uint uStateID, int fEnter)
        {

        }

        public void OnLoseActivation()
        {

        }

        public void Terminate()
        {

        }

        #endregion
    }
}
