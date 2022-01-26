using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;
using Size = System.Drawing.Size;

namespace CalendarSolver
{
	public class PuzzleSolution
	{
		private Size BoardSize { get; }
		//private List<Shape> Shapes { get; }
		private int[,] Solution { get; }
		private int ShapeCount { get; set; }

		public PuzzleSolution(Size boardSize, List<Shape> shapes)
		{
			//Shapes = shapes;
			BoardSize = boardSize;
			Solution = new int[boardSize.Height, boardSize.Width];
			for (var row = 0; row < boardSize.Height; row++)
			{
				for (var col = 0; col < boardSize.Width; col++)
				{
					Solution[row, col] = -1;
				}
			}

			foreach (var shape in shapes)
			{
				shape.Vertices.ForEach(v => Solution[v.Row, v.Col] = ShapeCount);
				ShapeCount++;
			}
		}

		public PuzzleSolution(Size boardSize, int[,] solution, int shapeCount)
		{
			BoardSize = boardSize;
			Solution = solution;
			ShapeCount = shapeCount;
		}

		public bool Fits(Shape shape)
		{
			// Make sure shape fits inside board
			if (shape.Vertices.Any(v => v.Row < 0 || v.Row >= BoardSize.Height || v.Col < 0 || v.Col >= BoardSize.Width))
				return false;

			// Make sure shape doesn't overlap
			return shape.Vertices.All(v => Solution[v.Row, v.Col] == -1);
		}

		public PuzzleSolution Add(Shape shape)
		{
			var solution = new PuzzleSolution(BoardSize, (int[,])Solution.Clone(), ShapeCount);
			shape.Vertices.ForEach(v => solution.Solution[v.Row, v.Col] = ShapeCount);
			solution.ShapeCount++;
			return solution;
		}

		public bool IsSolvable(List<List<Shape>> remainingShapes)
		{
			if (remainingShapes.Count == 0) return true;

			var minSpaceSize = remainingShapes.Min(v => v.First().Vertices.Count);

			// Check for unsolvable holes (less spaces connected than minimum remaining shape size)
			var holes = new List<List<Position>>();
			for (var row = 0; row < BoardSize.Height; row++)
			{
				for (var col = 0; col < BoardSize.Width; col++)
				{
					if (GetShapeIndex(row, col) != -1) continue;
					
					var adjacentHoles = new List<List<Position>>();
					foreach (var hole in holes)
					{
						foreach (var vertex in hole)
						{
							if (Math.Abs(vertex.Row - row) + Math.Abs(vertex.Col - col) != 1) continue;
							
							adjacentHoles.Add(hole);
							break;
						}
						
					}

					if (adjacentHoles.Count > 0)
					{
						// Add to adjacent holes and merge them
						adjacentHoles[0].Add(new Position(row, col));
						for (var i = 1; i < adjacentHoles.Count; i++)
						{
							adjacentHoles[0].AddRange(adjacentHoles[i]);
							holes.Remove(adjacentHoles[i]);
						}
					}
					else
					{
						// Create new hole
						holes.Add(new List<Position> { new(row, col) });
					}
				}
			}

			return !holes.Any(h => h.Count < minSpaceSize);
		}

		public void Display(int delay = 0)
		{
			var solution = new Mat(BoardSize.Height, BoardSize.Width, MatType.CV_8UC3);
			solution.SetTo(new Scalar(255, 255, 255));
			
			for (var row = 0; row < BoardSize.Height; row++)
			{
				for (var col = 0; col < BoardSize.Width; col++)
				{
					solution.Set(row, col, GetColour(row, col));
				}
			}

			var scaledSolution = new Mat();
			Cv2.Resize(solution, scaledSolution, new OpenCvSharp.Size(BoardSize.Width * 100, BoardSize.Height * 100), 0, 0, InterpolationFlags.Nearest);

			Cv2.ImShow("Solution", scaledSolution);
			Cv2.WaitKey(delay);
		}

		private int GetShapeIndex(int row, int col)
		{
			if (row < 0 || row >= BoardSize.Height || col < 0 || col >= BoardSize.Width) return 0;
			
			return Solution[row, col];
		}
		
		private Vec3b GetColour(int row, int col)
		{
			var lookup = new []
			{
				new Vec3b(0, 0, 0),
				new Vec3b(75, 75, 75),
				new Vec3b(255, 255, 255),
				new Vec3b(0, 0, 255),
				new Vec3b(0, 255, 255),
				new Vec3b(255, 0, 255),
				new Vec3b(255, 255, 0),
				new Vec3b(128, 0, 0),
				new Vec3b(0, 128, 0),
				new Vec3b(0, 0, 128),
				new Vec3b(128, 255, 0),
				new Vec3b(0, 128, 255),
				new Vec3b(0, 255, 128),
				new Vec3b(128, 0, 255),
				new Vec3b(255, 0, 128)
			};
			return lookup[GetShapeIndex(row, col) + 1];
		}
	}
}
