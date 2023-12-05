using System.Collections.Generic;
using System.Text.RegularExpressions;

using Point = (int x, int y);

namespace Day3;

public class Program
{
    public static void Main()
    {
        var (numbers, grid) = ParseInput();
        var part1 = numbers
            .Where(num =>
                GetNeighbours(num.Spaces)
                    .Select(n => grid.GetValueOrDefault(n, '.'))
                    .Any(c => c != '.')
            )
            .Sum(n => n.Value);
        Console.WriteLine(part1);

        var numLookup = numbers.SelectMany(n => n.Spaces.Select(s => (s, n))).ToDictionary(x => x.s, x => x.n);
        var part2 = grid
            .Where(kvp => kvp.Value == '*')
            .Select(kvp => kvp.Key)
            .Select(p => GetNeighbours([p]))
            .Select(neighbours => neighbours.Where(n => numLookup.ContainsKey(n)))
            .Select(neighbours => neighbours.Select(n => numLookup[n]).DistinctBy(n => n.Value).ToArray())
            .Where(n => n.Length == 2)
            .Select(n => n[0].Value * n[1].Value)
            .Sum();
        Console.WriteLine(part2);
    }

    public static Point[] GetNeighbours(Point[] spaces)
    {
        Point[] directions = [(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)];
        return spaces.SelectMany(space => directions.Select(d => (space.x + d.x, space.y + d.y))).Except(spaces).ToArray();
    }

    public static (Number[], Dictionary<Point, char>) ParseInput()
    {
        var grid = new Dictionary<Point, char>();
        var numbers = new List<Number>();
        int y = 0;
        foreach (var line in File.ReadLines("input.txt"))
        {
            for (int x = 0; x < line.Length; x++)
            {
                grid[(x, y)] = line[x];
            }
            numbers.AddRange(Regex.Matches(line, @"\d+").Select(m => new Number(m, y)));
            y++;
        }

        return (numbers.ToArray(), grid);
    }
}

public record struct Number
{
    public Point[] Spaces { get; set; }
    public int Value { get; set; }

    public Number(Match m, int y)
    {
        Spaces = Enumerable.Range(m.Index, m.Length).Select(x => (x, y)).ToArray();
        Value = int.Parse(m.Value);
    }
}