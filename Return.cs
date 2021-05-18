using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Return : Actions
    {
        string outVar;
        int dirVar;

        public Return(string outVar, SymbolTable _st)
        {
            this.outVar = outVar;
            this.dirVar = _st.getDir(outVar);
        }

        public override string ToString()
        {
            return "return " + dirVar;
        }
    }
}
