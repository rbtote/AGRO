using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Param : Actions
    {
        string varName,paramType;
        int dirVar;
        int count;
        public Param(string paramType, string varName, int count, SymbolTable _st)
        {
            this.paramType = paramType;
            this.varName = varName;
            this.dirVar = _st.getDir(varName);
            this.count = count;
        }

        public override string ToString()
        {
            return paramType + " " + dirVar + " param" + count;
        }
    }
}
