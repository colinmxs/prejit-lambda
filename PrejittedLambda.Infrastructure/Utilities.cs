namespace PrejittedLambda.Infrastructure
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class Utilities
    {
        /// <summary>
        /// Uses Newtonsoft to deserialize json file into Dictionary<string, string>
        /// </summary>
        public static Dictionary<string, string> LoadSettingsFromJsonFile(string fileName)
        {
            Dictionary<string, string> pairs;
            using (StreamReader reader = new StreamReader(fileName))
            {
                string json = reader.ReadToEnd();
                pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }

            return pairs;
        }


        public static string GetDirectory(string directoryName)
        {
            var projectRelativePath = @"";

            // Get currently executing test project path
            var currentDirectory = Directory.GetCurrentDirectory();

            // Find the path to the target folder
            var directoryInfo = new DirectoryInfo(currentDirectory);
            do
            {
                directoryInfo = directoryInfo.Parent;

                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));
                if (projectDirectoryInfo.Exists)
                {

                    var dropDirectoryInfo = new DirectoryInfo(Path.Combine(projectDirectoryInfo.FullName, directoryName));
                    if (dropDirectoryInfo.Exists)
                    {
                        return Path.Combine(projectDirectoryInfo.FullName, directoryName);
                    }
                }
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Drop directory could not be found {currentDirectory}.");
        }
    }
}
