using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHelper
{
    public static class FilePathHelper
    {
        /// <summary>获取文件名枚举集合中所有具有指定后缀名的路径</summary>
        /// <param name="paths">文件名</param>
        /// <param name="fileExtension">指定的后缀名</param>
        /// <returns>所有具有指定后缀名的文件名</returns>
        public static IEnumerable<string> GetPathsWithFileExtension(this IEnumerable<string> paths, string fileExtension)
        {
            var length = fileExtension.Length;
            return paths.Where(p => string.Compare(p, p.Length - length, fileExtension, 0, length, true) == 0);
        }

        public static string GetName(this string file)
        {
            var index = file.LastIndexOf('\\');
            return file.Substring(index);
        }

    }
}
