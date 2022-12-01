using AdventOfCSharp.Benchmarking;

namespace AdventOfCode.Benchmarks;

// Use this attribute to denote that the class acts as a benchmark describer
// A benchmark describer describes what solutions will be contained in the benchmarks
[BenchmarkDescriber]
// The AllDates attribute denotes that all referenced and discovered solutions will be included
// Do not forget to add references to the projects that contain the solutions
// You can use other attributes like Years[
[AllDates]
// Make sure that the declaring class is public partial, and NOT sealed
public partial class Consumer { }