using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Microsoft.VisualStudio;
using System.Diagnostics;
using System;
using TargetPlatforms;

namespace StaDynLanguage_Project
{
    /// <summary>
    /// Property page for configuration-independant project properties.
    /// </summary>
    [ComVisible(true), Guid(GuidList.guidGeneralPropertyPageString)]
    public class GeneralPropertyPage : AbstractPropertyPage
    {
        #region Fields

        private string assemblyName;
        private string defaultNamespace;
        private TargetPlatform targetPlatform;
        private DynVarOption dynVarOption;

        #endregion

        #region Properties

        /// <summary>
        /// <see cref="StaDyn.StaDynProject.PropertyTag"/>
        /// </summary>
        [DisplayName("Assembly name")]
        [Category("Assembly")]
        [Description("Assembly file name")]
        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; IsDirty = true; }
        }

        /// <summary>
        /// <see cref="StaDyn.StaDynProject.PropertyTag"/>
        /// </summary>
        [DisplayName("Default Namespace")]
        [Category("Project")]
        [Description("Root namespace for new code elements")]
        public string DefaultNamespace
        {
            get { return defaultNamespace; }
            set { defaultNamespace = value; IsDirty = true; }
        }

        /// <summary>
        /// <see cref="StaDyn.StaDynProject.PropertyTag"/>
        /// </summary>
        [DisplayName("Target platform")]
        [Category("Assembly")]
        [Description("Target platform for generated code")]
        public TargetPlatform TargetPlatform
        {
            get { return targetPlatform; }
            set { targetPlatform = value; IsDirty = true; }
        }

        /// <summary>
        /// <see cref="StaDyn.StaDynProject.PropertyTag"/>
        /// </summary>
        [DisplayName("Dynamic option")]
        [Category("Project")]
        [Description("Option for dynamic variables management")]
        public DynVarOption DynVarOption
        {
            get { return dynVarOption; }
            set { dynVarOption = value; IsDirty = true; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// Constructor.
        /// </summary>
        public GeneralPropertyPage() : base("General")
        { 
        }

        #endregion

        #region Overriden Methods

        /// <summary>
        /// Loads this object's property values from current project.
        /// </summary>
        protected override void BindProperties()
        {
            assemblyName = ProjectMgr.GetProjectProperty(PropertyTag.AssemblyName.ToString(), true);
            defaultNamespace = ProjectMgr.GetProjectProperty(PropertyTag.DefaultNamespace.ToString(), true);

            string strTargetPlatform = ProjectMgr.GetProjectProperty(PropertyTag.TargetPlatform.ToString(), true);
            if (strTargetPlatform != null && strTargetPlatform.Length > 0)
            {
                try
                {
                    targetPlatform = (TargetPlatform)Enum.Parse(typeof(TargetPlatform), strTargetPlatform);
                }
                catch (ArgumentException) { /* TODO: Default value? */ }
            }
            string strDynOption = ProjectMgr.GetProjectProperty(PropertyTag.DynVarOption.ToString(), true);
            if (strDynOption != null && strDynOption.Length > 0)
            {
                try
                {
                    dynVarOption = (DynVarOption)Enum.Parse(typeof(DynVarOption), strDynOption);
                }
                catch (ArgumentException) { /* TODO: Default value? */ }
            }
        }

        /// <summary>
        /// Saves this object's property values to current project.
        /// </summary>
        /// <returns>VSConstants.S_OK if success.</returns>
        protected override int ApplyChanges()
        {
            //TODO
            ProjectMgr.SetProjectProperty(PropertyTag.AssemblyName.ToString(), AssemblyName);
            ProjectMgr.SetProjectProperty(PropertyTag.DefaultNamespace.ToString(), DefaultNamespace);
            ProjectMgr.SetProjectProperty(PropertyTag.TargetPlatform.ToString(), TargetPlatform.ToString());
            ProjectMgr.SetProjectProperty(PropertyTag.DynVarOption.ToString(), DynVarOption.ToString());

            IsDirty = false;

            return VSConstants.S_OK;
        }

        #endregion

    }
}