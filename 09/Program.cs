namespace Day9;

public class Program
{
    public static void Main()
    {
        var input = File.ReadLines("input.txt").Select(line => line.Split(' ').Select(long.Parse).ToArray()).ToArray();

        var part1 = input.Sum(Extrapolate);
        Console.WriteLine(part1);
        var part2 = input.Sum(ExtrapolateLeft);
        Console.WriteLine(part2);
    }

    public static long Extrapolate(long[] input)
    {
        if (input.Distinct().Count() == 1) return input[0];

        var nextLevel = Enumerable.Range(1, input.Length - 1).Select(i => input[i] - input[i - 1]).ToArray();
        return input.Last() + Extrapolate(nextLevel);
    }

    public static long ExtrapolateLeft(long[] input)
    {
        if (input.Distinct().Count() == 1) return input[0];

        var nextLevel = Enumerable.Range(1, input.Length - 1).Select(i => input[i] - input[i - 1]).ToArray();
        return input.First() - ExtrapolateLeft(nextLevel);
    }
}