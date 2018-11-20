using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Zero
{
    /// <summary>
    /// Zip解压助手
    /// </summary>
    public class ZipHelper
    {
        /// <summary>
        /// 进度
        /// </summary>
        public float progress = 0f;

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool isDone = false;

        /// <summary>
        /// 错误内容
        /// </summary>
        public string error = null;

        string _zipFile;
        string _targetDir;
        byte[] _zipBytes;


        public ZipHelper()
        {

        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="zipFile">压缩文件路径</param>
        /// <param name="targetDir">解压目录</param>
        public void UnZip(string zipFile, string targetDir)
        {
            _zipFile = zipFile;
            UnZip(targetDir);
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="stream">压缩文件流</param>
        /// <param name="targetDir">解压目录</param>
        void UnZip(string targetDir)
        {           
            _targetDir = targetDir;
            ZipConstants.DefaultCodePage = 0;            
            Thread thread = new Thread(new ThreadStart(ProcessUnZip));
            thread.Start();
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="bytes">二进制数据</param>
        /// <param name="targetDir">解压目录</param>
        public void UnZip(byte[] bytes, string targetDir)
        {
            _zipBytes = bytes;
            UnZip(targetDir);
        }


        Stream GetNewStream()
        {
            if(_zipBytes != null)
            {
                return new MemoryStream(_zipBytes);
            }
            else if(_zipFile != null)
            {
                return File.OpenRead(_zipFile);
            }
            return null;
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="targetDir">压缩文件夹路径</param>
        /// <param name="zipFile">压缩文件夹保存位置</param>
        /// <param name="whiteExtList">扩展名白名单，如果不为null，则只压缩白名单中指定后缀的文件</param>
        //public void Zip(string targetDir, string zipFile, string[] whiteExtList = null)
        //{            
        //    _targetDir = targetDir;
        //    _zipFile = zipFile;

        //    try
        //    {
        //        ZipOutputStream s = new ZipOutputStream(File.Create(zipFile));                
        //        s.SetLevel(9); 
        //        byte[] buffer = new byte[4096];
        //        AddEntrys(s, buffer, targetDir, whiteExtList);                    
        //        s.Finish();                    
        //        s.Close();                
        //    }
        //    catch (Exception e)
        //    {
        //        error = e.Message;
        //    }

        //    isDone = true;
        //    progress = 1f;
        //}

        //void AddEntrys(ZipOutputStream s, byte[] buffer, string dir, string[] whiteExtList)
        //{
        //    string[] filenames = Directory.GetFiles(dir);
        //    string[] dirs = Directory.GetDirectories(dir);            

        //    //处理文件
        //    foreach (string file in filenames)
        //    {
        //        #region 扩展名白名单校验
        //        if (null != whiteExtList)
        //        {
        //            string ext = Path.GetExtension(file);

        //            bool isExtInWhiteList = false;
        //            for(int i = 0; i < whiteExtList.Length; i++)
        //            {
        //                if(whiteExtList[i] == ext)
        //                {
        //                    isExtInWhiteList = true;
        //                    break;
        //                }
        //            }

        //            if (false == isExtInWhiteList)
        //            {
        //                continue;
        //            }
        //        }
        //        #endregion

        //        string saveFile = file.Replace(_targetDir, "");
        //        ZipEntry entry = new ZipEntry(saveFile);                
        //        entry.DateTime = DateTime.Now;
        //        s.PutNextEntry(entry);

        //        using (FileStream fs = File.OpenRead(file))
        //        {
        //            int sourceBytes;
        //            do
        //            {
        //                sourceBytes = fs.Read(buffer, 0, buffer.Length);
        //                s.Write(buffer, 0, sourceBytes);
        //            } while (sourceBytes > 0);
        //        }
        //    }

        //    //处理文件夹
        //    foreach (string subDir in dirs)
        //    {
        //        AddEntrys(s, buffer, subDir, whiteExtList);
        //    }
        //}

        void ProcessUnZip()
        {
            try
            {
                ///第一次打开 获取文件总数
                ZipInputStream s = new ZipInputStream(GetNewStream());
                //total = s.Length;
                List<ZipEntry> entryList = new List<ZipEntry>();
                ZipEntry entry;
                while ((entry = s.GetNextEntry()) != null)
                {
                    entryList.Add(entry);
                }

                long total = entryList.Count;
                long current = 0;
                entryList.Clear();
                s.Close();

                //创建LUA脚本目录
                if(false == Directory.Exists(_targetDir))
                {
                    Directory.CreateDirectory(_targetDir);
                }

                ///第二次打开 
                s = new ZipInputStream(GetNewStream());

                while ((entry = s.GetNextEntry()) != null)
                {
                    string targetPath = _targetDir + entry.Name;
                    
                    if (entry.IsDirectory)
                    {
                        Directory.CreateDirectory(targetPath);
                    }
                    else if (entry.IsFile)
                    {
                        string dirName = Path.GetDirectoryName(targetPath);
                        if (false == Directory.Exists(dirName))
                        {
                            Directory.CreateDirectory(dirName);
                        }

                        FileStream fs = File.Create(targetPath);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                fs.Write(data, 0, size);
                            }
                            else
                            {
                                fs.Close();
                                break;
                            }
                        }
                        progress = ++current / (float)total;
                        //Thread.Sleep(100);
                    }
                }  
                
                s.Close();
            }
            catch (Exception e)
            {
                error = e.Message;                
            }

            isDone = true;
            progress = 1f;
        }


    }
}
