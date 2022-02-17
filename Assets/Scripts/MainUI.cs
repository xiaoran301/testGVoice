using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            _btnName.text = (TestGVoiceDemo.Instance.RoomState == LineState.OffLine) ? "离开房间" : "进入房间";
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

    public void OnToggleMic(bool val)
    {
        if (val)
        {
            TestGVoiceDemo.Instance.OpenMic();
        }
        else
        {
            TestGVoiceDemo.Instance.CloseMic();
        }
    }

    public void OnToggleSpeaker(bool val)
    {
        if (val)
        {
            TestGVoiceDemo.Instance.OpenSpeaker();
        }
        else
        {
            TestGVoiceDemo.Instance.CloseSpeaker();
        }
    }
}