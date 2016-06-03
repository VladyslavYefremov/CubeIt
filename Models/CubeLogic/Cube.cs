using System;
using System.Collections.Generic;
using System.Text;

namespace E_2.Models
{
    internal sealed class Cube
    {
        private const int SidesNumber = 6;

        public readonly Dictionary<CubeSide, Side> Sides = new Dictionary<CubeSide, Side>();

        private readonly List<CubeMove> _listOfMoves;

        public string Moves
        {
            get
            {
                var sb = new StringBuilder();

                foreach (var move in _listOfMoves)
                {
                    var name = Enum.GetName(typeof(CubeMove), move);

                    var isReverseMove = name.Contains("Op");
                    var isRotation = name.Contains("Rotate");

                    if (isRotation)
                    {
                        sb.Append(isReverseMove ? "y" : "Y");
                    }
                    else
                    {
                        sb.Append(isReverseMove ? name.ToLower()[0] : name[0]);
                    }
                }

                return sb.ToString();
            }
        }

        public Cube(IEnumerable<CubeData> cubeData)
        {
            LoadSides(cubeData);
            var isDataValid = ValidateSides();

            if (!isDataValid)
                throw new ArgumentException("Cube is not valid!");

            this._listOfMoves = new List<CubeMove>();
        }

        public void PerformMove(CubeMove move)
        {
            switch (move)
            {
                case CubeMove.Right:
                    PerformMoveRight();
                    break;
                case CubeMove.RightOp:
                    PerformMoveRight(false);
                    break;
                case CubeMove.Left:
                    PerformMoveLeft();
                    break;
                case CubeMove.LeftOp:
                    PerformMoveLeft(false);
                    break;
                case CubeMove.Front:
                    PerformMoveFront();
                    break;
                case CubeMove.FrontOp:
                    PerformMoveFront(false);
                    break;
                case CubeMove.Up:
                    PerformMoveUp();
                    break;
                case CubeMove.UpOp:
                    PerformMoveUp(false);
                    break;
                case CubeMove.Back:
                    PerformMoveBack();
                    break;
                case CubeMove.BackOp:
                    PerformMoveBack(false);
                    break;
                case CubeMove.Rotate:
                    PerformRotation();
                    break;
                case CubeMove.RotateOp:
                    PerformRotation(false);
                    break;
                case CubeMove.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(move), move, null);
            }

            _listOfMoves.Add(move);

            var isCubeValid = ValidateSides();

            if (!isCubeValid)
                throw new Exception($"Cube is not valid after performing the move: {move.ToString()}!");
        }

