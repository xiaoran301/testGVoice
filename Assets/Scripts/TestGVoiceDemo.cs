using System;
using System.Collections;
using System.Collections.Generic;
using gcloud_voice;
using TMPro;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;


public enum LineState
{
    OffLine = 0,
    Processing = 1,
    OnLine = 2,
}

public class TestGVoiceDemo : MonoBehaviour
{
    private static TestGVoiceDemo _Instance;
    private const string AppId = "285769993";
    private const string AppKey = "234a9f772ae631f1bc3635cdb29e9db2";

    private IGCloudVoice _voiceEngine;
    private string _openId;
    private bool _working = false;
    private string _curRoomName = "HelloVoiceRoom";

    public static TestGVoiceDemo Instance => _Instance;

    public delegate void OutputConsole(string str);

    public event OutputConsole OutputConsoleImplemnt;
    public LineState RoomState { set; get; }
    public LineState MicState { set; get; }
    public LineState SpeakerState { set; get; }

    public string CurRoomName => _curRoomName;

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_voiceEngine == null)
        {
            PrintDeviceInfo();
            _openId = SystemInfo.deviceUniqueIdentifier;
            _voiceEngine = IGCloudVoice.GCloudVoice.GetEngine();
            _voiceEngine.SetAppInfo(AppId, AppKey, _openId);
            var ret = _voiceEngine.Init();
            if (ret != (int) GCloudVoiceErr.GCLOUD_VOICE_SUCC)
            {
                Debug.LogErrorFormat("GVoice init failed,error({0}).", ret);
                return;
            }
            else
            {
                Debug.Log("GVoid init success.");
                PrintLog("GVoice init sucess");
                _working = true;
            }

            _voiceEngine.SetMode(GCloudVoiceMode.RealTime);
            PrintLog("set RealTime mode");

            // 注册回调
            _voiceEngine.OnJoinRoomComplete += OnJoinRoom;
            _voiceEngine.OnQuitRoomComplete += OnQuitRoom;

            // 启动poll携程
            StartCoroutine(UpdateVoicePoll());

            // 进入房间
            JoinRoom(CurRoomName);
        }
    }

    private void OnDestroy()
    {
        _voiceEngine = null;
    }

    void Update()
    {
    }

    IEnumerator UpdateVoicePoll()
    {
        if (!_working)
        {
            yield return null;
        }

        while (_working)
        {
            var ret = _voiceEngine.Poll();
            if (ret != (int) GCloudVoiceErr.GCLOUD_VOICE_SUCC)
            {
                Debug.LogErrorFormat("voice engine poll failed, error({0})", ret);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnJoinRoom(GCloudVoiceCompleteCode code, string roomName, int memberID)
    {
        if (code != GCloudVoiceCompleteCode.GV_ON_OK)
        {
            RoomState = LineState.OffLine;
            PrintLog(String.Format("Join room callback failed({0})", code));
            return;
        }

        RoomState = LineState.OnLine;

        PrintLog(String.Format("Join room callback sucess,room:{0}, memberID:{1}", roomName, memberID));
        OpenSpeaker();
    }

    void OnQuitRoom(GCloudVoiceCompleteCode code, string roomName, int memberID)
    {
        if (code != GCloudVoiceCompleteCode.GV_ON_OK)
        {
            PrintLog(String.Format("Quit room callback failed({0})", code));
            return;
        }

        RoomState = LineState.OffLine;
        PrintLog(String.Format("Quit room callback sucess,room:{0}, memberID:{1}", roomName, memberID));
    }

    void PrintDeviceInfo()
    {
        Debug.LogFormat("deviceId:{0},deviceName:{1},deviceModel:{2},", SystemInfo.deviceUniqueIdentifier,
            SystemInfo.deviceName, SystemInfo.deviceModel);
    }

    public void PrintLog(string str)
    {
        OutputConsoleImplemnt?.Invoke(str);
    }

    public void JoinRoom(string roomName)
    {
        if (RoomState != LineState.OffLine)
        {
            PrintLog("非Off，不能进入房间");
            return;
        }

        RoomState = LineState.Processing;
        _curRoomName = roomName;
        var ret = _voiceEngine.JoinTeamRoom(CurRoomName, 50000);
        var logStr = (ret == (int) GCloudVoiceErr.GCLOUD_VOICE_SUCC) ? "Sucess" : "Failed";
        PrintLog(String.Format("加入房间调用{0}({1}).", logStr, ret));
    }


    public void QuitRoom()
    {
        if (RoomState != LineState.OnLine)
        {
            PrintLog("非On，不能退出房间");
            return;
        }

        RoomState = LineState.Processing;
        var ret = _voiceEngine.QuitRoom(CurRoomName, 50000);
        var logStr = (ret == (int) GCloudVoiceErr.GCLOUD_VOICE_SUCC) ? "Sucess" : "Failed";
        PrintLog(String.Format("退出房间调用{0}({1}).", logStr, ret));
    }

    public void OpenMic()
    {
        if (MicState != LineState.OffLine)
        {
            PrintLog("非Off，不能打开mic");
            return;
        }

        MicState = LineState.Processing;
        var ret = _voiceEngine.OpenMic();
        var logStr = (ret == (int) GCloudVoiceErr.GCLOUD_VOICE_SUCC) ? "Sucess" : "Failed";
        PrintLog(String.Format("OpenMic调用{0}({1}).", logStr, ret));
    }

    public void CloseMic()
    {
        if (MicState != LineState.OnLine)
        {
            PrintLog("非On，不能关闭mic");
            return;
        }

        MicState = LineState.Processing;
        var ret = _voiceEngine.CloseMic();
        var logStr = (ret == (int) GCloudVoiceErr.GCLOUD_VOICE_SUCC) ? "Sucess" : "Failed";
        PrintLog(String.Format("CloseMic调用{0}({1}).", logStr, ret));
    }

    public void OpenSpeaker()
    {
        if (SpeakerState != LineState.OffLine)
        {
            PrintLog("非Off，不能打开Speaker");
            return;
        }

        SpeakerState = LineState.Processing;
        var ret = _voiceEngine.OpenSpeaker();
        var logStr = (ret == (int) GCloudVoiceErr.GCLOUD_VOICE_SUCC) ? "Sucess" : "Failed";
        PrintLog(String.Format("OpenSpeaker调用{0}({1}).", logStr, ret));
    }

    public void CloseSpeaker()
    {
        if (MicState != LineState.OnLine)
        {
            PrintLog("非On，不能关闭Speaker");
            return;
        }

        SpeakerState = LineState.Processing;
        var ret = _voiceEngine.CloseSpeaker();
        var logStr = (ret == (int) GCloudVoiceErr.GCLOUD_VOICE_SUCC) ? "Sucess" : "Failed";
        PrintLog(String.Format("CloseSpeaker调用{0}({1}).", logStr, ret));
    }
}