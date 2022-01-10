namespace CalendarSolver
{
	public class CShape : Shape
	{
		public CShape()
		{
			Vertices.Add(new Position(0, 1));
			Vertices.Add(new Position(0, 0));
			Vertices.Add(new Position(1, 0));
			Vertices.Add(new Position(2, 0));
			Vertices.Add(new Position(2, 1));
		}
	}
}
