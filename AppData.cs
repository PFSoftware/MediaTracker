using System;
using System.IO;

namespace PFSoftware.MediaTracker
{
    public static class AppData
    {
        internal static string Location = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PF Software", "MediaTracker");
    }
}