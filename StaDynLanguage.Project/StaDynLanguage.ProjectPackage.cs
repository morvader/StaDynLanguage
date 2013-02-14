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
using Microsoft.VisualStudio.Project;
using Microsoft.StaDynLanguage_Project.FileRegistration;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using StaDyn.StaDynProject;
using System.Reflection;

namespace StaDynLanguage_Project
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

    //[ProvideProjectFactory(
    //typeof(StaDynProjectFactory),
    //"StaDyn Project",
    //"StaDyn Project Files (*.sdproj);*.sdproj",
    //"sdproj", "sdproj",
    //@"C:\Users\Moreno\Documents\Visual Studio 2010\Projects\StaDynLanguage\StaDynLanguage.Project\Templates\Projects\StaDyn",
    //LanguageVsTemplate = "StaDynProject",
    //NewProjectRequireNewFolderVsTemplate = false)]

    //Set the projectsTemplatesDirectory to a non-existant path to prevent VS from including the working directory as a valid template path
    [ProvideProjectFactory(typeof(StaDynProjectFactory), "StaDyn", "StaDyn Project Files (*.sdproj);*.sdproj", "sdproj", "sdproj", ".\\NullPath", LanguageVsTemplate = "StaDyn")]

    [ProvideObject(typeof(GeneralPropertyPage))]
    [ProvideObject(typeof(BuildPropertyPage))]

    [Guid(GuidList.guidStaDynLanguage_ProjectPkgString)]
    [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")]
    public sealed class StaDynLanguage_ProjectPackage : ProjectPackage
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public StaDynLanguage_ProjectPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            this.RegisterProjectFactory(new StaDynProjectFactory(this));
            this.fileRegistration();
        }
        #endregion


        public override string ProductUserContext
        {
            get { throw new NotImplementedException(); }
        }

        private void fileRegistration()
        {
            string VS2010Path = Environment.GetCommandLineArgs()[0];

            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string StaDynExtensionPath= System.IO.Path.GetDirectoryName(path);
            string StaDynClassIconPath = System.IO.Path.Combine(StaDynExtensionPath, "SmallIcon.ico");
            string StaDynProjectIconPath = System.IO.Path.Combine(StaDynExtensionPath, "StaDynProjectNode.ico");

            if (!FileRegistration.IsAssociated(".stadyn"))
            {
                var res = MessageBox.Show("Do you want to registry the .stadyn file extension on your system? \n\n You must have administrative rights to complete this action", "Register Files", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    try
                    {
                        FileRegistration.Associate(".stadyn", "StaDynClass.Extension", "StaDyn Class File", StaDynClassIconPath, VS2010Path);
                        FileRegistration.Associate(".sdproj", "StaDynProject.Extension", "StaDyn Project File", StaDynProjectIconPath, VS2010Path);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Problems during registering files: " + e.Message, "Registry Issue", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

    }
}
