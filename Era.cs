using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Era : Actions
    {
        string funcName;
        public Era(string funcName)
        {
            this.funcName = funcName;
        }
        public override string ToString()
        {
            return "era " + funcName;
        }
    }
}
