package pieces.jing.zeronative;

import android.os.Bundle;
import android.view.View;

import androidx.appcompat.app.AppCompatActivity;

import pieces.jing.zerolib.UnityBridge;
import pieces.jing.zerolib.file.AssetFileCopy;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        UnityBridge.setActivity(this);
        findViewById(R.id.btnCopy).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                AssetFileCopy afc = new AssetFileCopy();
                afc.copyAssetsFile("package.zip", getExternalFilesDir("package.zip").toString());
            }
        });
    }


}
