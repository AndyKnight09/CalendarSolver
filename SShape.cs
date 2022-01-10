namespace CalendarSolver
{
	public class SShape : Shape
	{
		public SShape()
		{
			Vertices.Add(new Position(0, 1));
			Vertices.Add(new Position(0, 2));
			Vertices.Add(new Position(1, 0));
			Vertices.Add(new Position(1, 1));
		}
	}
}
