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
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Position) obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Row, Col);
		}

		public int Row { get; set; }
		public int Col { get; set; }

		public Position(int row, int col)
		{
			Row = row;
			Col = col;
		}

		public static Position operator+(Position a, Position b)
		{
			return new(a.Row + b.Row, a.Col + b.Col);
		}

		public static bool operator==(Position a, Position b)
		{
			return a.Row == b.Row && a.Col == b.Col;
		}

		public static bool operator !=(Position a, Position b)
		{
			return !(a == b);
		}
	}
}
