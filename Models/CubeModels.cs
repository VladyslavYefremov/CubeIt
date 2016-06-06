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

            InvokeTopCross();
            InvokeTopCorners();
            InvokeTopCross();
        }

        public void InvokeTopCorners()
        {
            var front =     _cube.Sides[CubeSide.Front];
            var top =       _cube.Sides[CubeSide.Up];
            var back =      _cube.Sides[CubeSide.Back];
            var left =      _cube.Sides[CubeSide.Left];
            var right =     _cube.Sides[CubeSide.Right];

            var formula = new[]
            {
                CubeMove.Up,    CubeMove.Right,
                CubeMove.UpOp,  CubeMove.LeftOp,
                CubeMove.Up,    CubeMove.RightOp,
                CubeMove.UpOp,  CubeMove.Left
            };

            const int maxStepsToPerform = 4;

            var movesToPerofrm = new List<CubeMove>();
            bool foundBase = false;
            var stepCounter = 0;

            while (true)
            {
                // TODO: Reorganize this part of loop
                var frontState = FindCornerAndGetState(right, top, front);
                var leftState = FindCornerAndGetState(front, top, left);
                var rightState = FindCornerAndGetState(back, top, right);
                var backState = FindCornerAndGetState(left, top, back);

                if (frontState.IsCornerAt(CubePosition.Top, CubeSide.Front) &&
                    leftState.IsCornerAt(CubePosition.Top, CubeSide.Left) &&
                    rightState.IsCornerAt(CubePosition.Top, CubeSide.Right) &&
                    backState.IsCornerAt(CubePosition.Top, CubeSide.Back)
                    )
                {
                    break;
                }

                if (stepCounter++ > maxStepsToPerform)
                    throw new Exception($"Couldn't find a solution for this cube! [StepCounter is > {maxStepsToPerform}] [Placing conrners!]");
                
                movesToPerofrm.Clear();

                if (frontState.IsCornerAt(CubePosition.Top, CubeSide.Front))
                {
                    movesToPerofrm.AddRange(formula);
                    foundBase = true;
                }
                else if (!foundBase && leftState.IsCornerAt(CubePosition.Top, CubeSide.Left))
                {
                    movesToPerofrm.Add(CubeMove.RotateOp);
                    foundBase = true;
                }
                else if (!foundBase && rightState.IsCornerAt(CubePosition.Top, CubeSide.Right))
                {
                    movesToPerofrm.Add(CubeMove.Rotate);
                    foundBase = true;
                }
                else if (!foundBase && backState.IsCornerAt(CubePosition.Top, CubeSide.Back))
                {
                    movesToPerofrm.Add(CubeMove.Rotate);
                    movesToPerofrm.Add(CubeMove.Rotate);
                    foundBase = true;
                }
                else
                {
                    movesToPerofrm.AddRange(formula);
                }

                if (movesToPerofrm.Count > 0)
                    foreach (var cubeMove in movesToPerofrm)
                        _cube.PerformMove(cubeMove);

                front = _cube.Sides[CubeSide.Front];
                top = _cube.Sides[CubeSide.Up];
                back = _cube.Sides[CubeSide.Back];
                left = _cube.Sides[CubeSide.Left];
                right = _cube.Sides[CubeSide.Right];
            }

            var formulaRolling = new[]
            {
                CubeMove.RightOp,    CubeMove.DownOp,
                CubeMove.Right,    CubeMove.Down,
                CubeMove.RightOp,    CubeMove.DownOp,
                CubeMove.Right,    CubeMove.Down
            };

            var stateNumber = 0;
            stepCounter = 0;

            while (true)
            {
                // TODO: Reorganize this part of loop
                var frontState = FindCornerAndGetState(right, top, front);
                var leftState = FindCornerAndGetState(front, top, left);
                var rightState = FindCornerAndGetState(back, top, right);
                var backState = FindCornerAndGetState(left, top, back);

                if (frontState.Vector == CubeCornerVector.Outer && 
                    leftState.Vector == CubeCornerVector.Outer &&
                    rightState.Vector == CubeCornerVector.Outer &&
                    backState.Vector == CubeCornerVector.Outer)
                {
                    break;
                }

                var statesList = new[]
                {
                    frontState,
                    rightState,
                    backState,
                    leftState
                };

                if (stepCounter++ > maxStepsToPerform + 2 || stateNumber >= statesList.Length)
                    throw new Exception($"Couldn't find a solution for this cube! [Rolling conrners out!]");

                movesToPerofrm.Clear();

                var currentState = statesList[stateNumber];

                if (currentState.Vector == CubeCornerVector.Outer)
                {
                    movesToPerofrm.Add(CubeMove.Up);
                    stateNumber++;
                }
                else
                {
                    movesToPerofrm.AddRange(formulaRolling);

                    if (currentState.Vector == CubeCornerVector.ToSide)
                        movesToPerofrm.AddRange(formulaRolling);
                }

                if (movesToPerofrm.Count > 0)
                    foreach (var cubeMove in movesToPerofrm)
                        _cube.PerformMove(cubeMove);
            }
        }

        private void InvokeTopCross()
        {
            const int maxStepsToPerform = 4;

            var top = _cube.Sides[CubeSide.Up];
            var topColor = top.Color;

            var isTopNormal = top.EdgeTopColor == topColor;
            var isBottomNormal = top.EdgeBottomColor == topColor;
            var isLeftNormal = top.EdgeLeftColor == topColor;
            var isRightNormal = top.EdgeRightColor == topColor;

            var formula = new[]
            {
                // F
                CubeMove.Right,
                CubeMove.Up,
                CubeMove.RightOp,
                CubeMove.UpOp,
                // F'
            };

            var movesToPerofrm = new List<CubeMove>();
            var stepCounter = 0;

            while (!isTopNormal || !isBottomNormal || !isLeftNormal || !isRightNormal)
            {
                if (stepCounter++ > maxStepsToPerform)
                    throw new Exception($"Couldn't find a solution for this cube! [StepCounter is > {maxStepsToPerform}]");

                movesToPerofrm.Clear();

                if (isTopNormal && isBottomNormal)
                {
                    movesToPerofrm.Add(CubeMove.Up);
                }
                else if (isLeftNormal && isRightNormal)
                {
                    movesToPerofrm.Add(CubeMove.Front);
                    movesToPerofrm.AddRange(formula);
                    movesToPerofrm.Add(CubeMove.FrontOp);
                }
                else if (isLeftNormal && isTopNormal)
                {
                    movesToPerofrm.Add(CubeMove.Front);
                    movesToPerofrm.AddRange(formula);
                    movesToPerofrm.AddRange(formula);
                    movesToPerofrm.Add(CubeMove.FrontOp);
                }
                else if (isRightNormal && isTopNormal)
                {
                    movesToPerofrm.Add(CubeMove.UpOp);
                }
                else if (isRightNormal && isBottomNormal)
                {
                    movesToPerofrm.Add(CubeMove.UpOp);
                }
                else if (isLeftNormal && isBottomNormal)
                {
                    movesToPerofrm.Add(CubeMove.Up);
                }
                else
                {   // Just a dot
                    movesToPerofrm.Add(CubeMove.Front);
                    movesToPerofrm.AddRange(formula);
                    movesToPerofrm.Add(CubeMove.FrontOp);
                }

                if (movesToPerofrm.Count > 0)
                    foreach (var cubeMove in movesToPerofrm)
                        _cube.PerformMove(cubeMove);

                top = _cube.Sides[CubeSide.Up];
                topColor = top.Color;

                isTopNormal = top.EdgeTopColor == topColor;
                isBottomNormal = top.EdgeBottomColor == topColor;
                isLeftNormal = top.EdgeLeftColor == topColor;
                isRightNormal = top.EdgeRightColor == topColor;
            }

            var stateLeft = new ElementState(CubePosition.Top, CubeSide.Left);
            var stateRight = new ElementState(CubePosition.Top, CubeSide.Right);
            var stateFront = new ElementState(CubePosition.Top, CubeSide.Front);
            var stateBack = new ElementState(CubePosition.Top, CubeSide.Back);

            var front = _cube.Sides[CubeSide.Front];
            top = _cube.Sides[CubeSide.Up];

            var currentElementState = FindEdgeAndGetState(front, top);

            while (!currentElementState.Equals(stateFront))
            {
                if(currentElementState.Equals(stateRight))
                    _cube.PerformMove(CubeMove.Up);
                else if (currentElementState.Equals(stateLeft))
                    _cube.PerformMove(CubeMove.UpOp);
                else if (currentElementState.Equals(stateBack))
                    _cube.PerformMove(CubeMove.Up);

                currentElementState = FindEdgeAndGetState(front, top);
            }

            // -------------------------------------------------------------

            var formulaExchange = new[]
            {
                CubeMove.Right,     CubeMove.Up,
                CubeMove.RightOp,   CubeMove.Up,
                CubeMove.Right,     CubeMove.Up,    CubeMove.Up,
                CubeMove.RightOp,   CubeMove.Up
            };

            var perfomanceCounter = 0;

            while (
                !FindEdgeAndGetState(_cube.Sides[CubeSide.Left], _cube.Sides[CubeSide.Up]).Equals(stateLeft) ||
                !FindEdgeAndGetState(_cube.Sides[CubeSide.Right], _cube.Sides[CubeSide.Up]).Equals(stateRight) ||
                !FindEdgeAndGetState(_cube.Sides[CubeSide.Back], _cube.Sides[CubeSide.Up]).Equals(stateBack) ||
                !FindEdgeAndGetState(_cube.Sides[CubeSide.Front], _cube.Sides[CubeSide.Up]).Equals(stateFront)
                )
            {
                if (perfomanceCounter++ > maxStepsToPerform)
                    throw new Exception("Couldn't resolve the cube! 1");

                var left = _cube.Sides[CubeSide.Left];
                var leftElementState = FindEdgeAndGetState(left, top);

                var secondPerfCounter = 0;

                while (!leftElementState.Equals(stateLeft))
                {
                    if (secondPerfCounter++ > maxStepsToPerform)
                        throw new Exception("Couldn't resolve the cube! 2");

                    movesToPerofrm.Clear();

                    if (leftElementState.Equals(stateBack))
                    {
                        movesToPerofrm.Add(CubeMove.RotateOp);
                        movesToPerofrm.AddRange(formulaExchange);
                    }
                    else if (leftElementState.Equals(stateRight))
                    {
                        movesToPerofrm.Add(CubeMove.Up);
                        movesToPerofrm.AddRange(formulaExchange);
                    }

                    if (movesToPerofrm.Count > 0)
                        foreach (var cubeMove in movesToPerofrm)
                            _cube.PerformMove(cubeMove);

                    left = _cube.Sides[CubeSide.Left];
                    leftElementState = FindEdgeAndGetState(left, top);
                }

                _cube.PerformMove(CubeMove.RotateOp);
            }
        }

        private void InvokeEdge()
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

        private void InvokeCorner()
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

        private void InvokeEdgeSecondPart()
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

        private ElementState FindEdgeAndGetState(Side first = null, Side second = null)
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

        private ElementState FindCornerAndGetState(Side first = null, Side second = null, Side third = null)
        {
            var front = _cube.Sides[CubeSide.Front];
            var bottom = _cube.Sides[CubeSide.Down];
            var top = _cube.Sides[CubeSide.Up];
            var back = _cube.Sides[CubeSide.Back];
            var left = _cube.Sides[CubeSide.Left];
            var right = _cube.Sides[CubeSide.Right];

            var frontColor = first == null ? front.Color : first.Color;
            var bottomColor = second == null ? bottom.Color : second.Color;
            var rightColor = third == null ? right.Color : third.Color;

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