using System.Text.RegularExpressions;

namespace Day2;

public class Program
{
    public static void Main()
    {
        var cubeRegex = @"(\d+) (red|green|blue)";
        var gameRegex = @"Game (\d+)";
        var maxes = new Dictionary<string, int>()
        {
            ["red"] = 12,
            ["green"] = 13,
            ["blue"] = 14,
        };

        var part1 = File.ReadLines("input.txt")
            .Where(line => Regex.Split(line, "[;:]").Skip(1).All(game => Regex.Matches(game, cubeRegex).All(m => int.Parse(m.Groups[1].Value) <= maxes[m.Groups[2].Value])))
            .Select(line => Regex.Match(line, gameRegex).Groups[1].Value)
            .Select(int.Parse)
            .Sum();
        Console.WriteLine(part1);

        var part2 = File.ReadLines("input.txt")
            .Select(line => line.Split(';'))
            .Select(games => games.Select(game => Regex.Matches(game, cubeRegex).Select(m => (count: int.Parse(m.Groups[1].Value), color: m.Groups[2].Value)).ToDictionary(g => g.color, g => g.count)))
            .Select(games => new[] { "red", "green", "blue" }.Select(c => games.Max(g => g.GetValueOrDefault(c, int.MinValue))).Aggregate(1, (acc, next) => acc * next))
            .Sum();
        Console.WriteLine(part2);
    }
}