namespace CalendarSolver
{
	public class TShape : Shape
	{
		public TShape()
		{
			Vertices.Add(new Position(0, 0));
			Vertices.Add(new Position(0, 1));
			Vertices.Add(new Position(0, 2));
			Vertices.Add(new Position(1, 1));
			Vertices.Add(new Position(2, 1));
		}
	}
}
