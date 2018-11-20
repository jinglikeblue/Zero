using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 2D纹理工具
    /// </summary>
    public class Texture2DUtil
    {
        public static Sprite ToSprite(Texture2D t)
        {
            Sprite sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
            return sprite;
        }        
    }
}