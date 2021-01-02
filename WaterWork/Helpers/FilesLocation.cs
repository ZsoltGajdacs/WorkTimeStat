using System;
using System.Globalization;
using System.IO;

namespace WorkTimeStat.Helpers
{
    /// <summary>
    /// Keeps the path strings for all serialized files
    /// </summary>
    internal static class FilesLocation
    {
        /// <summary>
        /// Gives back the location of the dir where all the files are saved
        /// </summary>
        /// <returns></returns>
        internal static string GetSaveDirPath()
        {
            FileInfo file = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
            return file.DirectoryName;
        }

        /// <summary>
        /// Gives back the name of the WorkKeeper
        /// </summary>
        /// <returns></returns>
        internal static string GetWaterWorkFileName()
        {
            return "waterwork" + DateTime.Now.Year.ToString(CultureInfo.InvariantCulture) + ".json";
        }
    }
}
