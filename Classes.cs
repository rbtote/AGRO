using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    public class Classes
    {
        public int quadIndex;
        public int intCount, floatCount, charCount, stringCount;
        public int methodCount;
        public Dictionary<string, int> variables = new Dictionary<string, int>();   // to store name and type
        public Classes parentClass = null;
        public Classes(int quadIndex)
        {
            this.quadIndex = quadIndex;
            intCount = floatCount = charCount = stringCount = 0;
        }

        public void setParentClass(Classes parentClass)
        {
            this.parentClass = parentClass;
        }

        public void copyParentVars()
        {
            foreach (string key in parentClass.variables.Keys)
            {
                variables[key] = parentClass.variables[key];
            }
        }

        public void setClassVars(SymbolTable st)
        {
            foreach (string key in st.symbols.Keys)
            {
                // TYPES:   t_int = 1, t_float = 2, t_char = 3, t_void = 4 ,t_obj = 5, t_string = 6
                // KINDS:   var = 0, func = 1
                switch (st.getType(key))
                {
                    // INT
                    case 1:
                        variables[key] = 1;
                        intCount++;
                        break;
                    // FLOAT
                    case 2:
                        variables[key] = 2;
                        floatCount++;
                        break;
                    // CHAR
                    case 3:
                        variables[key] = 3;
                        charCount++;
                        break;
                    // STRING
                    case 6:
                        stringCount++;
                        break;
                }
            }
            if (parentClass != null)
            {
                copyParentVars();
            }
        }
    }
}
