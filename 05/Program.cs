using System.Text.RegularExpressions;

namespace Day5;

public class Program
{
    public static void Main()
    {
        var input = "example.txt";
        var seeds = Regex.Matches(File.ReadLines(input).First(), @"\d+").Select(m => long.Parse(m.Value)).ToArray();
        var mapKeys = new[]
        {
            "seed-to-soil",
            "soil-to-fertilizer",
            "fertilizer-to-water",
            "water-to-light",
            "light-to-temperature",
            "temperature-to-humidity",
            "humidity-to-location",
        };

        var mapper = new Mapper {
            Mappings = mapKeys.ToDictionary(k => k, k => ReadMap(input, k).ToArray()),
        };

        var part1 = seeds.Min(seed =>
        {
            var current = seed;
            foreach (var k in mapKeys)
            {
                current = mapper.MapNumber(k, current);
            }
            return current;
        });
        Console.WriteLine(part1);

        var part2Seeds = Enumerable.Range(0, seeds.Length / 2).Select(i => i * 2).SelectMany(i => LongRange(seeds[i], seeds[i + 1]));
            // .Min(seed =>
            // {
            //     var current = seed;
            //     foreach (var k in mapKeys)
            //     {
            //         current = mapper.MapNumber(k, current);
            //     }
            //     return current;
            // });
        
        var minLock = new object();
        var part2 = long.MaxValue;
        Parallel.ForEach(part2Seeds, seed =>
        {
            var current = seed;
            foreach (var k in mapKeys)
            {
                current = mapper.MapNumber(k, current);
            }
            lock (minLock)
            {
                part2 = Math.Min(current, part2);
            }
        });
        Console.WriteLine(part2);
        // Console.WriteLine(part2);
        // var fullMappings = mapper.Mappings[mapKeys[0]];
        // foreach (var key in mapKeys.Skip(1))
        // {
        //     fullMappings = fullMappings.SelectMany(m => CrossMapping(m, mapper.Mappings[key])).ToArray();
        // }
        // Console.WriteLine(fullMappings);
    }

    private static IEnumerable<Mapping> CrossMapping(Mapping left, Mapping[] right)
    {
        var rightQueue = new Queue<Mapping>(right.Where(r => r.Source + r.Length >= left.Dest).OrderBy(r => r.Source));
        while (left.Length > 0)
        {
            if (!rightQueue.TryDequeue(out var nextRight))
            {
                yield return left;
                yield break;
            }
            if (left.Dest < nextRight.Source)
            {
                var prelude = new Mapping
                {
                    Source = left.Source,
                    Dest = left.Dest,
                    Length = Math.Min(left.Length, nextRight.Source - left.Dest)
                };
                yield return prelude;
                left = new Mapping
                {
                    Source = left.Source + prelude.Length,
                    Dest = left.Dest + prelude.Length,
                    Length = left.Length - prelude.Length,
                };
                if (left.Length <= 0) yield break;
            }
            var end = Math.Min(left.Dest + left.Length, nextRight.Source + nextRight.Length);
            var ret = new Mapping
            {
                Source = left.Source,
                Dest = nextRight.Dest,
                Length = end - left.Dest,
            };
            yield return ret;
            left = new Mapping
            {
                Source = left.Source + ret.Length,
                Dest = left.Dest + ret.Length,
                Length = left.Length - ret.Length,
            };
        }
    }

    private static IEnumerable<long> LongRange(long start, long count)
    {
        for (long i = 0; i < count; i++)
        {
            var x = start + i;
            yield return x;
        }
    }

    public static long MapNumber(Mapping[] mappings, long input)
    {
        var mapping = mappings.FirstOrDefault(m => input >= m.Source && input < m.Source + m.Length);
        if (mapping is null)
        {
            return input;
        }

        return mapping.Dest + (input - mapping.Source);
    }

    private static Mapping[] ReadMap(string input, string mapId)
    {
        var ranges = File.ReadLines(input)
            .SkipWhile(line => !line.Contains(mapId))
            .Skip(1)
            .TakeWhile(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => Regex.Matches(line, @"\d+"))
            .Select(matches => matches.Select(m => long.Parse(m.Value)).ToArray())
            .Select(nums => new Mapping { Dest = nums[0], Source = nums[1], Length = nums[2] })
            .ToArray();
        return ranges;
    }
}

public class Mapper
{
    public required Dictionary<string, Mapping[]> Mappings { get; set; }
    // private Dictionary<(string, long), long> Cache { get; set; } = new();

    public long MapNumber(string mapKey, long input)
    {
        // if (Cache.TryGetValue((mapKey, input), out var cached)) return cached;

        return /*Cache[(mapKey, input)] =*/ MapNumber(Mappings[mapKey], input);
    }
    private static long MapNumber(Mapping[] mappings, long input)
    {
        var mapping = mappings.FirstOrDefault(m => input >= m.Source && input < m.Source + m.Length);
        if (mapping is null)
        {
            return input;
        }

        return mapping.Dest + (input - mapping.Source);
    }

}

public class Mapping
{
    public required long Source { get; init; }
    public required long Dest { get; init; }
    public required long Length { get; init; }
}

public static class DictionaryExtensions
{
    public static T GetValueOrKey<T>(this Dictionary<T, T> dict, T key) where T : notnull
    {
        return dict.GetValueOrDefault(key, key);
    }
}