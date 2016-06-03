using System;
using System.Text;

namespace E_2.Models
{
    internal sealed class Side
    {
        private const byte Cells = 3;

        public CubeColors Color { get; set; }
        public CubeSide SideType { get; set; }

        public CubeColors EdgeLeftColor => LeftLine.Center;
        public CubeColors EdgeRightColor => RightLine.Center;
        public CubeColors EdgeTopColor => TopLine.Center;
        public CubeColors EdgeBottomColor => BottomLine.Center;

        public CubeColors CornerTopLeft => TopLine.Left;
        public CubeColors CornerTopRight => TopLine.Right;
        public CubeColors CornerBottomLeft => BottomLine.Right;
        public CubeColors CornerBottomRight => BottomLine.Left;

        private SideLine _topLine;
        private SideLine _bottomLine;
        private SideLine _leftLine;
        private SideLine _rightLine;

        public SideLine TopLine
        {
            get { return _topLine; }
            set
            {
                _topLine = value;
                LeftLine.Right = _topLine.Left;
                RightLine.Left = _topLine.Right;
            }
        }
        public SideLine BottomLine
        {
            get { return _bottomLine; }
            set
            {
                _bottomLine = value;
                LeftLine.Left = _bottomLine.Right;
                RightLine.Right = _bottomLine.Left;
            }
        }
        public SideLine LeftLine
        {
            get { return _leftLine; }
            set
            {
                _leftLine = value;
                BottomLine.Right = _leftLine.Left;
                TopLine.Left = _leftLine.Right;
            }
        }
        public SideLine RightLine
        {
            get { return _rightLine; }
            set
            {
                _rightLine = value;
                BottomLine.Left = _rightLine.Right;
                TopLine.Right = _rightLine.Left;
            }
        }

        public int CountColorUsage(CubeColors color)
        {
            var counter = TopLine.ColorUssage(color);
            counter += LeftLine.ColorUssage(color);
            counter += RightLine.ColorUssage(color);
            counter += BottomLine.ColorUssage(color);
            counter += Color == color ? 1 : 0;

            return counter;
        }

        public void Rotate(bool clockwise = true)
        {
            var topLine = TopLine.Clone() as SideLine;
            var rightLine = RightLine.Clone() as SideLine;
            var bottomLine = BottomLine.Clone() as SideLine;
            var leftLine = LeftLine.Clone() as SideLine;

            TopLine = clockwise ? leftLine : rightLine;
            RightLine = clockwise ? topLine : bottomLine;
            BottomLine = clockwise ? rightLine : leftLine;
            LeftLine = clockwise ? bottomLine : topLine;
        }

        public Side(String[] colors)
        {
            if (colors.Length < Cells * Cells)
                throw new ArgumentException("Not enough parameters!", nameof(colors));

            var container = new CubeColors[3, 3];

            for (var i = 0; i < Cells; i++)
            {
                for (var j = 0; j < Cells; j++)
                {
                    foreach (CubeColors color in Enum.GetValues(typeof(CubeColors)))
                    {
                        var index = i * Cells + j;

                        if (Enum.GetName(typeof(CubeColors), color).ToLower()[0] != colors[index][0])
                            continue;

                        container[i, j] = color;

                        if (index == 4) // Center of the side 
                            Color = color;
                    }
                }
            }

            _topLine = new SideLine(container[0, 0], container[0, 1], container[0, 2]);
            _leftLine = new SideLine(container[2, 0], container[1, 0], container[0, 0]);
            _rightLine = new SideLine(container[0, 2], container[1, 2], container[2, 2]);
            _bottomLine = new SideLine(container[2, 2], container[2, 1], container[2, 0]);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(TopLine.Left + " " + TopLine.Center + " " + TopLine.Right + "\n");
            sb.Append(LeftLine.Center + " " + Color + " " + RightLine.Center + "\n");
            sb.Append(BottomLine.Right + " " + BottomLine.Center + " " + BottomLine.Left + "\n");

            return sb.ToString();
        }
    }
}