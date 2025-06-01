using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBPosHelperConsole.Model
{
    public class Direction (BlockPos vector, string name)
    {
        private BlockPos _vector = vector;
        private string _name = name;
        public BlockPos Vector => _vector;
        public string Name => _name;

    }
    public readonly struct Directions
    {
        public static Direction Down { get;  } = new(new(0, -1, 0), "DOWN");
        public static Direction Up { get; } = new(new(0, 1, 0), "UP");
        //aka NORTH
        public static Direction Back { get; } = new(new(0, 0, -1), "NORTH");
        //aka SOUTH
        public static Direction Front { get; } = new(new(0, 0, 1), "SOUTH");
        //aka WEST
        public static Direction Left { get; } = new(new(-1, 0, 0), "WEST");
        //aka EAST
        public static Direction Right { get; } = new(new(1, 0, 0), "EAST");
    }
}
