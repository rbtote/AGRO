using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Cuadruple : Actions
    {
        public string varA, varB, varOut;
        public int typeOut, op;
        public Cuadruple(int op, string varA, string varB, string varOut, SymbolTable _st, Dictionary<int, string> dict)
        {
            this.varA = varA;
            this.varB = varB;
            this.varOut = varOut;
            this.op = op;
            this.typeOut = Cube.getInstance().outputCube(_st.getType(varA), _st.getType(varB), op, dict);
        }

        public bool isValid()
        {
            return typeOut != -1;
        }

        public override string ToString()
        {
            return op + " " + varA + " " + varB + " " + varOut;
        }
    }
}
