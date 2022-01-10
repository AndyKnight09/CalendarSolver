namespace CalendarSolver
{
	public class IShape : Shape
	{
		public IShape()
		{
			Vertices.Add(new Position(0, 0));
			Vertices.Add(new Position(1, 0));
			Vertices.Add(new Position(2, 0));
			Vertices.Add(new Position(3, 0));
		}
	}
}
