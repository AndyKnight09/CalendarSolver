using System;

namespace CalendarSolver
{
	class Program
	{
		static void Main(string[] args)
		{
			var solver = new PuzzleSolver()
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
