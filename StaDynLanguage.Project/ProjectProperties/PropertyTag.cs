using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaDynLanguage_Project
{
    /// <summary>
    /// Project property names.
    /// </summary>
    public enum PropertyTag
    {
        /// <summary>
        /// Name of the assembly that will be generated.
        /// </summary>
        AssemblyName,
        /// <summary>
        /// Default namespace for new code files in the project.
        /// </summary>
        DefaultNamespace,
        /// <summary>
        /// Target platform for the compiler.
        /// </summary>
        TargetPlatform,
        /// <summary>
        /// Option for the dynamic vars management for the compiler.
        /// </summary>
        DynVarOption,
        /// <summary>
        /// Path for the output of the project: Generated code, temp files, etc.
        /// </summary>
        OutputPath,
        /// <summary>
        /// Path were StaDyn helper library files are located.
        /// </summary>
        LibPath,

        StaDynPath
    }
}
