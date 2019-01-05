using UnityEngine;
using Zero;
using IL.Zero;

namespace IL.Demo
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
            var ac = ResMgr.Ins.Load<AudioClip>("hot_res/audios.ab", "click");
            AudioPlayer.Ins.PlayEffect(ac);
            //以同步方式创建Block
            ViewFactory.Create<Block>(_blockPrefab, _blocks);            
            
            //以异步方式创建Block
            //AView.CreateAsync<Block>("Block", "hot_res/prefabs/stages/gamestage.ab", this, _blocks, null, OnCreated);

        }
    }
}
