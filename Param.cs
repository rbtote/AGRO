using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Param : Actions
    {
        string varName;
        int dirVar;
        int count;
        public Param(string varName, int count, SymbolTable _st)
        {
            this.varName = varName;
            this.dirVar = _st.getDir(varName);
            this.count = count;
        }

        public override string ToString()
        {
            return "param " + dirVar + " param" + count;
        }
    }
}
