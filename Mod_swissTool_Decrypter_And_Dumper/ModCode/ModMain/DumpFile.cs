using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MelonLoader;

namespace MOD_swissTool
{
    public class DumpFile
    {
        public static void DumpData(bool dlc)
        {
            static void DumpConfs(object obj, string outdir)
            {
                Directory.CreateDirectory(outdir);

                var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                var fields = obj.GetType().GetProperties(flags); // Property instead of field due to il2cpp
                MelonLogger.Msg($"Total Fields in {obj.GetType().FullName}: {fields.Length}");

                foreach (var field in fields)
                {
                    MelonLogger.Msg($"Processing {field.PropertyType.Name} {field.Name}");
                    var conf = field.GetValue(obj);
                    if (conf == null)
                    {
                        MelonLogger.Msg($"Null value for {field.Name}");
                        continue;
                    }

                    // Look for _allConfList or allConfList
                    var confItems = conf.GetType().GetProperty("_allConfList", flags)?.GetValue(conf)
                        ?? conf.GetType().GetProperty("allConfList", flags)?.GetValue(conf);
                    if (confItems == null)
                    {
                        MelonLogger.Msg($"Couldn't find allConfList on {field.Name} or it's null");
                        continue;
                    }

                    var filename = CapitalizeFirst(field.Name) + ".json";
                    var json = JToken.Parse(CommonTool.ObjectToJson((Il2CppSystem.Object)confItems)).ToString(Formatting.Indented);

                    File.WriteAllText(Path.Combine(outdir, filename), json);
                }
            }

            static string CapitalizeFirst(string s)
            {
                if (string.IsNullOrEmpty(s))
                    return s;
                if (s.Length == 1)
                    return s.ToUpper();
                return char.ToUpper(s[0]) + s.Substring(1);
            }

            try
            {
                var outdir = Path.Combine(Directory.GetCurrentDirectory(), dlc ? "DataDumpDLC" : "DataDump");
                DumpConfs(dlc ? g.dlc.dlcConf.data : g.conf.data, outdir);
            }
            catch (Exception e)
            {
                MelonLogger.Error($"{e}");
            }
        }
    }
}
