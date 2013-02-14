using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Project;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;


namespace StaDynLanguage_Project
{
    [Guid(GuidList.guidStaDynProjectFactoryString)]
    class StaDynProjectFactory : ProjectFactory
    {
        private StaDynLanguage_ProjectPackage package;

        public StaDynProjectFactory(StaDynLanguage_ProjectPackage package)
            : base(package)
        {
            this.package = package;
        }

        protected override ProjectNode CreateProject()
        {
            StaDynProjectNode project = new StaDynProjectNode(this.package);

            project.SetSite((IOleServiceProvider)
                ((IServiceProvider)this.package).GetService(
                    typeof(IOleServiceProvider)));
            return project;
        }
    }
}
