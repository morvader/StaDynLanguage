// Guids.cs
// MUST match guids.h
using System;

namespace Microsoft.StaDynLanguage_Package
{
    static class GuidList
    {
        public const string guidStaDynLanguage_PackagePkgString = "80de0503-7f18-4393-9623-70fa01e2e130";
        public const string guidStaDynLanguage_PackageCmdSetString = "C5CF6E8A-97A7-4FC4-B9B8-63F183DCF0BA"; //"3565a773-f410-4679-8eb8-607539c795e4";

        public static readonly Guid guidStaDynLanguage_PackageCmdSet = new Guid(guidStaDynLanguage_PackageCmdSetString);
    };
}