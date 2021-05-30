using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Return : Actions
    {
        string outVar;
        int dirVar, dirGlobal;

        public Return(string outVar, int dirGlobal, SymbolTable _st)
        {
            this.dirGlobal = dirGlobal;
            this.outVar = outVar;
            this.dirVar = _st.getDir(this.outVar);
        }

        public override string ToString()
        {
            return "return " + dirVar + " " + dirGlobal;
        }
    }
}
