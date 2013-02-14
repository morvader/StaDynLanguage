// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace Microsoft.StaDynLanguage_Package
{
    static class PkgCmdIDList
    {
        public const uint cmdidMakeVarStatic = 0x101;
        public const uint cmdidMakeVarDynamic = 0x102;

        public const uint cmdidMakeEverythingStatic = 0x110;
        public const uint cmdidMakeEverythingDynamic = 0x111;

        public const uint cmdidBuildEverythingStatic = 0x120;
        public const uint cmdidBuildEverythingDynamic = 0x121;

        public const uint cmdidDeclareExplicit = 0x130;
        public const uint cmdidDeclareEverythingExplicit = 0x131;

        public const uint cmdidBuildManaged = 0x132;

    };
}