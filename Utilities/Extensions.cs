using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public static class DictionaryExtensions
{
    // Works in C#3/VS2008:
    // Returns a new dictionary of this ... others merged leftward.
    // Keeps the type of 'this', which must be default-instantiable.
    // Example: 
    //   result = map.MergeLeft(other1, other2, ...)
    public static T MergeLeft<T,K,V>(this T me, params IDictionary<K,V>[] others)
        where T : IDictionary<K,V>, new()
    {
        T newMap = new T();
        foreach (IDictionary<K,V> src in
            (new List<IDictionary<K,V>> { me }).Concat(others)) {
            // ^-- echk. Not quite there type-system.
            foreach (KeyValuePair<K,V> p in src) {
                newMap[p.Key] = p.Value;
            }
        }
        return newMap;
    }

}

public static class ShellHelper
{
    public static string Bash(this string cmd)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");
        
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();
        return result;
    }
}