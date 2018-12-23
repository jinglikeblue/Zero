using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zero;
using IL.Zero;

namespace IL.Demo
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
            UIWinMgr.Ins.Open<HelpWin>(null,false,false);
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
