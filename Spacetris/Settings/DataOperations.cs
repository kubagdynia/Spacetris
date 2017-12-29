using Newtonsoft.Json;
using Spacetris.Extensions;
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
                $"Saved Data to: {tempPath}".Log();
                #endif
            }
            catch (Exception e)
            {
                #if DEBUG
                $"Failed to save data to: {tempPath}".Log();
                $"Error: {e.Message}".Log();
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
                "Directory does not exist".Log();
                #endif

                return default(T);
            }

            // Exit if File does not exist
            if (!File.Exists(tempPath))
            {
                #if DEBUG
                "File does not exist".Log();
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
                $"Loaded data from: {tempPath}".Log();
                #endif
            }
            catch (Exception e)
            {
                #if DEBUG
                $"Failed to load data from: {tempPath}".Log();
                $"Error: {e.Message}".Log();
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