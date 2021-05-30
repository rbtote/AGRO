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
                // KINDS:   var = 0, func = 1, temporal = 2, pointer = 3

                int dims = 1;

                switch (st.symbols[key].Length)
                {
                    case 4:
                        dims = st.symbols[key][3];
                        break;
                    case 5:
                        dims = st.symbols[key][3] * st.symbols[key][4];
                        break;
                }

                switch (st.getType(key))
                {
                    // INT
                    case 1:
                        switch (st.getKind(key))
                        {
                            // VAR
                            case 0:
                            // FUNC
                            case 1:
                                intCount += dims;
                                break;
                            // VAR TEMP
                            case 2:
                                intTempCount += dims;
                                break;
                        }
                        break;
                    // FLOAT
                    case 2:
                        switch (st.getKind(key))
                        {
                            // VAR
                            case 0:
                            // FUNC
                            case 1:
                                floatCount += dims;
                                break;
                            // VAR TEMP
                            case 2:
                                floatTempCount += dims;
                                break;
                        }
                        break;
                    // CHAR
                    case 3:
                        switch (st.getKind(key))
                        {
                            // VAR
                            case 0:
                            // FUNC
                            case 1:
                                charCount += dims;
                                break;
                            // VAR TEMP
                            case 2:
                                charTempCount += dims;
                                break;
                        }
                        break;
                    // STRING
                    case 6:
                        switch (st.getKind(key))
                        {
                            // VAR
                            case 0:
                            // FUNC
                            case 1:
                                stringCount += dims;
                                break;
                            // VAR TEMP
                            case 2:
                                stringTempCount += dims;
                                break;
                        }
                        break;
                }
            }
        }
    }
}
