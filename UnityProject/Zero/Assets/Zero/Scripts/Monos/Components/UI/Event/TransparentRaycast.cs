using UnityEngine.UI;

namespace Zero
{
    /// <summary>
    /// 该组件是一个透明的可以响应UI事件的区域，效率比Image设置Alpha为0高
    /// </summary>
    public class TransparentRaycast : MaskableGraphic
    {
        protected TransparentRaycast()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}