using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Param : Actions
    {
        string varName, paramType;
        int dirVar;
        int count;
        public Param(string paramType, string varName, int count, SymbolTable _st, List<Actions> program)
        {
            this.paramType = paramType;
            this.varName = varName;
            this.dirVar = _st.getDir(varName);
            this.count = count;

            program.Add(this);

            int dims = 1;

            if (_st.getDim1(varName) != 0)
            {
                if (_st.getDim2(varName) != 0)
                {
                    dims = _st.getDim1(varName) * _st.getDim2(varName);
                }
                else
                {
                    dims = _st.getDim1(varName);
                }
            }

            int tmpDirVar = dirVar + 1;
            while (dims > 1)
            {
                program.Add(new Param(paramType, tmpDirVar, count));
                dims--;
                tmpDirVar++;
            }
        }
        public Param(string paramType, int dirVar, int count)
        {
            this.paramType = paramType;
            this.varName = "";
            this.dirVar = dirVar;
            this.count = count;
        }

        public override string ToString()
        {
            return paramType + " " + dirVar + " param" + count;
        }
    }
}
