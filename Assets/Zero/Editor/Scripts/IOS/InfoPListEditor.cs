#if UNITY_IPHONE
using System.IO;
using UnityEditor.iOS.Xcode;

namespace ZeroEditor.IOS
{
    /// <summary>
    /// 修改XCODE项目中的info.plist文件
    /// </summary>
    public class InfoPListEditor
    {
        string _path;

        PlistDocument _plist;

        /// <summary>
        /// info.plist文件的文档
        /// </summary>
        /// <value>The document.</value>
        public PlistDocument document
        {
            get { return _plist; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">info.plist文件的路径</param>
        public InfoPListEditor(string path)
        {
            _path = path;
            _plist = new PlistDocument();
            string plistStr = File.ReadAllText(path);
            _plist.ReadFromString(plistStr);
        }

        /// <summary>
        /// 保存修改的Plist文件
        /// </summary>
        public void Save()
        {
            File.WriteAllText(_path, _plist.WriteToString());
        }

        /// <summary>
        /// 添加内容到PList
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            PlistElementDict root = _plist.root;
            root.SetString(key, value);
        }

        /// <summary>
        /// 添加urlscheme白名单
        /// </summary>
        /// <param name="urlScheme">URL scheme.</param>
        public void AddLSApplicationQueriesScheme(string urlScheme)
        {
            const string KEY = "LSApplicationQueriesSchemes";
            PlistElementDict root = _plist.root;
            PlistElementArray urlSchemeList = root[KEY] as PlistElementArray;
            if (null == urlSchemeList)
            {
                urlSchemeList = root.CreateArray(KEY);
            }

            bool isInclude = false;

            foreach (PlistElement item in urlSchemeList.values)
            {
                if (item.AsString() == urlScheme)
                {
                    isInclude = true;
                    break;
                }
            }

            if (false == isInclude)
            {
                urlSchemeList.AddString(urlScheme);
            }
        }


        /// <summary>
        /// 添加URLSchemes
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="urlScheme">URL scheme.</param>
        public void AddUrlScheme(string identifier, string urlScheme)
        {
            const string KEY = "CFBundleURLTypes";
            const string IDENTIFIER_KEY = "CFBundleURLName";
            const string URLSCHEMES_KEY = "CFBundleURLSchemes";

            PlistElementDict root = _plist.root;
            PlistElementArray urlTypeList = root[KEY] as PlistElementArray;
            if (null == urlTypeList)
            {
                urlTypeList = root.CreateArray(KEY);
            }

            PlistElementDict urlType = null;
            foreach (PlistElementDict item in urlTypeList.values)
            {
                if (item[IDENTIFIER_KEY].AsString() == identifier)
                {
                    urlType = item;
                    break;
                }
            }

            if (null == urlType)
            {
                urlType = urlTypeList.AddDict();
            }

            urlType.SetString(IDENTIFIER_KEY, identifier);

            PlistElementArray urlSchemes = urlType[URLSCHEMES_KEY] as PlistElementArray;
            if (null == urlSchemes)
            {
                urlSchemes = urlType.CreateArray(URLSCHEMES_KEY);
            }

            bool isInclude = false;

            foreach (PlistElement item in urlSchemes.values)
            {
                if (item.AsString() == urlScheme)
                {
                    isInclude = true;
                    break;
                }
            }

            if (false == isInclude)
            {
                urlSchemes.AddString(urlScheme);
            }
        }
    }
}
#endif
