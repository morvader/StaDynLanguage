using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using StaDynLanguage_Project;

namespace StaDyn.StaDynProject
{
    /// <summary>
    /// Provides access to project configuration properties and to other related services 
    /// (getting active project, active project files, etc.).
    /// 
    /// Uses Singleton pattern.
    /// </summary>
    public class ProjectConfiguration
    {
        #region Singleton

        private static ProjectConfiguration instance;
        /// <summary>
        /// Get singleton instance.
        /// </summary>
        public static ProjectConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new ProjectConfiguration();
                return instance;
            }
        }
        private ProjectConfiguration() { }

        #endregion

        #region GetProperty

        /// <summary>
        /// Gets a project property. StaDyn projects properties are stored in the XML ".sdproj" file as "Property" elements.
        /// </summary>
        /// <param name="key">Name of the property.</param>
        /// <returns>Value of the property</returns>
        public string GetProperty(string key)
        {
            StaDynProjectNode projectNode = GetActiveProjectNode();
            if (projectNode == null)
              return null;
                //throw new InvalidOperationException("Current project is not a StaDyn project");
            return projectNode.ProjectMgr.GetProjectProperty(key);
        }

        #endregion

        #region SetProperty

        /// <summary>
        /// Sets a project property. StaDyn projects properties are stored in the XML ".sdproj" file as "Property" elements.
        /// </summary>
        /// <param name="key">Name of the property.</param>
        /// <param name="value">Value of the property</param>
        public void SetProperty(string key, string value)
        {
            StaDynProjectNode projectNode = GetActiveProjectNode();
            if (projectNode == null)
                throw new InvalidOperationException("Current project is not a StaDyn project");
            projectNode.ProjectMgr.SetProjectProperty(key, value);
        }

        #endregion

        #region GetActiveProject

        /// <summary>
        /// Gets active project.
        /// </summary>
        /// <returns>Active project or null if failed.</returns>
        public Project GetActiveProject()
        {
            DTE dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SDTE)) as DTE;
            Project activeProject = null;

            if (dte != null)
            {

                Array activeSolutionProjects = dte.ActiveSolutionProjects as Array;
                if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
                {
                    activeProject = activeSolutionProjects.GetValue(0) as Project;
                }
            }

            return activeProject;
        }

        #endregion

        #region GetActiveProjectNode

        /// <summary>
        /// Gets active StaDynProjectNode object if active project is StaDyn Project.
        /// </summary>
        /// <returns>Active StaDyn ProjectNode or null if failed or current project is not StaDyn project.</returns>
        public StaDynProjectNode GetActiveProjectNode()
        {
            var activeProject = GetActiveProject();
            if(activeProject!=null)
                return activeProject.Object as StaDynProjectNode;
            return null;
        }

        #endregion

        #region GetActiveProjectCompileItems

        /// <summary>
        /// Gets project files from the active project whose extension is ".stadyn" and have
        /// their BuildAction property set to "Compile".
        /// </summary>
        /// <returns>File names of active project StaDyn code files.</returns>
        public ProjectItem[] GetActiveProjectCompileItems()
        {
            List<ProjectItem> items = new List<ProjectItem>();
            Project proj = GetActiveProject();
            Property prop = null;

            foreach (ProjectItem item in proj.ProjectItems)
            {
                if (item.FileCount <= 0)
                    continue;
                if(!".stadyn".Equals(Path.GetExtension(item.get_FileNames(1))))
                    continue;
                if(item.Properties==null)
                    continue;
                if((prop=item.Properties.Item("BuildAction"))==null)
                    continue;
                if(prop.Value==null)
                    continue;
                if("Compile".Equals(prop.Value.ToString()))
                    items.Add(item);

            }
            return items.ToArray();
        }

        #endregion

        #region GetActiveProjectFilePath

        /// <summary>
        /// Gets active project file path.
        /// </summary>
        /// <returns>Active project file path.</returns>
        public string GetActiveProjectFilePath()
        {
            Project p = GetActiveProject();
            return p != null ? Path.GetDirectoryName(p.FullName) : null;
        }

        #endregion
    }
}
