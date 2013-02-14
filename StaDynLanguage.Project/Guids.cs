// Guids.cs
// MUST match guids.h
using System;

namespace StaDynLanguage_Project
{
    static class GuidList
    {
        public const string guidStaDynLanguage_ProjectPkgString = "b98dcaac-51e5-4efa-b1f3-dc545fc5305e";
        public const string guidStaDynLanguage_ProjectCmdSetString = "ff2b5efb-1616-4d61-9234-6961cb62cf8c";

        public const string guidStaDynProjectFactoryString = "471EC4BB-E47E-4229-A789-D1F5F83B52D4";
        //public const string guidGeneralPropertyPageString = "1415FE1B-4561-46ca-ACAC-FF766AFE1732";
        public const string guidGeneralPropertyPageString = "E4E8B3A9-1AAE-46F9-B2A4-C921A8D4FDE1";
        public const string guidBuildPropertyPageString = "A362AB69-A705-4a07-88EC-5925E2C177AB";


        public static readonly Guid guidStaDynLanguage_ProjectCmdSet = new Guid(guidStaDynLanguage_ProjectCmdSetString);

        public static readonly Guid guidStaDynProjectFactory =  new Guid(guidStaDynProjectFactoryString);
        public static readonly Guid guidGeneralPropertyPage = new Guid(guidGeneralPropertyPageString);
        public static readonly Guid guidBuildPropertyPage = new Guid(guidBuildPropertyPageString);
    };
}