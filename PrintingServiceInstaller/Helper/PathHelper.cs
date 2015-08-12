using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PrintingServiceInstaller
{
    public static class PathHelper
    {
        public static string GetFullPath(string partial_path)
        {
            var exeAssembly = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            return Path.Combine(exeAssembly, partial_path);
        }
    }
}
