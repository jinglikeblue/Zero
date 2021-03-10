package pieces.jing.zerolib.file;

import android.app.Activity;
import android.util.Log;

import java.io.File;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.io.OutputStream;

import pieces.jing.zerolib.UnityBridge;

/**
 * Assets目录下的文件，复制工具
 */
public final class AssetFileCopy {

    boolean _isDone = false;

    String _error = null;

    /**
     * 是否完成
     *
     * @return
     */
    public boolean isDone() {
        return _isDone;
    }

    /**
     * 错误信息，没有则为null
     *
     * @return
     */
    public String error() {
        return _error;
    }

    boolean _isWorking = false;

    /**
     * 测试复制package.zip文件
     */
    public boolean copyAssetsFile(final String fileName, final String targetPath) {
        if (_isWorking) {
            return false;
        }
        _isWorking = true;
        Thread thread = new Thread() {
            @Override
            public void run() {
                try {
                    Activity activity = UnityBridge.getActivity();
                    InputStream fs = activity.getResources().getAssets().open(fileName);
                    File targetFile = new File(targetPath);
                    if (targetFile.exists()) {
                        targetFile.delete();
                    } else if (!targetFile.getParentFile().exists()) {
                        targetFile.getParentFile().mkdirs();
                    }

                    OutputStream os = new FileOutputStream(targetFile);

                    Log.i("copyAssetsFile", "开始拷贝[" + fileName + "]到：" + targetFile.toString());

                    byte[] buffer = new byte[4096];
                    int c;
                    while ((c = fs.read(buffer)) > 0) {
                        os.write(buffer, 0, c);
//                        Thread.sleep(1);
                    }
                    fs.close();
                    os.close();

                    Log.i("copyAssetsFile", "文件拷贝完成：" + targetFile.toString());
                } catch (Exception e) {
                    e.printStackTrace();
                    //文件不存在或处理失败
                    Log.i("copyAssetsFile", "文件拷贝失败：" + fileName);
                    _error = e.getMessage();
                } finally {
                    _isDone = true;
                    _isWorking = false;
                }
            }
        };
        thread.start();
        return true;
    }
}
