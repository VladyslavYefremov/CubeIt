using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;

namespace E_2.Models
{
    // TODO: Implement motion RLUDFB
    #region ENUMS
    internal enum CubeColors : byte
    {
        White,
        Yellow,
        Red,
        Orange,
        Blue,
        Green
    }

    enum CubePosition : byte
    {
        Left,
        Right,
        Top,
        Bottom
    }

    enum CubeSide : byte
    {
        Front,
        Back,
        Up,
        Down,
        Left,
        Right
    }

    enum CubeMove : byte
    {
        Right, RightOp,
        Left, LeftOp,
        Front, FrontOp,
        Up, UpOp,
        Back, BackOp,
        Rotate, RotateOp,
        None
    }

    enum CubeCornerVector
    {
        None, // If not corner-element
        Outer,
        ToSide,
        FromSide
    }

    #endregion

    #region Additionals
    public class CubeData
    {
        public string Direction { get; set; }

        public List<string> Cells { get; set; }
    }

    class ElementState
    {
        public CubePosition Position { get; }

        public bool IsNormal { get; }

        public CubeSide OnSide { get; }

        public CubeCornerVector Vector { get; }

        public ElementState(CubePosition pos, CubeSide side, bool isNormal = true, CubeCornerVector vector = CubeCornerVector.None)
        {
            Position = pos;
            OnSide = side;
            IsNormal = isNormal;
            Vector = vector;
        }

        public bool Equals(ElementState obj)
        {
            if (obj == null)
                throw new ArgumentException();

            if (this.Position != obj.Position)
                return false;

            if (this.OnSide != obj.OnSide)
                return false;

            if (this.Vector != obj.Vector)
                return false;

            return this.IsNormal == obj.IsNormal;
        }

        public override string ToString()
        {
            return $"Position: {Enum.GetName(typeof(CubePosition), Position)} " +
                   $"Side: {Enum.GetName(typeof(CubeSide), OnSide)} " +
                   (Vector != CubeCornerVector.None ? $"Vector: {Enum.GetName(typeof(CubeCornerVector), Vector)} " : "") +
                   $"IsNormal: {IsNormal}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                throw new ArgumentException();

            var stateObject = obj as ElementState;

            if (stateObject == null)
                throw new ArgumentException();

            return this.Equals(stateObject);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ OnSide.GetHashCode() + IsNormal.GetHashCode() ^ Vector.GetHashCode();
        }
    }

    class CubeCrossMashineState
    {
        public ElementState State { get; set; }

        public CubeMove Move { get; set; }
    }
    #endregion

    class Cube
    {
        private const int SidesNumber = 6;

        public readonly Dictionary<CubeSide, Side> Sides = new Dictionary<CubeSide, Side>();

        private readonly List<CubeMove> listOfMoves;

