using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ErrorManagement;

namespace StaDynLanguage.Utils
{
    class LocationComparer
    {
        //Implements Singleton
        private static LocationComparer instance = new LocationComparer();

        public static LocationComparer Instance
        {
            get { return instance; }
        }

        public bool greaterThan(Location location1, Location location2)
        {
            //if (location1.FileName != location2.FileName)
            //    return false;
            if (location1.Line > location2.Line)
                return true;
            else if (location1.Line == location2.Line && location1.Column > location2.Column)
                return true;
            else if (location1.Line == location2.Line && location1.Column == location2.Column)
                return false;
            return false;
        }

        public bool gt(Location location1, Location location2)
        {
            return this.greaterThan(location1, location2);
        }
        public bool lt(Location location1, Location location2)
        {
            return this.lessThan(location1, location2);
        }
        public bool lessThan(Location location1, Location location2)
        {
            if (location1.GetHashCode() == location2.GetHashCode())
                return false;
            if (!this.greaterThan(location1, location2))
                return true;
            return false;
        }

        public bool equals(Location location1, Location location2)
        {
            return location1.GetHashCode() == location2.GetHashCode();
        }
    }
}
