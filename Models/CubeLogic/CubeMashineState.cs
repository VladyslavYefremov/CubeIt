using System.Collections.Generic;
using System.Linq;

namespace E_2.Models
{
    internal static class CubeMashineState
    {
        public static readonly Dictionary<ElementState, CubeMove> CrossEdges = new Dictionary
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

        public static readonly Dictionary<ElementState, IEnumerable<CubeMove>> CrossCorners = new Dictionary
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

        private static readonly CubeMove[] EdgeTurnRightFormula = new[]{
            CubeMove.Up, CubeMove.Right, CubeMove.UpOp, CubeMove.RightOp,
            CubeMove.UpOp, CubeMove.FrontOp, CubeMove.Up, CubeMove.Front
        };

        private static readonly CubeMove[] EdgeTurnLeftFormula = {
            CubeMove.UpOp, CubeMove.LeftOp, CubeMove.Up, CubeMove.Left,
            CubeMove.Up, CubeMove.Front, CubeMove.UpOp, CubeMove.FrontOp
        };

        public static readonly Dictionary<ElementState, IEnumerable<CubeMove>> Equator = new Dictionary
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
    }
}