using System.Collections.Generic;
using UnityEngine;
using Zero;
using ZeroIL.Zero;

namespace ZeroIL.RouShan
{
    class GameStage : AView
    {
        Transform _blocks;
        GameObject _blockPrefab;

        Boss _boss;

        List<GameObject> _blockObjList = new List<GameObject>();              

        protected override void OnInit()
        {
            _blocks = GetChild("Blocks");
            _blockPrefab = (GameObject)GetBindingObject("blockPrefab")[0];
            //_blockPrefab = (GameObject)gameObject.GetComponent<ObjectBindingData>().Find("blockPrefab").Value.list[0];
            _boss = CreateViewChlid<Boss>("Boss");

            UIPanelMgr.Ins.Switch<GamePanel>(this);
        }

        public void CreateBlock()
        {
            GameObject block = GameObject.Instantiate(_blockPrefab,_blocks);
            _blockObjList.Add(block);            
        }
    }
}
