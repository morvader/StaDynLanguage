using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;

namespace StaDynLanguage
{
    class CompletionComparer : IEqualityComparer<Completion>
    {
        #region IEqualityComparer<Completion> Members

        public bool Equals(Completion x, Completion y)
        {
            return x.DisplayText == y.DisplayText;
        }

        public int GetHashCode(Completion obj)
        {
            return obj.DisplayText.GetHashCode();
        }

        #endregion

    }
}
