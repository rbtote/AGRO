using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Goto : Actions
    {
        public string varCond, oper;
        public int dirVarCond;
        public int typeGoto, direction, valid;
        public Goto(int typeGoto, string varCond, SymbolTable _st, Dictionary<int, string> dict)
        {
            this.typeGoto = typeGoto;
            this.oper = dict[typeGoto];
            if (typeGoto != 68) //68 is plain goto
            {
                this.varCond = varCond;
                this.dirVarCond = _st.getDir(varCond);
                this.valid = Cube.getInstance().outputCube(_st.getType(varCond), 1, 17, dict); //The variable received with int(1) equals(17)
            }
            //this.direction = direction;
        }

        public void setDirection(int direction)
        {
            this.direction = direction;
        }

        public override string ToString()
        {
            if(typeGoto != 68)
            {
                return oper + " " + dirVarCond + " " + direction;
            }
            return oper + " " + direction;
        }
    }
}
