using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    public class SymbolTable
    {
        // Counters for variables
        int globalInt = 0;
        int globalFloat = 2000;
        int globalChar = 4000;
        int globalString = 6000;
        int globalTempInt = 8000;
        int globalTempFloat = 9000;
        int globalTempChar = 10000;
        int globalTempString = 11000;

        //Local variables
        public int localInt = 12000;
        public int localFloat = 16000;
        public int localChar = 20000;
        public int localString = 24000;
        public int localTempInt = 28000;
        public int localTempFloat = 32000;
        public int localTempChar = 36000;
        public int localTempString = 40000;

        //Constant variables
        int constInt = 44000;
        int constFloat = 46000;
        int constChar = 48000;
        int constString = 50000;

        //Pointers
        int pointersMem = 52000;
        /*
          "symbols": {
            id: [type, kind, dir, dim1?0, dim2?0, access:[-1|1]]
            symbols.len(6)
          }
        */

        public SymbolTable parentSymbolTable;
        public Dictionary<string, int[]> symbols = new Dictionary<string, int[]>();
        public Dictionary<string, Dictionary<string, int>> objects = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, string> objectClasses = new Dictionary<string, string>();
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
        /// 
        /// </summary>
        public void updateLocalOffsetsFromParent()
        {
            this.localInt = parentSymbolTable.localInt;
            this.localFloat = parentSymbolTable.localFloat;
            this.localChar = parentSymbolTable.localChar;
            this.localTempInt = parentSymbolTable.localTempInt;
            this.localTempFloat = parentSymbolTable.localTempFloat;
            this.localTempChar = parentSymbolTable.localTempChar;
        }

        /// <summary>
        /// 
        /// </summary>
        public void updateLocalOffsetsToParent()
        {
            parentSymbolTable.localInt = this.localInt;
            parentSymbolTable.localFloat = this.localFloat;
            parentSymbolTable.localChar = this.localChar;
            parentSymbolTable.localTempInt = this.localTempInt;
            parentSymbolTable.localTempFloat = this.localTempFloat;
            parentSymbolTable.localTempChar = this.localTempChar;
        }

        /// <summary>
        /// Adds a symbol to the current symbol table, including type, kind and assigns a direction
        /// </summary>
        /// <param name="name">Name of the variable to add</param>
        /// <param name="type">Type of the variable to add (int,float,char,string)</param>
        /// <param name="kind">Kind of the variable to add (temporal, variable)</param>
        /// <returns></returns>
        public bool putSymbol(string name, int type, int kind, int dim1, int dim2, int access)
        {
            int dir = 0;
            if (symbols.ContainsKey(name))
            {
                return false;
            }
            if (dim1 > 0)
            {
                dir = assignDirArray(type, dim1, dim2);
            }
            else
            {
                dir = assignDir(type, kind);
            }
            int[] symbol = { type, kind, dir, dim1, dim2, access };
            symbols.Add(name, symbol);

            return true;
        }

        public bool putConstantInt(string name, int type, int kind, int value)
        {
            bool flag = false;
            if (id > 0)
                parentSymbolTable.putConstantInt(name, type, kind, value);
            else
                flag = true;
            if (flag)
            {
                if (symbols.ContainsKey(name))
                    return false;
                int dir = assignDir(type, kind);
                int[] symbol = { type, kind, dir, 0, 0, 1 };
                Program.constants[dir] = "" + value;
                symbols.Add(name, symbol);
                return true;
            }
            return true;
        }

        public bool putConstantFloat(string name, int type, int kind, float value)
        {
            bool flag = false;
            if (id > 0)
                parentSymbolTable.putConstantFloat(name, type, kind, value);
            else
                flag = true;
            if (flag)
            {
                if (symbols.ContainsKey(name))
                    return false;
                int dir = assignDir(type, kind);
                int[] symbol = { type, kind, dir, 0, 0, 1 };
                Program.constants[dir] = "" + value;
                symbols.Add(name, symbol);
                return true;
            }
            return true;
        }

        public bool putConstantString(string name, int type, int kind, string value)
        {
            bool flag = false;
            if (id > 0)
                parentSymbolTable.putConstantString(name, type, kind, value);
            else
                flag = true;
            if (flag)
            {
                if (symbols.ContainsKey(name))
                    return false;
                int dir = assignDir(type, kind);
                int[] symbol = { type, kind, dir, 0, 0, 1 };
                Program.constants[dir] = "" + value;
                symbols.Add(name, symbol);
                return true;
            }
            return true;
        }

        public bool putConstantChar(string name, int type, int kind, char value)
        {
            bool flag = false;
            if (id > 0)
                parentSymbolTable.putConstantChar(name, type, kind, value);
            else
                flag = true;
            if (flag)
            {
                if (symbols.ContainsKey(name))
                    return false;
                int dir = assignDir(type, kind);
                int[] symbol = { type, kind, dir, 0, 0, 1 };
                Program.constants[dir] = "" + value;
                symbols.Add(name, symbol);
                return true;
            }
            return true;
        }

        public bool putSymbolArray(string name, int type, int kind, int dim1, int dim2, int access)
        {
            if (symbols.ContainsKey(name))
            {
                return false;
            }
            int dir = assignDirArray(type, dim1, dim2);
            List<int> symbol = new List<int>();
            symbol.Add(type);
            symbol.Add(kind);
            symbol.Add(dir);
            symbol.Add(dim1);
            symbol.Add(dim2);
            symbol.Add(access);
            symbols.Add(name, symbol.ToArray());

            return true;
        }

        public bool putObject(string objName, Classes classObj)
        {
            //Create dictionary for <attribute, directionAssigned
            Dictionary<string, int> attributes = new Dictionary<string, int>();
            //Append objName to objects dictionary
            objects[objName] = attributes;
            // Check all the variables of the class
            //     structure <varName, type>
            foreach (string varName in classObj.symbolsClass.Keys)
            {
                string nameObjVar = objName + "." + varName;
                //Assign direction to all variables of the class, maintaining access
                // t_int = 1, t_float = 2, t_char = 3

                // Jump if
                putSymbol(nameObjVar, classObj.symbolsClass[varName][0], classObj.symbolsClass[varName][1], classObj.symbolsClass[varName][3], classObj.symbolsClass[varName][4], classObj.symbolsClass[varName][5]);
                //Ej:     miCarro.Velocidad, type=2, kind=0, dim1=0, dim2=0, access=-1

                //Save these directions in the objects dictionary
                attributes[varName] = getDir(nameObjVar);
            }
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

        public int getAccess(string name)
        {
            return getSymbol(name)[5];
        }

        public int getDim1(string name)
        {
            return getSymbol(name)[3];
        }

        public int getDim2(string name)
        {
            return getSymbol(name)[4];
        }

        /// <summary>
        /// Assigns a virutal direction to a variable, depending on its type, kind and the symboltable level (for globals)
        /// </summary>
        /// <param name="type">The variable type</param>
        /// <param name="kind">The variable kind</param>
        /// <returns> A direction where the variable will be assigned </returns>
        private int assignDir(int type, int kind)
        {
            const int var = 0, temporal = 2, pointer = 3, constant = 4, array = 5;                            // Kind
            const int t_int = 1, t_float = 2, t_char = 3, t_string = 6;     // Type
            int dir = 0;
            if (id == 0)     //Global table
            {
                switch (kind)
                {
                    case temporal:
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
                        break;
                    case constant:
                        switch (type)
                        {
                            case t_int:
                                dir = constInt;
                                constInt++;
                                break;
                            case t_float:
                                dir = constFloat;
                                constFloat++;
                                break;
                            case t_char:
                                dir = constChar;
                                constChar++;
                                break;
                            case t_string:
                                dir = constString;
                                constString++;
                                break;
                        }
                        break;
                    case var:
                    case array:
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
                            case t_string:
                                dir = globalString;
                                globalString++;
                                break;
                        }
                        break;
                }
            }
            else
            {
                switch (kind)
                {
                    case var:
                    case array:
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
                            case t_string:
                                dir = localString;
                                localString++;
                                break;
                        }
                        break;
                    case temporal:
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
                        break;
                    case constant:
                        switch (type)
                        {
                            case t_int:
                                dir = constInt;
                                constInt++;
                                break;
                            case t_float:
                                dir = constFloat;
                                constFloat++;
                                break;
                            case t_char:
                                dir = constChar;
                                constChar++;
                                break;
                            case t_string:
                                dir = constString;
                                constString++;
                                break;
                        }
                        break;

                }
            }

            if (kind == pointer)
            {
                dir = pointersMem;
                pointersMem++;
            }
            return dir;
        }


        private int assignDirArray(int type, int dim1, int dim2)
        {
            const int t_int = 1, t_float = 2, t_char = 3;     // Type
            int dir = 0;
            int size = dim2 > 0 ? dim1 * dim2 : dim1;
            if (id == 0)     //Global table
            {
                switch (type)
                {
                    case t_int:
                        dir = globalInt;
                        globalInt += size;
                        break;
                    case t_float:
                        dir = globalFloat;
                        globalFloat += size;
                        break;
                    case t_char:
                        dir = globalChar;
                        globalChar += size;
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case t_int:
                        dir = localInt;
                        localInt += size;
                        break;
                    case t_float:
                        dir = localFloat;
                        localFloat += size;
                        break;
                    case t_char:
                        dir = localChar;
                        localChar += size;
                        break;
                }
            }
            return dir;
        }
    }
}
