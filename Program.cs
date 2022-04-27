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
				SolveInParallel = false, // Setting to true will override some other settings
				DisplayPartialSolutionLevel = 1, // Set to -1 to turn off display of partial solutions
				DisplaySolutionsDuringSolve = true,
				FailedSolutionsDisplayTimeMs = 5,
				RequiredSolutions = 1000,
				PercentageIncrementLevel = 0
			};

            if (solver.Solve(DateTime.Now)) return;

			// Failed so try again with Flips
            solver.AllowFlips = true;

            solver.Solve(DateTime.Now);
        }
	}
}
