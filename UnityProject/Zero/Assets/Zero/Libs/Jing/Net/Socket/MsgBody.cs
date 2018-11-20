using System.Reflection;

namespace Jing
{
    /// <summary>
    /// (未完成)
    /// </summary>
    public class MsgBody
    {
        public static byte[] Struct2Bytes(object obj)
        {
            ByteArray ba = new ByteArray();

            LoopObj(obj, ba);

            return ba.ToBytes();
        }

        public static void LoopObj(object obj, ByteArray ba)
        {            
            foreach (FieldInfo field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                object fieldValue = field.GetValue(obj);
                if(field.FieldType == typeof(int))
                {
                    ba.WriteInt((int)fieldValue);
                }
                else if(field.FieldType == typeof(string))
                {
                    ba.WriteString((string)fieldValue);
                }
                else
                {
                    LoopObj(fieldValue, ba);
                }                
            }
        }
    }

    public struct LoginMsg
    {
        public int userId;
        public string userName;
        public Detail2 cls;
        public Detail detail;
    }

    public struct Detail
    {
        public int age;
        public int leve;
        public string nickname;
    }

    public class Detail2
    {
        public int a = 1;
        public int b = 2;
        public string h = "hello";
    }

}
