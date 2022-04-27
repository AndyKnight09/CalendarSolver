using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;

namespace CalendarSolver
{
	public class PuzzleSolver
	{
		private static readonly Size BoardSize = new(7, 8);

		public bool SolveInParallel { get; set; } = false;
		public int RequiredSolutions { get; set; }= 1000;
		public bool AllowFlips { get; set; } = false;
		public bool DisplaySolutionsDuringSolve { get; set; } = true;
		public int DisplayPartialSolutionLevel { get; set; } = 4;
		public int FailedSolutionsDisplayTimeMs { get; set; } = 10;
		public int PercentageIncrementLevel { get; set; } = 1;
		
		private double PercentageCompletion { get; set; }
		private double PercentageIncrement { get; set; }

		private PuzzleSolution Puzzle { get; set; }
		private List<List<Shape>> Shapes { get; set; }
		private List<PuzzleSolution> Solutions { get; } = new();

		private Stopwatch Stopwatch { get; set; }
		
		public bool Solve(DateTime date)
		{
			Console.WriteLine("Solving puzzle...");

			Stopwatch = Stopwatch.StartNew();
			
			Puzzle = InitialisePuzzle(date);
			Shapes = GetAllShapePermutations();

			PercentageCompletion = 0;
			PercentageIncrement = 100.0;
			for (var i = 0; i <= PercentageIncrementLevel; i++)
			{
				PercentageIncrement /= Shapes[0].Count * BoardSize.Width * BoardSize.Height;
			}

			if (SolveInParallel)
			{
				// Disable intermediate solution display for parallel runs
				DisplayPartialSolutionLevel = -1;
				DisplaySolutionsDuringSolve = false;

				ParallelSolve(Puzzle, Shapes);
			}
			else
			{
				Solve(Puzzle, Shapes);
			}

			Console.WriteLine();
			Console.WriteLine($"Took {Stopwatch.Elapsed.ToString()} to find {Solutions.Count} solutions");
			Console.WriteLine();

			if (!DisplaySolutionsDuringSolve)
            {
				foreach (var solution in Solutions)
                {
                    solution.Display();
                }
			}

            return Solutions.Count > 0;
        }

		private static PuzzleSolution InitialisePuzzle(DateTime date)
		{
			var initialShapes = new List<Shape>
			{
				// Add shape for board (it isn't square)
				new()
				{
					Vertices = new List<Position>
					{
						new(0, 6),
						new(1, 6),
						new(7, 0),
						new(7, 1),
						new(7, 2),
						new(7, 3)
					}
				},
				// Add shape for current date
				new DateShape(date)
			};

			return new PuzzleSolution(BoardSize, initialShapes);
		}

		private List<List<Shape>> GetAllShapePermutations()
		{
			var iShapes = GetShapePermutations(new IShape(), 2);
			var l1Shapes = GetShapePermutations(new L1Shape(), 4);
			var sShapes = GetShapePermutations(new SShape(), 2);
			var bShapes = GetShapePermutations(new BShape(), 4);
			var cShapes = GetShapePermutations(new CShape(), 4);
			var l2Shapes = GetShapePermutations(new L2Shape(), 4);
			var l3Shapes = GetShapePermutations(new L3Shape(), 4);
			var z1Shapes = GetShapePermutations(new Z1Shape(), 4);
			var z2Shapes = GetShapePermutations(new Z2Shape(), 2);
			var tShapes = GetShapePermutations(new TShape(), 4);

			if (AllowFlips)
			{
				// No extra I permutations by flipping
				l1Shapes.AddRange(l1Shapes.Select(s => s.Flip()).ToList());
				sShapes.AddRange(sShapes.Select(s => s.Flip()).ToList());
				bShapes.AddRange(bShapes.Select(s => s.Flip()).ToList());
				// No extra C permutations by flipping
				l2Shapes.AddRange(l2Shapes.Select(s => s.Flip()).ToList());
				// No extra L3 permutations by flipping
				z1Shapes.AddRange(z1Shapes.Select(s => s.Flip()).ToList());
				z2Shapes.AddRange(z2Shapes.Select(s => s.Flip()).ToList());
				// No extra T permutations by flipping
			}

			var allShapes = new List<List<Shape>>
			{
				// Shapes with 4 blocks
				iShapes,
				l1Shapes,
				sShapes,
				// Shapes with 5 blocks
				bShapes,
				cShapes,
				l2Shapes,
				l3Shapes,
				z1Shapes,
				z2Shapes,
				tShapes
			};

			return allShapes;
		}

