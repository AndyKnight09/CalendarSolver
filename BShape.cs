﻿namespace CalendarSolver
{
	public class BShape : Shape
	{
		public BShape()
		{
			Vertices.Add(new Position(0, 0));
			Vertices.Add(new Position(1, 0));
			Vertices.Add(new Position(1, 1));
			Vertices.Add(new Position(2, 0));
			Vertices.Add(new Position(2, 1));
		}
	}
}
