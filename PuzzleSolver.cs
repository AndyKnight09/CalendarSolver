using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;

namespace CalendarSolver
{
	public class PuzzleSolver
	{
		static readonly Size BoardSize = new Size(7, 8);

		public int RequiredSolutions { get; set; }= 1000;
		public bool AllowFlips { get; set; } = false;
		public bool DisplaySolutionsDuringSolve { get; set; } = true;
		public bool DisplayFailedSolutions { get; set; } = false;
		public int FailedSolutionsDisplayTimeMs { get; set; } = 10;

		private double PercentageCompletion { get; set; }

		private PuzzleSolution Puzzle { get; set; }
		private List<List<Shape>> Shapes { get; set; }
		private List<PuzzleSolution> Solutions { get; } = new List<PuzzleSolution>();
		
		public void Solve(DateTime date)
		{
			Console.WriteLine("Solving puzzle...");

			var sw = Stopwatch.StartNew();
			
			Puzzle = InitialisePuzzle(date);
			Shapes = GetAllShapePermutations();

			PercentageCompletion = 0;

			Solve(Puzzle, Shapes);

			Console.WriteLine();
			Console.WriteLine($"Took {sw.Elapsed.ToString()} to find {Solutions.Count} solutions");
			Console.WriteLine();

			if (!DisplaySolutionsDuringSolve)
			{
				foreach (var solution in Solutions)
				{
					solution.Display();
				}
			}
		}

		private PuzzleSolution InitialisePuzzle(DateTime date)
		{
			var initialShapes = new List<Shape>
			{
				// Add shape for board (it isn't square)
				new Shape()
				{
					Vertices = new List<Position>()
					{
						new Position(0, 6),
						new Position(1, 6),
						new Position(7, 0),
						new Position(7, 1),
						new Position(7, 2),
						new Position(7, 3),
					}
				},
				// Add shape for current date
				new DateShape(date)
			};

			return new PuzzleSolution(BoardSize, initialShapes);
		}

		private List<List<Shape>> GetAllShapePermutations()
		{
			var I_Shapes = GetShapePermutations(new IShape(), 2);
			var L1_Shapes = GetShapePermutations(new L1Shape(), 4);
			var S_Shapes = GetShapePermutations(new SShape(), 2);
			var B_Shapes = GetShapePermutations(new BShape(), 4);
			var C_Shapes = GetShapePermutations(new CShape(), 4);
			var L2_Shapes = GetShapePermutations(new L2Shape(), 4);
			var L3_Shapes = GetShapePermutations(new L3Shape(), 4);
			var Z1_Shapes = GetShapePermutations(new Z1Shape(), 4);
			var Z2_Shapes = GetShapePermutations(new Z2Shape(), 2);
			var T_Shapes = GetShapePermutations(new TShape(), 4);

			if (AllowFlips)
			{
				// No extra I permutations by flipping
				L1_Shapes.AddRange(L1_Shapes.Select(s => s.Flip()).ToList());
				S_Shapes.AddRange(S_Shapes.Select(s => s.Flip()).ToList());
				B_Shapes.AddRange(B_Shapes.Select(s => s.Flip()).ToList());
				// No extra C permutations by flipping
				L2_Shapes.AddRange(L2_Shapes.Select(s => s.Flip()).ToList());
				// No extra L3 permutations by flipping
				Z1_Shapes.AddRange(Z1_Shapes.Select(s => s.Flip()).ToList());
				Z2_Shapes.AddRange(Z2_Shapes.Select(s => s.Flip()).ToList());
				// No extra T permutations by flipping
			}

			var allShapes = new List<List<Shape>>
			{
				// Shapes with 4 blocks
				I_Shapes,
				L1_Shapes,
				S_Shapes,
				// Shapes with 5 blocks
				B_Shapes,
				C_Shapes,
				L2_Shapes,
				L3_Shapes,
				Z1_Shapes,
				Z2_Shapes,
				T_Shapes
			};

			return allShapes;
		}

		private List<Shape> GetShapePermutations(Shape shape, int numPermutations)
		{
			if (numPermutations == 2)
			{
				return new List<Shape>
				{
					shape,
					shape.Rotate90()
				};
			}

			if (numPermutations == 4)
			{
				return new List<Shape>
				{
					shape,
					shape.Rotate90(),
					shape.Rotate180(),
					shape.Rotate270()
				};
			}

			throw new Exception("Unexpected number of shape permutations specified");
		}

		private PuzzleSolution Solve(PuzzleSolution solution, List<List<Shape>> remainingShapes, int level = 0)
		{
			// Check if we have solved puzzle
			if (remainingShapes.Count == 0) return solution;

			var percentageIncrement = 100.0 / (Shapes[0].Count * BoardSize.Width * BoardSize.Height * Shapes[1].Count * BoardSize.Width * BoardSize.Height);

			// Try each permutation of next shape
			var permutation = 0;
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
							// Create new solution including this shape
							var newSolution = solution.Add(shapeInPosition);
							var newRemainingShapes = remainingShapes.Skip(1).ToList();

							// Check if solution is still solvable
							if (newSolution.IsSolvable(newRemainingShapes))
							{
								// Solve for the next shape
								newSolution = Solve(newSolution, newRemainingShapes, level + 1);

								// Check if we have found a valid solution
								if (newSolution != null)
								{
									if (level == 0)
									{
										Console.WriteLine("Found valid solution");
										Solutions.Add(newSolution);

										if (DisplaySolutionsDuringSolve)
										{
											newSolution.Display();
										}

										// If we have fount enough solutions return
										if (Solutions.Count == RequiredSolutions)
										{
											return null;
										}
									}
									else
									{
										return newSolution;
									}
								}
							}
							else if (DisplayFailedSolutions)
							{
								newSolution.Display(FailedSolutionsDisplayTimeMs);
							}
						}

						// How far through search space are we?
						if (level == 1)
						{
							PercentageCompletion += percentageIncrement;
						}

						// Output some diagnostics
						if (level <= 1)
						{
							Console.WriteLine($"{PercentageCompletion:0.0}% Tested shape {level} in permutation {permutation} of {permutationCount} at position ({row},{col})");
						}
					}
				}

				permutation++;
			}

			// No solution found
			return null;
		}
	}
}
