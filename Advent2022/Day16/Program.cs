using System.Diagnostics;
using System.Text.RegularExpressions;
using static Common.Common;

long start_time = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
Dictionary<State1, long> scores_cache = new();

Graph parse(string file_name)
{
    List<string> input = readInput(file_name);
    List<Match> matches = input.Select(_ => Regex.Match(_, @"^Valve (?<valve_name>[A-Z]+) has flow rate=(?<flow_rate>\d+); tunnels? leads? to valves? (?<neighbors>.*)$")).ToList();
    List<(string name, int flow_rate)> verticies = new List<(string, int)>();
    List<(string, string)> edges = new List<(string, string)>();
    foreach (Match m in matches)
    {
        string name = m.Groups["valve_name"].Value;
        int flow_rate = int.Parse(m.Groups["flow_rate"].Value);
        verticies.Add((name, flow_rate));
        edges.AddRange(m.Groups["neighbors"].Value.Split(", ").Select(_ => (name, _)));
    }

    return new Graph(verticies, edges);
}

long stateValveScore(State1 state) => state.graph.FlowRates[state.location] * state.time_remaining;

long SingleActorSolve(State1 state)
{
    if (scores_cache.ContainsKey(state))
    {
        return (long)scores_cache[state];
    }

    long score = stateValveScore(state) + state.Neighbors().Select(_ => SingleActorSolve(_)).Concat(new long[] { 0 }).Max();
    scores_cache.Add(state, score);
    return score;
}

long MultiActorSolve(State1 state) => stateValveScore(state) +
    state.Neighbors().Select(_ => MultiActorSolve(_))
        .Concat(new[] { SingleActorSolve(new State1(state.graph, "AA", 26, new HashSet<string>(state.choices))) })
        .Max();

long timeSinceStart() => (Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond) - start_time;

void part1(string file_name)
{
    scores_cache.Clear();
    Graph g = parse(file_name);
    State1 initial_state = new State1(g, "AA", 30, g.VertexNames);
    Console.WriteLine(SingleActorSolve(initial_state));
    Console.WriteLine($"time: {timeSinceStart()}");
}

void part2(string file_name)
{
    scores_cache.Clear();
    Graph g = parse(file_name);
    State1 initial_state = new State1(g, "AA", 26, g.VertexNames);
    Console.WriteLine(MultiActorSolve(initial_state));
    Console.WriteLine($"time: {timeSinceStart()}");
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");

public class Graph
{
    public Dictionary<string, Dictionary<string, int>> Distances { get; set; }
    public Dictionary<string, int> FlowRates { get; set; }

    public HashSet<string> VertexNames { get; set; }

    public Graph(List<(string name, int flow_rate)> verticies, List<(string, string)> edges)
    {
        Distances = new Dictionary<string, Dictionary<string, int>>();
        FlowRates = new Dictionary<string, int>();
        VertexNames = new HashSet<string>();

        IEnumerable<string> names = verticies.Select(_ => _.name);

        foreach (var (name, flow_rate) in verticies)
        {
            Distances[name] = new Dictionary<string, int>();

            foreach (string j in names)
            {
                Distances[name][j] = int.MaxValue;
            }

            Distances[name][name] = 0;
            FlowRates[name] = flow_rate;

            if (flow_rate != 0)
            {
                VertexNames.Add(name);
            }
        }

        foreach (var (u, v) in edges)
        {
            Distances[u][v] = 1;
        }

        foreach (string k in names)
        {
            foreach (string i in names)
            {
                foreach (string j in names)
                {
                    // infinity + anything = infinity otherwise add like normal
                    int link_dist = (Distances[i][k] == int.MaxValue || Distances[k][j] == int.MaxValue) ? int.MaxValue : Distances[i][k] + Distances[k][j];
                    Distances[i][j] = Math.Min(Distances[i][j], link_dist);
                }
            }
        }
    }
}

class State1 : IEquatable<State1>
{
    public Graph graph { get; init; }
    public string location { get; init; }
    public int time_remaining { get; init; }
    public HashSet<string> choices { get; init; }

    public State1(Graph graph, string location, int time_remaining, HashSet<string> choices)
    {
        this.graph = graph;
        this.location = location;
        this.time_remaining = time_remaining;
        this.choices = choices;
    }

    private int choiceTimeRemaining(string choice) => time_remaining - graph.Distances[location][choice] - 1;

    public List<State1> Neighbors() => choices.Where(_ => choiceTimeRemaining(_) > 0)
        .Select(_ => new State1(graph, _, choiceTimeRemaining(_), choices.Except(new[] { _ }).ToHashSet()))
        .ToList();

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(location);
        hash.Add(time_remaining);
        hash.Add(String.Join(',', choices.Order()));
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj) => Equals(obj as State1);

    public bool Equals(State1? other) => (other != null) && location.Equals(other.location) && time_remaining == other.time_remaining && choices.SetEquals(other.choices);
}