namespace MBPosHelperConsole
{
    public class MBPosHelper
    {
        public static int Main()
        {
            CommandHelper cmdHelper = new();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Welcome to MBPosHelper!");
            Console.WriteLine();

            cmdHelper.Help();

            string[] args ;
            string? input;

            try
            {
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(":");
                    Console.ForegroundColor = ConsoleColor.White;
                    input = Console.ReadLine();
                    if (input == null) break;

                    args = input.Split(' ');
                    cmdHelper.Execute(args[0], args);
                }
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Bye!");
                Console.ReadKey();
                return 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return 1;
            }
        }
    }
}
