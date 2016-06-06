using System;

namespace E_2.Models
{
    internal sealed class ElementState
    {
        public CubePosition Position { get; }

        public bool IsNormal { get; }

        public CubeSide OnSide { get; }

        public CubeCornerVector Vector { get; }

        public bool IsCornerAt(CubePosition position, CubeSide side)
        {
            return (Position == position && OnSide == side);
        }

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
}