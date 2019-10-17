using UnityEngine;
using Zero;
using ILZero;
using System;
using ILGenerated;

namespace ILDemo
{
    class GameStage : AView
    {
        Transform _blocks;
        GameObject _blockPrefab;

        Boss _boss;              

        protected override void OnInit()
        {
            _blocks = GetChild("Blocks");
            _blockPrefab = ObjectBindingData.Find(gameObject, "blockPrefab")[0] as GameObject;            
            _boss = CreateViewChild<Boss>("Boss");

            UIPanelMgr.Ins.SwitchASync<GamePanel>(this);
        }

        public void CreateBlock()
        {
            var ac = ResMgr.Ins.Load<AudioClip>("audios.ab", "click");
            AudioDevice.Get("effect").Play(gameObject,ac);            
            //以异步方式创建Block
            ViewFactory.CreateAsync<Block>(AssetBundleName.PREFABS_STAGES_GAMESTAGE, "Block", _blocks, null, OnCreatedBlock, OnProgressBlock, OnLoadedBlock);
        }

        private void OnProgressBlock(float progress)
        {
            Log.I("方块资源加载进度:{0}", progress);
        }

        private void OnLoadedBlock(UnityEngine.Object obj)
        {
            Log.I("方块资源加载完成:{0}", obj.ToString());
        }

        private void OnCreatedBlock(Block obj)
        {
            Log.I("方块视图创建完成:{0}", obj.ToString());
        }
    }
}
