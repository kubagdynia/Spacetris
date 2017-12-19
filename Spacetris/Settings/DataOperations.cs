using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Spacetris.Settings
{
    public static class DataOperations
    {
        public static void SaveData<T>(T data, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            
            string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            // Convert To Json then to bytes
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            byte[] jsonByte = Encoding.UTF8.GetBytes(jsonData);

            // Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
            }

            try
            {
                File.WriteAllBytes(tempPath, jsonByte);
#if DEBUG
                Console.WriteLine("Saved Data to: " + tempPath);
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("Failed to save data to: " + tempPath);
                Console.WriteLine("Error: " + e.Message);
#endif
                throw;
            }
        }

        public static T LoadData<T>(string fileName, out bool fileExists)
        {
            fileExists = false;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            
            string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            // Exit if Directory does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
#if DEBUG
                Console.WriteLine("Directory does not exist");
#endif
                return default(T);
            }

            // Exit if File does not exist
            if (!File.Exists(tempPath))
            {
#if DEBUG
                Console.WriteLine("File does not exist");
#endif
                return default(T);
            }

            fileExists = true;

            // Load saved Json
            byte[] jsonByte;
            try
            {
                jsonByte = File.ReadAllBytes(tempPath);
#if DEBUG
                Console.WriteLine("Loaded data from: " + tempPath);
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("Failed to load data from: " + tempPath);
                Console.WriteLine("Error: " + e.Message);
#endif
                throw;
            }

            // Convert to json string
            string jsonData = Encoding.ASCII.GetString(jsonByte);

            // Convert to Object
            object resultValue = JsonConvert.DeserializeObject<T>(jsonData);
            return (T)Convert.ChangeType(resultValue, typeof(T));
        }

        public static T LoadData<T>(string fileName) => LoadData<T>(fileName, out _);
    }
}