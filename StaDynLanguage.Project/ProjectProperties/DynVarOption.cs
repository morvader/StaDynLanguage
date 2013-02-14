using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaDynLanguage_Project
{
    /// <summary>
    /// Option for the dynamic vars management for the compiler.
    /// </summary>
    public enum DynVarOption
    {
        /// <summary>
        /// The compiler must search ".dyn" files for var dynamism.
        /// </summary>
        Managed,
        /// <summary>
        /// The compiler must ignore ".dyn" files and set every var reference to static.
        /// </summary>
        EverythingStatic,
        /// <summary>
        /// The compiler must ignore ".dyn" files and set every var reference to dynamic.
        /// </summary>
        EverythingDynamic
    }
}
