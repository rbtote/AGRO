using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    public class Function
    {
        public int variableCount, tmpCount, quadIndex;
        public int intCount, floatCount, charCount, stringCount;
        public int intTempCount, floatTempCount, charTempCount, stringTempCount;
        public List<int> parameterTypes;
        public Function(int quadIndex)
        {
            this.quadIndex = quadIndex;
            intCount = floatCount = charCount = stringCount = 0;
            intTempCount = floatTempCount = charTempCount = stringTempCount = 0;
            parameterTypes = new List<int>();
        }

        public void countVars(SymbolTable st)
        {
            foreach (string key in st.symbols.Keys)
            {
                // TYPES:   t_int = 1, t_float = 2, t_char = 3, t_void = 4 ,t_obj = 5, t_string = 6
                // KINDS:   var = 0, func = 1, temporal = 2;
                switch (st.getType(key))
                {
                    // INT
                    case 1:
                        switch (st.getKind(key))
                        {
                            // VAR
                            case 0:
                                intCount++;
                                break;
                            // VAR TEMP
                            case 2:
                                intTempCount++;
                                break;
                        }
                        break;
                    // FLOAT
                    case 2:
                        switch (st.getKind(key))
                        {
                            // VAR
                            case 0:
                                floatCount++;
                                break;
                            // VAR TEMP
                            case 2:
                                floatTempCount++;
                                break;
                        }
                        break;
                    // CHAR
                    case 3:
                        switch (st.getKind(key))
                        {
                            // VAR
                            case 0:
                                charCount++;
                                break;
                            // VAR TEMP
                            case 2:
                                charTempCount++;
                                break;
                        }
                        break;
                    // STRING
                    case 6:
                        switch (st.getKind(key))
                        {
                            // VAR
                            case 0:
                                stringCount++;
                                break;
                            // VAR TEMP
                            case 2:
                                stringTempCount++;
                                break;
                        }
                        break;
                }
            }
        }
    }
}