        public string Moves
        {
            get
            {
                var sb = new StringBuilder();

                foreach (var move in listOfMoves)
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

            this.listOfMoves = new List<CubeMove>();
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

            listOfMoves.Add(move);

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

            back.RightLine =    clockwise ? topSideRightLine : bottomSideRightLine;
            top.RightLine =     clockwise ? frontSideRightLine : backSideRightLine;
            front.RightLine =   clockwise ? bottomSideRightLine : topSideRightLine;
            bottom.RightLine =  clockwise ? backSideRightLine : frontSideRightLine;
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

            back.LeftLine =     clockwise ? bottomSideLeftLine : topSideLeftLine;
            top.LeftLine =      clockwise ? backSideLeftLine : frontSideLeftLine;
            front.LeftLine =    clockwise ? topSideLeftLine : bottomSideLeftLine;
            bottom.LeftLine =   clockwise ? frontSideLeftLine : backSideLeftLine;
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

            back.BottomLine =   clockwise ? leftSideRightLine : rightSideLeftLine;
            right.LeftLine =    clockwise ? backSideBottomLine : frontSideTopLine;
            front.TopLine =     clockwise ? rightSideLeftLine : leftSideRightLine;
            left.RightLine =    clockwise ? frontSideTopLine : backSideBottomLine;
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

            left.TopLine =     clockwise ? topSideTopLine : bottomSideBottomLine;
            bottom.BottomLine = clockwise ? leftSideLeftLine : rightSideRightLine;
            right.TopLine =   clockwise ? bottomSideBottomLine : topSideTopLine;
            top.TopLine =       clockwise ? rightSideRightLine : leftSideLeftLine;
        }

        private void PerformRotation(bool clockwise = true)
        {
            var front = this.Sides[CubeSide.Front];
            var bottom = this.Sides[CubeSide.Down];
            var top = this.Sides[CubeSide.Up];
            var back = this.Sides[CubeSide.Back];
            var left = this.Sides[CubeSide.Left];
            var right = this.Sides[CubeSide.Right];

            front.SideType =    clockwise ? CubeSide.Left : CubeSide.Right;
            left.SideType =     clockwise ? CubeSide.Back : CubeSide.Front;
            back.SideType =     clockwise ? CubeSide.Right : CubeSide.Left;
            right.SideType =    clockwise ? CubeSide.Front : CubeSide.Back;

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
                    colorsCounter[(int) color] += side.CountColorUsage(color);
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

    class CubeCross
    {
        #region Cross Mashine State dictionary

        private static readonly Dictionary<ElementState, CubeMove> CrossMashineState = new Dictionary
            <ElementState, CubeMove>()
        {
            {new ElementState(CubePosition.Bottom, CubeSide.Front), CubeMove.None},
            {new ElementState(CubePosition.Left, CubeSide.Front), CubeMove.FrontOp},
            {new ElementState(CubePosition.Right, CubeSide.Front), CubeMove.Front},
            {new ElementState(CubePosition.Top, CubeSide.Front), CubeMove.Front}, // or FrontOp
            
            {new ElementState(CubePosition.Bottom, CubeSide.Left, false), CubeMove.LeftOp},
            {new ElementState(CubePosition.Top, CubeSide.Left, false), CubeMove.Left},
            {new ElementState(CubePosition.Bottom, CubeSide.Right, false), CubeMove.Right},
            {new ElementState(CubePosition.Top, CubeSide.Right, false), CubeMove.RightOp},

            {new ElementState(CubePosition.Bottom, CubeSide.Front, false), CubeMove.Front}, // Or FrontOp
            {new ElementState(CubePosition.Right, CubeSide.Front, false), CubeMove.Right},
            {new ElementState(CubePosition.Left, CubeSide.Front, false), CubeMove.LeftOp},
            {new ElementState(CubePosition.Top, CubeSide.Front, false), CubeMove.Up}, // or UpOp
            
            {new ElementState(CubePosition.Top, CubeSide.Right), CubeMove.Up},
            {new ElementState(CubePosition.Top, CubeSide.Left), CubeMove.UpOp},
            {new ElementState(CubePosition.Top, CubeSide.Back), CubeMove.Up}, // Or UpOp
            {new ElementState(CubePosition.Left, CubeSide.Back), CubeMove.Back},

            {new ElementState(CubePosition.Right, CubeSide.Back), CubeMove.BackOp},
            {new ElementState(CubePosition.Bottom, CubeSide.Back), CubeMove.BackOp}, // or Back

            {new ElementState(CubePosition.Top, CubeSide.Back, false), CubeMove.Up}, // Or UpOp
            {new ElementState(CubePosition.Left, CubeSide.Back, false), CubeMove.Back}, // Or BackOp
            {new ElementState(CubePosition.Right, CubeSide.Back, false), CubeMove.Left}, // Or BackOp
            {new ElementState(CubePosition.Bottom, CubeSide.Back, false), CubeMove.BackOp}, // or Back

            {new ElementState(CubePosition.Bottom, CubeSide.Left), CubeMove.Left}, // or oposite
            {new ElementState(CubePosition.Bottom, CubeSide.Right), CubeMove.Right}, // or oposite
        };

        #endregion

        #region Cross Mashine Shate dictionary for corner elements
        private static readonly Dictionary<ElementState, IEnumerable<CubeMove>> CrossMashineStateForCorner = new Dictionary
            <ElementState, IEnumerable<CubeMove>>()
        {
            {new ElementState(CubePosition.Bottom, CubeSide.Front, true, CubeCornerVector.Outer), new[] {CubeMove.None}},
            {
                new ElementState(CubePosition.Bottom, CubeSide.Front, true, CubeCornerVector.ToSide),
                new[] {CubeMove.Right, CubeMove.UpOp, CubeMove.RightOp}
            },
            {
                new ElementState(CubePosition.Bottom, CubeSide.Front, true, CubeCornerVector.FromSide),
                new[] {CubeMove.Right, CubeMove.Up, CubeMove.RightOp}
            },

            {new ElementState(CubePosition.Top, CubeSide.Front, true, CubeCornerVector.ToSide), new[] {CubeMove.Up}},
            {
                new ElementState(CubePosition.Top, CubeSide.Front, true, CubeCornerVector.Outer), new[]
                {
                    CubeMove.Right, CubeMove.Back, CubeMove.Up, CubeMove.BackOp, CubeMove.RightOp,
                }
            },
            {
                new ElementState(CubePosition.Top, CubeSide.Front, true, CubeCornerVector.FromSide),
                new[] {CubeMove.Right, CubeMove.Up, CubeMove.RightOp}
            },

            {new ElementState(CubePosition.Top, CubeSide.Left, true, CubeCornerVector.Outer), new[] {CubeMove.UpOp}},
            {new ElementState(CubePosition.Top, CubeSide.Left, true, CubeCornerVector.FromSide), new[] {CubeMove.UpOp}},
            {
                new ElementState(CubePosition.Top, CubeSide.Left, true, CubeCornerVector.ToSide),
                new[] {CubeMove.Right, CubeMove.UpOp, CubeMove.RightOp}
            },

            {
                new ElementState(CubePosition.Bottom, CubeSide.Left, true, CubeCornerVector.Outer),
                new[] {CubeMove.Front, CubeMove.Up, CubeMove.FrontOp, CubeMove.UpOp}
            },
            {
                new ElementState(CubePosition.Bottom, CubeSide.Left, true, CubeCornerVector.ToSide),
                new[] {CubeMove.Front, CubeMove.UpOp, CubeMove.FrontOp}
            },
            {
                new ElementState(CubePosition.Bottom, CubeSide.Left, true, CubeCornerVector.FromSide),
                new[] {CubeMove.Front, CubeMove.Up, CubeMove.FrontOp, CubeMove.UpOp}
            },

            {
                new ElementState(CubePosition.Bottom, CubeSide.Right, true, CubeCornerVector.Outer),
                new[] {CubeMove.Back, CubeMove.Up, CubeMove.BackOp}
            },
            {
                new ElementState(CubePosition.Bottom, CubeSide.Right, true, CubeCornerVector.FromSide),
                new[] {CubeMove.Back, CubeMove.Up, CubeMove.BackOp}
            },
            {
                new ElementState(CubePosition.Bottom, CubeSide.Right, true, CubeCornerVector.ToSide),
                new[] {CubeMove.Back, CubeMove.UpOp, CubeMove.BackOp}
            },

            {new ElementState(CubePosition.Top, CubeSide.Right, true, CubeCornerVector.Outer), new[] {CubeMove.Up}},
            {new ElementState(CubePosition.Top, CubeSide.Right, true, CubeCornerVector.FromSide), new[] {CubeMove.Up}},
            {new ElementState(CubePosition.Top, CubeSide.Right, true, CubeCornerVector.ToSide), new[] {CubeMove.Up}},

            {
                new ElementState(CubePosition.Bottom, CubeSide.Back, true, CubeCornerVector.Outer),
                new[] {CubeMove.BackOp, CubeMove.UpOp, CubeMove.Back }
            },
            {
                new ElementState(CubePosition.Bottom, CubeSide.Back, true, CubeCornerVector.ToSide),
                new[] {CubeMove.Left, CubeMove.UpOp, CubeMove.LeftOp }
            },
            {
                new ElementState(CubePosition.Bottom, CubeSide.Back, true, CubeCornerVector.FromSide),
                new[] {CubeMove.Left, CubeMove.UpOp, CubeMove.UpOp, CubeMove.LeftOp}
            },

            {new ElementState(CubePosition.Top, CubeSide.Back, true, CubeCornerVector.Outer), new[] {CubeMove.UpOp}},
            {new ElementState(CubePosition.Top, CubeSide.Back, true, CubeCornerVector.ToSide), new[] {CubeMove.UpOp}},
            {new ElementState(CubePosition.Top, CubeSide.Back, true, CubeCornerVector.FromSide), new[] {CubeMove.UpOp}}
        };
        #endregion

        private static readonly CubeMove[] EdgeTurnRightFormula = new[]{
            CubeMove.Up, CubeMove.Right, CubeMove.UpOp, CubeMove.RightOp,
            CubeMove.UpOp, CubeMove.FrontOp, CubeMove.Up, CubeMove.Front
        };

        private static readonly CubeMove[] EdgeTurnLeftFormula = {
            CubeMove.UpOp, CubeMove.LeftOp, CubeMove.Up, CubeMove.Left,
            CubeMove.Up, CubeMove.Front, CubeMove.UpOp, CubeMove.FrontOp
        };

        private static readonly Dictionary<ElementState, IEnumerable<CubeMove>> CrossMashineStateAfrerCross = new Dictionary
            <ElementState, IEnumerable<CubeMove>>()
        {
            {new ElementState(CubePosition.Right, CubeSide.Front), new[] {CubeMove.None}},
            {
                new ElementState(CubePosition.Right, CubeSide.Front, false),
                EdgeTurnRightFormula
            },
            {
                new ElementState(CubePosition.Top, CubeSide.Front),
                EdgeTurnRightFormula
            },
            {
                new ElementState(CubePosition.Top, CubeSide.Front, false),
                new[] { CubeMove.UpOp }
            },
            {
                new ElementState(CubePosition.Top, CubeSide.Right, false),
                new[] { CubeMove.Rotate }.Concat(EdgeTurnLeftFormula).Concat(new[]{CubeMove.RotateOp}).ToList()
            },
            {
                new ElementState(CubePosition.Top, CubeSide.Right),
                new[] { CubeMove.Up }
            },
            {
                new ElementState(CubePosition.Top, CubeSide.Left, false),
                new[] { CubeMove.UpOp, CubeMove.UpOp }
            },
            {
                new ElementState(CubePosition.Top, CubeSide.Left),
                new[] { CubeMove.UpOp }
            },
            {
                new ElementState(CubePosition.Left, CubeSide.Front, false),
                EdgeTurnLeftFormula
            },
            {
                new ElementState(CubePosition.Left, CubeSide.Front),
                EdgeTurnLeftFormula
            },
            {
                new ElementState(CubePosition.Top, CubeSide.Back),
                new[] { CubeMove.UpOp } 
            },
            {
                new ElementState(CubePosition.Top, CubeSide.Back, false),
                new[] { CubeMove.Up }
            },
            {
                new ElementState(CubePosition.Right, CubeSide.Back, false),
                new[] {CubeMove.Rotate, CubeMove.Rotate }.Concat(EdgeTurnRightFormula).Concat(new[]{CubeMove.RotateOp,CubeMove.RotateOp}).ToList()
            },
            {
                new ElementState(CubePosition.Right, CubeSide.Back),
                new[] {CubeMove.Rotate, CubeMove.Rotate }.Concat(EdgeTurnRightFormula).Concat(new[]{CubeMove.RotateOp,CubeMove.RotateOp}).ToList()
            },
            {
                new ElementState(CubePosition.Left, CubeSide.Back, false),
                new[] {CubeMove.Rotate, CubeMove.Rotate }.Concat(EdgeTurnLeftFormula).Concat(new[]{CubeMove.RotateOp,CubeMove.RotateOp}).ToList()
            },
            {
                new ElementState(CubePosition.Left, CubeSide.Back),
                new[] {CubeMove.Rotate, CubeMove.Rotate }.Concat(EdgeTurnLeftFormula).Concat(new[]{CubeMove.RotateOp,CubeMove.RotateOp}).ToList()
            }
        };

        private readonly Cube _cube;

        public CubeCross(Cube cube)
        {
            this._cube = cube;
        }

        public void Invoke()
        {
            const int edgesOrCorners = 4;

            var edgeSortedCounter = 0;

            while (edgeSortedCounter < edgesOrCorners)
            {
                var currentEdgeState = FindEdgeAndGetState();

                if (!CrossMashineState.ContainsKey(currentEdgeState))
                    throw new Exception($"Couldn't find solution for the state: {currentEdgeState}!");

                var moveToPerform = CrossMashineState[currentEdgeState];

                if (moveToPerform != CubeMove.None)
                    InvokeEdge();

                edgeSortedCounter++;
                _cube.PerformMove(CubeMove.Rotate);
            }

            var cornerSortedCounter = 0;

            while (cornerSortedCounter < edgesOrCorners)
            {
                var currentCornerState = FindCornerAndGetState();

                if (!CrossMashineStateForCorner.ContainsKey(currentCornerState))
                    throw new Exception($"Couldn't find solution for the state: {currentCornerState}!");

                var movesToPerform = CrossMashineStateForCorner[currentCornerState];

                var isFinalState = movesToPerform.Any(move => move == CubeMove.None);

                if (!isFinalState)
                    InvokeCorner();

                cornerSortedCounter++;
                _cube.PerformMove(CubeMove.Rotate);
            }

            edgeSortedCounter = 0;

            while (edgeSortedCounter < edgesOrCorners)
            {
                var currentCornerState = FindEdgeAndGetState(_cube.Sides[CubeSide.Front], _cube.Sides[CubeSide.Right]);

                if (!CrossMashineStateAfrerCross.ContainsKey(currentCornerState))
                    throw new Exception($"Couldn't find solution for the state: {currentCornerState}!");

                var movesToPerform = CrossMashineStateAfrerCross[currentCornerState];

                var isFinalState = movesToPerform.Any(move => move == CubeMove.None);

                if (!isFinalState)
                    InvokeEdgeSecondPart();

                edgeSortedCounter++;
                _cube.PerformMove(CubeMove.Rotate);
            }
        }

        public void InvokeEdge()
        {
            Stack<CubeMove> movesToRestore = new Stack<CubeMove>();

            do
            {
                var edgeCurrentState = FindEdgeAndGetState();

                if (!CrossMashineState.ContainsKey(edgeCurrentState))
                    throw new Exception($"Couldn't find solution for the state: {edgeCurrentState}!");

                var moveToPerform = CrossMashineState[edgeCurrentState];

                if (moveToPerform == CubeMove.None)
                    break;

                bool isRestorableMove = !(moveToPerform == CubeMove.Front || moveToPerform == CubeMove.FrontOp
                    || moveToPerform == CubeMove.Up || moveToPerform == CubeMove.UpOp);

                _cube.PerformMove(moveToPerform);

                var testMove = FindEdgeAndGetState();

                if (movesToRestore.Count > 0)
                {
                    var moveToRestore = movesToRestore.Pop();

                    if (moveToRestore != moveToPerform)
                    {
                        var name = Enum.GetName(typeof(CubeMove), moveToRestore);

                        var isReverseMove = name.Contains("Op"); // Like R' F' U' L' or B'

                        moveToRestore = (CubeMove) ((byte) moveToRestore + (isReverseMove ? -1 : 1));

                        _cube.PerformMove(moveToRestore);
                    }
                    else
                    {
                        isRestorableMove = false;
                    }
                       
                }

                if (isRestorableMove)
                    movesToRestore.Push(moveToPerform);

            } while (true);
        }

        public void InvokeCorner()
        {
            var endFound = false;

            do
            {
                //CrossMashineStateForCorner
                var cornerCurrentState = FindCornerAndGetState();

                if (!CrossMashineStateForCorner.ContainsKey(cornerCurrentState))
                    throw new Exception($"Couldn't find solution for the state: {cornerCurrentState}!");

                var movesToPerform = CrossMashineStateForCorner[cornerCurrentState];

                foreach (var move in movesToPerform)
                {
                    if (move == CubeMove.None)
                    {
                        endFound = true;
                        break;
                    }

                    _cube.PerformMove(move);
                }

            } while (!endFound);
        }

        public void InvokeEdgeSecondPart()
        {
            var endFound = false;

            do
            {
                var edgeCurrentState = FindEdgeAndGetState(_cube.Sides[CubeSide.Front], _cube.Sides[CubeSide.Right]);

                if (!CrossMashineStateAfrerCross.ContainsKey(edgeCurrentState))
                    throw new Exception($"Couldn't find solution for the state: {edgeCurrentState}!");

                var movesToPerform = CrossMashineStateAfrerCross[edgeCurrentState];

                foreach (var move in movesToPerform)
                {
                    if (move == CubeMove.None)
                    {
                        endFound = true;
                        break;
                    }

                    _cube.PerformMove(move);
                }

            } while (!endFound);
        }

        public ElementState FindEdgeAndGetState(Side first = null, Side second = null)
        {
            var front = _cube.Sides[CubeSide.Front];
            var bottom = _cube.Sides[CubeSide.Down];
            var top = _cube.Sides[CubeSide.Up];
            var back = _cube.Sides[CubeSide.Back];
            var left = _cube.Sides[CubeSide.Left];
            var right = _cube.Sides[CubeSide.Right];

            var frontColor = first == null ? front.Color : first.Color;
            var bottomColor = second == null ? bottom.Color : second.Color;

            ElementState state = null;

            #region Front
            // TOP
            if (front.EdgeTopColor == frontColor && top.EdgeBottomColor == bottomColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Front);
            }
            else if (front.EdgeTopColor == bottomColor && top.EdgeBottomColor == frontColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Front, false);
            }
            // LEFT
            else if (front.EdgeLeftColor == frontColor && left.EdgeBottomColor == bottomColor)
            {
                state = new ElementState(CubePosition.Left, CubeSide.Front);
            }
            else if (front.EdgeLeftColor == bottomColor && left.EdgeBottomColor == frontColor)
            {
                state = new ElementState(CubePosition.Left, CubeSide.Front, false);
            }
            // RIGHT
            else if (front.EdgeRightColor == frontColor && right.EdgeBottomColor == bottomColor)
            {
                state = new ElementState(CubePosition.Right, CubeSide.Front);
            }
            else if (front.EdgeRightColor == bottomColor && right.EdgeBottomColor == frontColor)
            {
                state = new ElementState(CubePosition.Right, CubeSide.Front, false);
            }
            // BOTTOM
            else if (front.EdgeBottomColor == frontColor && bottom.EdgeTopColor == bottomColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Front);
            }
            else if (front.EdgeBottomColor == bottomColor && bottom.EdgeTopColor == frontColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Front, false);
            }
                #endregion
            #region BACK
            // TOP
            else if (back.EdgeBottomColor == frontColor && top.EdgeTopColor == bottomColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Back);
            }
            else if (back.EdgeBottomColor == bottomColor && top.EdgeTopColor == frontColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Back, false);
            }
            // LEFT
            else if (back.EdgeRightColor == frontColor && right.EdgeTopColor == bottomColor)
            {
                state = new ElementState(CubePosition.Left, CubeSide.Back);
            }
            else if (back.EdgeRightColor == bottomColor && right.EdgeTopColor == frontColor)
            {
                state = new ElementState(CubePosition.Left, CubeSide.Back, false);
            }
            // RIGHT
            else if (back.EdgeLeftColor == frontColor && left.EdgeTopColor == bottomColor)
            {
                state = new ElementState(CubePosition.Right, CubeSide.Back);
            }
            else if (back.EdgeLeftColor == bottomColor && left.EdgeTopColor == frontColor)
            {
                state = new ElementState(CubePosition.Right, CubeSide.Back, false);
            }
            // BOTTOM
            else if (back.EdgeTopColor == frontColor && bottom.EdgeBottomColor == bottomColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Back);
            }
            else if (back.EdgeTopColor == bottomColor && bottom.EdgeBottomColor == frontColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Back, false);
            }
                #endregion
            #region Right
            // - TOP
            else if (right.EdgeLeftColor == frontColor && top.EdgeRightColor == bottomColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Right);
            }
            else if (right.EdgeLeftColor == bottomColor && top.EdgeRightColor == frontColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Right, false);
            }
            // - BOTTOM
            else if (right.EdgeRightColor == frontColor && bottom.EdgeRightColor == bottomColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Right);
            }
            else if (right.EdgeRightColor == bottomColor && bottom.EdgeRightColor == frontColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Right, false);
            }
                #endregion
            #region Left
            // - TOP
            else if (left.EdgeRightColor == frontColor && top.EdgeLeftColor == bottomColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Left);
            }
            else if (left.EdgeRightColor == bottomColor && top.EdgeLeftColor == frontColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Left, false);
            }
            // - BOTTOM
            else if (left.EdgeLeftColor == frontColor && bottom.EdgeLeftColor == bottomColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Left);
            }
            else if (left.EdgeLeftColor == bottomColor && bottom.EdgeLeftColor == frontColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Left, false);
            }

            #endregion

            if (state == null)
                throw new Exception("State is null!");

            return state;
        }

        public ElementState FindCornerAndGetState()
        {
            var front = _cube.Sides[CubeSide.Front];
            var bottom = _cube.Sides[CubeSide.Down];
            var top = _cube.Sides[CubeSide.Up];
            var back = _cube.Sides[CubeSide.Back];
            var left = _cube.Sides[CubeSide.Left];
            var right = _cube.Sides[CubeSide.Right];

            var frontColor = front.Color;
            var bottomColor = bottom.Color;
            var rightColor = right.Color;

            ElementState state = null;

            #region Right bottom corner on front side
            if (front.CornerBottomRight == frontColor && right.CornerBottomRight == rightColor &&
                bottom.CornerTopRight == bottomColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Front, true, CubeCornerVector.Outer);
            }
            else if (front.CornerBottomRight == bottomColor && right.CornerBottomRight == frontColor &&
                     bottom.CornerTopRight == rightColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Front, true, CubeCornerVector.ToSide);
            }
            else if (front.CornerBottomRight == rightColor && right.CornerBottomRight == bottomColor &&
                     bottom.CornerTopRight == frontColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Front, true, CubeCornerVector.FromSide);
            }
            #endregion

            #region Right top corner on front side
            else if (front.CornerTopRight == bottomColor && top.CornerBottomRight == frontColor &&
                     right.CornerBottomLeft == rightColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Front, true, CubeCornerVector.ToSide);
            }
            else if (front.CornerTopRight == rightColor && top.CornerBottomRight == bottomColor &&
                     right.CornerBottomLeft == frontColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Front, true, CubeCornerVector.Outer);
            }
            else if (front.CornerTopRight == frontColor && top.CornerBottomRight == rightColor &&
                     right.CornerBottomLeft == bottomColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Front, true, CubeCornerVector.FromSide);
            }
            #endregion

            #region Bottom corner on left side
            else if (left.CornerBottomLeft == frontColor && front.CornerBottomLeft == rightColor &&
                     bottom.CornerTopLeft == bottomColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Left, true, CubeCornerVector.Outer);
            }
            else if (left.CornerBottomLeft == bottomColor && front.CornerBottomLeft == frontColor &&
                     bottom.CornerTopLeft == rightColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Left, true, CubeCornerVector.ToSide);
            }
            else if (left.CornerBottomLeft == rightColor && front.CornerBottomLeft == bottomColor &&
                     bottom.CornerTopLeft == frontColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Left, true, CubeCornerVector.FromSide);
            }
            #endregion

            #region Top corner on left side
            else if (front.CornerTopLeft == frontColor && left.CornerBottomRight == rightColor &&
                     top.CornerBottomLeft == bottomColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Left, true, CubeCornerVector.Outer);
            }
            else if (front.CornerTopLeft == bottomColor && left.CornerBottomRight == frontColor &&
                     top.CornerBottomLeft == rightColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Left, true, CubeCornerVector.FromSide);
            }
            else if (front.CornerTopLeft == rightColor && left.CornerBottomRight == bottomColor &&
                     top.CornerBottomLeft == frontColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Left, true, CubeCornerVector.ToSide);
            }
            #endregion

            #region Bottom corner on right side
            else if (right.CornerTopRight == frontColor && back.CornerTopRight == rightColor &&
                     bottom.CornerBottomRight == bottomColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Right, true, CubeCornerVector.Outer);
            }
            else if (right.CornerTopRight == rightColor && back.CornerTopRight == bottomColor &&
                     bottom.CornerBottomRight == frontColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Right, true, CubeCornerVector.FromSide);
            }
            else if (right.CornerTopRight == bottomColor && back.CornerTopRight == frontColor &&
                     bottom.CornerBottomRight == rightColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Right, true, CubeCornerVector.ToSide);
            }
            #endregion
           
            #region Top corner on right side
            else if (right.CornerTopLeft == rightColor && top.CornerTopRight == bottomColor &&
                     back.CornerBottomRight == frontColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Right, true, CubeCornerVector.Outer);
            }
            else if (right.CornerTopLeft == frontColor && top.CornerTopRight == rightColor &&
                     back.CornerBottomRight == bottomColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Right, true, CubeCornerVector.FromSide);
            }
            else if (right.CornerTopLeft == bottomColor && top.CornerTopRight == frontColor &&
                     back.CornerBottomRight == rightColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Right, true, CubeCornerVector.ToSide);
            }
            #endregion

            #region Bottom corner on back side
            else if (back.CornerTopLeft == frontColor && left.CornerTopLeft == rightColor &&
                     bottom.CornerBottomLeft == bottomColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Back, true, CubeCornerVector.Outer);
            }
            else if (back.CornerTopLeft == bottomColor && left.CornerTopLeft == frontColor &&
                     bottom.CornerBottomLeft == rightColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Back, true, CubeCornerVector.ToSide);
            }
            else if (back.CornerTopLeft == rightColor && left.CornerTopLeft == bottomColor &&
                     bottom.CornerBottomLeft == frontColor)
            {
                state = new ElementState(CubePosition.Bottom, CubeSide.Back, true, CubeCornerVector.FromSide);
            }
            #endregion

            #region Top corner on back side
            else if (back.CornerBottomLeft == rightColor && left.CornerTopRight == frontColor &&
                     top.CornerTopLeft == bottomColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Back, true, CubeCornerVector.Outer);
            }
            else if (back.CornerBottomLeft == bottomColor && left.CornerTopRight == rightColor &&
                     top.CornerTopLeft == frontColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Back, true, CubeCornerVector.ToSide);
            }
            else if (back.CornerBottomLeft == frontColor && left.CornerTopRight == bottomColor &&
                     top.CornerTopLeft == rightColor)
            {
                state = new ElementState(CubePosition.Top, CubeSide.Back, true, CubeCornerVector.FromSide);
            }
            #endregion
            
            if (state == null)
                throw new Exception("State is null!");

            return state;
        }
    }

    internal class Side
    {
        public class SideLine: ICloneable
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
        public SideLine BottomLine {
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
            if (colors.Length < Cells*Cells)
                throw new ArgumentException("Not enough parameters!", nameof(colors));

            var container = new CubeColors[3, 3];

            for (var i = 0; i < Cells; i++)
            {
                for (var j = 0; j < Cells; j++)
                {
                    foreach (CubeColors color in Enum.GetValues(typeof(CubeColors)))
                    {
                        var index = i*Cells + j;

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