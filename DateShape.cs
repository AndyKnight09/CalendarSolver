using System;

namespace CalendarSolver
{
	public class DateShape : Shape
	{
		public DateShape(DateTime date)
		{
			var dow = (int) date.DayOfWeek;

			Vertices.Add(new Position((date.Month - 1) / 6, (date.Month - 1) % 6));
			Vertices.Add(new Position(2 + (date.Day - 1) / 7, (date.Day - 1) % 7));
			Vertices.Add(new Position(dow < 4 ? 6 : 7, dow < 4 ? dow + 3 : dow));
		}
	}
}
