using Common;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static Common.Common;

long start_time = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
long timeSinceStart() => (Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond) - start_time;

List<PuzzleState> solveBlueprints(IEnumerable<string> blueprints, int time) =>
    blueprints.Select(_ => new PuzzleState(time, _))
              .Select(_ => new DynamicProgrammingSolver<PuzzleState>(_))
              .Select(_ => _.Solve())
              .ToList();

void part1(string file_name)
{
    long sum = solveBlueprints(readInput(file_name), 24).Select(_ => _.Score() * _.blueprint).Sum();
    Console.WriteLine(sum);
    Console.WriteLine($"time: {timeSinceStart()}");
}

void part2(string file_name)
{
    long product = solveBlueprints(readInput(file_name).Take(3), 32).Select(_ => _.Score()).Product();
    Console.WriteLine(product);
    Console.WriteLine($"time: {timeSinceStart()}");
}

part1("sample_input.txt"); // ~  2 seconds
part1("input.txt");        // ~ 12 seconds
part2("sample_input.txt"); // ~ 99 seconds
part2("input.txt");        // ~ 65 seconds
// total run = ~ 3 minutes

public abstract class IState<T>
{
    // Get the list of all valid neighbors
    public abstract List<T> Neighbors();

    // Is this state better than the other
    public abstract bool IsBetterThan(T other);

    // If we explore this state could it potentially be better than the other state already is without exploration
    public abstract bool IsPotentiallyBetterThan(T other);

    public override bool Equals(object? obj) => Equals((T?)obj);
    public abstract bool Equals(T? other);
    public abstract override int GetHashCode();

    public abstract long Priority();
}

public class MaxComparer : IComparer<long>
{
    public int Compare(long x, long y) => y.CompareTo(x);
}

class DynamicProgrammingSolver<T> where T : IState<T>
{
    T start_state;
    IComparer<long> heap_compare;
    T? best_state = null;
    HashSet<T> solved = new();

    public DynamicProgrammingSolver(T start_state, IComparer<long>? heap_compare = null)
    {
        this.start_state = start_state;
        this.heap_compare = (heap_compare != null) ? heap_compare : new MaxComparer();
    }

    public T? Solve()
    {
        PriorityQueue<T, long> frontier = new(heap_compare);
        frontier.Enqueue(start_state, start_state.Priority());

        while (frontier.TryDequeue(out var curr, out var curr_priority))
        {
            if (solved.Contains(curr))
            {
                // don't repeat work
                continue;
            }

            if (best_state == null || curr.IsBetterThan(best_state))
            {
                best_state = curr;
            }

            solved.Add(curr);

            if (!curr.IsPotentiallyBetterThan(best_state))
            {
                // prune
                break;
            }

            curr.Neighbors().ForEach(_ => frontier.Enqueue(_, _.Priority()));
        }
        return best_state;
    }
}

class PuzzleState : IState<PuzzleState>
{
    private const int GEODE = 3;

    public int minutes_left { get; set; }
    public int blueprint { get; init; }
    public List<long> resources { get; set; } = new() { 0, 0, 0, 0 };
    public List<long> robots { get; set; } = new() { 1, 0, 0, 0 };
    List<List<long>> costs { get; init; } = new();
    List<long> max_costs { get; init; } = new();

    public PuzzleState(int minutes_left, string blueprint_desc)
    {
        this.minutes_left = minutes_left;
        var matches = Regex.Matches(blueprint_desc, @"\d+").Select(_ => int.Parse(_.Value)).ToList();
        blueprint = matches[0];
        costs.Add(new List<long> { matches[1], 0, 0, 0 });
        costs.Add(new List<long> { matches[2], 0, 0, 0 });
        costs.Add(new List<long> { matches[3], matches[4], 0, 0 });
        costs.Add(new List<long> { matches[5], 0, matches[6], 0 });
        max_costs = Enumerable.Range(0, costs.Count).Select(t => costs.Select(_ => _[t]).Max()).ToList();
    }

    public PuzzleState(PuzzleState other)
    {
        this.minutes_left = other.minutes_left;
        this.blueprint = other.blueprint;
        this.resources = other.resources.ToList();
        this.robots = other.robots.ToList();
        this.costs = other.costs;
        this.max_costs = other.max_costs;
    }

    public long Score() => resources[GEODE] + minutes_left * robots[GEODE];

    // pretend we can build a geode robot every minute
    private long BestPossibleScore() => Score() + ((minutes_left * (minutes_left - 1)) / 2);

    public override bool IsBetterThan(PuzzleState other) => Score() > other.Score();

    public override bool IsPotentiallyBetterThan(PuzzleState other) => BestPossibleScore() > other.Score();

    private List<long> GetTotalResources() => resources.Select((v, i) => v + robots[i] * minutes_left).ToList();

    private bool HaveEnoughToBuild(int type) => GetTotalResources().Select((v, i) => v >= costs[type][i]).All(_ => _);

    private bool EnoughOf(int type) => type != GEODE && robots[type] >= max_costs[type];

    private void WaitForBuild()
    {
        var IntDivRoundUp = (long numer, long denom) => (numer + denom - 1) / denom;

        int minutes_needed = 1 + resources.Select((v, i) => v < 0 ? (int)IntDivRoundUp(-v, robots[i]) : 0).Max();
        minutes_left -= minutes_needed;
        resources = resources.Select((v, i) => v + robots[i] * minutes_needed).ToList();
    }

    private void PayForRobot(List<long> cost) => resources = resources.Select((v, i) => v - cost[i]).ToList();

    private PuzzleState? TryBuild(int type)
    {
        if (!HaveEnoughToBuild(type) || EnoughOf(type))
        {
            return null;
        }

        PuzzleState puzzle_state = new PuzzleState(this);
        puzzle_state.PayForRobot(costs[type]);
        puzzle_state.WaitForBuild();
        puzzle_state.robots[type]++;
        return puzzle_state;
    }

    public override List<PuzzleState> Neighbors() => Enumerable.Range(0, robots.Count).Select(_ => TryBuild(_)).Where(_ => _ != null).ToList();

    public override bool Equals(PuzzleState? other) => other != null && minutes_left == other.minutes_left && resources.SequenceEqual(other.resources) && robots.SequenceEqual(other.robots);

    public override int GetHashCode()
    {
        HashCode code = new();
        code.Add(minutes_left);
        resources.ForEach(code.Add);
        robots.ForEach(code.Add);
        return code.ToHashCode();
    }

    public override long Priority() => BestPossibleScore();
}