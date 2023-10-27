namespace D4Ocr;

using System.Diagnostics;

public static class ProcessLocator
{
    public static IntPtr MainHandle()
    {
        var process = Process.GetProcessesByName("Diablo IV").FirstOrDefault();

        if (process == null)
        {
            throw new InvalidOperationException("Diablo IV process not found.");
        }

        return process.MainWindowHandle;
    }
}