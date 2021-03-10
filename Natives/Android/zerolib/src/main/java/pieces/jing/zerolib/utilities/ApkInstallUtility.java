package pieces.jing.zerolib.utilities;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.util.Log;

import androidx.core.content.FileProvider;

import java.io.File;

import pieces.jing.zerolib.UnityBridge;

/**
 * APK安装工具
 */
public class ApkInstallUtility {
    public static boolean install(final String filePath) {
        boolean success = true;

        Activity activity = UnityBridge.getActivity();
        String pkgName = activity.getPackageName();
        Log.i("ZeroLib", "获取到的包名：" + pkgName);

        File file = new File(filePath);
        file = new File(file.getAbsolutePath());

        try {
            Intent intent = new Intent();
            Uri uri;
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                uri = FileProvider.getUriForFile(activity, pkgName + ".fileProvider", file);
            } else {
                uri = Uri.fromFile(file);
            }

            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
            intent.setDataAndType(uri, "application/vnd.android.package-archive");
            activity.startActivity(intent);
        } catch (Exception e) {
            e.printStackTrace();
            success = false;
        }
        return success;
    }
}
