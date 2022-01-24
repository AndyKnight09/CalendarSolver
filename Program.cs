using System;

namespace CalendarSolver
{
	internal class Program
	{
		private static void Main()
		{
			var solver = new PuzzleSolver
			{
				AllowFlips = false,
				DisplayFailedSolutionLevel = 1, // Set to -1 to turn off
				DisplaySolutionsDuringSolve = true,
				FailedSolutionsDisplayTimeMs = 10,
				RequiredSolutions = 1000
			};

			solver.Solve(DateTime.Now);
		}
	}
}
