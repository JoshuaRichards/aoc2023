namespace Day10;

using Point = (int x, int y);
using FPoint = (double x, double y);
using Grid = Dictionary<(int x, int y), char>;
using FGrid = Dictionary<(double x, double y), char>;

class Program
{
    public static void Main()
    {
        var grid = GetGrid("input.txt");
        var part1 = GetFurthestDist(grid);
        Console.WriteLine(part1);

        var fGrid = MakeFGrid(grid);

        var loop = GetLoop(fGrid);
        var dirtGroups = GetNonLoopGroups(fGrid, loop).ToArray();
        var maxX = fGrid.Keys.Max(p => p.x);
        var maxY = fGrid.Keys.Max(p => p.y);
        var part2 = dirtGroups
            .Where(g => !g.Any(p => p.x == 0 || p.y == 0 || p.x == maxX || p.y == maxY))
            .Sum(g => g.Count(p => p.x == (int)p.x && p.y == (int)p.y));
        Console.WriteLine(part2);

        PrintGrid(fGrid, dirtGroups, loop);
        Console.WriteLine(part2);
    }

    public static FGrid MakeFGrid(Grid grid)
    {
        var maxX = grid.Keys.Max(p => p.x);
        var maxY = grid.Keys.Max(p => p.y);
        var fGrid = grid.ToDictionary(kvp => (FPoint)kvp.Key, kvp => kvp.Value);

        for (double y = 0; y <= maxY; y += 0.5)
        {
            for (double x = 0; x <= maxX; x += 0.5)
            {
                if (fGrid.ContainsKey((x, y))) continue;

                fGrid[(x, y)] = '.';
            }
        }

        return fGrid;
    }

    public static void PrintGrid(FGrid grid, HashSet<FPoint>[] dirtGroups, HashSet<FPoint> loop)
    {
        var maxX = grid.Keys.Max(p => p.x);
        var maxY = grid.Keys.Max(p => p.y);

        for (double y = 0; y <= maxY; y += 0.5)
        {
            for (double x = 0; x <= maxX; x += 0.5)
            {
                var value = grid[(x, y)];
                if (x == (int)x && y == (int)y && dirtGroups.Any(g => g.Contains((x, y))))
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                }
                if (loop.Contains((x, y)))
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                }
                Console.Write(value);
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }

    public static int GetFurthestDist(Grid grid)
    {
        var start = grid.First(kvp => kvp.Value == 'S').Key;
        var visited = new Dictionary<Point, int>();
        var queue = new Queue<(Point, int)>();
        queue.Enqueue((start, 0));

        while (queue.Any())
        {
            var (current, dist) = queue.Dequeue();
            if (visited.TryGetValue(current, out var prevDist) && prevDist < dist) continue;
            visited[current] = dist;

            foreach (var next in GetConnectingNeighbours(grid, current))
            {
                queue.Enqueue((next, dist + 1));
            }
        }

        return visited.Values.Max();
    }

    public static IEnumerable<HashSet<FPoint>> GetNonLoopGroups(FGrid grid, HashSet<FPoint> loop)
    {
        var remaining = grid.Select(kvp => kvp.Key).Where(p => !loop.Contains(p)).ToHashSet();
        while (remaining.Any())
        {
            var group = GetNonLoopGroup(grid, remaining.First(), loop);
            yield return group;
            remaining.ExceptWith(group);
        }
    }

    public static HashSet<FPoint> GetNonLoopGroup(FGrid grid, FPoint start, HashSet<FPoint> loop)
    {
        var visited = new HashSet<FPoint>();
        var queue = new HashSet<FPoint> { start };

        while (queue.Any())
        {
            var current = queue.First();
            queue.Remove(current);
            visited.Add(current);
            foreach (var next in GetNeighbouringNonLoop(grid, current, loop))
            {
                if (visited.Contains(next) || queue.Contains(next)) continue;
                queue.Add(next);
            }
        }

        return visited;
    }

