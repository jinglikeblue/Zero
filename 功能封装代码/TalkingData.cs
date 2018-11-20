using System.Collections.Generic;
using UnityEngine;

public class TalkingData  {

    /// <summary>
    /// 单例模式
    /// </summary>
    public static TalkingData ins = new TalkingData();

    private TalkingData()
    {
        //TalkingData.ins.Init("AB294CEC7CF847E490E6F183D9E8F82A");
        //TalkingData.ins.SetAccount("test");
        //TalkingData.ins.OnChargeStart("or", "buy", 1, "aaa", 2, "we");
        //TalkingData.ins.OnChargeSuccess("or");
        //TalkingData.ins.OnPurchase("ooo", 3, 1.3f);
        //TalkingData.ins.OnReward(100, "test");
        //TalkingData.ins.OnUse("ooo", 1);
        //TalkingData.ins.OnMissionBegin("1");
        //TalkingData.ins.OnMissionCompleted("1");
        //TalkingData.ins.OnEvent("action", "what", "fuck");
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="appId"></param>
    public void Init(string appId, string channelId)
    {
        TalkingDataGA.OnStart(appId, channelId);
    }    

    /// <summary>
    /// 设置账号
    /// </summary>
    /// <param name="account"></param>
    public void SetAccount(string account)
    {
        var a = TDGAAccount.SetAccount(account);        
    }

    /// <summary>
    /// 支付开始
    /// </summary>
    /// <param name="orderId">订单ID</param>
    /// <param name="iapId">充值物品ID</param>
    /// <param name="amount">现金金额或现金等价物的额度</param>
    /// <param name="type">货币单位 例：人民币 CNY；美元 USD；欧元 EUR；</param>
    /// <param name="vAmount">虚拟币金额</param>
    /// <param name="paymentType">支付的途径，最多16个字符。例如：“支付宝”、“苹果官方”、“XX 支付SDK</param>
    public void OnChargeStart(string orderId, string iapId, double amount, string type, double vAmount, string paymentType)
    {
        TDGAVirtualCurrency.OnChargeRequest(orderId, iapId, amount, type, vAmount, paymentType);
    }

    /// <summary>
    /// 支付成功
    /// </summary>
    /// <param name="orderId"></param>
    public void OnChargeSuccess(string orderId)
    {
        TDGAVirtualCurrency.OnChargeSuccess(orderId);
    }

    /// <summary>
    /// 获得虚拟币记录
    /// </summary>
    /// <param name="virtualCurrency"></param>
    /// <param name="reason"></param>
    public void OnReward(double virtualCurrency, string reason = "")
    {
        TDGAVirtualCurrency.OnReward(virtualCurrency, reason);
    }

    /// <summary>
    /// 虚拟币消费
    /// </summary>
    /// <param name="item">购买物品</param>
    /// <param name="itemAmount">数量</param>
    /// <param name="virtualCurrency">消费虚拟币</param>
    public void OnPurchase(string item, int itemAmount, double virtualCurrency)
    {
        TDGAItem.OnPurchase(item, itemAmount, virtualCurrency);
    }

    /// <summary>
    /// 物品使用
    /// </summary>
    /// <param name="item"></param>
    /// <param name="itemNumber"></param>
    public void OnUse(string item, int itemNumber)
    {
        TDGAItem.OnUse(item, itemNumber);
    }

    /// <summary>
    /// 任务开始
    /// </summary>
    /// <param name="missionId"></param>
    public void OnMissionBegin(string missionId)
    {
        TDGAMission.OnBegin(missionId);
    }

    /// <summary>
    /// 任务完成
    /// </summary>
    /// <param name="missionId"></param>
    public void OnMissionCompleted(string missionId)
    {
        TDGAMission.OnCompleted(missionId);
    }

    /// <summary>
    /// 任务失败
    /// </summary>
    /// <param name="missionId"></param>
    /// <param name="cause"></param>
    public void OnMissionFailed(string missionId, string cause = "")
    {
        TDGAMission.OnFailed(missionId, cause);
    }

    /// <summary>
    /// 事件
    /// </summary>
    /// <param name="actionId"></param>
    /// <param name="parameters"></param>
    public void OnEvent(string actionId, Dictionary<string, object> parameters)
    {
        TalkingDataGA.OnEvent(actionId, parameters);
    }

    /// <summary>
    /// 事件
    /// </summary>
    /// <param name="actionId"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void OnEvent(string actionId, string key, string value)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add(key, value); 
        TalkingDataGA.OnEvent(actionId, dic);
    }
}
