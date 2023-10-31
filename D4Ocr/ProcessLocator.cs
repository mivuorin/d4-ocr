using System.Diagnostics;

namespace D4Ocr;

public static class ProcessLocator
{
    public static IntPtr MainHandle(string processName)
    {
        var process = Process.GetProcessesByName(processName).FirstOrDefault();

        if (process == null)
        {
            throw new InvalidOperationException($"{processName} process not found.");
        }

        return process.MainWindowHandle;
    }
}