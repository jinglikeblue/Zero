using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zero;
using ZeroIL.Zero;

namespace ZeroIL.RouShan
{
    class MenuPanel : AView
    {
        GameObject btnTap;
        Button btnHelp;


        protected override void OnDisable()
        {
            UIEventListener.Get(btnTap).onClick -= OnClickGo;
            btnHelp.onClick.RemoveListener(OpenHelpWin);
        }

        protected override void OnEnable()
        {
            UIEventListener.Get(btnTap).onClick += OnClickGo;
            btnHelp.onClick.AddListener(OpenHelpWin);
        }

        protected override void OnInit()
        {
            btnTap = GetChild("BtnTap").gameObject;
            btnHelp = GetChildComponent<Button>("BtnHelp");
        }

        public void OnClickGo(PointerEventData data)
        {
            Go();
        }

        void Go()
        {
            UIPanelMgr.Ins.ClearNowPanel();
            StageMgr.Ins.Switch<GameStage>();            
        }

        void OpenHelpWin()
        {
            UIWinMgr.Ins.Open<HelpWin>();
        }
    }
}
