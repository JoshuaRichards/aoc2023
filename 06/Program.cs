using System.Text.RegularExpressions;

namespace Day6;

public class Program
{
    public static void Main()
    {
        var file = "input.txt";
        (int Time, int Distance)[] input =
            Regex.Matches(File.ReadLines(file).First(), @"\d+")
                .Select(m => int.Parse(m.Value))
                .Zip(Regex.Matches(File.ReadLines(file).Skip(1).First(), @"\d+").Select(m => int.Parse(m.Value)))
                .ToArray();

        var part1 = input
            .Select(i => Enumerable.Range(0, i.Time)
                .Select(holdTime => (i.Time - holdTime) * holdTime)
                .Count(dist => dist > i.Distance)
            )
            .Aggregate((acc, next) => acc * next);

        Console.WriteLine(part1);

        var part2Time = long.Parse(string.Concat(input.Select(i => i.Time)));
        var part2Dist = long.Parse(string.Concat(input.Select(i => i.Distance)));

        var part2 = LongRange(0, part2Time)
            .Select(holdTime => (part2Time - holdTime) * holdTime)
            .Count(dist => dist > part2Dist);

        Console.WriteLine(part2);
    }

    public static IEnumerable<long> LongRange(long start, long count)
    {
        for (long i = 0; i < count; i++)
        {
            yield return start + i;
        }
    }
}