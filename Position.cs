using System;

namespace CalendarSolver
{
	public class Position
	{
		protected bool Equals(Position other)
		{
			return Row == other.Row && Col == other.Col;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((Position) obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Row, Col);
		}

		public int Row { get; }
		public int Col { get; }

		public Position(int row, int col)
		{
			Row = row;
			Col = col;
		}

		public static Position operator+(Position a, Position b)
		{
			return new Position(a.Row + b.Row, a.Col + b.Col);
		}

		public static bool operator==(Position a, Position b)
		{
			if (ReferenceEquals(null, a)) return false;
			if (ReferenceEquals(null, b)) return false;

			return a.Row == b.Row && a.Col == b.Col;
		}

		public static bool operator !=(Position a, Position b)
		{
			return !(a == b);
		}
	}
}
