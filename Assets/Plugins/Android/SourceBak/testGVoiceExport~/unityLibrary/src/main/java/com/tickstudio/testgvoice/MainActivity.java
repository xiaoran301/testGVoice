

package com.tickstudio.testgvoice;

import android.os.Bundle;
import android.util.Log;

import com.gcloudsdk.gcloud.voice.GCloudVoiceEngine;

public class MainActivity extends UnityPlayerActivity{
    private static final String TAG = "MainActivity";



    protected void onCreate(Bundle savedInstanceState){
        super.onCreate(savedInstanceState);
        Log.v(TAG,"GCloudVoiceEngineBefore...");
        GCloudVoiceEngine.getInstance().init(getApplicationContext(),this);
        Log.v(TAG,"GCloudVoiceEngineAfter...");
    }


}