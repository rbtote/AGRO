using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Param : Actions
    {
        string varName;
        int count;
        public Param(string varName, int count)
        {
            this.varName = varName;
            this.count = count;
        }

        public override string ToString()
        {
            return "param " + varName + " param" + count;
        }
    }
}
