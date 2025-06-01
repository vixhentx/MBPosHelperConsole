using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MBPosHelperConsole.Model
{
    public class GTPattern
    {
        //[x][y][z]:
        // x-为机器左边，y-为机器下面，z-为机器后面
        char[,,] symbol = new char[256, 256, 256];
        Dictionary<char, List<BlockPos>> map = [];
        public char this[int x, int y, int z]
        {
            get => symbol[x, y, z];
            set
            {
                List<BlockPos>? list;
                if (!map.TryGetValue(value, out list) || list == null)
                {
                    list = new();
                    map.Add(value, list);
                }
                list.Add(new(x, y, z));
            }
        }
        public char this[BlockPos pos]
        {
            get => symbol[pos.X,pos.Y,pos.Z];
            set => symbol[pos.X, pos.Y, pos.Z] = value;
        }
        public List<BlockPos> this[char symbol] => map[symbol];
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int SizeZ { get; set; }

        public bool IsIn(BlockPos pos)
        {
            return pos.X >= 0 && pos.X <= SizeX
                && pos.Y >= 0 && pos.Y <= SizeY
                && pos.Z >= 0 && pos.Z <= SizeZ
                ;
        }
        public GTPattern(string pattern)
        {
            // 匹配 aisle(...)
            Regex callRegex = new Regex(
                pattern:
                @"
                    \.aisle\s*        
                    (\(                 
                    (?:                 
                    (?:                 
                        ""(?:\\""|[^""])*"" 
                        | [^()]         
                    )*                  
                    )                   
                    \))
                ",
                options:
                RegexOptions.IgnorePatternWhitespace
            );

            MatchCollection callMatches = callRegex.Matches(pattern);
            SizeZ = callMatches.Count;

            int z = 0;
            foreach (Match callMatch in callMatches)
            {
                if (callMatch.Success)
                {
                    string args = callMatch.Groups[1].Value;
                    Regex stringRegex = new Regex(
                        pattern:
                        @"
                            ""               
                            (                  
                                (?:            
                                    \\""       
                                    | [^""]    
                                )*             
                            )                  
                            ""
                        ",
                        options:
                        RegexOptions.IgnorePatternWhitespace
                    );

                    MatchCollection argMatches = stringRegex.Matches(args);
                    int y = 0;
                    foreach (Match argMatch in argMatches)
                    {
                        if (argMatch.Success)
                        {
                            // 处理转义
                            string cleaned = argMatch.Groups[1].Value
                                .Replace("\\\"", "\"")
                                .Replace("\\\\", "\\");

                            int x = 0;
                            foreach (char c in cleaned)
                            {
                                this[x, y, z] = c;
                                x++;
                            }
                            SizeX = x;
                            y++;
                        }
                    }
                    SizeY = y;

                    z++;
                }
                //else throw new InvalidCastException("Failed to match");
            }

            BlockPos controllerPos = this['@'][0];
            foreach (var vList in map.Values)
                foreach (var pos in vList)
                    pos.Subtract(controllerPos);
        }
    }
}
