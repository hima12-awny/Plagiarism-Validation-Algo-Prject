<<<<<<< HEAD
﻿
namespace PlagiarismValidation
=======

//namespace PalgirismValidation
//{

//    internal partial class Program
//    {

//        static void Main(string[] args)
//        {

//            Tests tests = new Tests();


//            string prompt = "\nEnter your choice Test Cases level: " +
//                "\n[1] Sample " +
//                "\n[2] Easy " +
//                "\n[3] Meduim " +
//                "\n[4] Hard " +
//                "\n[any key for exit] > ";

//            Console.Write(prompt);

//            ConsoleKeyInfo cki = Console.ReadKey();
//            Console.WriteLine();

//            while (cki.Key == ConsoleKey.D1 || 
//                cki.Key == ConsoleKey.D2 || 
//                cki.Key == ConsoleKey.D3 || 
//                cki.Key == ConsoleKey.D4)
//            {

//                int hardniessLevelSelection = cki.KeyChar - '0';

//                switch (hardniessLevelSelection)
//                {
//                    case 1:
//                        tests.runSmapleTest();
//                        break;

//                    case 2:
//                    case 3:
//                    case 4:
//                        tests.runTestCasesLevel(hardniessLevelSelection-1);
//                        break;
//                }



//                Console.WriteLine();
//                Console.Write(prompt);
//                cki = Console.ReadKey();
//                Console.WriteLine();
//            }

//        }
//    }
//}

namespace PalgirismValidation
>>>>>>> a0404217630a331d78b3977388ad2cf37e8337f3
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            Tests tests = new Tests();

            string prompt = "\nEnter the Test Cases level and optionally the case number separated by space:\n" +
                            "[Level Number] [Case Number] (press any key for exit) > ";

            Console.Write(prompt);
            string input = Console.ReadLine();

            while (!string.IsNullOrWhiteSpace(input))
            {
                string[] inputs = input.Split(' ');

                if (inputs.Length >= 1 && int.TryParse(inputs[0], out int level))
                {
                    int caseNumber = -1;
                    if (inputs.Length >= 2)
                    {
                        int.TryParse(inputs[1], out caseNumber);
                    }

                    switch (level)
                    {
                        case 1:
                            tests.runSmapleTest(caseNumber);
                            break;

                        case 2:
                        case 3:
                        case 4:
                            tests.runTestCasesLevel(level-1, caseNumber);
                            break;

                        default:
                            Console.WriteLine("Invalid level number.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input format.");
                }

                Console.WriteLine();
                Console.Write(prompt);
                input = Console.ReadLine();
            }
        }
    }
}
