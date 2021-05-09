using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class Function
    {
        public int variableCount, tmpCount, quadIndex;
        public List<int> parameterTypes;
        public Function(int quadIndex)
        {
            this.quadIndex = quadIndex;
            parameterTypes = new List<int>();
        }
    }
}
