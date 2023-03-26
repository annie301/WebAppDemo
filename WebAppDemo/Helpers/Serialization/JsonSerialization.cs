using Newtonsoft.Json;

namespace WebAppDemo.Helpers.Serialization
{
    public static class JsonSerialization
    {
        
        ///<param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param> 
        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter? writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                writer?.Close();
            }
        }        
    }
}
