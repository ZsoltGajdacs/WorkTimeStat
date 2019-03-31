﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterWork.Helpers
{
    internal class Serializer
    {
        public void JsonObjectSerialize<T>(string path, ref T serializable)
        {
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

        public T JsonObjectDeserialize<T>(string path)
        {
            if (new FileInfo(path).Exists)
            {
                TextReader reader = null;
                try
                {
                    reader = new StreamReader(path);
                    var fileContents = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<T>(fileContents);
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
    }
}
