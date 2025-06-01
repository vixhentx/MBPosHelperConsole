using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBPosHelperConsole.Model
{
    public class BlockPos
    {
        public int X;
        public int Y;
        public int Z;
        public BlockPos() { }
        public BlockPos(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public BlockPos Copy() => new BlockPos(X, Y, Z);
        public BlockPos Subtract(BlockPos right)
        {
            X -= right.X;
            Y -= right.Y;
            Z -= right.Z;
            return this;
        }
        public static BlockPos operator +(BlockPos left, BlockPos right) => new BlockPos(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        public static BlockPos operator -(BlockPos left, BlockPos right) => new BlockPos(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }
}
