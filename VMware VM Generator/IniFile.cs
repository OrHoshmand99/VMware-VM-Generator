using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class IniFile
{
    private readonly string path;
    private readonly Dictionary<string, Dictionary<string, string>> data =
        new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

    public IniFile(string path)
    {
        this.path = path;
        Load();
    }

    private void Load()
    {
        if (!File.Exists(path))
            return;

        string currentSection = "";

        foreach (var rawLine in File.ReadAllLines(path, Encoding.UTF8))
        {
            string line = rawLine.Trim();

            if (line.StartsWith(";") || line == "")
                continue;

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                currentSection = line.Substring(1, line.Length - 2).Trim();
                if (!data.ContainsKey(currentSection))
                    data[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                int idx = line.IndexOf('=');
                if (idx > 0)
                {
                    string key = line.Substring(0, idx).Trim();
                    string value = line.Substring(idx + 1).Trim();

                    if (!data.ContainsKey(currentSection))
                        data[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    data[currentSection][key] = value;
                }
            }
        }
    }

    public string Read(string key, string section)
    {
        if (data.ContainsKey(section) && data[section].ContainsKey(key))
            return data[section][key];

        return "";
    }

    public void Write(string key, string value, string section)
    {
        if (!data.ContainsKey(section))
            data[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        data[section][key] = value;

        Save();
    }

    private void Save()
    {
        var sb = new StringBuilder();

        foreach (var section in data)
        {
            sb.AppendLine($"[{section.Key}]");

            foreach (var kv in section.Value)
                sb.AppendLine($"{kv.Key}={kv.Value}");

            sb.AppendLine();
        }

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
    }
}
