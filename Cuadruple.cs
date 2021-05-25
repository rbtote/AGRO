using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Cuadruple : Actions
    {
        public string varA, varB, varOut, oper;
        public int typeOut, op, dirA, dirB, dirOut;
        public Cuadruple(int op, string varA, string varB, string varOut, SymbolTable _st, Dictionary<int, string> dict)
        {
            this.varA = varA;
            this.varB = varB;
            this.varOut = varOut;
            this.dirA = _st.getDir(varA);
            this.dirB = _st.getDir(varB);
            this.op = op;
            this.oper = dict[op];
            this.typeOut = Cube.getInstance().outputCube(_st.getType(varA), _st.getType(varB), op, dict);

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
            return oper + " " + dirA + " " + dirB + " " + dirOut;
        }
    }
}