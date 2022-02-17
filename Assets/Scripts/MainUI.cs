using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private InputField _txtInputRoomName;

    [SerializeField] private Text _btnName;

    [SerializeField] private Text _outPut;
    [SerializeField] private Toggle _toggleMic;
    [SerializeField] private Toggle _toggleSpeaker;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        _outPut.text = "";
        TestGVoiceDemo.Instance.OutputConsoleImplemnt += AppendOut;
    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }

    void Refresh()
    {
        // 按钮
        if (TestGVoiceDemo.Instance.RoomState == LineState.Processing)
        {
            _btnName.text = "处理中...";
        }
        else
        {
            _btnName.text = (TestGVoiceDemo.Instance.RoomState == LineState.OffLine) ? "进入房间" : "离开房间";
        }

        // _toggleMic.isOn = (TestGVoiceDemo.Instance.MicState == LineState.OnLine);
        // _toggleSpeaker.isOn = (TestGVoiceDemo.Instance.SpeakerState == LineState.OnLine);
    }

    private void OnDestroy()
    {
        TestGVoiceDemo.Instance.OutputConsoleImplemnt -= AppendOut;
    }

    private void AppendOut(string str)
    {
        var outStr = str + "\n";
        _outPut.text += outStr;
    }

    public void OnClickEnterRoom()
    {
        if (TestGVoiceDemo.Instance.RoomState == LineState.OffLine)
        {
            TestGVoiceDemo.Instance.JoinRoom(_txtInputRoomName.text);
        }
        else if (TestGVoiceDemo.Instance.RoomState == LineState.OnLine)
        {
            TestGVoiceDemo.Instance.QuitRoom();
        }
    }
    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        AppendOut($"{permissionName} PermissionDeniedAndDontAskAgain");
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        AppendOut($"{permissionName} PermissionCallbacks_PermissionGranted");
    }

    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        AppendOut($"{permissionName} PermissionCallbacks_PermissionDenied");
    }

    public void OnToggleMic(bool val)
    {
        if (!val)
        {
#if UNITY_ANDROID
            
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
            callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
            callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
            Permission.RequestUserPermission(Permission.Microphone, callbacks);
            
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                AppendOut("没有mic权限，请求权限");
                Permission.RequestUserPermission(Permission.Microphone);
                _toggleMic.isOn = false;
                return;
            }

            AppendOut("已拥有mic权限");
#endif
            if (TestGVoiceDemo.Instance.OpenMic())
            {
                TestGVoiceDemo.Instance.MicState = LineState.OnLine;
            }
            else
            {
                TestGVoiceDemo.Instance.MicState = LineState.OffLine;
            }
        }
        else
        {
            TestGVoiceDemo.Instance.CloseMic();
        }
    }


    public void OnToggleSpeaker(bool val)
    {
        if (!val)
        {
            TestGVoiceDemo.Instance.OpenSpeaker();
        }
        else
        {
            TestGVoiceDemo.Instance.CloseSpeaker();
        }
    }

    public void OnCfgDropDownList(int idx)
    {
        var AppId = "285769993";
        var AppKey = "234a9f772ae631f1bc3635cdb29e9db2";
        if (idx == 0)
        {
            AppId = "gcloud.chiji";
            AppKey = "123";
        }

        TestGVoiceDemo.Instance.AppId = AppId;
        TestGVoiceDemo.Instance.AppKey = AppKey;
    }

    public void OnClickClearLog()
    {
        _outPut.text = "";
    }
}