		private static List<Shape> GetShapePermutations(Shape shape, int numPermutations)
		{
			return numPermutations switch
			{
				2 => new List<Shape> { shape, shape.Rotate90() },
				4 => new List<Shape> { shape, shape.Rotate90(), shape.Rotate180(), shape.Rotate270() },
				_ => throw new Exception("Unexpected number of shape permutations specified")
			};
		}

		private bool Solve(PuzzleSolution solution, IReadOnlyList<List<Shape>> remainingShapes, int level = 0)
		{
			// Check if we already have enough solutions
			if (Solutions.Count >= RequiredSolutions) return true;

			// Check if we have solved puzzle
			if (remainingShapes.Count == 0)
			{
				Solutions.Add(solution);

				Console.WriteLine($"Found {Solutions.Count} valid solution(s) in {Stopwatch.Elapsed.ToString()}");

				if (DisplaySolutionsDuringSolve)
				{
					Stopwatch.Stop();
					solution.Display();
					Stopwatch.Start();
				}

				return true;
			}
			
			// Try each permutation of next shape
			var permutation = 1;
			var permutationCount = remainingShapes[0].Count;
			foreach (var shape in remainingShapes[0])
			{
				// Try all positions
				for (var row = 0; row < BoardSize.Height; row++)
				{
					for (var col = 0; col < BoardSize.Width; col++)
					{
						var shapeInPosition = shape.Offset(new Position(row, col));
						if (solution.Fits(shapeInPosition))
						{
							// Create incremental solution including this shape
							var incrementalSolution = solution.Add(shapeInPosition);
							var newRemainingShapes = remainingShapes.Skip(1).ToList();

							// Check if solution is still solvable
							if (incrementalSolution.IsSolvable(newRemainingShapes))
							{
								// Solve for the next shape
								if (Solve(incrementalSolution, newRemainingShapes, level + 1))
								{
									// Check if we already have enough solutions
									if (Solutions.Count >= RequiredSolutions) return true;
								}
							}
							
							if (level == DisplayPartialSolutionLevel)
							{
								incrementalSolution.Display(FailedSolutionsDisplayTimeMs);
							}
						}

						// How far through search space are we?
						if (level == PercentageIncrementLevel)
						{
							PercentageCompletion += PercentageIncrement;
						}

						// Output some diagnostics
						if (level <= PercentageIncrementLevel)
						{
							Console.WriteLine($"{PercentageCompletion:0.0}% Tested shape {level} in permutation {permutation} of {permutationCount} at position ({row},{col})");
						}
					}
				}

				permutation++;
			}

			return false;
		}

		private bool ParallelSolve(PuzzleSolution solution, IReadOnlyList<List<Shape>> remainingShapes, int level = 0)
		{
			// Check if we already have enough solutions
			if (Solutions.Count >= RequiredSolutions) return true;

			// Check if we have solved puzzle
			if (remainingShapes.Count == 0)
			{
				Solutions.Add(solution);

				Console.WriteLine($"Found {Solutions.Count} valid solution(s) in {Stopwatch.Elapsed.ToString()}");

				if (DisplaySolutionsDuringSolve)
				{
					Stopwatch.Stop();
					solution.Display();
					Stopwatch.Start();
				}

				return true;
			}

			// Try each permutation of next shape
			var permutation = 1;
			var permutationCount = remainingShapes[0].Count;
			foreach (var shape in remainingShapes[0])
			{
				// Try all positions
				Parallel.For(0, BoardSize.Height, new Action<int>((row) =>
				{
					for (var col = 0; col < BoardSize.Width; col++)
					{
						var shapeInPosition = shape.Offset(new Position(row, col));
						if (solution.Fits(shapeInPosition))
						{
							// Create incremental solution including this shape
							var incrementalSolution = solution.Add(shapeInPosition);
							var newRemainingShapes = remainingShapes.Skip(1).ToList();

							// Check if solution is still solvable
							if (incrementalSolution.IsSolvable(newRemainingShapes))
							{
								// Solve for the next shape
								if (Solve(incrementalSolution, newRemainingShapes, level + 1))
								{
									// Check if we already have enough solutions
									if (Solutions.Count >= RequiredSolutions) return;
								}
							}

							if (level == DisplayPartialSolutionLevel)
							{
								incrementalSolution.Display(FailedSolutionsDisplayTimeMs);
							}
						}

						// How far through search space are we?
						if (level == PercentageIncrementLevel)
						{
							PercentageCompletion += PercentageIncrement;
						}

						// Output some diagnostics
						if (level <= PercentageIncrementLevel)
						{
							Console.WriteLine(
								$"{PercentageCompletion:0.0}% Tested shape {level} in permutation {permutation} of {permutationCount} at position ({row},{col})");
						}
					}
				}));

				permutation++;
			}

			return false;
		}
	}
}
