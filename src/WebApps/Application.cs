using System.Reflection;

namespace Peereflits.Shared.Logging;

internal static class Application
{
    private const string UnknownApplicationName = "[Unknown]";

    public static string Name
    {
        get
        {
            string result = Assembly.GetEntryAssembly()?.GetName().Name ?? UnknownApplicationName;
            return result == UnknownApplicationName
                           ? Assembly.GetExecutingAssembly().GetName().Name ?? UnknownApplicationName
                           : result;
        }
    }
}