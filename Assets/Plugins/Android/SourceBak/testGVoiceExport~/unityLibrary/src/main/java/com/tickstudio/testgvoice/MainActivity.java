

package com.tickstudio.testgvoice;

import android.os.Bundle;
import com.gcloudsdk.gcloud.voice.GCloudVoiceEngine;

public class MainActivity extends UnityPlayerActivity{
    protected void onCreate(Bundle savedInstanceState){
        super.onCreate(savedInstanceState);
        GCloudVoiceEngine.getInstance().init(getApplicationContext(),this);
    }
}