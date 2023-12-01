using System.Text.RegularExpressions;

namespace Day1;

public class Program
{
    public static void Main()
    {
        var part1 = File.ReadLines("input.txt").Select(l => $"{l.First(c => c >= '0' && c <= '9')}{l.Last(c => c >= '0' && c <= '9')}").Select(int.Parse).Sum();
        Console.WriteLine(part1);
        var regex = @"[1234567890]|one|two|three|four|five|six|seven|eight|nine";

        var part2 = File.ReadLines("input.txt")
            .Select(l => new[] {Regex.Matches(l, regex).First(), Regex.Matches(l, regex, RegexOptions.RightToLeft).First()})
            .Select(m => new [] {m.First().Value, m.Last().Value})
            .Select(m => m.Select(v => v switch {
                var d when Regex.IsMatch(d, "[0-9]") => int.Parse(d),
                "one" => 1,
                "two" => 2,
                "three" => 3,
                "four" => 4,
                "five" => 5,
                "six" => 6,
                "seven" => 7,
                "eight" => 8,
                "nine" => 9,
                _ => throw new NotImplementedException(),
            }).ToArray())
            .Select(a => int.Parse($"{a[0]}{a[1]}"))
            .Sum();
        Console.WriteLine(part2);
    }
}