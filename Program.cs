using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AGRO_GRAMM
{
    class Program
    {
        static void Main(string[] args)
        {
            const string fileTest = @"./../../../TestQuads.agro";

            Scanner scanner = new Scanner(fileTest);
            Parser parser = new Parser(scanner);

            parser.Parse();

            if (parser.errors.count == 0) Console.WriteLine("No errors in program");
            Console.WriteLine(parser.errors.count + " errors detected");

            /*
            
            SymbolTable st = new SymbolTable();
            st.putSymbol("a", 1, 0);
            st.putSymbol("b", 1, 0);
            st.putSymbol("c", 2, 0);
            st.putSymbol("d", 3, 0);
            st.putSymbol("t1", 1, 0);
            st.putSymbol("t2", 1, 0);


            Cuadruple[] actions = {
                new Cuadruple("+", "a", "b", "t1", st),
                new Cuadruple("+", "c", "t2", "t2", st),
                new Cuadruple("+", "d", "d", "d", st),
                new Cuadruple("+", "c", "d", "d", st)
            };
            

            for (int i = 0; i < actions.Length; i++)
            {
                Console.WriteLine("Operation " + i + ": " + (actions[i].isValid() ? "" : "NOT") + " VALID");
            }
            
            SymbolTable st = new SymbolTable();

            st.putSymbol("a", 1, 0);
            st.putSymbol("b", 1, 0);
            st.putSymbol("c", 2, 0);
            st.putSymbol("d", 3, 0);
            st.putSymbol("t1", 1, 0);
            st.putSymbol("t2", 1, 0);

            Cuadruple[] actions = {
                
            };


            var actionss = actions.ToList();

            actionss.Add(new Cuadruple("+", "a", "b", "t1", st));

            actions = actionss.ToArray();

            Stack<String> stackOp = new Stack<string>();

            for (int i = 0; i < actions.Length; i++)
            {
                Console.WriteLine("Operation " + i + ": " + (actions[i].isValid() ? "" : "NOT") + " VALID");
            }
            */
        }
    }
}
