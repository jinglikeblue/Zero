using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zero;
using ILZero;

namespace ILDemo
{
    class GamePanel : AView
    {
        GameStage stage;
        protected override void OnData(object data)
        {
            stage = data as GameStage;
        }

        protected override void OnDisable()
        {
            UIEventListener.Get(GetChild("TouchPad").gameObject).onClick -= OnTouchPadClick;
            GetChild("BtnReset").GetComponent<Button>().onClick.RemoveListener(OnResetClick);
            GetChild("BtnHelp").GetComponent<Button>().onClick.RemoveListener(OnBtnHelpClick);
        }

        private void OnBtnHelpClick()
        {
            AudioDevice.GC();
            UIWinMgr.Ins.Open<HelpWin>(null,true,false);
        }

        protected override void OnEnable()
        {
            UIEventListener.Get(GetChild("TouchPad").gameObject).onClick += OnTouchPadClick;
            GetChild("BtnReset").GetComponent<Button>().onClick.AddListener(OnResetClick);
            GetChild("BtnHelp").GetComponent<Button>().onClick.AddListener(OnBtnHelpClick);
        }

        void OnTouchPadClick(PointerEventData e)
        {
            stage.CreateBlock();
        }

        void OnResetClick()
        {                     
            StageMgr.Ins.Switch<GameStage>();
        }
    }
}
