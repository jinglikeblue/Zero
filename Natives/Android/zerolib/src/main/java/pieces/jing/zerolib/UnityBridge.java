package pieces.jing.zerolib;

import android.app.Activity;
import android.util.Log;
import android.widget.Toast;

import org.json.JSONException;
import org.json.JSONObject;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

/**
 * Created by Jing on 2018-1-18.
 */
public class UnityBridge {

    /**
     * unity项目启动时的的上下文
     */
    private static Activity _unityActivity;

    /**
     * 设置一个Activity
     *
     * @param activity
     */
    public static void setActivity(Activity activity) {
        _unityActivity = activity;
    }


    /**
     * 获取unity项目的上下文
     *
     * @return
     */
    public static Activity getActivity() {
        if (null == _unityActivity) {
            try {
                Class<?> classtype = Class.forName("com.unity3d.player.UnityPlayer");
                Activity activity = (Activity) classtype.getDeclaredField("currentActivity").get(classtype);
                _unityActivity = activity;
            } catch (ClassNotFoundException e) {

            } catch (IllegalAccessException e) {

            } catch (NoSuchFieldException e) {

            }
        }
        return _unityActivity;
    }

    /**
     * 调用Unity的方法
     *
     * @param gameObjectName 调用的GameObject的名称
     * @param functionName   方法名
     * @param args           参数
     * @return 调用是否成功
     */
    public static boolean callUnity(String gameObjectName, String functionName, String args) {
        try {
            Class<?> classtype = Class.forName("com.unity3d.player.UnityPlayer");
            Method method = classtype.getMethod("UnitySendMessage", String.class, String.class, String.class);
            method.invoke(classtype, gameObjectName, functionName, args);
            return true;
        } catch (ClassNotFoundException e) {

        } catch (NoSuchMethodException e) {

        } catch (IllegalAccessException e) {

        } catch (InvocationTargetException e) {

        }
        return false;
    }

    /**
     * 呼叫unity的Android桥接器
     *
     * @param msgId   消息ID
     * @param jsonObj
     * @return
     */
    public static boolean callNativeBridge(String msgId, JSONObject jsonObj) {
        if (null == jsonObj) {
            jsonObj = new JSONObject();
        }
        try {
            jsonObj.put("MsgId", msgId);
        } catch (JSONException e) {
            e.printStackTrace();
            return false;
        }
        String json = jsonObj.toString();
        Log.i("请求NativeBridge", "OnMessage: " + json);
        return callUnity("NativeBridge", "OnMessage", json);
    }


    /**
     * Toast显示unity发送过来的内容
     *
     * @param content 消息的内容
     * @return 调用是否成功
     */
    public static boolean showToast(String content) {
        Toast.makeText(getActivity(), content, Toast.LENGTH_SHORT).show();

        //回调Unity测试
        callNativeBridge("showToastResp", null);
        return true;
    }
}