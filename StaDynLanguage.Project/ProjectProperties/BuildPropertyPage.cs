using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Microsoft.VisualStudio;

namespace StaDynLanguage_Project
{
    /// <summary>
    /// Property page for configuration-dependant project properties.
    /// </summary>
    [ComVisible(true), Guid(GuidList.guidBuildPropertyPageString)]
    public class BuildPropertyPage : AbstractPropertyPage
    {
        #region Properties

        private string outputPath;
        /// <summary>
        /// <see cref="StaDyn.StaDynProject.PropertyTag"/>
        /// </summary>
        [DisplayName("Output path")]
        [Category("Generation")]
        [Description("Path for output files")]
        public string OutputPath
        {
            get { return outputPath; }
            set { outputPath = value; IsDirty = true; }
        }

        #endregion
        
        #region Ctor

        /// <summary>
        /// Constructor.
        /// </summary>
        public BuildPropertyPage() : base("Build")
        {            
        }

        #endregion

        #region Overriden Methods

        /// <summary>
        /// Loads this object's property values from current project.
        /// </summary>
        protected override void BindProperties()
        {
            outputPath = ProjectMgr.GetProjectProperty(PropertyTag.OutputPath.ToString(), true);
        }

        /// <summary>
        /// Saves this object's property values to current project.
        /// </summary>
        /// <returns>VSConstants.S_OK if success.</returns>
        protected override int ApplyChanges()
        {
            ProjectMgr.SetProjectProperty(PropertyTag.OutputPath.ToString(), OutputPath);

            IsDirty = false;

            return VSConstants.S_OK;
        }

        #endregion

    }
}