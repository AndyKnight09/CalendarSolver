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
				DisplayPartialSolutionLevel = 1, // Set to -1 to turn off display of partial solutions
				DisplaySolutionsDuringSolve = true,
				FailedSolutionsDisplayTimeMs = 5,
				RequiredSolutions = 1000,
				PercentageIncrementLevel = 0
			};

			solver.Solve(DateTime.Now);
		}
	}
}
