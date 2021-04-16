using System;
using System.Collections.Generic;
using System.Text;

namespace AGRO_GRAMM
{
    class SymbolTable
    {

        /*
          "symbols": {
            id: [type, kind]
          }
        */

        public SymbolTable parentSymbolTable;
        private Dictionary<string, int[]> symbols = new Dictionary<string, int[]>();
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

        public SymbolTable newChildSymbolTable()
        {
            return new SymbolTable(this);
        }

        public bool putSymbol(string name, int type, int kind)
        {
            if (symbols.ContainsKey(name))
            {
                return false;
            }

            int[] symbol = { type, kind };
            symbols.Add(name, symbol);

            return true;
        }

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
    }
}
