using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zero;

namespace ILDemo
{
    [ZeroHotConfig("Test","tests/test.json"), HideLabel]
    public class TestConfigVO 
    {
        [LabelText("字符串")]
        public string s = "string";

        [LabelText("浮点数")]
        public float f = 0.1f;

        [LabelText("字典")]
        public Dictionary<string, double> dic = new Dictionary<string, double>();

        [LabelText("二维数组")]
        [TableList]
        public string[,] array = new string[10,10];

        [LabelText("纹理")]
        public Texture t;
    }
}