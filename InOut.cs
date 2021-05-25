using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class InOut : Actions
    {
        public string varOut, oper;
        public int typeOut, op, dirOut;
        public InOut(int op, string varOut, SymbolTable _st, Dictionary<int, string> dict)
        {
            this.varOut = varOut;
            this.op = op;
            this.oper = dict[op];
            this.typeOut = Cube.getInstance().outputCube(_st.getType(varOut), _st.getType(varOut), op, dict);

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
            return oper + " " + dirOut;
        }
    }
}
