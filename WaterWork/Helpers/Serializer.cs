using Newtonsoft.Json;
using System.IO;

namespace WaterWork.Helpers
{
    /// <summary>
    /// Keeps all the serializer methods
    /// </summary>
    internal static class Serializer
    {
        internal static void JsonObjectSerialize<T>(string path, ref T serializable, bool doBackup)
        {
            if (doBackup)
            {
                CreateBackup(path);
            }

            TextWriter writer = null;
            try
            {
                string output = JsonConvert.SerializeObject(serializable);
                writer = new StreamWriter(path, false);
                writer.Write(output);
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }

        internal static T JsonObjectDeserialize<T>(string path)
        {
            if (new FileInfo(path).Exists)
            {
                TextReader reader = null;
                try
                {
                    reader = new StreamReader(path);
                    string fileContents = reader.ReadToEnd();

                    JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };

                    return JsonConvert.DeserializeObject<T>(fileContents, jsonSerializerSettings);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
            else
            {
                return default(T);
            }
        }

        private static void CreateBackup(string path)
        {
            if (File.Exists(path))
            {
                string backupPath = path + ".bak";
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                File.Copy(path, backupPath);
            }
        }
    }
}