        private void PerformMoveRight(bool clockwise = true)
        {
            var front = this.Sides[CubeSide.Front];
            var bottom = this.Sides[CubeSide.Down];
            var top = this.Sides[CubeSide.Up];
            var back = this.Sides[CubeSide.Back];
            var right = this.Sides[CubeSide.Right];

            right.Rotate(clockwise);

            var topSideRightLine = top.RightLine;
            var backSideRightLine = back.RightLine;
            var frontSideRightLine = front.RightLine;
            var bottomSideRightLine = bottom.RightLine;

            back.RightLine = clockwise ? topSideRightLine : bottomSideRightLine;
            top.RightLine = clockwise ? frontSideRightLine : backSideRightLine;
            front.RightLine = clockwise ? bottomSideRightLine : topSideRightLine;
            bottom.RightLine = clockwise ? backSideRightLine : frontSideRightLine;
        }
        private void PerformMoveLeft(bool clockwise = true)
        {
            var front = this.Sides[CubeSide.Front];
            var bottom = this.Sides[CubeSide.Down];
            var top = this.Sides[CubeSide.Up];
            var back = this.Sides[CubeSide.Back];
            var left = this.Sides[CubeSide.Left];

            left.Rotate(clockwise);

            var topSideLeftLine = top.LeftLine;
            var backSideLeftLine = back.LeftLine;
            var frontSideLeftLine = front.LeftLine;
            var bottomSideLeftLine = bottom.LeftLine;

            back.LeftLine = clockwise ? bottomSideLeftLine : topSideLeftLine;
            top.LeftLine = clockwise ? backSideLeftLine : frontSideLeftLine;
            front.LeftLine = clockwise ? topSideLeftLine : bottomSideLeftLine;
            bottom.LeftLine = clockwise ? frontSideLeftLine : backSideLeftLine;
        }
        private void PerformMoveUp(bool clockwise = true)
        {
            var front = this.Sides[CubeSide.Front];
            //var bottom = this.Sides[CubeSide.Down];
            var top = this.Sides[CubeSide.Up];
            var back = this.Sides[CubeSide.Back];
            var left = this.Sides[CubeSide.Left];
            var right = this.Sides[CubeSide.Right];

            top.Rotate(clockwise);

            var backSideBottomLine = back.BottomLine;
            var rightSideLeftLine = right.LeftLine;
            var frontSideTopLine = front.TopLine;
            var leftSideRightLine = left.RightLine;

            back.BottomLine = clockwise ? leftSideRightLine : rightSideLeftLine;
            right.LeftLine = clockwise ? backSideBottomLine : frontSideTopLine;
            front.TopLine = clockwise ? rightSideLeftLine : leftSideRightLine;
            left.RightLine = clockwise ? frontSideTopLine : backSideBottomLine;
        }
        private void PerformMoveFront(bool clockwise = true)
        {
            var front = this.Sides[CubeSide.Front];
            var bottom = this.Sides[CubeSide.Down];
            var top = this.Sides[CubeSide.Up];
            //var back = this.Sides[CubeSide.Back];
            var left = this.Sides[CubeSide.Left];
            var right = this.Sides[CubeSide.Right];

            front.Rotate(clockwise);

            var leftSideRightLine = left.BottomLine;
            var rightSideLeftList = right.BottomLine;
            var topSideBottomLine = top.BottomLine;
            var bottomSideTopLine = bottom.TopLine;

            bottom.TopLine = clockwise ? rightSideLeftList : leftSideRightLine;
            left.BottomLine = clockwise ? bottomSideTopLine : topSideBottomLine;
            top.BottomLine = clockwise ? leftSideRightLine : rightSideLeftList;
            right.BottomLine = clockwise ? topSideBottomLine : bottomSideTopLine;
        }
        private void PerformMoveBack(bool clockwise = true)
        {
            var front = this.Sides[CubeSide.Front];
            var bottom = this.Sides[CubeSide.Down];
            var top = this.Sides[CubeSide.Up];
            var back = this.Sides[CubeSide.Back];
            var left = this.Sides[CubeSide.Left];
            var right = this.Sides[CubeSide.Right];

            back.Rotate(clockwise);

            var topSideTopLine = top.TopLine;
            var leftSideLeftLine = left.TopLine;
            var rightSideRightLine = right.TopLine;
            var bottomSideBottomLine = bottom.BottomLine;

            left.TopLine = clockwise ? topSideTopLine : bottomSideBottomLine;
            bottom.BottomLine = clockwise ? leftSideLeftLine : rightSideRightLine;
            right.TopLine = clockwise ? bottomSideBottomLine : topSideTopLine;
            top.TopLine = clockwise ? rightSideRightLine : leftSideLeftLine;
        }

        private void PerformRotation(bool clockwise = true)
        {
            var front = this.Sides[CubeSide.Front];
            var bottom = this.Sides[CubeSide.Down];
            var top = this.Sides[CubeSide.Up];
            var back = this.Sides[CubeSide.Back];
            var left = this.Sides[CubeSide.Left];
            var right = this.Sides[CubeSide.Right];

            front.SideType = clockwise ? CubeSide.Left : CubeSide.Right;
            left.SideType = clockwise ? CubeSide.Back : CubeSide.Front;
            back.SideType = clockwise ? CubeSide.Right : CubeSide.Left;
            right.SideType = clockwise ? CubeSide.Front : CubeSide.Back;

            front.Rotate(clockwise);
            right.Rotate(clockwise);
            back.Rotate(clockwise);
            left.Rotate(clockwise);

            top.Rotate(clockwise);
            bottom.Rotate(!clockwise);

            Sides.Clear();

            Sides.Add(front.SideType, front); // As left || right
            Sides.Add(left.SideType, left); // As Back  || Front
            Sides.Add(back.SideType, back); // As Right || Left
            Sides.Add(right.SideType, right); // As Front || Back
            Sides.Add(top.SideType, top); // As Front
            Sides.Add(bottom.SideType, bottom); // As Front
        }

        private bool ValidateSides()
        {
            if (Sides.Count != SidesNumber)
                return false;

            var colorsCounter = new int[SidesNumber];

            // Count used colors
            foreach (var sideRow in Sides)
            {
                var side = sideRow.Value;

                foreach (CubeColors color in Enum.GetValues(typeof(CubeColors)))
                    colorsCounter[(int)color] += side.CountColorUsage(color);
            }

            // All colors have to be used
            // the same amoutn of times
            // 
            for (var i = 0; i < SidesNumber - 1; i++)
                if (colorsCounter[i] != colorsCounter[i + 1])
                    return false;

            return true;
        }

        private void LoadSides(IEnumerable<CubeData> cubeData)
        {
            foreach (var sideData in cubeData)
            {
                var createdSide = new Side(sideData.Cells.ToArray());

                foreach (CubeSide sideType in Enum.GetValues(typeof(CubeSide)))
                {
                    if (Enum.GetName(typeof(CubeSide), sideType).ToLower()[0] != sideData.Direction[0])
                        continue;

                    createdSide.SideType = sideType;
                    break;
                }

                Sides.Add(createdSide.SideType, createdSide);
            }
        }
    }
}