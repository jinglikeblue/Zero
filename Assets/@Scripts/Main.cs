namespace ZeroHot
{
    public class Main
    {
        /// <summary>
        /// 入口方法
        /// </summary>
        public static void Startup()
        {
            ViewAutoRegister.Register();

            new ILDemo.DemoMain();
        }
    }
}
