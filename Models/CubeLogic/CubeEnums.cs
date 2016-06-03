namespace E_2.Models
{
    internal enum CubeColors : byte
    {
        White,
        Yellow,
        Red,
        Orange,
        Blue,
        Green
    }

    internal enum CubePosition : byte
    {
        Left,
        Right,
        Top,
        Bottom
    }

    internal enum CubeSide : byte
    {
        Front,
        Back,
        Up,
        Down,
        Left,
        Right
    }

    internal enum CubeMove : byte
    {
        Right, RightOp,
        Left, LeftOp,
        Front, FrontOp,
        Up, UpOp,
        Back, BackOp,
        Rotate, RotateOp,
        None
    }

    internal enum CubeCornerVector
    {
        None, // If not corner-element
        Outer,
        ToSide,
        FromSide
    }
}