using Sirenix.OdinInspector;
using System.Collections.Generic;
using Zero;

namespace ILDemo
{       
    [ZeroHotConfig("Tests/Test","tests/test.json"), HideLabel]
    public class TestConfigVO 
    {
        [Title("字符串")]        
        public string s = "string";

        [Title("浮点数")]        
        public float f = 0.1f;

        [Title("字典")]
        public Dictionary<string, double> dic = new Dictionary<string, double>();

        [Title("数组")]        
        public int[] array = new int[5];
    }
}