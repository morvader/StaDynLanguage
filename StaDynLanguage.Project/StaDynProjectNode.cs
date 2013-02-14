using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Project;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace StaDynLanguage_Project
{

    public class StaDynProjectNode : ProjectNode
    {
         
        #region Fields

        private StaDynLanguage_ProjectPackage package;
        private int initOffset;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="package">StaDynProjectPackage object</param>
        public StaDynProjectNode(StaDynLanguage_ProjectPackage package)
        {
          Trace.WriteLine("StaDynProjectNode Constructor");
            this.package = package;
            imageOffset = this.ImageHandler.ImageList.Images.Count;

            initOffset = imageOffset;

            foreach (Image img in imageList.Images)
            {
                this.ImageHandler.AddImage(img);
            }
            this.CanProjectDeleteItems = true;

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets GUID of the StaDynProjectFactory
        /// </summary>
        public override Guid ProjectGuid
        {
            get { return GuidList.guidStaDynProjectFactory; }
        }

        /// <summary>
        /// Gets name of the project type.
        /// </summary>
        public override string ProjectType
        {
            get { return "StaDynProjectType"; }
        }

        #endregion

        #region AddFileFromTemplate

        /// <summary>
        /// Called to add a file to the project from a template.
        /// </summary>
        /// <param name="source">Full path of template file</param>
        /// <param name="target">Full path of file once added to the project</param>
        public override void AddFileFromTemplate(string source, string target)
        {
            //string nameSpace = this.FileTemplateProcessor.GetFileNamespace(target, this);
            string fileName = Path.GetFileNameWithoutExtension(target);
            string nameSpace = ProjectMgr.GetProjectProperty(PropertyTag.DefaultNamespace.ToString());

            //this.FileTemplateProcessor.AddReplace("$defaultNameSpace$", fileName);
            //this.FileTemplateProcessor.AddReplace("$defaultAssemblyName$", fileName);

            this.FileTemplateProcessor.AddReplace(Resources.TokenNameSpace, nameSpace);
            this.FileTemplateProcessor.AddReplace(Resources.TokenClassName, fileName);

            this.FileTemplateProcessor.UntokenFile(source, target);
            this.FileTemplateProcessor.Reset();
        }

        #endregion

        #region CreateFileNode

        /// <summary>
        /// Create a file node based on an msbuild item.
        /// </summary>
        /// <param name="item">msbuild item</param>
        /// <returns>FileNode added</returns>
        public override FileNode CreateFileNode(ProjectElement item)
        {
            //StaDyn code files (*.stadyn) are wrongly added to project as "Content" project item elements instead of 
            //"Compile". This is a workaround. TODO: Look for registry entry responsible for this behaviour.
            if (Resources.StaDynFileExt.Equals(Path.GetExtension(item.GetFullPathForElement())) && "Content".Equals(item.ItemName))
                item.ItemName = "Compile";
            return base.CreateFileNode(item);

        }

        #endregion

        #region Image handling

        private static ImageList imageList;
        static StaDynProjectNode()
        {
            imageList = Utilities.GetImageList(typeof(StaDynProjectNode).Assembly.GetManifestResourceStream(
                "Microsoft.StaDynLanguage_Project.Resources.StaDynProjectNodeDynamisim.bmp"));
        }

        internal static int imageOffset;
        /// <summary>
        /// Gets index for the project node icon.
        /// </summary>
        public override int ImageIndex
        {
            get { return imageOffset + 0; }
        }

        #endregion

        #region Property Pages handling

        /// <summary>
        /// List of Guids of the config independent property pages. It is called by the GetProperty for VSHPROPID_PropertyPagesCLSIDList property.
        /// </summary>
        /// <returns>List of Guids of the config independent property pages.</returns>
        protected override Guid[] GetConfigurationIndependentPropertyPages()
        {
            Guid[] result = new Guid[1];
            result[0] = typeof(GeneralPropertyPage).GUID;
            return result;
        }

        /// <summary>
        /// Returns a list of Guids of the configuration dependent property pages. It is called by the GetProperty for VSHPROPID_CfgPropertyPagesCLSIDList property.
        /// </summary>
        /// <returns>List of Guids of the configuration dependent property pages.</returns>
        protected override Guid[] GetConfigurationDependentPropertyPages()
        {
            Guid[] result = new Guid[1];
            result[0] = typeof(BuildPropertyPage).GUID;
            return result;
        }

        #endregion

        #region Load

        /// <summary>
        /// Loads a project file. Called from the factory CreateProject to load the project.
        /// </summary>
        /// <remarks>
        /// This method is overriden in order to include a call to setLibPath. This is needed to
        /// set the project LibPath property and allow MSBuild to find the build DLL (StaDynBuildTasks.dll).
        /// LibPath must be set after creating the project to look into the registry for the needed path.
        /// </remarks>
        /// <param name="fileName">File name of the project that will be created. </param>
        /// <param name="location">Location where the project will be created.</param>
        /// <param name="name">If applicable, the name of the template to use when cloning a new project.</param>
        /// <param name="flags">Set of flag values taken from the VSCREATEPROJFLAGS enumeration.</param>
        /// <param name="iidProject">Identifier of the interface that the caller wants returned. </param>
        /// <param name="canceled">An out parameter specifying if the project creation was canceled</param>
        public override void Load(string fileName, string location, string name, uint flags, ref Guid iidProject, out int canceled)
        {
          Trace.WriteLine("Loading Project");
            base.Load(fileName, location, name, flags, ref iidProject, out canceled);
            checkProjectIcon();

        }

        #endregion

        public void checkProjectIcon()
        {
            string dynVarOption = GetProjectProperty(PropertyTag.DynVarOption.ToString());

            DynVarOption option = (DynVarOption)Enum.Parse(typeof(DynVarOption), dynVarOption);

            switch (option)
            {
                case DynVarOption.Managed:
                    imageOffset = initOffset;
                    break;
                case DynVarOption.EverythingStatic:
                    imageOffset = initOffset + 1;
                    break;
                case DynVarOption.EverythingDynamic:
                    imageOffset = initOffset + 2;
                    break;
                default:
                    break;
            }


            this.ReDraw(UIHierarchyElement.Icon);
        }

        public string getStaDynExtensionPath()
        {
             return GetProjectProperty(PropertyTag.StaDynPath.ToString());
                
        }

        public DynVarOption getDynamismMode()
        {
            string dynVarOption = GetProjectProperty(PropertyTag.DynVarOption.ToString());
            DynVarOption option = (DynVarOption)Enum.Parse(typeof(DynVarOption), dynVarOption);
            return option;
        }

        protected internal override void SetConfiguration(string config)
        {
            //base.SetConfiguration(config);
        }

    }
}
