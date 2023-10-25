namespace Spacetris.DataStructures;

public struct Point2 : IEquatable<Point2>
{
    /// <summary>
		/// A point at the origin (0, 0)
		/// </summary>
		public static readonly Point2 Zero = new(0, 0);

        /// <summary>
        /// The X component of the Point
        /// </summary>
		public int X;

        /// <summary>
        ///  The Y component of the Point
        /// </summary>
        public int Y;

        /// <summary>
        /// Constructs a new Point
        /// </summary>
		public Point2(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs a new Point
        /// </summary>
		public Point2(double x, double y)
        {
            X = (int)x;
            Y = (int)y;
        }

        /// <summary>
        /// Gets or sets the value at the index of the Point
        /// </summary>
		public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    default: throw new IndexOutOfRangeException("Point access at index: " + index);
                }
            }
            set
            {
                switch (index)
                {
                    case 0: X = value; return;
                    case 1: Y = value; return;
                    default: throw new IndexOutOfRangeException("Point access at index: " + index);
                }
            }
        }

        /// <summary>
        /// Calculate the minimum of two points
        /// </summary>
		public static Point2 Min(Point2 a, Point2 b)
        {
            return new Point2(
                a.X < b.X ? a.X : b.X,
                a.Y < b.Y ? a.Y : b.Y);
        }

        /// <summary>
        /// Calculate the maximum of two points
        /// </summary>
		public static Point2 Max(Point2 a, Point2 b)
        {
            return new Point2(
                a.X > b.X ? a.X : b.X,
                a.Y > b.Y ? a.Y : b.Y);
        }

        /// <summary>
		/// Calculates the distance between two points 
		/// </summary>
		public static float Distance(Point2 a, Point2 b)
        {
            Point2 diff = new Point2(
                a.X - b.X,
                a.Y - b.Y);

            return (float)Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
        }

        /// <summary>
        /// Adds two points to each other
        /// </summary>
        public static Point2 operator +(Point2 left, Point2 right)
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }

        /// <summary>
        /// Subtracts the two points from each other
        /// </summary>
        public static Point2 operator -(Point2 left, Point2 right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            return left;
        }

        /// <summary>
        /// Multiplies the specified point with the specified factor
        /// </summary>
        public static Point2 operator *(Point2 left, int right)
        {
            left.X *= right;
            left.Y *= right;
            return left;
        }

        /// <summary>
        /// Multiplies the specified point with the specified factor
        /// </summary>
        public static Point2 operator *(int left, Point2 right)
        {
            right.X *= left;
            right.Y *= left;
            return right;
        }

        /// <summary>
        /// Multiplies the specified points
        /// </summary>
        public static Point2 operator *(Point2 left, Point2 right)
        {
            left.X *= right.X;
            left.Y *= right.Y;
            return left;
        }

        /// <summary>
        /// Divides the specified point with the specified value.
        /// </summary>
        public static Point2 operator /(Point2 left, int right)
        {
            left.X /= right;
            left.Y /= right;
            return left;
        }

        /// <summary>
        /// Divides the specified points
        /// </summary>
        public static Point2 operator /(Point2 left, Point2 right)
        {
            left.X /= right.X;
            left.Y /= right.Y;
            return left;
        }

        /// <summary>
        /// Compares the specified instances for equality
        /// </summary>
        public static bool operator ==(Point2 left, Point2 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares the specified instances for inequality
        /// </summary>
        public static bool operator !=(Point2 left, Point2 right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a string that represents the current Point
        /// </summary>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        /// <summary>
        /// Returns the hashcode for this instance
        /// </summary>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        /// <summary>
        /// Indicates whether the current point is equal to another point
        /// </summary>
        public bool Equals(Point2 other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is Point2 point2 && Equals(point2);
        }
}