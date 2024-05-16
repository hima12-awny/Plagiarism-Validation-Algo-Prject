using PlagiarismValidation;

namespace PalgirismValidation
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            Tests tests = new Tests();

            string prompt = "\nEnter the Test Cases level and optionally the case number separated by space:\n" +
                            "[Level Number] [Case Number || NULL to run all] (press any key for exit)\n > ";

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
