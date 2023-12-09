using System.Text.RegularExpressions;

namespace Day8;

public class Program
{
    public static void Main()
    {
        var input = "input.txt";

        var instructions = File.ReadLines(input).First();
        var nodes = File.ReadLines(input).Skip(2)
            .Select(line => Regex.Matches(line, @"[A-Z]{3}").Select(m => m.Value).ToArray())
            .ToDictionary(matches => matches[0], matches => new Dictionary<char, string> { ['L'] = matches[1], ['R'] = matches[2] });

        var steps = 0;
        var currentNode = "AAA";
        while (currentNode != "ZZZ")
        {
            currentNode = nodes[currentNode][instructions[steps % instructions.Length]];
            steps++;
        }

        var part1 = steps;
        Console.WriteLine(part1);

        var currentNodes = nodes.Keys.Where(k => k[2] == 'A').ToArray();
        var nodeSteps = currentNodes.Select(n =>
        {
            var steps = 0L;
            var currentNode = n;
            while (currentNode[2] != 'Z')
            {
                currentNode = nodes[currentNode][instructions[(int)(steps % instructions.Length)]];
                steps++;
            }
            return steps;
        }).ToArray();

        var part2 = nodeSteps.Aggregate(Lcm);
        Console.WriteLine(part2);
    }

    public static long Gcd(long a, long b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }
        return a;
    }

    public static long Lcm(long a, long b)
    {
        return (a * b) / Gcd(a, b);
    }
}