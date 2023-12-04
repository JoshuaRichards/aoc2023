using System.Text.RegularExpressions;

namespace Day4;

public class Program
{
    public static void Main()
    {
        var part1 = File.ReadLines("input.txt")
            .Select(line => Regex.Split(line, @"[:|]").Skip(1).Select(part => Regex.Matches(part, @"\d+").Select(m => int.Parse(m.Value)).ToArray()).ToArray())
            .Select(arr => arr[0].Intersect(arr[1]).Count())
            .Select(count => Math.Floor(Math.Pow(2, count - 1)))
            .Sum();
        Console.WriteLine(part1);

        var cards = File.ReadLines("input.txt")
            .Select(line => Regex.Split(line, @"[:|]").Skip(1).Select(part => Regex.Matches(part, @"\d+").Select(m => int.Parse(m.Value)).ToArray()).ToArray())
            .Select(arr => arr[0].Intersect(arr[1]).Count())
            .Select((count, i) => (cardno: i + 1, winnings: Enumerable.Range(i + 2, count).ToArray()))
            .ToDictionary(x => x.cardno, x => x.winnings);
        
        var cardsOwned = cards.Keys.ToDictionary(c => c, c => 1);
        foreach (var currentCard in cardsOwned.Keys)
        {
            foreach (var wonCard in cards[currentCard])
            {
                cardsOwned[wonCard] += cardsOwned[currentCard];
            }
        }
        var part2 = cardsOwned.Values.Sum();
        Console.WriteLine(part2);
    }
}