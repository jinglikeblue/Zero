using Jing;
using System.IO;
using UnityEditor;

namespace Zero.Edit
{
    public class HotResPublishModel
    {
        const string CONFIG_NAME = "HotResCfg.json";

        const string HOT_RES_BACKUP_ROOT = "HotResBackup";
        
        HotResConfigVO _cfg;

        public HotResConfigVO Cfg
        {
            get
            {
                return _cfg;
            }
        }

        /// <summary>
        /// DLL代码在Asset中的位置
        /// </summary>
        public string DllDirInAssets { get; }

        /// <summary>
        /// DLL代码在备份目录的位置
        /// </summary>
        public string DllDirInBackup { get; }

        public HotResPublishModel()
        {
            LoadConfig();
            DllDirInAssets = _cfg.ilScriptDir;
            DllDirInBackup = FileSystem.CombineDirs(false, HOT_RES_BACKUP_ROOT, _cfg.ilScriptDir);
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

        /// <summary>
        /// 排除DLL代码
        /// </summary>
        public void ExcludeDllCodes()
        {
            if (Directory.Exists(DllDirInAssets))
            {
                if (false == Directory.Exists(DllDirInBackup))
                {
                    Directory.CreateDirectory(DllDirInBackup);
                }
                FileUtil.ReplaceDirectory(DllDirInAssets, DllDirInBackup);
                FileUtil.DeleteFileOrDirectory(DllDirInAssets);
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 导入DLL代码
        /// </summary>
        public void IncludeDllCodes()
        {
            //代码有备份，且Asset中没代码才还原
            if (Directory.Exists(DllDirInBackup) && false == Directory.Exists(DllDirInAssets))
            {
                if (false == Directory.Exists(DllDirInAssets))
                {
                    Directory.CreateDirectory(DllDirInAssets);
                }
                FileUtil.ReplaceDirectory(DllDirInBackup, DllDirInAssets);
                FileUtil.DeleteFileOrDirectory(DllDirInBackup);
                AssetDatabase.Refresh();
            }
        }
    }
}