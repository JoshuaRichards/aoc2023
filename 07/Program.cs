namespace Day7;

public class Program
{
    public static void Main()
    {
        var comparer = new HandComparer();
        var part1 = Solve("input.txt", comparer);
        Console.WriteLine(part1);

        comparer.JokerMode = true;
        var part2 = Solve("input.txt", comparer);
        Console.WriteLine(part2);
    }

    public static long Solve(string input, IComparer<string> comparer)
    {
        return File.ReadLines(input)
            .Select(line => line.Split(' '))
            .Select(split => (hand: split[0], bid: long.Parse(split[1])))
            .OrderBy(c => c.hand, comparer)
            .Select((c, i) => c.bid * (i + 1L))
            .Sum();
    }

    public static bool IsFiveOfAKind(string hand, bool jokerMode)
    {
        var distinctCount = hand.Distinct().Count();
        if (distinctCount == 1) return true;
        if (!jokerMode) return false;

        return distinctCount == 2 && hand.Contains('J');
    }

    public static bool IsFourOfAKind(string hand, bool jokerMode)
    {
        if (!jokerMode || !hand.Contains('J'))
        {
            var distinct = hand.Distinct().ToArray();
            if (distinct.Length != 2) return false;

            return distinct.Any(d => hand.Count(c => c == d) == 4);
        }
        else
        {
            return hand.Distinct().Count() == 3 && !IsFullHouse(hand, true);
        }
    }

    public static bool IsFullHouse(string hand, bool jokerMode)
    {
        if (!jokerMode || !hand.Contains('J'))
        {
            var distinct = hand.Distinct().ToArray();
            if (distinct.Length != 2) return false;

            return new HashSet<int> { 2, 3 }.SetEquals(distinct.Select(d => hand.Count(c => c == d)));
        }

        return hand.Count(c => c == 'J') == 1 && hand.Distinct().Count(d => hand.Count(c => c == d) == 2) == 2;
    }

    public static bool IsThreeOfAKind(string hand, bool jokerMode)
    {
        if (!jokerMode || !hand.Contains('J'))
        {
            var distinct = hand.Distinct().ToArray();
            if (distinct.Length != 3) return false;

            return new HashSet<int> { 1, 3 }.SetEquals(distinct.Select(d => hand.Count(c => c == d)));
        }
        else
        {
            return hand.Distinct().Count() == 4;
        }
    }
    public static bool IsTwoPair(string hand, bool jokerMode)
    {
        if (jokerMode && hand.Contains('J')) return false;
        var distinct = hand.Distinct().ToArray();
        if (distinct.Length != 3) return false;

        return distinct.Count(d => hand.Count(c => c == d) == 2) == 2;
    }

    public static bool IsOnePair(string hand, bool jokerMode)
    {
        if (hand.Distinct().Count() == 4)
            return true;

        if (!jokerMode) return false;
        return hand.Distinct().Count() == 5 && hand.Contains('J');
    }

    public static bool IsHighCard(string hand, bool jokerMode)
    {
        if (jokerMode && hand.Contains('J')) return false;
        return hand.Distinct().Count() == 5;
    }

    public class HandComparer : IComparer<string>
    {
        private readonly char[] CardOrder = new[] { 'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2' };
        private readonly char[] JokerOrder = new[] { 'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J' };
        public bool JokerMode { get; set; }

        public readonly Dictionary<string, int> typeValues = new();

        public int Compare(string? x, string? y)
        {
            ArgumentNullException.ThrowIfNull(x);
            ArgumentNullException.ThrowIfNull(y);

            var xVal = TypeValue(x);
            var yVal = TypeValue(y);
            if (JokerMode)
            {
                typeValues[x] = xVal;
                typeValues[y] = yVal;
            }
            if (yVal != xVal) return xVal - yVal;

            var cardOrder = JokerMode ? JokerOrder : CardOrder;
            var xStr = string.Concat(x.Select(c => Array.IndexOf(cardOrder, c).ToString("d2")));
            var yStr = string.Concat(y.Select(c => Array.IndexOf(cardOrder, c).ToString("d2")));
            return yStr.CompareTo(xStr);
        }

        private int TypeValue(string hand)
        {
            return hand switch
            {
                var h when IsFiveOfAKind(h, JokerMode) => 6,
                var h when IsFourOfAKind(h, JokerMode) => 5,
                var h when IsFullHouse(h, JokerMode) => 4,
                var h when IsThreeOfAKind(h, JokerMode) => 3,
                var h when IsTwoPair(h, JokerMode) => 2,
                var h when IsOnePair(h, JokerMode) => 1,
                var h when IsHighCard(h, JokerMode) => 0,
                _ => throw new NotImplementedException(),
            };
        }
    }
}

