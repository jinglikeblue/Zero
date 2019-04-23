using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jing
{
    public class CSVFile
    {
        List<string[]> _data = new List<string[]>();

        /// <summary>
        /// 数据
        /// </summary>
        public string[][] Data
        {
            get { return _data.ToArray(); }
        }

        int _rowCount = 0;

        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount
        {
            get { return _rowCount; }
        }

        int _colCount = 0;

        /// <summary>
        /// 列数
        /// </summary>
        public int ColCount
        {
            get { return _colCount; }
        }

        /// <summary>
        /// 通过数据生成
        /// </summary>
        /// <param name="data"></param>
        public CSVFile(byte[] data, Encoding encoding)
        {            
            var content = encoding.GetString(data);
            string[] rows = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            ParseRows(rows);
        }

        public CSVFile(string path)
        {
            System.Text.Encoding encoding = GetEncoding(path);
            string[] rows = File.ReadAllLines(path, encoding);
            ParseRows(rows);
        }       
        
        void ParseRows(string[] rows)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                var cols = GetCols(rows[i]);
                if (null != cols)
                {
                    _data.Add(cols.ToArray());
                }
            }

            _rowCount = _data.Count;
            if (_rowCount > 0)
            {
                _colCount = _data[0].Length;
            }
        }

        /// <summary>
        /// 得到表格的值
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public string GetValue(int row, int col)
        {
            return _data[row][col];
        }

        /// <summary>
        /// 分割一行字符串中的列
        /// </summary>
        /// <param name="rowContent"></param>
        /// <returns></returns>
        List<string> GetCols(string rowContent)
        {
            //引号
            const char QUOTATION_MARKS = '"';
            //逗号
            const char COMMA = ',';

            List<string> cols = new List<string>();
            //分割标记(同时也是一列字符串的第一个字符的索引)
            int splitMark = 0;
            int charIdx = 0;
            bool isSpecial = false;
            while (charIdx < rowContent.Length)
            {
                char c = rowContent[charIdx];
                int nextIdx = charIdx + 1;

                if (charIdx == splitMark)
                {                    
                    if (c == QUOTATION_MARKS)
                    {
                        isSpecial = true;                        
                    }
                    else
                    {
                        isSpecial = false;
                        if(nextIdx == rowContent.Length)
                        {
                            //结束符
                            string colContent = rowContent.Substring(splitMark);
                            cols.Add(colContent);
                            break;
                        }
                    }
                }
                else
                {
                    if(isSpecial)
                    {
                        //处理含有特殊字符串的内容
                        if (c == QUOTATION_MARKS)
                        {                            
                            if (nextIdx == rowContent.Length)
                            {
                                //结束符
                                string colContent = rowContent.Substring(splitMark + 1, charIdx - splitMark - 1);
                                colContent = colContent.Replace("\"\"", "\"");
                                cols.Add(colContent);                                
                                //跳过下一个引号
                                charIdx++;
                            }
                            else
                            {
                                char nextC = rowContent[nextIdx];
                                if(nextC == QUOTATION_MARKS)
                                {
                                    //跳过双引号
                                    charIdx++;
                                }
                                else if(nextC == COMMA)
                                {
                                    //分割符
                                    string colContent = rowContent.Substring(splitMark + 1, charIdx - splitMark - 1);
                                    colContent = colContent.Replace("\"\"", "\"");
                                    cols.Add(colContent);
                                    charIdx++;
                                    splitMark = nextIdx + 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        //处理普通字符串的内容
                        if(c == COMMA)
                        {
                            //分割符
                            string colContent = rowContent.Substring(splitMark, charIdx - splitMark);
                            cols.Add(colContent);
                            splitMark = charIdx + 1;                            
                        }

                        if (nextIdx == rowContent.Length)
                        {
                            //结束符
                            string colContent = rowContent.Substring(splitMark);
                            cols.Add(colContent);
                            break;
                        }
                    }
                }

                charIdx++;
            }

            return cols;
        }

        System.Text.Encoding GetEncoding(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            System.Text.Encoding r = GetEncoding(fs);
            fs.Close();
            return r;
        }


        System.Text.Encoding GetEncoding(FileStream fs)
        {
            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            byte[] ss = r.ReadBytes(3);
            r.Close();
            //编码类型 Coding=编码类型.ASCII;   
            if (ss[0] >= 0xEF)
            {
                if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                {
                    return System.Text.Encoding.UTF8;
                }
                else if (ss[0] == 0xFE && ss[1] == 0xFF)
                {
                    return System.Text.Encoding.BigEndianUnicode;
                }
                else if (ss[0] == 0xFF && ss[1] == 0xFE)
                {
                    return System.Text.Encoding.Unicode;
                }
                else
                {
                    return System.Text.Encoding.Default;
                }
            }
            else
            {
                return System.Text.Encoding.Default;
            }
        }
    }
}
