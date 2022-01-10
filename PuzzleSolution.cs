using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using OpenCvSharp;
using Size = System.Drawing.Size;

namespace CalendarSolver
{
	public class PuzzleSolution
	{
		private Size BoardSize { get; }
		private List<Shape> Shapes { get; }

		public PuzzleSolution(Size boardSize, List<Shape> shapes)
		{
			Shapes = shapes;
			BoardSize = boardSize;
		}

		public bool Fits(Shape shape)
		{
			// Make sure shape fits inside board
			if (shape.Vertices.Any(v => v.Row < 0 || v.Row >= BoardSize.Height || v.Col < 0 || v.Col >= BoardSize.Width))
				return false;

			// Make sure shape doesn't overlap
			return !shape.Vertices.Any(v => Shapes.Any(s1 => s1.Vertices.Any(s1v => s1v == v)));
		}

		public PuzzleSolution Add(Shape shape)
		{
			var solution = new PuzzleSolution(BoardSize, new List<Shape>());
			solution.Shapes.AddRange(Shapes);
			solution.Shapes.Add(shape);
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
					if (GetShapeIndex(row, col) == -1)
					{
						var adjacentHoles = new List<List<Position>>();
						foreach (var hole in holes)
						{
							if (hole.Any(v => Math.Abs(v.Row - row) + Math.Abs(v.Col - col) == 1))
							{
								adjacentHoles.Add(hole);
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
							holes.Add(new List<Position>(){ new Position(row, col) });
						}
					}
				}
			}

			return !holes.Any(h => h.Count < minSpaceSize);
		}

		public void Print()
		{
			Console.WriteLine();
			for (var row = -1; row < BoardSize.Height + 1; row++)
			{
				for (var col = -1; col < BoardSize.Width + 1; col++)
				{
					Console.Write(GetChar(row, col));
				}
				Console.WriteLine();
			}
			Console.WriteLine();
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

		public void PrettyPrint()
		{
			Console.WriteLine();
			for (var row = 0; row < BoardSize.Height; row++)
			{
				Console.Write("+");

				for (var col = 0; col < BoardSize.Width; col++)
				{
					Console.Write(GetHorizontalChar(row, col));

					Console.Write("+");
				}
				Console.WriteLine();

				Console.Write("|");

				for (var col = 0; col < BoardSize.Width; col++)
				{
					Console.Write(GetChar(row, col));

					Console.Write(GetVerticalChar(row, col));
				}
				Console.WriteLine();
			}
			
			Console.Write("+");
			for (var col = 0; col < BoardSize.Width; col++)
			{
				Console.Write("-");
				Console.Write("+");
			}

			Console.WriteLine();
		}

		private int GetShapeIndex(int row, int col)
		{
			if (row < 0 || row >= BoardSize.Height || col < 0 || col >= BoardSize.Width) return 0;

			for (var i = 0; i < Shapes.Count; i++)
			{
				if (Shapes[i].Vertices.Any(v => v.Row == row && v.Col == col))
				{
					return i;
				}
			}

			return -1;
		}

		private char GetChar(int row, int col)
		{
			const string lookup = " #*ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			return lookup[GetShapeIndex(row, col) + 1];
		}

		private Vec3b GetColour(int row, int col)
		{
			Vec3b[] lookup = new []
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

		private char GetHorizontalChar(int row, int col)
		{
			if (row <= 0 || row >= BoardSize.Height) return '-';

			var shapeBelow = GetShapeIndex(row, col);
			var shapeAbove = GetShapeIndex(row - 1, col);

			return shapeBelow == shapeAbove ? ' ' : '-';
		}

		private char GetVerticalChar(int row, int col)
		{
			var shapeLeft = GetShapeIndex(row, col);
			var shapeRight = GetShapeIndex(row, col + 1);

			return shapeLeft == shapeRight ? ' ' : '|';

			return ' ';
		}
	}
}
