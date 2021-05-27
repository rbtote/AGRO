using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    public class SymbolTable
    {
        // Counters for variables
        int globalInt = 1001;
        int globalFloat = 5001;
        int globalChar = 9001;
        int globalTempInt = 12001;
        int globalTempFloat = 16001;
        int globalTempChar = 20001;
        int globalTempString = 24001;

        //Local variables
        int localInt = 28001;
        int localFloat = 30001;
        int localChar = 32001;
        int localTempInt = 34001;
        int localTempFloat = 36001;
        int localTempChar = 38001;
        int localTempString = 40001;

        //Constant variables
        int constInt = 42001;
        int constFloat = 44001;
        int constChar = 46001;

        //Pointers
        int pointersMem = 50001;
        /*
         * 
            INT     = 1001-5000, 12001-16000, 28001-30000, 34001-36000 ,42001-44000
            FLOAT   = 5001-9000, 16001-20000, 30001-32000, 36001-38000 ,44001-46000
            CHAR    = 9001-12000, 20001-24000, 32001-34000, 38001-40000 ,46001-50000
            STRING  = 24001-28000, 40001-42000
            POINTERS = 50001-x
         * 
          "symbols": {
            id: [type, kind, dir, val?null]
          }
        */

        public SymbolTable parentSymbolTable;
        public Dictionary<string, int[]> symbols = new Dictionary<string, int[]>();
        public int id;

        public SymbolTable()
        {
            parentSymbolTable = null;
            symbols = new Dictionary<string, int[]>();
            id = 0;
        }

        public SymbolTable(SymbolTable parentSymbolTable)
        {
            this.parentSymbolTable = parentSymbolTable;
            symbols = new Dictionary<string, int[]>();
            this.id++;
        }

        /// <summary>
        /// Creates a new symbol table that is the child of the current symbol table
        /// </summary>
        /// <returns> A child symbol table </returns>
        public SymbolTable newChildSymbolTable()
        {
            return new SymbolTable(this);
        }

        /// <summary>
        /// Adds a symbol to the current symbol table, including type, kind and assigns a direction
        /// </summary>
        /// <param name="name">Name of the variable to add</param>
        /// <param name="type">Type of the variable to add (int,float,char,string)</param>
        /// <param name="kind">Kind of the variable to add (temporal, variable)</param>
        /// <returns></returns>
        public bool putSymbol(string name, int type, int kind)
        {
            if (symbols.ContainsKey(name))
            {
                return false;
            }
            int dir = assignDir(type, kind);
            int[] symbol = { type, kind, dir };
            symbols.Add(name, symbol);

            return true;
        }

        /// <summary>
        /// Gets the array of type,kind and direction of a variable in the table
        /// </summary>
        /// <param name="name">Variable to search in the symbol table</param>
        /// <returns>Array of type, kind,dir of the variable</returns>
        public int[] getSymbol(string name)
        {

            if (symbols.ContainsKey(name))
            {
                return symbols[name];
            }
            else
            {
                if (parentSymbolTable != null)
                    return parentSymbolTable.getSymbol(name);
            }

            return null;
        }

        /// <summary>
        /// Gets the type of a variable in the symbol table
        /// </summary>
        /// <param name="name">Name of the variable to get</param>
        /// <returns>The variable type (int)</returns>
        public int getType(string name)
        {
            return getSymbol(name)[0];
        }

        /// <summary>
        /// Gets the kind of a variable in the symbol table
        /// </summary>
        /// <param name="name">Name of the variable to get</param>
        /// <returns>The variable kind (int)</returns>
        public int getKind(string name)
        {
            return getSymbol(name)[1];
        }

        /// <summary>
        /// Gets the direction of a variable in the symbol table
        /// </summary>
        /// <param name="name">Name of the variable to get</param>
        /// <returns>The variable direction assigned (int)</returns>
        public int getDir(string name)
        {
            return getSymbol(name)[2];
        }

        /// <summary>
        /// Assigns a virutal direction to a variable, depending on its type, kind and the symboltable level (for globals)
        /// </summary>
        /// <param name="type">The variable type</param>
        /// <param name="kind">The variable kind</param>
        /// <returns> A direction where the variable will be assigned </returns>
        private int assignDir(int type, int kind)
        {
            const int temporal = 2, pointer = 3;
            const int t_int = 1, t_float = 2, t_char = 3, t_string = 6;
            int dir = 0;
            if (id == 0)     //Global table
            {
                if (kind == temporal)
                {
                    switch (type)
                    {
                        case t_int:
                            dir = globalTempInt;
                            globalTempInt++;
                            break;
                        case t_float:
                            dir = globalTempFloat;
                            globalTempFloat++;
                            break;
                        case t_char:
                            dir = globalTempChar;
                            globalTempChar++;
                            break;
                        case t_string:
                            dir = globalTempString;
                            globalTempString++;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case t_int:
                            dir = globalInt;
                            globalInt++;
                            break;
                        case t_float:
                            dir = globalFloat;
                            globalFloat++;
                            break;
                        case t_char:
                            dir = globalChar;
                            globalChar++;
                            break;
                    }
                }
            }
            else
            {
                if (kind == temporal)
                {
                    switch (type)
                    {
                        case t_int:
                            dir = localTempInt;
                            localTempInt++;
                            break;
                        case t_float:
                            dir = localTempFloat;
                            localTempFloat++;
                            break;
                        case t_char:
                            dir = localTempChar;
                            localTempChar++;
                            break;
                        case t_string:
                            dir = localTempString;
                            localTempString++;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case t_int:
                            dir = localInt;
                            localInt++;
                            break;
                        case t_float:
                            dir = localFloat;
                            localFloat++;
                            break;
                        case t_char:
                            dir = localChar;
                            localChar++;
                            break;
                    }
                }
            }

            if (kind == pointer)
            {
                dir = pointersMem;
                pointersMem++;
            }
            return dir;
        }
    }
}
