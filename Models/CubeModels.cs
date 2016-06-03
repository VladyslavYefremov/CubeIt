using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;

namespace E_2.Models
{
    internal sealed class CubeSolver
    {
        private readonly Cube _cube;

        public CubeSolver(Cube cube)
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

                if (!CubeMashineState.CrossEdges.ContainsKey(currentEdgeState))
                    throw new Exception($"Couldn't find solution for the state: {currentEdgeState}!");

                var moveToPerform = CubeMashineState.CrossEdges[currentEdgeState];

                if (moveToPerform != CubeMove.None)
                    InvokeEdge();

                edgeSortedCounter++;
                _cube.PerformMove(CubeMove.Rotate);
            }

            var cornerSortedCounter = 0;

            while (cornerSortedCounter < edgesOrCorners)
            {
                var currentCornerState = FindCornerAndGetState();

                if (!CubeMashineState.CrossCorners.ContainsKey(currentCornerState))
                    throw new Exception($"Couldn't find solution for the state: {currentCornerState}!");

                var movesToPerform = CubeMashineState.CrossCorners[currentCornerState];

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

                if (!CubeMashineState.Equator.ContainsKey(currentCornerState))
                    throw new Exception($"Couldn't find solution for the state: {currentCornerState}!");

                var movesToPerform = CubeMashineState.Equator[currentCornerState];

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

                if (!CubeMashineState.CrossEdges.ContainsKey(edgeCurrentState))
                    throw new Exception($"Couldn't find solution for the state: {edgeCurrentState}!");

                var moveToPerform = CubeMashineState.CrossEdges[edgeCurrentState];

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

                if (!CubeMashineState.CrossCorners.ContainsKey(cornerCurrentState))
                    throw new Exception($"Couldn't find solution for the state: {cornerCurrentState}!");

                var movesToPerform = CubeMashineState.CrossCorners[cornerCurrentState];

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

                if (!CubeMashineState.Equator.ContainsKey(edgeCurrentState))
                    throw new Exception($"Couldn't find solution for the state: {edgeCurrentState}!");

                var movesToPerform = CubeMashineState.Equator[edgeCurrentState];

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
}