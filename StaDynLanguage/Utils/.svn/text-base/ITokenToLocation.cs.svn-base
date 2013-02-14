using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ErrorManagement;
using antlr;

namespace StaDynLanguage.Utils
{
    class ITokenToLocation
    {
         //Implements Singleton
        static ITokenToLocation instance = null;

        ITokenToLocation()
        {
        }

        public static ITokenToLocation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ITokenToLocation();
                }
                return instance;
            }
        }

        public Location convertToLocation(IToken token){
             return new Location(token.getFilename(),token.getLine(),token.getColumn());
        }
    }
}
