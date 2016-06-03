using System;

namespace E_2.Models
{
    internal sealed class SideLine : ICloneable
    {
        public CubeColors Left { get; set; }
        public CubeColors Center { get; set; }
        public CubeColors Right { get; set; }

        public SideLine(CubeColors left, CubeColors center, CubeColors right)
        {
            Left = left;
            Center = center;
            Right = right;
        }

        public int ColorUssage(CubeColors color)
        {
            var counter = 0;

            if (Left == color)
                counter++;

            if (Center == color)
                counter++;

            if (Right == color)
                counter++;

            return counter;
        }

        public object Clone()
        {
            var obj = new SideLine(Left, Center, Right);

            return obj;
        }
    }
}