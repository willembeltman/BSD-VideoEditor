using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoEditor.Services
{
    public static class CheckFileType
    {
        static string[] supportedExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv" };

        public static string[] Filter(IEnumerable<string> files)
        {
            return files
                .Where(a => Check(a))
                .ToArray();
        }
        public static bool Check(IEnumerable<string> files)
        {
            foreach (var file in files)
                if (!Check(file))
                    return false; // Return false if any file does not match the desired file types
            return true;
        }
        public static bool Check(string file)
        {
            string extension = Path.GetExtension(file);
            return
                !string.IsNullOrEmpty(extension) &&
                supportedExtensions.Contains(extension.ToLower());
        }
    }
}
