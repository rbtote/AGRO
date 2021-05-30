using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Return : Actions
    {
        string outVar, funcName;
        int dirVar, dirGlobal;

        public Return(string outVar, string funcName, SymbolTable _st)
        {
            this.outVar = outVar;
            this.funcName = funcName;
            this.dirVar = _st.getDir(this.outVar);
            this.dirGlobal = _st.getDir(this.funcName);
        }

        public override string ToString()
        {
            return "return " + dirVar + " " + dirGlobal;
        }
    }
}
