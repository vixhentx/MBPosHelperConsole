namespace MBPosHelperConsole
{
    public class MBPosHelper
    {
        GTPattern? pattern;
        public static void Main()
        {

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Welcome to MBPosHelper!");

            string[] args ;
            string? input;

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(":");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (input == null) break;

                args = input.Split(' ');
            }
        }
    }
}
