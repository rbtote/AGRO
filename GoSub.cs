using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class GoSub : Actions
    {
        string funcName;
        public GoSub(string funcName)
        {
            this.funcName = funcName;
        }

        public override string ToString()
        {
            return "goSub " + funcName;
        }
    }
}
