using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Microsoft.VisualStudio.Project;

namespace StaDynLanguage_Project
{
    /// <summary>
    /// Provides support for the creation of property pages, implementing EnvDTE80.IInternalExtenderProvider.
    /// </summary>
    [ComVisible(true)]
    public abstract class AbstractPropertyPage : SettingsPage, EnvDTE80.IInternalExtenderProvider
    {
        #region Ctor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Property page name.</param>
        public AbstractPropertyPage(string name)
        {
            this.Name = name;
        }

        #endregion
     
        #region IInternalExtenderProvider Members

        bool EnvDTE80.IInternalExtenderProvider.CanExtend(string extenderCATID, string extenderName, object extendeeObject)
        {
            IVsHierarchy outerHierarchy = HierarchyNode.GetOuterHierarchy(this.ProjectMgr);
            if (outerHierarchy is EnvDTE80.IInternalExtenderProvider)
                return ((EnvDTE80.IInternalExtenderProvider)outerHierarchy).CanExtend(extenderCATID, extenderName, extendeeObject);
            return false;
        }

        object EnvDTE80.IInternalExtenderProvider.GetExtender(string extenderCATID, string extenderName, object extendeeObject, EnvDTE.IExtenderSite extenderSite, int cookie)
        {
            IVsHierarchy outerHierarchy = HierarchyNode.GetOuterHierarchy(this.ProjectMgr);
            if (outerHierarchy is EnvDTE80.IInternalExtenderProvider)
                return ((EnvDTE80.IInternalExtenderProvider)outerHierarchy).GetExtender(extenderCATID, extenderName, extendeeObject, extenderSite, cookie);
            return null;
        }

        object EnvDTE80.IInternalExtenderProvider.GetExtenderNames(string extenderCATID, object extendeeObject)
        {
            IVsHierarchy outerHierarchy = HierarchyNode.GetOuterHierarchy(this.ProjectMgr);
            if (outerHierarchy is EnvDTE80.IInternalExtenderProvider)
                return ((EnvDTE80.IInternalExtenderProvider)outerHierarchy).GetExtenderNames(extenderCATID, extendeeObject);
            return null;
        }

        #endregion
    }
}