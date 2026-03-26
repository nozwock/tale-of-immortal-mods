using System;
using System.Diagnostics;

namespace MOD_YzmLuck;

internal class Tool
{
    internal static void Log(object info, ConsoleColor color = ConsoleColor.Cyan)
    {
        ModTitle();
        string text = Convert.ToString(info);
        Console.ForegroundColor = color;
        Console.Write(text + "\n");
        Console.ResetColor();
    }

    internal static void Log()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("=======================================================================");
        Console.ResetColor();
    }

    private static void ModTitle()
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("[先天气运多选MOD] ");
    }

    internal static void Error(string info, string objName = null, int index = 2)
    {
        string methodPath = GetMethodPath(index);
        ModTitle();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Error：" + methodPath + " ");
        if (objName != null)
        {
            Console.Write("：" + objName + " ");
        }
        Console.Write(info + "\n");
        Console.ResetColor();
    }

    internal static void Error(Exception ex)
    {
        ModTitle();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Error：" + ex.Message + "\n");
        Console.WriteLine(ex);
        Console.ResetColor();
    }

    internal static void Warning(string info, string objName = null, int index = 2)
    {
        string methodPath = GetMethodPath(index);
        ModTitle();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Warning：" + methodPath);
        if (objName != null)
        {
            Console.Write("：" + objName);
        }
        Console.Write(info + "\n");
        Console.ResetColor();
    }

    internal static bool ObjIsNull(object obj, string objName, int index = 2)
    {
        if (obj != null)
        {
            return false;
        }
        string methodPath = GetMethodPath(index);
        ModTitle();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Error：" + methodPath + "中的" + objName + "对象为null\n");
        Console.ResetColor();
        return true;
    }

    internal static string GetMethodPath(int index)
    {
        StackFrame frame = new StackTrace(fNeedFileInfo: true).GetFrame(index);
        string name = frame.GetMethod().ReflectedType.Name;
        string name2 = frame.GetMethod().Name;
        return name + "." + name2;
    }
}
