using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Return : Actions
    {
        string outVar;

        public Return(string outVar)
        {
            this.outVar = outVar;
        }

        public override string ToString()
        {
            return "return " + outVar;
        }
    }
}
