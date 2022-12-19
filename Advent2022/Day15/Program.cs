using Common;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using static Common.Common;

List<(Point sensor, Point beacon)> parse(string file_name)
{
    List<(Point sensor, Point beacon)> sensor_beacons = new List<(Point sensor, Point beacon)>();

    foreach (MatchCollection matches in readInput(file_name).Select(i => Regex.Matches(i, @"[-+]?\d+")))
    {
        List<int> ints = matches.Select(elem => int.Parse(elem.Value)).ToList();
        Point sensor = new Point(ints[0], ints[1]);
        Point beacon = new Point(ints[2], ints[3]);
        sensor_beacons.Add((sensor, beacon));
    }

    return sensor_beacons;
}

int dist(Point a, Point b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

CoverageRange getRowCoverage(List<(Point sensor, Point beacon)> sensor_beacons, int row)
{
    CoverageRange coverage_range = new CoverageRange();
    foreach(var (sensor, beacon) in sensor_beacons)
    {
        int roam = dist(sensor, beacon) - Math.Abs(row - sensor.Y);
        if (roam >= 0)
        {
            coverage_range.Add((sensor.X - roam, sensor.X + roam));
        }
    }

    return coverage_range.Simplify();
}

CoverageRange getColCoverage(List<(Point sensor, Point beacon)> sensor_beacons, int col)
{
    CoverageRange coverage_range = new CoverageRange();
    foreach (var (sensor, beacon) in sensor_beacons)
    {
        int roam = dist(sensor, beacon) - Math.Abs(col - sensor.X);
        if (roam >= 0)
        {
            coverage_range.Add((sensor.Y - roam, sensor.Y + roam));
        }
    }

    return coverage_range.Simplify();
}

int countRow(List<(Point sensor, Point beacon)> sensor_beacons, int row)
{
    var row_coverage = getRowCoverage(sensor_beacons, row);

    return row_coverage.Count() - sensor_beacons.Select(pair => pair.beacon).Distinct().Count(b => b.Y == row);
}

Point locateBeacon(List<(Point sensor, Point beacon)> sensor_beacons, int max_coordinate)
{
    IEnumerable<int> range = Enumerable.Range(0, max_coordinate + 1);
    int y = range.OrderBy(i => getRowCoverage(sensor_beacons, i).Clamp(0, max_coordinate).Count()).First();
    int x = range.OrderBy(i => getColCoverage(sensor_beacons, i).Clamp(0, max_coordinate).Count()).First();

    return new Point(x, y);
}

void part1(string file_name, int row)
{
    List<(Point sensor, Point beacon)> sensor_beacons = parse(file_name);
    Console.WriteLine(countRow(sensor_beacons, row));
}

void part2(string file_name, int max_coordinate)
{
    List<(Point sensor, Point beacon)> sensor_beacons = parse(file_name);
    Point location = locateBeacon(sensor_beacons, max_coordinate);
    Console.WriteLine("{0}: {1}", location, (long)location.X * 4000000 + location.Y);
}

part1("sample_input.txt", 10);
part1("input.txt", 2000000);
part2("sample_input.txt", 20);
part2("input.txt", 4000000);

class CoverageRange
{
    List<(int min, int max)> ranges;

    private CoverageRange(List<(int min, int max)> ranges)
    {
        this.ranges = ranges;
    }

    public CoverageRange()
    {
        ranges = new List<(int min, int max)>();
    }

    public void Add((int min, int max) range)
    {
        ranges.Add(range);
    }

    public CoverageRange Simplify()
    {
        List<(int min, int max)> simplified_ranges = new List<(int min, int max)>();

        IEnumerable<(int min, int max)> ordered_ranges = ranges.OrderBy(range => range.min).ThenBy(range => range.max);

        if (ordered_ranges.Count() > 0)
        {
            (int min, int max) curr = ordered_ranges.First();
            foreach (var (min, max) in ordered_ranges.Skip(1))
            {
                if (min <= curr.max)
                {
                    curr.max = Math.Max(max, curr.max);
                }
                else
                {
                    simplified_ranges.Add(curr);
                    curr = (min, max);
                }
            }
            simplified_ranges.Add(curr);
        }
        return new CoverageRange(simplified_ranges);
    }

    public CoverageRange Clamp(int min, int max)
    {
        return new CoverageRange(ranges.Select(range => (Math.Max(min, range.min), Math.Min(max, range.max))).ToList());
    }

    public int Count()
    {
        return this.Simplify().ranges.Select(range => range.max - range.min + 1).Sum();
    }
}