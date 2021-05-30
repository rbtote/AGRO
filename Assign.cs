using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Assign : Actions
    {
        public string varA, varOut, oper;
        public int typeOut, op, dirA, dirOut;
        public Assign(int op, string varA, string varOut, SymbolTable _st, Dictionary<int, string> dict)
        {
            this.varA = varA;
            this.varOut = varOut;
            this.dirA = _st.getDir(varA);
            this.op = op;
            this.oper = dict[op];
            this.typeOut = Cube.getInstance().outputCube(_st.getType(varA), _st.getType(varOut), op, dict);

        }

        public void setDirOut(SymbolTable _st, string varOut)
        {
            this.dirOut = _st.getDir(varOut);
        }

        public bool isValid()
        {
            return typeOut != -1;
        }

        public override string ToString()
        {
            return oper + " " + dirA + " " + dirOut;
        }
    }
}
