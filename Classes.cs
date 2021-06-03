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
        //                      id: [type, kind, dir, dim1?0, dim2?0, access:[-1|1]]
        public Dictionary<string, int[]> symbolsClass = new Dictionary<string, int[]>();   // to store name, type and access
        public Classes parentClass = null;
        public Classes(int quadIndex)
        {
            this.quadIndex = quadIndex;
            intCount = floatCount = charCount = stringCount = 0;
        }

        public void setParentClass(Classes parentClass)
        {
            this.parentClass = parentClass;
            copyParentVars();
        }

        public void copyParentVars()
        {
            foreach (string key in parentClass.symbolsClass.Keys)
            {
                symbolsClass[key] = parentClass.symbolsClass[key];
            }
        }

        /// <summary>
        /// Sets to the class in the dirClasse the values of the symbolTable
        /// </summary>
        /// <param name="st">The symbol table of the class</param>
        public void setClassVars(SymbolTable st)
        {
            foreach (string key in st.symbols.Keys)
            {


                // TYPES:   t_int = 1, t_float = 2, t_char = 3, t_void = 4 ,t_obj = 5, t_string = 6
                // KINDS:   var = 0, func = 1
                // ACCESS:  public = 1, private = -1
                //              [type, kind, dir, dim1?0, dim2?0, access:[-1|1]]

                symbolsClass[key] = st.getSymbol(key);

                // Omit func allocation
                if (st.symbols[key][1] == 1) continue;

                int dims = 1;

                if (st.getDim1(key) != 0)
                {
                    if (st.getDim2(key) != 0)
                    {
                        dims = st.getDim1(key) * st.getDim2(key);
                    }
                    else
                    {
                        dims = st.getDim1(key);
                    }
                }



                switch (st.getType(key))
                {
                    // INT
                    case 1:
                        intCount += dims;
                        break;
                    // FLOAT
                    case 2:
                        floatCount += dims;
                        break;
                    // CHAR
                    case 3:
                        charCount += dims;
                        break;
                    // VOID
                    case 4:
                        break;
                    // STRING
                    case 6:
                        stringCount += dims;
                        break;
                }
            }

        }
    }
}
