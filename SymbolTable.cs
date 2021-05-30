﻿using System;
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

        //Local variables
        int localInt = 28001;
        int localFloat = 30001;
        int localChar = 32001;
        int localTempInt = 34001;
        int localTempFloat = 36001;
        int localTempChar = 38001;

        //Constant variables
        int constInt = 42001;
        int constFloat = 44001;
        int constChar = 46001;
        int constString = 48001;

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
        public Dictionary<string, Dictionary<string, int>> objects = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, SymbolTable> objectsContext = new Dictionary<string, SymbolTable>();
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
        public bool putSymbol(string name, int type, int kind, int access)
        {
            if (symbols.ContainsKey(name))
            {
                return false;
            }
            int dir = assignDir(type, kind);
            int[] symbol = { type, kind, dir, access };
            symbols.Add(name, symbol);

            return true;
        }

        public bool putConstantInt(string name, int type, int kind, int value)
        {
            bool flag = false;
            if(id > 0)
                parentSymbolTable.putConstantInt(name, type, kind, value);
            else
                flag = true;
            if (flag)
            {
                if (symbols.ContainsKey(name))
                    return false;
                int dir = assignDir(type, kind);
                int[] symbol = { type, kind, dir };
                Program.constants[dir] = ""+value;
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
                int[] symbol = { type, kind, dir };
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
                int[] symbol = { type, kind, dir };
                Program.constants[dir] = "" + value;
                symbols.Add(name, symbol);
                return true;
            }
            return true;
        }

        public bool putSymbolArray(string name, int type, int kind, int dim1, int dim2)
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
            if (dim2 > 0)
            {
                symbol.Add(dim2);
            }
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
            foreach (string varName in classObj.variables.Keys)
            {
                string nameObjVar = objName + "." + varName;
                //Assign direction to all variables of the class, except for private ones
                if (classObj.variables[varName][1] == 1)
                {
                    // t_int = 1, t_float = 2, t_char = 3
                    putSymbol(nameObjVar, classObj.variables[varName][0], 0, 1);
                    //Ej:     miCarroVelocidad, 2, 0

                    //Save these directions in the objects dictionary
                    attributes[varName] = getDir(nameObjVar);
                }
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
            return getSymbol(name)[3];
        }

        /// <summary>
        /// Assigns a virutal direction to a variable, depending on its type, kind and the symboltable level (for globals)
        /// </summary>
        /// <param name="type">The variable type</param>
        /// <param name="kind">The variable kind</param>
        /// <returns> A direction where the variable will be assigned </returns>
        private int assignDir(int type, int kind)
        {
            const int temporal = 2, pointer = 3, constant = 4, array = 5;                            // Kind
            const int t_int = 1, t_float = 2, t_char = 3, t_string = 6;     // Type
            int dir = 0;
            if(id == 0)     //Global table
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
                    default:
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
                        break;

                }
            }
            else
            {
                switch (kind)
                {
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
                    default:
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
            int size = dim2 > 0 ? dim1*dim2 : dim1;
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
