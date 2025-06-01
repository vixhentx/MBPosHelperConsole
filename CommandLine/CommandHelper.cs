using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System;
using System.Text;

#pragma warning disable CA1822
#pragma warning disable IDE0051
namespace MBPosHelperConsole.CommandLine
{
    public class CommandHelper
    {
        Dictionary<string, Action<IEnumerable<string>>> handlers = [];
        Dictionary<string, CmdInfo> infos = [];
        CmdEnvironment env = new();
        public CommandHelper()
        {
            var methods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var m in methods)
            {
                var att = m.GetCustomAttribute<CmdInfo>();
                if (att == null || m.GetParameters().FirstOrDefault()?.ParameterType != typeof(IEnumerable<string>)) continue;
                handlers.Add(
                    key: att.Name,
                    value: (Action<IEnumerable<string>>)Delegate.CreateDelegate(
                        typeof(Action<IEnumerable<string>>),
                        this,
                        m
                    ));
                infos.Add(att.Name, att);
            }
        }
        public void Execute(string command, IEnumerable<string> args)
        {
            try
            {
                var handler = handlers[command.ToLower()];
                handler(args);
            }
            catch (KeyNotFoundException)
            {
                HandlerWrongName(command);
            }
        }
        public void Help(string? command = null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (command != null)
            {
                try
                {
                    var info = infos[command];
                    info.ShowHelpText();
                }
                catch (KeyNotFoundException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Command {command} NOT FOUND");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                foreach (var info in infos.Values) info.ShowHelpText();
            }
        }
        private void HandlerWrongName(string command)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Invalid Command: {command}");
        }
        [CmdInfo(name: "help", description: "Show help.",
            usage: "Just type \"help\", \r\n" +
                "Or help [command1] [command2] ..."
            )]
        private void Help(IEnumerable<string> args)
        {
            if (args.Count() == 1) Help();
            var list = args.ToArray();
            for (int i = 1; i < list.Count(); i++) Help(list[i]);
        }
        [CmdInfo(name: "pattern", description: "Set the GTPattern.", usage: "Just type \"pattern\", and then it will instruct u.")]
        private void Pattern(IEnumerable<string> args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Please paste the pattern here, end with EOF");

            Console.ForegroundColor = ConsoleColor.White;
            string rawPattern = "";
            string? tmp;
            while ((tmp = Console.ReadLine()) != null)
            {
                rawPattern += tmp;
            }
            try
            {
                env.Pattern = new(rawPattern);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("New GTPattern set.");
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid GTPattern.");
            }
        }
        [CmdInfo(name: "symbol", description: "Set the symbol to query.",
            usage: "Just type \"symbol\", and then it will instruct u. \r\n" +
                    "Or symbol [one single char] ."
            )]
        private void Symbol(IEnumerable<string> args)
        {
            if (args.Count() != 2 || args.ToArray()[1].Count() != 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Which symbol do you want to query? Input EOF to quit.");
                string? tmp;
                while ((tmp = Console.ReadLine()) != null)
                {
                    if (tmp.Length != 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Invalid symbol:{tmp}");
                    }
                    else break;
                }
                if (tmp == null) return;
                env.Symbol = tmp[0];
            }
            else env.Symbol = args.ToArray()[1][0];
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Succeed to set symbol to {env.Symbol}");
        }
        [CmdInfo(name: "getblock", description: "Get the Set<BlockPos> of the bounding symbol for RenderBlocks")]
        private void GetBlock(IEnumerable<string> args)
        {
            if (env.Pattern == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("GTPattern not set!");
                return;
            }
            char symbol;
            if (env.Symbol != null)
            {
                symbol = (char)env.Symbol;
            }
            else if (args.Count() == 2 && args.ToArray()[1].Length == 1)
            {
                symbol = args.ToArray()[1][0];
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Symbol not set!");
                return;
            }
            try
            {
                var posList = env.Pattern[symbol];
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"//{symbol}");
                Console.WriteLine($"public static Set<BlockPos> blockOffset{env.BlockIndex} = new HashSet<BlockPos>();");
                Console.WriteLine("static{");
                foreach (var pos in posList)
                {
                    Console.WriteLine($"\tblockOffset{env.BlockIndex}.add(new BlockPos(,{pos.X},{pos.Y},{pos.Z}));");
                }
                Console.WriteLine("}");
                env.BlockIndex++;
            }
            catch (KeyNotFoundException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Symbol NOT FOUND");
            }
        }
        [CmdInfo(name: "getface", description: "Get the Set<BlockPos> of the bounding symbol for RenderFaces")]
        private void GetFace(IEnumerable<string> args)
        {
            if (env.Pattern == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("GTPattern not set!");
                return;
            }
            char symbol;
            if (env.Symbol != null)
            {
                symbol = (char)env.Symbol;
            }
            else if (args.Count() == 2 && args.ToArray()[1].Length == 1)
            {
                symbol = args.ToArray()[1][0];
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Symbol not set!");
                return;
            }
            try
            {
                var posList = env.Pattern[symbol];
                Console.ForegroundColor = ConsoleColor.Cyan;

                foreach(var dir in Model.Directions.All)
                {
                    Console.WriteLine($"//{symbol} : {dir.Name}");
                    Console.WriteLine($"public static Set<BlockPos> blockOffset{dir.Name}{env.FaceIndex} = new HashSet<BlockPos>();");
                    Console.WriteLine("static{");
                    foreach (var pos in posList)
                    {
                        var tPos = pos + dir.Vector;
                        if(!env.Pattern.IsIn(tPos) || env.Pattern[tPos] != symbol)
                        {
                            Console.WriteLine($"\tblockOffset{dir.Name}{env.FaceIndex}.add(new BlockPos(,{pos.X},{pos.Y},{pos.Z}));");
                        }
                    }
                    Console.WriteLine("}");
                }

                env.FaceIndex++;
            }
            catch (KeyNotFoundException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Symbol NOT FOUND");
            }
        }
    }

    internal class CmdInfo(string name, string? description = null, string? usage = null) : Attribute
    {
        public string Name { get; private set; } = name;
        public string? Description { get; private set; } = description;
        public string? Usage { get; private set; } = usage;
        public string HelpText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Command: {Name}");
                if (Description != null) sb.AppendLine($"Description: {Description}");
                if (Usage != null) sb.AppendLine($"Usage: {Usage}");
                return sb.ToString();
            }
        }
        public void ShowHelpText()
        {
            Console.WriteLine(HelpText);
        }
    }
}
