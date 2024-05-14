
namespace PalgirismValidation
{

    internal partial class Program
    {

        static void Main(string[] args)
        {

            Tests tests = new Tests();


            string prompt = "\nEnter your choice Test Cases level: " +
                "\n[1] Sample " +
                "\n[2] Easy " +
                "\n[3] Meduim " +
                "\n[4] Hard " +
                "\n[any key for exit] > ";

            Console.Write(prompt);

            ConsoleKeyInfo cki = Console.ReadKey();
            Console.WriteLine();

            while (cki.Key == ConsoleKey.D1 || 
                cki.Key == ConsoleKey.D2 || 
                cki.Key == ConsoleKey.D3 || 
                cki.Key == ConsoleKey.D4)
            {

                int hardniessLevelSelection = cki.KeyChar - '0';

                switch (hardniessLevelSelection)
                {
                    case 1:
                        tests.runSmapleTest();
                        break;

                    case 2:
                    case 3:
                    case 4:
                        tests.runTestCasesLevel(hardniessLevelSelection-1);
                        break;
                }



                Console.WriteLine();
                Console.Write(prompt);
                cki = Console.ReadKey();
                Console.WriteLine();
            }

        }
    }
}
