namespace Archetype.Shared.Configuration;

public static class MemeoryStack
{
    private static readonly Stack<Assembly> _assemblies = [];

    public static void AddAssembly(Assembly assembly)
        => _assemblies.Push(assembly);

    public static IReadOnlyCollection<Assembly> GetAll()
        => [.. _assemblies];

    public static void Flush()
        => _assemblies.Clear();
}
