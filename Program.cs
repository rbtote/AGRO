using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AGRO_GRAMM
{
    class Program
    {
        public static Dictionary<int, string> constants = new Dictionary<int, string>();

        static void Main(string[] args)
        {
            
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: AGRO.exe programName");
                return;
            }
            

            string programName = args[0];
            //string programName = @".\..\..\..\VM\programs\programArrays";
            string extensionInput = ".agro";
            string extensionOuput = ".code";
            string extensionDirFunc = ".dirfunc";
            string extensionConstants = ".constants";
            string extensionClasses = ".classes";
            string dir = @"";
            string fileTest = dir + programName + extensionInput;

            Scanner scanner = new Scanner(fileTest);
            Parser parser = new Parser(scanner);

            parser.Parse();

            if (parser.errors.count == 0) Console.WriteLine("No errors in program");
            else { Console.WriteLine(parser.errors.count + " errors detected"); return; }


            // Write output code file
            try
            {
                using StreamWriter outputFile = new StreamWriter(dir + programName + extensionInput + extensionOuput);

                // Write in console and output file .agro.out
                foreach (Actions act in parser.program)
                {
                    string line = act.ToString();
                    Console.WriteLine(line);
                    outputFile.WriteLine(line);
                }
            }
            catch (IOException)
            {
                throw new FatalError("Cannot open file " + dir + programName + extensionInput + extensionOuput);
            }

            // Write output DirFunc file
            /*
                FUNCTION_COUNT
                NAME QUAD_DIR INT_COUNT FLOAT_COUNT CHAR_COUNT STRING_COUNT INT_TEMP_COUNT FLOAT_TEMP_COUNT CHAR_TEMP_COUNT STRING_TEMP_COUNT
                NAME...(again for each function)
             */
            try
            {
                using StreamWriter outputFile = new StreamWriter(dir + programName + extensionInput + extensionDirFunc);
                int QUAD_DIR, INT_COUNT, FLOAT_COUNT, CHAR_COUNT, STRING_COUNT, INT_TEMP_COUNT, FLOAT_TEMP_COUNT, CHAR_TEMP_COUNT, STRING_TEMP_COUNT, POINTER_COUNT;

                // Global memory
                Function globalScope = new Function(Int32.Parse(parser.program[0].ToString().Split(' ')[1]));
                globalScope.countVars(parser.sTable);
                QUAD_DIR = globalScope.quadIndex;
                INT_COUNT = globalScope.intCount;
                FLOAT_COUNT = globalScope.floatCount;
                CHAR_COUNT = globalScope.charCount;
                STRING_COUNT = globalScope.stringCount;
                INT_TEMP_COUNT = globalScope.intTempCount;
                FLOAT_TEMP_COUNT = globalScope.floatTempCount;
                CHAR_TEMP_COUNT = globalScope.charTempCount;
                STRING_TEMP_COUNT = globalScope.stringTempCount;
                // ADD TO POINTER COUNT
                POINTER_COUNT = globalScope.pointerCount;
                // Write function memory counters in this given order
                outputFile.WriteLine($"_global||{QUAD_DIR} {INT_COUNT} {FLOAT_COUNT} {CHAR_COUNT} {STRING_COUNT} {INT_TEMP_COUNT} {FLOAT_TEMP_COUNT} {CHAR_TEMP_COUNT} {STRING_TEMP_COUNT}");

                // Main memory
                Function mainScope = new Function(Int32.Parse(parser.program[0].ToString().Split(' ')[1]));
                mainScope.countVars(parser.mainSymbolTable);
                QUAD_DIR = mainScope.quadIndex;
                INT_COUNT = mainScope.intCount;
                FLOAT_COUNT = mainScope.floatCount;
                CHAR_COUNT = mainScope.charCount;
                STRING_COUNT = mainScope.stringCount;
                INT_TEMP_COUNT = mainScope.intTempCount;
                FLOAT_TEMP_COUNT = mainScope.floatTempCount;
                CHAR_TEMP_COUNT = mainScope.charTempCount;
                STRING_TEMP_COUNT = mainScope.stringTempCount;
                // ADD TO POINTER COUNT
                POINTER_COUNT += mainScope.pointerCount;
                // Write function memory counters in this given order
                outputFile.WriteLine($"_main||{QUAD_DIR} {INT_COUNT} {FLOAT_COUNT} {CHAR_COUNT} {STRING_COUNT} {INT_TEMP_COUNT} {FLOAT_TEMP_COUNT} {CHAR_TEMP_COUNT} {STRING_TEMP_COUNT}");


                // Clases memory?

                // Functions memory

                foreach (string key in parser.dirFunc.Keys)
                {
                    QUAD_DIR = parser.dirFunc[key].quadIndex;
                    INT_COUNT = parser.dirFunc[key].intCount;
                    FLOAT_COUNT = parser.dirFunc[key].floatCount;
                    CHAR_COUNT = parser.dirFunc[key].charCount;
                    STRING_COUNT = parser.dirFunc[key].stringCount;
                    INT_TEMP_COUNT = parser.dirFunc[key].intTempCount;
                    FLOAT_TEMP_COUNT = parser.dirFunc[key].floatTempCount;
                    CHAR_TEMP_COUNT = parser.dirFunc[key].charTempCount;
                    STRING_TEMP_COUNT = parser.dirFunc[key].stringTempCount;
                    // ADD TO POINTER COUNT
                    POINTER_COUNT += parser.dirFunc[key].pointerCount;

                    string funcParams = String.Join(' ', parser.dirFunc[key].parameterTypes);

                    // Write function memory counters in this given order
                    outputFile.WriteLine($"{key}|{funcParams}|{QUAD_DIR} {INT_COUNT} {FLOAT_COUNT} {CHAR_COUNT} {STRING_COUNT} {INT_TEMP_COUNT} {FLOAT_TEMP_COUNT} {CHAR_TEMP_COUNT} {STRING_TEMP_COUNT}");
                }

                // POINTER MEMORY
                outputFile.WriteLine($"_pointer||{0} {POINTER_COUNT} {0} {0} {0} {0} {0} {0} {0}");
            }
            catch (IOException)
            {
                throw new FatalError("Cannot open file " + dir + programName + extensionInput + extensionDirFunc);
            }

            // Write Constants file
            try
            {
                using StreamWriter outputFile = new StreamWriter(dir + programName + extensionInput + extensionConstants);

                foreach (int constDir in constants.Keys)
                {
                    outputFile.WriteLine($"{constDir} {constants[constDir]}");
                }
            }
            catch (IOException)
            {
                throw new FatalError("Cannot open file " + dir + programName + extensionInput + extensionConstants);
            }

            // Write Classes file
            try
            {
                using StreamWriter outputFile = new StreamWriter(dir + programName + extensionInput + extensionClasses);

                foreach (string key in parser.dirClasses.Keys)
                {
                    outputFile.WriteLine($"{key} {parser.dirClasses[key].intCount} {parser.dirClasses[key].floatCount} {parser.dirClasses[key].charCount} {parser.dirClasses[key].stringCount}");
                }
            }
            catch (IOException)
            {
                throw new FatalError("Cannot open file " + dir + programName + extensionInput + extensionConstants);
            }

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
