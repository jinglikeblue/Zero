using Jing;
using System.IO;
using UnityEditor;

namespace Zero.Edit
{
    public class HotResPublishModel
    {
        const string CONFIG_NAME = "HotResCfg.json";
        HotResConfigVO _cfg;

        public HotResConfigVO Cfg
        {
            get
            {
                return _cfg;
            }
        }

        public HotResPublishModel()
        {
            LoadConfig();            
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        public void LoadConfig()
        {
            _cfg = EditorConfigUtil.LoadConfig<HotResConfigVO>(CONFIG_NAME);
            if (null == _cfg)
            {
                _cfg = new HotResConfigVO();
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig()
        {
            EditorConfigUtil.SaveConfig(_cfg, CONFIG_NAME);
        }

        /// <summary>
        /// 构建热更AssetBundle资源
        /// </summary>
        public void BuildAssetBundle()
        {
            //标记目标目录
            new AssetBundleBuildCommand(_cfg.resDir, _cfg.abHotResDir, _cfg.isKeepManifest).Execute();
        }


        /// <summary>
        /// 构建热更DLL文件
        /// </summary>
        public void BuildDll()
        {
            if (Copy2DllProj())
            {
                var cmd = new DllBuildCommand(_cfg.resDir, _cfg.devenvPath, _cfg.ilProjCsprojPath);
                cmd.Execute();
            }
        }

        /// <summary>
        /// 构建res.json文件
        /// </summary>
        public void BuildResJsonFile()
        {
            new ResJsonBuildCommand(_cfg.resDir).Execute();
        }

        /// <summary>
        /// 拷贝代码到Proj项目
        /// </summary>
        public bool Copy2DllProj()
        {
            string projCodeDir = Path.Combine(_cfg.ilProjDir, "codes");

            if (Directory.Exists(_cfg.ilScriptDir))
            {
                if (Directory.Exists(projCodeDir))
                {
                    Directory.Delete(projCodeDir, true);
                }
                FileUtil.CopyFileOrDirectory(_cfg.ilScriptDir, projCodeDir);
                Jing.FileSystem.DeleteFilesByExt(projCodeDir, "meta");

                AssetDatabase.Refresh();
                return true;
            }
            return false;
        }
    }
}