using Jing;
using System.IO;

namespace Zero.Edit
{
    class ResJsonBuildCommand
    {
        string _resDir;
        string _manifestName;

        public ResJsonBuildCommand(string resDir, string manifestName)
        {
            _resDir = resDir;
            _manifestName = manifestName;
        }

        public void Execute()
        {
            var resDir = FileSystem.CombineDirs(false, _resDir, ZeroEditorUtil.PlatformDirName);
            if (false == Directory.Exists(resDir))
            {
                Directory.CreateDirectory(resDir);
            }

            var filePath = FileSystem.CombinePaths(resDir, "res.json");

            //首先删除旧的
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            ResVerVO res = new ResVerVOBuilder(resDir).Build();
            res.manifestName = _manifestName;
            string jsonStr = LitJson.JsonMapper.ToJson(res);
            File.WriteAllText(filePath, jsonStr);
        }
    }
}
