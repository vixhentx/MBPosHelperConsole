using MBPosHelperConsole;

Console.WriteLine("Please paste the pattern of the multiblock(Controller must be '@'). Ctrl+Z to confirm.");
Console.WriteLine("Such as:");
Console.WriteLine(".aisle(\"AAA\")");
Console.WriteLine(".aisle(\"A#A\")");
Console.WriteLine(".aisle(\"A@A\")");
Console.WriteLine("~Z");
Console.WriteLine("---------------");
string pattern_input = "";
string? tmp;
while((tmp = Console.ReadLine()) != null)
{
    pattern_input += tmp;
}
Console.WriteLine("Pattern Confirmed!");
GTPattern pattern;
try
{
    pattern = new GTPattern(pattern_input);
}
catch
{
    Console.WriteLine("Invalid Input");
    Console.ReadKey();
    return;
}

int index = 0;
while (true)
{
    Console.WriteLine("Which block do you query?");
    while ((tmp = Console.ReadLine())?.Length != 1) 
    {
        Console.WriteLine("Invalid input!");
    }
    char symbol = tmp[0];
    try
    {
        var list = pattern[symbol];
        Console.WriteLine($"//{symbol}");
        Console.WriteLine($"static Set<BlockPos> offsetSet{index} = new HashSet<BlockPos>();");
        Console.WriteLine("static{");
        foreach (var pos in list)
        {
            Console.WriteLine($"\toffsetSet{index}.add(new BlockPos({pos.X},{pos.Y},{pos.Z}));");
        }
        Console.WriteLine("}");
        index++;
    }
    catch (Exception ex) 
    {
        Console.WriteLine(ex.ToString()); 
    }
}