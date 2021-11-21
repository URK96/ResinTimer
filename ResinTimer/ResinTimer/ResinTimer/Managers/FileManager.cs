using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Xamarin.Essentials;

namespace ResinTimer.Managers
{
    internal static class FileManager
    {
        internal static string CacheDirPath => FileSystem.CacheDirectory;
        internal static string AppDataDirPath => FileSystem.AppDataDirectory;

        internal static async Task<bool> SaveString(string filePath, string str)
        {
            try
            {
                using StreamWriter sw = new(filePath, false);

                await sw.WriteAsync(str);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        internal static async Task<string> LoadString(string filePath)
        {
            string str = null;

            try
            {
                using StreamReader sr = new(filePath);

                str = await sr.ReadToEndAsync();
            }
            catch (Exception)
            {
                str = null;
            }

            return str;
        }

        internal static async Task<bool> SaveObject<T>(string filePath, T obj)
        {
            try
            {
                using StreamWriter sw = new(filePath, false);

                await sw.WriteAsync(JsonConvert.SerializeObject(obj));
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        internal static async Task<T> LoadObject<T>(string filePath)
        {
            T obj = default;

            try
            {
                using StreamReader sr = new(filePath);

                obj = JsonConvert.DeserializeObject<T>(await sr.ReadToEndAsync());
            }
            catch (Exception)
            {
                obj = default;
            }

            return obj;
        }
    }
}
