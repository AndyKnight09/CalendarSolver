using System.Collections.Generic;
using System.Linq;

namespace CalendarSolver
{
	public class Shape
	{
		public List<Position> Vertices { get; set; } = new List<Position>();

		public Shape Rotate90()
		{
			return new Shape()
			{
				Vertices = Vertices.Select(v => new Position(-v.Col, v.Row)).ToList()
			};
		}

		public Shape Rotate180()
		{
			return new Shape()
			{
				Vertices = Vertices.Select(v => new Position(-v.Row, -v.Col)).ToList()
			};
		}

		public Shape Rotate270()
		{
			return new Shape()
			{
				Vertices = Vertices.Select(v => new Position(v.Col, -v.Row)).ToList()
			};
		}

		public Shape Flip()
		{
			return new Shape()
			{
				Vertices = Vertices.Select(v => new Position(-v.Row, v.Col)).ToList()
			};
		}

		public Shape Offset(Position position)
		{
			return new Shape()
			{
				Vertices = Vertices.Select(v => v + position).ToList()
			};
		}
	}
}