    public static HashSet<FPoint> GetLoop(FGrid grid)
    {
        var start = (Point) grid.First(kvp => kvp.Value == 'S').Key;
        var first = GetConnectingNeighbours(grid, start).First();
        var visited = new HashSet<FPoint> { start };
        var queue = new Queue<FPoint>();
        queue.Enqueue(first);

        while (queue.Any())
        {
            var current = queue.Dequeue();
            visited.Add(current);
            foreach (var neighbour in GetConnectingNeighbours(grid, (Point)current).Where(n => !visited.Contains(n)))
            {
                if (neighbour.x != (int)neighbour.x || neighbour.y != (int)neighbour.y)
                {
                    visited.Add(neighbour);
                    continue;
                }
                queue.Enqueue(neighbour);
            }
        }

        return visited;
    }

    public static Grid GetGrid(string input)
    {
        var y = 0;
        var grid = new Grid();
        foreach (var line in File.ReadLines(input))
        {
            for (int x = 0; x < line.Length; x++)
            {
                grid[(x, y)] = line[x];
            }
            y++;
        }
        return grid;
    }

    public static IEnumerable<Point> GetConnectingNeighbours(Grid grid, Point coord)
    {
        var value = grid[coord];
        Point dest;
        if (new[] { '|', 'J', 'L', 'S' }.Contains(value) &&
            grid.TryGetValue(dest = (coord.x, coord.y - 1), out char destValue) &&
            new[] { '|', 'F', '7', 'S' }.Contains(destValue)
        )
            yield return dest;

        if (new[] { '|', '7', 'F', 'S' }.Contains(value) &&
            grid.TryGetValue(dest = (coord.x, coord.y + 1), out destValue) &&
            new[] { '|', 'J', 'L', 'S' }.Contains(destValue)
        )
            yield return dest;

        if (new[] { '-', 'J', '7', 'S' }.Contains(value) &&
            grid.TryGetValue(dest = (coord.x - 1, coord.y), out destValue) &&
            new[] { '-', 'F', 'L', 'S' }.Contains(destValue)
        )
            yield return dest;

        if (new[] { '-', 'F', 'L', 'S' }.Contains(value) &&
            grid.TryGetValue(dest = (coord.x + 1, coord.y), out destValue) &&
            new[] { '-', 'J', '7', 'S'}.Contains(destValue)
        )
            yield return dest;
    }

    public static IEnumerable<FPoint> GetConnectingNeighbours(FGrid grid, Point coord)
    {
        var value = grid[coord];
        Point dest;
        if (new[] { '|', 'J', 'L', 'S' }.Contains(value) &&
            grid.TryGetValue(dest = (coord.x, coord.y - 1), out char destValue) &&
            new[] { '|', 'F', '7', 'S' }.Contains(destValue)
        )
        {
            yield return (coord.x, coord.y - 0.5);
            yield return dest;
        }

        if (new[] { '|', '7', 'F', 'S' }.Contains(value) &&
            grid.TryGetValue(dest = (coord.x, coord.y + 1), out destValue) &&
            new[] { '|', 'J', 'L', 'S' }.Contains(destValue)
        )
        {
            yield return (coord.x, coord.y + 0.5);
            yield return dest;
        }

        if (new[] { '-', 'J', '7', 'S' }.Contains(value) &&
            grid.TryGetValue(dest = (coord.x - 1, coord.y), out destValue) &&
            new[] { '-', 'F', 'L', 'S' }.Contains(destValue)
        )
        {
            yield return (coord.x - 0.5, coord.y);
            yield return dest;
        }

        if (new[] { '-', 'F', 'L', 'S' }.Contains(value) &&
            grid.TryGetValue(dest = (coord.x + 1, coord.y), out destValue) &&
            new[] { '-', 'J', '7', 'S'}.Contains(destValue)
        )
        {
            yield return (coord.x + 0.5, coord.y);
            yield return dest;
        }
    }

    public static IEnumerable<FPoint> GetNeighbouringNonLoop(FGrid grid, FPoint coord, HashSet<FPoint> loop)
    {
        return new FPoint[] { (-0.5, 0), (0.5, 0), (0, -0.5), (0, 0.5) }
            .Select(p => (x: coord.x + p.x, y: coord.y + p.y))
            .Where(p => grid.ContainsKey(p))
            .Where(p => !loop.Contains(p))
            .ToArray();
    }
}
