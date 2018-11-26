using UnityEngine;
using ZeroIL.Zero;

namespace ZeroIL.RouShan
{
    class GameStage : AView
    {
        Transform _blocks;
        GameObject _blockPrefab;

        Boss _boss;              

        protected override void OnInit()
        {
            _blocks = GetChild("Blocks");
            _blockPrefab = (GameObject)GetBindingObject("blockPrefab")[0];            
            _boss = CreateViewChlid<Boss>("Boss");

            UIPanelMgr.Ins.Switch<GamePanel>(this);
        }

        public void CreateBlock()
        {            
            //以同步方式创建Block
            AView.Create<Block>(_blockPrefab, this, _blocks);

            //以异步方式创建Block
            //AView.CreateAsync<Block>("Block", "hot_res/prefabs/stages/gamestage.ab", this, _blocks, null, OnCreated);

        }
    }
}
