package pieces.jing.zero;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;

import java.io.File;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

public class ZeroLib {
    /**
     * unity项目启动时的的上下文
     */
    public static Activity unityActivity;

    /**
     * 获取unity项目的上下文
     * @return
     */
    public static Activity getActivity(){
        if(null == unityActivity) {
            try {
                Class<?> classtype = Class.forName("com.unity3d.player.UnityPlayer");
                Activity activity = (Activity) classtype.getDeclaredField("currentActivity").get(classtype);
                unityActivity = activity;
            } catch (ClassNotFoundException e) {

            } catch (IllegalAccessException e) {

            } catch (NoSuchFieldException e) {

            }
        }
        return unityActivity;
    }

    /**
     * 调用Unity的方法
     * @param gameObjectName    调用的GameObject的名称
     * @param functionName      方法名
     * @param args              参数
     * @return                  调用是否成功
     */
    public static boolean callUnity(String gameObjectName, String functionName, String args){
        try {
            Class<?> classtype = Class.forName("com.unity3d.player.UnityPlayer");
            Method method =classtype.getMethod("UnitySendMessage", String.class,String.class,String.class);
            method.invoke(classtype,gameObjectName,functionName,args);
            return true;
        } catch (ClassNotFoundException e) {

        } catch (NoSuchMethodException e) {

        } catch (IllegalAccessException e) {

        } catch (InvocationTargetException e) {

        }
        return false;
    }

    /**
     * 安装APK
     * 调用该方法要求Target API Level不超过 Android 6.0 (API Level 23)
     * @param filePath APK文件路径
     * @return
     */
    public boolean install(final String filePath){
        boolean success = true;
        try {
            Uri uri = Uri.fromFile(new File(filePath));
            Intent intent = new Intent();
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            intent.setAction(Intent.ACTION_VIEW);
            intent.setDataAndType(uri, "application/vnd.android.package-archive");
            getActivity().startActivity(intent);
        }catch(Exception e){
            e.printStackTrace();
            success = false;
        }
        return success;
    }
}
