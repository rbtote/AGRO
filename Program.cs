using System;

namespace AGRO_GRAMM
{
    class Program
    {
        static void Main(string[] args)
        {

            const string fileTest = @"C:\Users\rober\OneDrive\Documentos\ITESM\10mo\Compis\AGRO\AGRO_GRAMMAR\AGRO_GRAMM\symbolTest.agro";

            Scanner scanner = new Scanner(fileTest);
            Parser parser = new Parser(scanner);

            parser.Parse();

            if (parser.errors.count == 0) Console.WriteLine("No errors in program");
            Console.WriteLine(parser.errors.count + " errors detected");

        }
    }
}
