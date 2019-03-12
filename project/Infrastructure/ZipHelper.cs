using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Ionic.Zip;

namespace Infrastructure
{
    /// <summary>
    /// Zip操作 -- 基于DotNetZip的封装
    /// </summary>
    public class ZipHelper
    {
        #region 得到指定的输入流的ZIP压缩刘对象[原有流对象不会改变] + static Stream ZipCompress
        /// <summary>
        /// 得到指定的输入流的ZIP压缩刘对象[原有流对象不会改变]
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="entryName"></param>
        /// <returns></returns>
        public static Stream ZipCompress(Stream sourceStream, string entryName = "zip")
        {
            MemoryStream compressedStream = new MemoryStream();
            if (sourceStream != null)
            {
                long sourceOldPosition = 0;
                try
                {
                    sourceOldPosition = sourceStream.Position;
                    sourceStream.Position = 0;
                    using (ZipFile zip = new ZipFile())
                    {
                        zip.AddEntry(entryName, sourceStream);
                        zip.Save(compressedStream);
                        compressedStream.Position = 0;
                    }
                }
                catch
                {

                }
                finally
                {
                    try
                    {
                        sourceStream.Position = sourceOldPosition;
                    }
                    catch
                    {

                    }
                }
            }
            return compressedStream;
        }
        #endregion

        #region 得到指定的字节数组的ZIP解压流对象 + static Stream ZipDecompress
        /// <summary>
        /// 得到指定的字节数组的ZIP解压流对象
        /// 当前方法仅适用于只有一个压缩文件的压缩包, 即方法内只取压缩包中的第一个压缩文件
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Stream ZipDecompress(byte[] data)
        {
            Stream decompressedStream = new MemoryStream();
            if (data != null)
            {
                try
                {
                    MemoryStream dataStream = new MemoryStream(data);
                    using (ZipFile zip = ZipFile.Read(dataStream))
                    {
                        if (zip.Entries.Count > 0)
                        {
                            zip.Entries.First().Extract(decompressedStream);

                            // Extract方法中会操作ms，后续使用时必须先将Stream位置归零，否则会导致后续读取不到任何数据
                            // 返回该Stream对象之前进行一次位置归零动作
                            decompressedStream.Position = 0;
                        }
                    }
                }
                catch
                {

                }
            }
            return decompressedStream;
        }
        #endregion

        #region 压缩ZIP文件 + static bool CompressMulti
        /// <summary>
        /// 压缩ZIP文件
        /// 支持多文件和多目录，或是多文件和多目录一起压缩
        /// </summary>
        /// <param name="list">待压缩的文件或目录集合</param>
        /// <param name="strZipName">压缩后的文件名</param>
        /// <param name="IsDirStruct">是否按目录结构压缩</param>
        /// <returns>成功：true/失败：false</returns>
        public static bool CompressMulti(List<string> list, string strZipName, bool IsDirStruct)
        {
            try
            {
                using (ZipFile zip = new ZipFile(Encoding.Default))
                {
                    foreach (var path in list)
                    {
                        //取目录名
                        string fileName = Path.GetFileName(path);

                        //判断路径是否为目录
                        if (Directory.Exists(path))
                        {
                            //按目录结构压缩
                            if (IsDirStruct)
                            {
                                zip.AddDirectory(path, fileName);
                            }
                            else
                            {
                                //目录下的文件都压缩到Zip的根目录
                                zip.AddDirectory(path);
                            }
                        }

                        //判断路径是否为文件
                        if (File.Exists(path))
                        {
                            zip.AddFile(path);
                        }
                        //压缩
                        zip.Save(strZipName);
                        return true;
                    }
                }
            }
            catch
            {

            }
            return false;
        }
        #endregion

        #region 解压ZIP文件 + static bool Decompression
        /// <summary>
        /// 解压ZIP文件
        /// </summary>
        /// <param name="strZipPath">待解压的ZIP文件</param>
        /// <param name="strUnZipPath">解压的目录</param>
        /// <param name="overWrite">true:同名覆盖, false:同名不覆盖</param>
        /// <returns>成功：true/失败：false</returns>
        public static bool Decompression(string strZipPath, string strUnZipPath, bool overWrite)
        {
            try
            {
                ReadOptions options = new ReadOptions();
                //设置编码, 解决解压文件中的中文乱码
                options.Encoding = Encoding.Default;
                using (ZipFile zip = ZipFile.Read(strZipPath, options))
                {
                    foreach (var entry in zip)
                    {
                        //解压路径为空时, 解压到以文件名命名的文件下
                        if (string.IsNullOrEmpty(strUnZipPath))
                        {
                            strUnZipPath = strZipPath.Split('.').First();
                        }

                        //解压文件
                        if (overWrite)
                        {
                            //解压文件, 如果已存在, 就覆盖
                            entry.Extract(strUnZipPath, ExtractExistingFileAction.OverwriteSilently);
                        }
                        else
                        {
                            //解压文件, 如果已存在, 不覆盖
                            entry.Extract(strUnZipPath, ExtractExistingFileAction.DoNotOverwrite);
                        }
                    }
                    return true;
                }
            }
            catch
            {

            }
            return false;
        } 
        #endregion
    }
}
