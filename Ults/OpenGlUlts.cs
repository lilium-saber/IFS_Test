namespace AvaloniaApp.Ults;

internal static class OpenGlUlts
{
    private static float _maxLightStrength = 0.25f;

    internal static float MinLightStrength
    {
        get => _maxLightStrength;
        set => _maxLightStrength = value <= 0.1f ? 0.1f : value;
    }
}