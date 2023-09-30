using System.Text;
using System.Text.Json;
using Spacetris.Extensions;

namespace Spacetris.Settings;

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
            string jsonData = JsonSerializer.Serialize(data);
            byte[] jsonByte = Encoding.UTF8.GetBytes(jsonData);

            // Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                string path = Path.GetDirectoryName(tempPath);
                if (path is not null)
                {
                    Directory.CreateDirectory(path);
                }
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
                return default;
            }

            // Exit if File does not exist
            if (!File.Exists(tempPath))
            {
#if DEBUG
                "File does not exist".Log();
#endif
                return default;
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
            object resultValue = JsonSerializer.Deserialize<T>(jsonData);
            if (resultValue is not null)
            {
                return (T)Convert.ChangeType(resultValue, typeof(T));
            }

            throw new ArgumentNullException(nameof(resultValue));
        }

        public static T LoadData<T>(string fileName) => LoadData<T>(fileName, out _);
}