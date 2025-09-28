using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MOD_swissTool
{
    public class DumpFile
    {
        public static void dumpData(bool dlc)
        {
            string DumpFileName;
            string contents;
            string[] ClassNames;
            string classListDir = g.mod.GetModPathRoot("swissTool") + "\\ModAssets"; // Obtain the directory of the mod folder
            string currentDirectory = Directory.GetCurrentDirectory();
            // Variable names depending on Main or DLC
            string ClassListFileName;
            string DumpFolderName;
            // Determine Class list file name and output folder
            if (!dlc)
            {
                ClassListFileName = "\\dumpClassList.txt";
                DumpFolderName = "\\DataDump";
            }
            else
            {
                ClassListFileName = "\\dumpClassListDLC.txt";
                DumpFolderName = "\\DataDumpDLC";
            }
            // Check if the list-file with Class names exists
            if (!File.Exists(classListDir + ClassListFileName))
            {
                Console.WriteLine($"Doesn't exist: {classListDir + ClassListFileName}");
                return; // I can't believe I hadn't thought of adding a return earlier
            }
            else
            {
                Console.WriteLine($"Found in: {classListDir + ClassListFileName}");
                ClassNames = File.ReadAllLines(classListDir + ClassListFileName);
            }

            // Create directory for those files
            currentDirectory += DumpFolderName;
            Directory.CreateDirectory(currentDirectory);

            // Loop through all names
            foreach (string className in ClassNames)
            {
                DumpFileName = char.ToUpper(className[0]) + className.Substring(1);
                JToken jt = JToken.Parse(getClass(className, dlc));
                contents = jt.ToString(Formatting.Indented); // Turn the data into Json string

                // Save the data...
                File.WriteAllText(DumpFileName + ".json", contents);
                string sourceFileName = Directory.GetCurrentDirectory() + "\\" + DumpFileName + ".json";
                string text = currentDirectory + "\\" + DumpFileName + ".json";
                if (File.Exists(text))
                {
                    File.Delete(text);
                }
                File.Move(sourceFileName, text);
            }
        }

        public static string getClass(string className, bool dlc)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            PropertyInfo info;
            Object gotClass;

            if (!dlc) info = g.conf.GetType().GetProperty(className, flags);
            else info = g.dlc.dlcConf.GetType().GetProperty(className, flags);
            if (info == null)
            {
                Console.WriteLine("The Class doesn't exist");
                return "";
            }
            Console.WriteLine($"Found the Class: {className}");

            if (!dlc) gotClass = info.GetValue(g.conf);
            else gotClass = info.GetValue(g.dlc.dlcConf);
            PropertyInfo info2 = gotClass.GetType().GetProperty("_allConfList", flags);
            if (info2 == null)
            { // If _allConfList doesn't exist, try to get the allConfList field
                Console.WriteLine("_allConfList got nothing");
                info2 = gotClass.GetType().GetProperty("allConfList", flags);
                if (info2 == null) 
                {
                    Console.WriteLine("allConfList got nothing either");
                    return ""; 
                }
            }
            // Convert to Json and return the string
            return CommonTool.ObjectToJson((Il2CppSystem.Object)info2.GetValue(gotClass));
        }

        public static string getClass2(string className)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            PropertyInfo info = g.data.GetType().GetProperty(className, flags);
            if (info == null)
            {
                Console.WriteLine("The Class doesn't exist");
                return "";
            }
            Console.WriteLine($"Found the Class: {className}");
            Object gotClass = info.GetValue(g.data);

            // Convert to Json and return the string
            return CommonTool.ObjectToJson((Il2CppSystem.Object)info.GetValue(g.data));
        }

        // Used to check the list first
        public static void ReadFile()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            if (File.Exists(currentDirectory + "\\dumpClassList.txt"))
            {
                string[] lines = File.ReadAllLines(currentDirectory + "\\dumpClassList.txt");
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
                return;
            }
            else
            {
                Console.WriteLine("Nothing");
                return;
            }
        }
    }
}
