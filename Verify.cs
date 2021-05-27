using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Verify : Actions
    {
        int dirVar, limitValue;

        public Verify(string resultExp, int limitValue ,SymbolTable _st)
        {
            this.limitValue = limitValue;
            this.dirVar = _st.getDir(resultExp);
        }

        public override string ToString()
        {
            return "verify " + dirVar + " " + limitValue;
        }
    }
}
