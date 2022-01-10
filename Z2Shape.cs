namespace CalendarSolver
{
	public class Z2Shape : Shape
	{
		public Z2Shape()
		{
			Vertices.Add(new Position(0, 0));
			Vertices.Add(new Position(0, 1));
			Vertices.Add(new Position(1, 1));
			Vertices.Add(new Position(2, 1));
			Vertices.Add(new Position(2, 2));
		}
	}
}
