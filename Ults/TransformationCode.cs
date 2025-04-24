using System.Collections.Generic;

namespace AvaloniaApp.Ults;

public static class TransformationCode
{
    public static List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> 
        TransformationTree0 { get; } =
        [(0.04m, 0, 0, -0.44m, 0, 0.39m, 8m),  
        (0.04m, 0, 0, -0.44m, 0, 0.59m, 8m),
        (0.389m, 0.289m, -0.389m, 0.345m, 0, 0.22m, 21m),
        (0.345m, -0.257m, 0.289m, 0.306m, 0, 0.240m, 21m),
        (0.390m, -0.275m, 0.225m, 0.476m, 0, 0.440m, 21m),
        (0.408m, 0.19m, -0.19m, 0.408m, 0, 0.480m, 21m)];

    public static List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)>
        TransformationTree1 { get; } =
        [(0.05m, 0, 0, 0.6m, 0, 0, 10),
        (0.05m, 0, 0, -0.5m, 0, 1, 10),
        (0.46m, 0.32m, -0.386m, 0.383m, 0, 0.6m, 20),
        (0.47m, -0.154m, 0.171m, 0.423m, 0, 1, 20),
        (0.43m, 0.275m, -0.26m, 0.476m, 0, 1, 20),
        (0.421m, -0.357m, 0.354m, 0.307m, 0, 0.7m, 20)];

    public static List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)>
        TransformationLeaf0 { get; } =
        [(0.6m, 0, 0, 0.6m, 0.18m, 0.36m, 25),
        (0.6m, 0, 0, 0.6m, 0.18m, 0.12m, 25),
        (0.4m, 0.3m, -0.3m, 0.4m, 0.27m, 0.36m, 25),
        (0.4m, -0.3m, 0.3m, 0.4m, 0.27m, 0.09m, 25)];
    
    public static List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)>
        TransformationLeaf1 { get; } =
        [(0.18m, 0, 0, 0.5m, -0.1m, -1.3m, 7),
        (0.45m, 0.5m, -0.43m, 0.52m, 1.5m, -0.78m, 36),
        (0.5m, -0.5m, 0.45m, 0.45m, -1.6m, -0.7m, 37),
        (0.5m, 0.01m, 0, 0.5m, 0.1m, 1.6m, 20)];
    
    public static List<(decimal a, decimal b, decimal c, decimal d, decimal e, decimal f, decimal p)> 
        TransformationFern0 { get; } =
        [(0, 0, 0, 0.16m, 0, 0, 1m),
        (0.85m, 0.04m, -0.04m, 0.85m, 0, 1.6m, 85m),
        (0.2m, -0.26m, 0.23m, 0.22m, 0, 1.6m, 7m),
        (-0.15m, 0.28m, 0.26m, 0.24m, 0, 0.44m, 7m)];
}