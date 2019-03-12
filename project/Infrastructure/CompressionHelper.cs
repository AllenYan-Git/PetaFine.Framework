using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using SharpCompress.Reader;
using SharpCompress.Common;
using SharpCompress.Archive;

namespace Infrastructure
{
    /// <summary>
    /// 压缩解压缩帮助类
    /// 可以用来解压 RAR / ZIP
    /// </summary>
    public class CompressionHelper
    {
        public static bool UnCompressRar(string sourcePath, string destPath)
        {
            if (!File.Exists(sourcePath))
            {
                return false;
            }
            try
            {
                using (Stream stream = File.OpenRead(sourcePath))
                {
                    var reader = ReaderFactory.Open(stream);
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            //Console.WriteLine(reader.Entry.FilePath);
                            reader.WriteEntryToDirectory(destPath, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        //public static bool UnCompressZIP(string sourcePath, string destPath)
        //{
        //    if (!File.Exists(sourcePath))
        //    {
        //        return false;
        //    }

        //    var archive = ArchiveFactory.Open(sourcePath);
        //    foreach (var entry in archive.Entries)
        //    {
        //        if (!entry.IsDirectory)
        //        {
        //            //Console.WriteLine(entry.FilePath);
        //            entry.WriteToDirectory(destPath, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
        //        }
        //    }
        //    return true;
        //}
    }
}
