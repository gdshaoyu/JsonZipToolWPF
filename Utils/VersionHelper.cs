using System.IO;
using System.Reflection;

namespace JsonZipToolWPF.Utils
{
    public static class VersionHelper
    {
        public static string GetVersion()
        {
            try
            {
                // 首先尝试从文件读取（开发环境）
                string versionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Properties", "version.txt");
                if (File.Exists(versionFile))
                {
                    return File.ReadAllText(versionFile).Trim();
                }

                // 如果文件不存在，尝试从嵌入资源读取（发布环境）
                var assembly = Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream("JsonZipToolWPF.Properties.version.txt");
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    return reader.ReadToEnd().Trim();
                }

                return "未知版本";
            }
            catch
            {
                return "未知版本";
            }
        }
    }
} 