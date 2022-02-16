#if UNITY_IOS
#define UNITY_IPHONE
#endif

using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Collections.Generic;

namespace gcloud_voice
{
	public class GCloudVoiceEngine : IGCloudVoice
	{
		#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		public const string LibName = "GCloudVoice";
		#else
		#if UNITY_IPHONE || UNITY_XBOX360
		public const string LibName = "__Internal";
		#else
		public const string LibName = "GCloudVoice";
		#endif
		#endif

		enum NoticeMessageType {
			MSG_ON_JOINROOM_COMPLETE = 1,
			MSG_ON_QUITROOM_COMPLETE = 2,
			MSG_ON_UPLOADFILE_COMPLETE = 3,
			MSG_ON_DOWNFILE_COMPLETE = 4,
			MSG_ON_MEMBER_VOICE = 5,
			MSG_ON_APPLY_AUKEY_COMPLETE = 6,
			MSG_ON_PLAYFILE_COMPLETE = 7,
			MSG_ON_SPEECH_TO_TEXT = 8,
			MSG_ON_ROOM_OFFLINE = 9,
			MSG_ON_STREAM_SPEECH_TO_TEXT = 10,
        	MSG_ON_ROLE_CHANGED = 11,
        	MSG_ON_EVENT_NOTIFY = 12,
			MSG_ON_MUTE_STATE = 13,
        	MSG_ON_REPORT_PLAYER = 14,
        	MSG_ON_CHECK_REPORTED = 15,
        	MSG_ON_UPLOAD_SAVEDATA_COMPLETE = 16,
        	MSG_ON_DOWNLOAD_SAVEDATA_COMPLETE = 17,
			MSG_ON_SYNCHROMEMINFO_NOTIFY = 18,
			MSG_ON_SPEECH_TRANSLATE = 19,
			MSG_ON_RSTS = 20,
			MSG_ON_TTS = 21,
			MSG_ON_TTT = 22,
			MSG_ON_QUERY_WX_MEMBERS = 23,
			MSG_ON_QUERY_USER_INFO = 24,
			MSG_ON_WX_MEMBER_VOICE = 25,
			MSG_ON_UPLOAD_USER_INFO = 26,
			MSG_ON_REPORT_KEY_WORDS = 27,
			MSG_ON_ENABLE_TRANSLATE = 28,
			MSG_ON_DEVICE_NOTIFY = 29,
			MSG_ON_UPDATE_MIC_LEVEL = 30,
			MSG_ON_ENABLE_MAGIC_VOICE = 31,
			MSG_ON_TRANSLATE_TEXT = 32,
			MSG_ON_TTS_STREAM = 33,
            MSG_ON_ENABLE_RECV_MAGIC_VOICE = 34,
            MSG_ON_REPORT_ABROAD = 35,
            MSG_ON_MAGIC_VOICE_MSG = 36,
			MSG_ON_RT_SECINFO = 37,
            MSG_ON_LOG_CHANGED = 38,
			MSG_ON_MIC_SPEECH_TO_SPEECH = 39,
			MSG_ON_MIC_SPEECH_TO_TEXT = 40,
			MSG_ON_TTS_FILE = 41,
			MSG_ON_SPEECH_FILE_TRANSLATE = 42,
			MSG_ON_SPEECH_FILE_TO_TEXT = 43,
			MSG_ON_AI_RECOGNITION = 51,
		};

		public class NoticeMessage
		{
			public int what;
			public int intArg1;
			public int intArg2;
			public string strArg;
			public byte[] custom;
			public int datalen;
			public NoticeMessage()
			{
				what = -1;
				intArg1 = 0;
				intArg2 = 0;
				strArg = "";
				datalen = 0;
				custom = new byte[2048];
			}
			public void clear()
			{
				what = -1;
				intArg1 = 0;
				intArg2 = 0;
				strArg = "";
				datalen = 0;
			}
		}
		
        public override event JoinRoomCompleteHandler            OnJoinRoomComplete;
        public override event MemberVoiceHandler                 OnMemberVoice;
        public override event RoomMemberVoiceHandler             OnRoomMemberVoice;
        public override event ChangeRoleCompleteHandler          OnRoleChangeComplete;
        public override event StatusUpdateHandler                OnStatusUpdate;
        public override event QuitRoomCompleteHandler            OnQuitRoomComplete;
        public override event ApplyMessageKeyCompleteHandler     OnApplyMessageKeyComplete;
        public override event UploadReccordFileCompleteHandler   OnUploadReccordFileComplete;
        public override event DownloadRecordFileCompleteHandler  OnDownloadRecordFileComplete;
        public override event PlayRecordFilCompleteHandler       OnPlayRecordFilComplete;
        public override event SpeechToTextHandler                OnSpeechToText;
        public override event StreamSpeechToTextHandler          OnStreamSpeechToText;
        public override event EventUpdateHandler                 OnEvent;
        public override event MuteSwitchResultHandler            OnMuteSwitchState;
        public override event ReportPlayerHandler 				 OnReportPlayer;
        public override event SaveRecFileIndexHandler 			 OnSaveRecFileIndex;
		public override event RoomMemberChangedCompleteHandler   OnRoomMemberInfo;
		public override event SpeechTranslateHandler 			 OnSpeechTranslate;
		public override event SpeechFileTranslateHandler         OnSpeechFileTranslate;
		public override event SpeechFileToTextHandler            OnSpeechFileToText;
		public override event TextTranslateHandler				 OnTextTranslate;
		public override event TextToSpeechHandler				 OnTextToSpeech;
		public override event TextToSpeechFileHandler            OnTextToSpeechFile;
		public override event TextToStreamSpeechHandler			 OnTextToStreamSpeech;
		public override event RSTSHandler 			 			 OnRSTS;
		public override event RSTSSpeechToSpeechHandler          OnRSTSSpeechToSpeech;
		public override event RSTSSpeechToTextHandler            OnRSTSSpeechToText;
		public override event EnableTranslateHandler 			 OnEnableTranslate;
		public override event RealTimeTranslateTextHandler       OnRealTimeTranslateText;
		public override event QueryUserInfoHandler               OnQueryUserInfo;
		public override event WXMemberVoiceHandler               OnWXMemberVoice;
		public override event QueryWXMembersHandler              OnQueryWXMembers;
		public override event UpdateUserInfoHandler              OnUpdateUserInfo;
		public override event UpdateMicLevelHandler 			 OnUpdateMicLevel;
        public override event EnableMagicVoiceHandler            OnEnableMagicVoice;
        public override event EnableRecvMagicVoiceHandler        OnEnableRecvMagicVoice;
		public override event STTReportHandler 					 OnSTTReport;
		public override event MagicVoiceMsgHandler               OnMagicVoiceMsg;
		public override event RTSecInfoHandler                   OnRTSecInfo;
		public override event AIRecognitionHandler 			 	 OnAIRecognition;
	
		private static bool bInit = false;
		private static bool bIsSetAppInfo = false;
		private static bool bPrintLog = true;
		private int pollBufLen = 2048;
		private byte[] pollBuf;
		private NoticeMessage pollMsg = null;
		private int[] memberVoice = null;
		private byte[] roomNameBuf;
        private int membersBufLen = 3072; //maybe save 22 members with max 128len openid
        private byte[] membersBuf;

        #region DllImport
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_CreateInstance();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetServerInfo([MarshalAs(UnmanagedType.LPArray)] string URL, [MarshalAs(UnmanagedType.LPArray)] string defaultipsvr);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetAppInfo([MarshalAs(UnmanagedType.LPArray)] string appID, [MarshalAs(UnmanagedType.LPArray)]string appKey,[MarshalAs(UnmanagedType.LPArray)] string openID);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_Init();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetMode(int mode);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_Poll(byte[] buf, int length);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_Pause();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_Resume();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_JoinTeamRoom([MarshalAs(UnmanagedType.LPArray)] string roomName, int msTimeout);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_JoinRangeRoom([MarshalAs(UnmanagedType.LPArray)] string roomName, int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_JoinNationalRoom([MarshalAs(UnmanagedType.LPArray)] string roomName, int role, int msTimeout);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_ChangeRole(int role, [MarshalAs(UnmanagedType.LPArray)] string roomName);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_QuitRoom([MarshalAs(UnmanagedType.LPArray)] string roomName, int msTimeout);
		
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_JoinTeamRoom_Scenes([MarshalAs(UnmanagedType.LPArray)] string scenesName, [MarshalAs(UnmanagedType.LPArray)] string roomName, int msTimeout);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_JoinRangeRoom_Scenes([MarshalAs(UnmanagedType.LPArray)] string scenesName, [MarshalAs(UnmanagedType.LPArray)] string roomName, int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_JoinNationalRoom_Scenes([MarshalAs(UnmanagedType.LPArray)] string scenesName, [MarshalAs(UnmanagedType.LPArray)] string roomName, int role, int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_QuitRoom_Scenes([MarshalAs(UnmanagedType.LPArray)] string scenesName, int msTimeout);
		
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_OpenMic();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_CloseMic();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_OpenSpeaker();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_CloseSpeaker();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableRoomMicrophone([MarshalAs(UnmanagedType.LPArray)] string roomName, bool enable);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableRoomSpeaker([MarshalAs(UnmanagedType.LPArray)] string roomName, bool enable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableMultiRoom(bool enable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_UpdateCoordinate([MarshalAs(UnmanagedType.LPArray)] string roomName, long x, long y, long z, long r);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_ApplyMessageKey(int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StartRecording([MarshalAs(UnmanagedType.LPArray)] string filePath, bool bOptimized);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StopRecording();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_UploadRecordedFile([MarshalAs(UnmanagedType.LPArray)] string filePath, int msTimeout, bool bPermanent);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_DownloadRecordedFile([MarshalAs(UnmanagedType.LPArray)] string fileID, [MarshalAs(UnmanagedType.LPArray)] string downloadFilePath, int msTimeout, bool bPermanent);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_PlayRecordedFile([MarshalAs(UnmanagedType.LPArray)] string downloadFilePath);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StopPlayFile();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SpeechToText([MarshalAs(UnmanagedType.LPArray)] string fileID, int language, int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_ForbidMemberVoice(int member, bool bEnable, [MarshalAs(UnmanagedType.LPArray)] string roomName);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableLog(bool enable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetDataFree(bool enable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetMicLevel(bool bFadeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetSpeakerLevel();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetSpeakerVolume(int vol);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_TestMic() ;
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetFileParam([MarshalAs(UnmanagedType.LPArray)] string filepath, int [] bytes, float []seconds);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_invoke( uint nCmd, uint nParam1, uint nParam2, [MarshalAs(UnmanagedType.LPArray)] uint [] pOutput );

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_JoinNationalRoom_Token([MarshalAs(UnmanagedType.LPArray)] string roomName, int role, [MarshalAs(UnmanagedType.LPArray)] string token, int timestamp, int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_JoinTeamRoom_Token( [MarshalAs(UnmanagedType.LPArray)] string roomName,  [MarshalAs(UnmanagedType.LPArray)] string token, int timestamp, int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_ApplyMessageKey_Token([MarshalAs(UnmanagedType.LPArray)] string token, int timestamp, int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SpeechToText_Token([MarshalAs(UnmanagedType.LPArray)] string fileID, [MarshalAs(UnmanagedType.LPArray)] string token, int timestamp, int msTimeout, int language);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableSpeakerOn(bool enable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetMicVol(int vol);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetVoiceEffects(int mode);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_IsSpeaking();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableReverb(bool bEnable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetReverbMode(int mode);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetVoiceIdentify();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetBGMPath([MarshalAs(UnmanagedType.LPArray)] string path);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StartBGMPlay();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StopBGMPlay();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_PauseBGMPlay();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_ResumeBGMPlay();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetBGMPlayState();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetBGMVol(int vol);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableNativeBGMPlay(bool bEnable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetBitRate(int bitrate);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetAudioDeviceConnectionState();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableBluetoothSCO(bool enable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_CheckDeviceMuteState();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetAudience([MarshalAs(UnmanagedType.LPArray)] int [] audience, int count, [MarshalAs(UnmanagedType.LPArray)] string path);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetMicState();
		
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetSpeakerState();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetMuteResult();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_SetReportBufferTime(int nTimeSec);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_SetReportedPlayerInfo([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]string[] cszOpenID, int[] nMemberID, int nCount);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_ReportPlayer([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]string[] cszOpenID, int nCount, [MarshalAs(UnmanagedType.LPArray)] string strInfo);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_EnableCivilVoice(bool bEnable);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StartSaveVoice();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StopSaveVoice();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetRecSaveTs(int ts);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetPlayFileIndex([MarshalAs(UnmanagedType.LPArray)] string fileid, int fileindex);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StartPlaySaveVoiceTs(int ts);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StopPlaySaveVoice();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_DelAllSaveVoiceFile([MarshalAs(UnmanagedType.LPArray)] string fileid, int fileindex);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_GetRoomMembers([MarshalAs(UnmanagedType.LPArray)] string roomName, byte[] buf, int len);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SpeechTranslate([MarshalAs(UnmanagedType.LPArray)] string fileID, int srcLang, int targetLang, int transType, int nTimeoutMS);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SpeechFileTranslate([MarshalAs(UnmanagedType.LPArray)] string filePath, int srcLang, int targetLang, int voiceType, float voiceRate, float volume,  int nTimeoutMS);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SpeechFileToText([MarshalAs(UnmanagedType.LPArray)] string filePath, int srcLang, int targetLang, int nTimeoutMS);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_TextTranslate([MarshalAs(UnmanagedType.LPArray)] string text, int srcLang, int targetLang, int nTimeoutMS);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_TextToSpeech([MarshalAs(UnmanagedType.LPArray)] string text, int lang, int voiceType, int nTimeoutMS);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_TextToSpeechFile([MarshalAs(UnmanagedType.LPArray)] string text, int lang, [MarshalAs(UnmanagedType.LPArray)] string filePath, int voiceType, float voiceRate, float volume, int nTimeoutMS);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_RSTSStartRecording(int srcLang, int[] pTargetLangs, int nTargetLangCnt, int transType, int nTimeoutMS);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_RSTSSpeechToSpeech(int srcLang, int[] pTargetLangs, int nTargetLangCnt, [MarshalAs(UnmanagedType.LPArray)] string dirPath, int voiceType, float voiceRate, float volume, int nTimeoutMS);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_RSTSSpeechToText(int srcLang, int[] pTargetLangs, int nTargetLangCnt, int nTimeoutMS);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_RSTSStopRecording();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableWXMiniApp([MarshalAs(UnmanagedType.LPArray)] string roomName, bool enable);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_QueryWXMembers([MarshalAs(UnmanagedType.LPArray)] string roomName);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_QueryUserInfo([MarshalAs(UnmanagedType.LPArray)] string roomName, int memberID, [MarshalAs(UnmanagedType.LPArray)] string openID);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_UpdateSelfInfo([MarshalAs(UnmanagedType.LPArray)] string roomName, string info);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetCivilBinPath([MarshalAs(UnmanagedType.LPArray)] string path);
				
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetPlayerVolume([MarshalAs(UnmanagedType.LPArray)] string playerid, uint vol);
		
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetPlayerVolume([MarshalAs(UnmanagedType.LPArray)] string playerid);

		
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableKeyWordsDetect(bool bEnable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableTranslate([MarshalAs(UnmanagedType.LPArray)] string roomName, bool enable, int myLang, int transType);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableMagicVoice([MarshalAs(UnmanagedType.LPArray)] string magicType, bool enable);
  
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_EnableRecvMagicVoice(bool enable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetPlayerInfoAbroad([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]string[] cszOpenID, int[] nMemberID,int []pLang,int cnt);		

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_TextToStreamSpeechStart([MarshalAs(UnmanagedType.LPArray)] string text, [MarshalAs(UnmanagedType.LPArray)] string voiceType, int nTimeoutMS, [MarshalAs(UnmanagedType.LPArray)] string filePath);
		
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_TextToStreamSpeechStop();	

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_RoomGeneralDataChannel([MarshalAs(UnmanagedType.LPArray)] string roomName, [MarshalAs(UnmanagedType.LPArray)] string content);			

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_APITrace([MarshalAs(UnmanagedType.LPArray)] string api, [MarshalAs(UnmanagedType.LPArray)] string callInfo);	

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetMagicVoiceMsgType([MarshalAs(UnmanagedType.LPArray)] string magicType);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetTransSecInfo([MarshalAs(UnmanagedType.LPArray)] string secInfo);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableReportALL(bool enable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableReportALLAbroad(bool enable);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_EnableCivilFile(bool bEnable);
	

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_Enable3DVoice(bool bEnable);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_Set3DPosition(float x, float y, float z);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_Set3DForward(float x, float y, float z);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_Set3DUpward(float x, float y, float z);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_Set3DDistProperties(int model, float minDistance, float maxDistance, float roll);	

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_AuditionFileForMagicType([MarshalAs(UnmanagedType.LPArray)] string file, [MarshalAs(UnmanagedType.LPArray)] string type);	

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_IsSaveMagicFile(bool bSave);
        
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_SetServerUrl(int urlType, [MarshalAs(UnmanagedType.LPArray)]string url);
		
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableDualLink(bool enable);
		
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_SetSoundTag(int vtype, float[] pParam, int nCount);

		#endregion

		public GCloudVoiceEngine()
		{
			int ret = GCloudVoice_CreateInstance();
			if(ret != 0)
			{
				PrintLog("Create GCloudVoiceInstance failed!");
			}
			pollBuf = new byte[pollBufLen];
			pollMsg = new NoticeMessage();
			roomNameBuf = new byte[256];
			memberVoice = new int[1024];
            membersBuf = new byte[membersBufLen];
		}

		public static void PrintLog(object logMsg)
		{
			if (null == logMsg)
			{
				return;
			}
			
			if (bPrintLog)
			{
				Debug.Log("GVoiceC#Log " + logMsg);
			}
		}

		public override int SetAppInfo(string appID, string appKey, string openID)
		{
			int ret = GCloudVoice_SetAppInfo(appID, appKey, openID);
			if(ret == 0)
			{
				bIsSetAppInfo = true;
			}
			return ret;
		}

		public override int SetServerInfo(string URL, string defaultipsvr = "")
		{
			int ret = GCloudVoice_SetServerInfo(URL, defaultipsvr);
			return ret;
		}

		public override int Init()
		{
			if(!bIsSetAppInfo)
			{
				PrintLog("please set appinfo first");
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_SETAPPINFO;
			}
			if(!bInit)
			{
				int ret = GCloudVoice_Init();
				if(ret != 0)
				{
					PrintLog("Init GCloudVoice failed!");
					return ret;
				}
				bInit = true;
			}
			return (int)GCloudVoiceErr.GCLOUD_VOICE_SUCC;
		}

		public override  int SetMode(GCloudVoiceMode nMode)
		{
			PrintLog("GCloudVoice_C# API: _SetMode");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			return GCloudVoice_SetMode((int)nMode);
		}

		public NoticeMessage NoticeMessage_ParseFrom(byte[] buf, int buflen)
		{
			int guard = 0;
			if (buflen - guard < sizeof(Int32)) {
				PrintLog("notifymsg,parse error, buf small then sizeof(int)");
				return null;
			}
			pollMsg.what = BitConverter.ToInt32 (buf, guard);
			guard += sizeof(UInt32);
			pollMsg.intArg1 = BitConverter.ToInt32 (buf, guard);
			guard += sizeof(UInt32);
			pollMsg.intArg2 = BitConverter.ToInt32 (buf, guard);
			guard += sizeof(UInt32);
			int strlen = BitConverter.ToInt32 (buf, guard);
			guard += sizeof(UInt32);
			if (strlen == 0) {
				pollMsg.strArg = "";
			} else {
				byte[] bstr = new byte[strlen];
				Array.Copy(buf, guard, bstr, 0, strlen);
				pollMsg.strArg = System.Text.Encoding.Default.GetString ( bstr );
			}
			guard += strlen;
			pollMsg.datalen = BitConverter.ToInt32 (buf, guard);
			guard += sizeof(UInt32);
			if (pollMsg.datalen > 0) {
				Array.Copy(buf, guard, pollMsg.custom, 0, pollMsg.datalen);
			}
			return pollMsg;

		}

		public  override  int Poll()
		{
			int ret = GCloudVoice_Poll(pollBuf, pollBufLen);
			if (ret != 0) {
				if(ret == (int)GCloudVoiceErr.GCLOUD_VOICE_POLL_MSG_NO)
				{
					//poll no msg, return succ
					return 0;
				}
				return ret;
			}
			pollMsg.clear ();
			NoticeMessage msg = NoticeMessage_ParseFrom (pollBuf, pollBufLen);
			if (msg == null) {
				return (int)GCloudVoiceErr.GCLOUD_VOICE_POLL_MSG_PARSE_ERR;
			}
            switch (msg.what){
                case (int)NoticeMessageType.MSG_ON_JOINROOM_COMPLETE:
                    if (OnJoinRoomComplete != null) {
                        PrintLog ("c# poll callback OnJoinRoomComplete not null");
                        OnJoinRoomComplete ((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.intArg2);
                    }
                    break;
                case (int)NoticeMessageType.MSG_ON_QUITROOM_COMPLETE:
                    if (OnQuitRoomComplete != null) {
                        OnQuitRoomComplete ((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.intArg2);
                    }
                    break;
                case (int)NoticeMessageType.MSG_ON_APPLY_AUKEY_COMPLETE:
                    if (OnApplyMessageKeyComplete != null) {
                        OnApplyMessageKeyComplete ((GCloudVoiceCompleteCode)msg.intArg1);
                    }
                    break;
                case (int)NoticeMessageType.MSG_ON_UPLOADFILE_COMPLETE:
                    if (OnUploadReccordFileComplete != null) {
                        OnUploadReccordFileComplete ((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.custom != null ? System.Text.Encoding.Default.GetString (msg.custom, 0, msg.datalen) : "");
                    }
                    break;
                case (int)NoticeMessageType.MSG_ON_DOWNFILE_COMPLETE:
                    if (OnDownloadRecordFileComplete != null) {
                        OnDownloadRecordFileComplete ((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.custom != null ? System.Text.Encoding.Default.GetString (msg.custom, 0, msg.datalen) : "");
                    }
                    break;
                case (int)NoticeMessageType.MSG_ON_PLAYFILE_COMPLETE:
                    if (OnPlayRecordFilComplete != null) {
                        OnPlayRecordFilComplete ((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg);
                    }
                    break;
                case (int)NoticeMessageType.MSG_ON_SPEECH_TO_TEXT:
                    if (OnSpeechToText != null) {
                        OnSpeechToText ((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.custom != null ? System.Text.Encoding.UTF8.GetString (msg.custom, 0, msg.datalen) : "");
                    }
                    break;
                case (int)NoticeMessageType.MSG_ON_STREAM_SPEECH_TO_TEXT:
                    if (OnStreamSpeechToText != null) {
                        OnStreamSpeechToText ((GCloudVoiceCompleteCode)msg.intArg1, msg.intArg2, msg.custom != null ? System.Text.Encoding.UTF8.GetString (msg.custom, 0, msg.datalen) : "", msg.strArg);
                    }
                    break;
                case (int)NoticeMessageType.MSG_ON_MEMBER_VOICE:
                    Array.Clear(memberVoice, 0, memberVoice.Length);
                    int memcount = msg.intArg1;
                    if (msg.intArg2 == 1) { // for signal room
                    	for (int i = 0; i < memcount; i++) {
                        	memberVoice [2 * i] = System.BitConverter.ToInt32 (pollMsg.custom, 2 * i * 4);
                        	memberVoice [2 * i + 1] = System.BitConverter.ToInt32 (pollMsg.custom, (2 * i + 1) * 4);
                    	}
	                    if (OnMemberVoice != null) {
	                        OnMemberVoice(memberVoice, memcount);
	                    }                    	

                    } else if (msg.intArg2 == 3) { // for multiroom
                    	string[] rooms = pollMsg.strArg.Split('$');
                    	int guard = 0;
                    	for (int i = 0; i < rooms.Length/2; i++) {
                    		string room = rooms[2*i];
							int count = Convert.ToInt32(rooms[2*i+1]);
							for (int j=0;j<count;j++) {
								int memID = System.BitConverter.ToInt32 (pollMsg.custom, 2 * guard * 4);
								int status = System.BitConverter.ToInt32 (pollMsg.custom, (2 * guard + 1) * 4);
								guard++;
		                        if (OnRoomMemberVoice != null) {
		                            OnRoomMemberVoice(room, memID, status);
		                        }
	                    	}
                    	}
                    }

                    break;
                case (int)NoticeMessageType.MSG_ON_ROOM_OFFLINE:
                    if (OnStatusUpdate != null) {
                        OnStatusUpdate ((GCloudVoiceCompleteCode)msg.intArg1,  msg.strArg, msg.intArg2);
                    }
                    break;
                case (int)NoticeMessageType.MSG_ON_ROLE_CHANGED:
                    if (OnRoleChangeComplete != null){
                        OnRoleChangeComplete((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.intArg2, msg.custom[0]);
                    }
                    break;
                case (int)NoticeMessageType.MSG_ON_EVENT_NOTIFY:
                    if (OnEvent != null){
                        OnEvent((GCloudVoiceEvent)msg.intArg1, msg.strArg);
                    }
                    break;
                case (int)NoticeMessageType.MSG_ON_MUTE_STATE:
                    if(OnMuteSwitchState != null){
                        OnMuteSwitchState(msg.intArg1);
                    }
                    break;
	            case (int)NoticeMessageType.MSG_ON_REPORT_PLAYER:
    	            if (OnReportPlayer != null)
        	        {
            	        OnReportPlayer((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg);
                	}
					break;

	            case (int)NoticeMessageType.MSG_ON_UPLOAD_SAVEDATA_COMPLETE:
                	if(OnSaveRecFileIndex != null)
					{
                    	OnSaveRecFileIndex((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.intArg2);
                	}
					break;
				case (int)NoticeMessageType.MSG_ON_SYNCHROMEMINFO_NOTIFY:
					if(OnRoomMemberInfo != null)
					{
						OnRoomMemberInfo((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.intArg2, msg.custom != null ? System.Text.Encoding.Default.GetString (msg.custom, 0, msg.datalen) : "");
					}
                    break;
				case (int)NoticeMessageType.MSG_ON_SPEECH_TRANSLATE:
					if (OnSpeechTranslate != null)
					{
						string strTargetText = "";
						string strTargetFileID = "";
						int nDurationMS = 0;
						if(msg.custom != null){
							string strArgs = System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen);
							string[] args = strArgs.Split('|');
							if(args.Length >= 3){
								nDurationMS = Convert.ToInt32(args[0]);
								strTargetFileID = args[1];
								strTargetText = args[2];						
							}
						}
						OnSpeechTranslate((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, strTargetText, strTargetFileID, nDurationMS);
					}
					break;
				case (int)NoticeMessageType.MSG_ON_SPEECH_FILE_TRANSLATE:
					if (OnSpeechFileTranslate != null)
					{
						string strTargetText = "";
						string strTargetFilePath = "";
						int nDurationMS = 0;
						if(msg.custom != null){
							string strArgs = System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen);
							string[] args = strArgs.Split('|');
							if(args.Length >= 3){
								nDurationMS = Convert.ToInt32(args[0]);
								strTargetFilePath = args[1];
								strTargetText = args[2];
							}
						}
						OnSpeechFileTranslate((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, strTargetText, strTargetFilePath, nDurationMS);
					}
					break;
					
				case (int)NoticeMessageType.MSG_ON_SPEECH_FILE_TO_TEXT:
					if (OnSpeechFileToText != null)
					{
						string strTargetText = "";
						string strTargetFileID = "";
						int nDurationMS = 0;
						if(msg.custom != null){
							string strArgs = System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen);
							string[] args = strArgs.Split('|');
							if(args.Length >= 2){
								nDurationMS = Convert.ToInt32(args[0]);
								strTargetText = args[1];
							}
						}
						OnSpeechFileToText((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, strTargetText, nDurationMS);
					}
					break;
				case (int)NoticeMessageType.MSG_ON_TTS:
					if (OnTextToSpeech != null)
					{
						string strFileID = "";
						SpeechLanguageType lang = SpeechLanguageType.SPEECH_LANGUAGE_ZH;
						if(msg.custom != null){
							string strArgs = System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen);
							string[] args = strArgs.Split('|');
							if(args.Length >= 2){
								lang = (SpeechLanguageType)Convert.ToInt32(args[0]);
								strFileID = args[1];					
							}
						}						
						OnTextToSpeech((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, lang, strFileID);
					}
					break;
				case (int)NoticeMessageType.MSG_ON_TTS_FILE:
					if (OnTextToSpeechFile != null)
					{
						string strFilePath = "";
						SpeechLanguageType lang = SpeechLanguageType.SPEECH_LANGUAGE_ZH;
						if(msg.custom != null){
							string strArgs = System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen);
							string[] args = strArgs.Split('|');
							if(args.Length >= 2){
								lang = (SpeechLanguageType)Convert.ToInt32(args[0]);
								strFilePath = args[1];
							}
						}
						OnTextToSpeechFile((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, lang, strFilePath);
					}
					break;
				case (int)NoticeMessageType.MSG_ON_TTS_STREAM:
					if (OnTextToStreamSpeech != null)
					{					
						OnTextToStreamSpeech((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.intArg2);
					}
					break;
				case (int)NoticeMessageType.MSG_ON_TTT:
					if(OnTextTranslate != null){
						SpeechLanguageType srcLang = SpeechLanguageType.SPEECH_LANGUAGE_ZH;
						SpeechLanguageType targetLang = SpeechLanguageType.SPEECH_LANGUAGE_ZH;
						string srcText = msg.strArg;
						string targetText = "";
						if(msg.custom != null){
							string strArgs = System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen);
							string[] args = strArgs.Split('|');
							if(args.Length >= 3){
								srcLang = (SpeechLanguageType)Convert.ToInt32(args[0]);
								targetLang = (SpeechLanguageType)Convert.ToInt32(args[1]);
								targetText = args[2];						
							}
						}
						OnTextTranslate((GCloudVoiceCompleteCode)msg.intArg1,srcLang,srcText,targetLang,targetText);
					}
					break;
				case (int)NoticeMessageType.MSG_ON_RSTS:
					if (OnRSTS != null)
					{
						string strTargetText = "";
						string strTargetFileID = "";
						int nDurationMS = 0;
						SpeechLanguageType srcLang = SpeechLanguageType.SPEECH_LANGUAGE_ZH;
						SpeechLanguageType targetLang = SpeechLanguageType.SPEECH_LANGUAGE_ZH;
						
						if(msg.custom != null){
							string strArgs = System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen);
							string[] args = strArgs.Split('|');
							if(args.Length >= 5){
								srcLang = (SpeechLanguageType)Convert.ToInt32(args[0]);
								targetLang = (SpeechLanguageType)Convert.ToInt32(args[1]);
								nDurationMS = Convert.ToInt32(args[2]);
								strTargetFileID = args[3];
								strTargetText = args[4];
							}
						}
						OnRSTS((GCloudVoiceCompleteCode)msg.intArg1, srcLang, targetLang, msg.strArg, strTargetText, strTargetFileID, nDurationMS);
					}
					break;
				case (int)NoticeMessageType.MSG_ON_MIC_SPEECH_TO_SPEECH:
					if (OnRSTSSpeechToSpeech != null)
					{
						string strTargetText = "";
						string strTargetFilePath = "";
						int nDurationMS = 0;
						SpeechLanguageType srcLang = SpeechLanguageType.SPEECH_LANGUAGE_ZH;
						SpeechLanguageType targetLang = SpeechLanguageType.SPEECH_LANGUAGE_ZH;
						
						if(msg.custom != null){
							string strArgs = System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen);
							string[] args = strArgs.Split('|');
							if(args.Length >= 5){
								srcLang = (SpeechLanguageType)Convert.ToInt32(args[0]);
								targetLang = (SpeechLanguageType)Convert.ToInt32(args[1]);
								nDurationMS = Convert.ToInt32(args[2]);
								strTargetFilePath = args[3];
								strTargetText = args[4];
							}
						}
						OnRSTSSpeechToSpeech((GCloudVoiceCompleteCode)msg.intArg1, srcLang, targetLang, msg.strArg, strTargetText, strTargetFilePath, nDurationMS);
					}
					break;
					
				case (int)NoticeMessageType.MSG_ON_MIC_SPEECH_TO_TEXT:
					if (OnRSTSSpeechToText != null)
					{
						string strTargetText = "";
						int nDurationMS = 0;
						SpeechLanguageType srcLang = SpeechLanguageType.SPEECH_LANGUAGE_ZH;
						SpeechLanguageType targetLang = SpeechLanguageType.SPEECH_LANGUAGE_ZH;
						
						if(msg.custom != null){
							string strArgs = System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen);
							string[] args = strArgs.Split('|');
							if(args.Length >= 4){
								srcLang = (SpeechLanguageType)Convert.ToInt32(args[0]);
								targetLang = (SpeechLanguageType)Convert.ToInt32(args[1]);
								nDurationMS = Convert.ToInt32(args[2]);
								strTargetText = args[3];
							}
						}
						OnRSTSSpeechToText((GCloudVoiceCompleteCode)msg.intArg1, srcLang, targetLang, msg.strArg, strTargetText, nDurationMS);
					}
					break;
					
				case (int)NoticeMessageType.MSG_ON_QUERY_USER_INFO:
					if (OnQueryUserInfo != null)
					{
						int code = msg.intArg1;
						string roomName = msg.custom != null
							? System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen)
							: "";
						WXMemberInfo memberInfo = Parse2MemberInfo(msg.strArg);
						OnQueryUserInfo((GCloudVoiceCompleteCode)code, roomName, memberInfo);
					}
					break;					

				case (int)NoticeMessageType.MSG_ON_QUERY_WX_MEMBERS:
		            if (OnQueryWXMembers != null)
		            {
			            int code = msg.intArg1;
			            WXMemberInfo[] memberInfos = Parse2MemberInfos(msg.strArg);
			            string roomName = msg.custom != null
				            ? System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen)
				            : "";
			            OnQueryWXMembers((GCloudVoiceCompleteCode)code, roomName, memberInfos);
		            }
		            break;

	            case (int)NoticeMessageType.MSG_ON_UPLOAD_USER_INFO:
		            if (OnUpdateUserInfo!= null)
		            {
			            int code = msg.intArg1;
			            int memberID = msg.intArg2;
			            string roomName = msg.strArg;
			            OnUpdateUserInfo((GCloudVoiceCompleteCode)code, roomName, memberID);
		            }
		            break;		            
	            case (int)NoticeMessageType.MSG_ON_WX_MEMBER_VOICE:
		            if (OnWXMemberVoice != null)
		            {
			            int count = msg.intArg1;
			            string roomName = msg.strArg;
			            int[] members = new int[count * 2];
			            Array.Clear(members, 0, count);
			            if (msg.custom != null)
			            {
				            for (int i = 0; i < count; i++)
				            {
					            members[2 * i] = System.BitConverter.ToInt16(msg.custom, 2 * i);
					            members[2 * i + 1] = 1;
				            }
			            }

			            OnWXMemberVoice(roomName, members, count);
		            }
		            break;
					case (int)NoticeMessageType.MSG_ON_REPORT_KEY_WORDS:
						{
							if(OnReportPlayer != null)
							{
							OnReportPlayer((GCloudVoiceCompleteCode)msg.intArg1,msg.strArg);
							}
						}
						break;
					case (int)NoticeMessageType.MSG_ON_UPDATE_MIC_LEVEL:
					{
						if(OnUpdateMicLevel != null)
						{
							OnUpdateMicLevel(msg.intArg1);
						}
					}
					break;

	            case (int)NoticeMessageType.MSG_ON_ENABLE_TRANSLATE:
                	if(OnEnableTranslate != null)
					{
                    	OnEnableTranslate((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, (RealTimeTranslateType)msg.intArg2);
                	}
				break;
	            case (int)NoticeMessageType.MSG_ON_TRANSLATE_TEXT:
                	if(OnRealTimeTranslateText != null)
					{
						string roomName = "";
						string sessionID = "";
						string text = msg.strArg;
						int seq = 0;
						int memberID = 0;
						
						if(msg.custom != null){
							string strArgs = System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen);
							string[] args = strArgs.Split('|');
							if(args.Length >= 4){
								seq = Convert.ToInt32 (args[0]);
								memberID = Convert.ToInt32 (args[1]);
								sessionID = args[2];
								roomName = args[3];
							}
						}
                    	OnRealTimeTranslateText(roomName, memberID, sessionID, seq, text);
                	}
				break;
				case (int)NoticeMessageType.MSG_ON_ENABLE_MAGIC_VOICE:
					if(OnEnableMagicVoice != null){
						OnEnableMagicVoice((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, Convert.ToBoolean(msg.intArg2));
					}
				break;
                case (int)NoticeMessageType.MSG_ON_ENABLE_RECV_MAGIC_VOICE:
                    if(OnEnableRecvMagicVoice != null){
                        OnEnableRecvMagicVoice((GCloudVoiceCompleteCode)msg.intArg1, Convert.ToBoolean(msg.intArg2));
                    }
                break;
				case (int)NoticeMessageType.MSG_ON_REPORT_ABROAD:
				{
					if(OnSTTReport != null)
					{
						string fileid = "";
						string openid = "";
						if(msg.custom != null)
						{
							string strArgs = System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen);
							string[] args = strArgs.Split('|');
							if(args.Length >= 2)
							{
								openid = args[0];
								fileid = args[1];						
							}
						}
						OnSTTReport((GCloudVoiceCompleteCode)msg.intArg1, msg.strArg,openid,fileid);
					}
				}
				break;
                case (int)NoticeMessageType.MSG_ON_LOG_CHANGED:
                {
                    bPrintLog = Convert.ToBoolean(msg.intArg1);
                }
                break;
				case (int)NoticeMessageType.MSG_ON_MAGIC_VOICE_MSG:
				{
					if(OnMagicVoiceMsg != null)
					{
						OnMagicVoiceMsg((GCloudVoiceCompleteCode)msg.intArg1,msg.strArg);
					}
				}
				break;
				case (int)NoticeMessageType.MSG_ON_RT_SECINFO:
				{
					if(OnRTSecInfo != null)
					{
						string strsecinfo = "";
						if(msg.custom != null)
						{
							strsecinfo = System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen);
						}
						OnRTSecInfo(msg.intArg1, msg.strArg, strsecinfo);
					}
				}
				break;
				case (int)NoticeMessageType.MSG_ON_AI_RECOGNITION:
				{
					if(OnAIRecognition != null)
					{
						string info = "";
						if(msg.custom != null)
						{
							info = System.Text.Encoding.UTF8.GetString(msg.custom, 0, msg.datalen);
						}
						OnAIRecognition( msg.strArg, info);
					}
				}
				break;
              default:
                  break;
            }
            return (int)GCloudVoiceErr.GCLOUD_VOICE_SUCC;
        }

		public override  int Pause()
		{
			PrintLog("GCloudVoice_C# API: _Pause");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_Pause();
			PrintLog("GCloudVoice_C# API: _Pause nRet=" + nRet);
			return nRet;
		}

		public  override int Resume()
		{
			PrintLog("GCloudVoice_C# API: _Resume");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_Resume();
			PrintLog("GCloudVoice_C# API: _Resume nRet=" + nRet);
			return nRet;
		}

		public override int JoinTeamRoom(string roomName, int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: JoinTeamRoom");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_JoinTeamRoom(roomName, msTimeout);
			PrintLog("GCloudVoice_C# API: JoinTeamRoom  nRet=" + nRet);
			return nRet;
		}

		public override int JoinRangeRoom(string roomName, int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: JoinRangeRoom");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_JoinRangeRoom(roomName, msTimeout);
			PrintLog("GCloudVoice_C# API: JoinRangeRoom  nRet=" + nRet);
			return nRet;
		}


		public override int JoinTeamRoom(string roomName, string token, int timestamp, int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: JoinTeamRoom"+" mstimeout:"+msTimeout);
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_JoinTeamRoom_Token(roomName, token, timestamp, msTimeout);
			PrintLog("GCloudVoice_C# API: JoinTeamRoom  nRet=" + nRet);
			return nRet;
		}

		public override int JoinNationalRoom(string roomName, GCloudVoiceRole role, int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: JoinNationalRoom");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_JoinNationalRoom(roomName, (int)role, msTimeout);
			PrintLog("GCloudVoice_C# API: JoinNationalRoom  nRet=" + nRet);
			return nRet;
		}

        public override int ChangeRole(GCloudVoiceRole role, string roomName="")
        {
            PrintLog("GCloudVoice_C# API: ChangeRole");
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            int nRet = GCloudVoice_ChangeRole((int)role, roomName);
            PrintLog("GCloudVoice_C# API: GCloudVoice_ChangeRole  nRet=" + nRet);
            return nRet;
        }

		public override int JoinNationalRoom(string roomName, string token, int timestamp, GCloudVoiceRole role, int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: JoinNationalRoom");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_JoinNationalRoom_Token(roomName, (int)role, token, timestamp, msTimeout);
			PrintLog("GCloudVoice_C# API: JoinNationalRoom  nRet=" + nRet);
			return nRet;
		}

		public override int QuitRoom(string roomName, int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: QuitRoom");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_QuitRoom(roomName, msTimeout);
			PrintLog("GCloudVoice_C# API: QuitRoom  nRet=" + nRet);
			return nRet;
		}
		
		public override int JoinTeamRoom_Scenes(string scenesName, string roomName, int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: JoinTeamRoom_Scenes"+" mstimeout:"+msTimeout);
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_JoinTeamRoom_Scenes(scenesName, roomName, msTimeout);
			PrintLog("GCloudVoice_C# API: JoinTeamRoom_Scenes  nRet=" + nRet);
			return nRet;
		}
		
		public override int JoinRangeRoom_Scenes(string scenesName, string roomName, int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: JoinRangeRoom_Scenes"+" mstimeout:"+msTimeout);
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_JoinRangeRoom_Scenes(scenesName, roomName, msTimeout);
			PrintLog("GCloudVoice_C# API: JoinRangeRoom_Scenes  nRet=" + nRet);
			return nRet;
		}
		
		public override int JoinNationalRoom_Scenes(string scenesName, string roomName, GCloudVoiceRole role, int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: JoinNationalRoom_Scenes");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_JoinNationalRoom_Scenes(scenesName, roomName, (int)role, msTimeout);
			PrintLog("GCloudVoice_C# API: JoinNationalRoom_Scenes  nRet=" + nRet);
			return nRet;
		}
		
		public override int QuitRoom_Scenes(string scenesName, int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: QuitRoom_Scenes");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_QuitRoom_Scenes(scenesName, msTimeout);
			PrintLog("GCloudVoice_C# API: QuitRoom_Scenes  nRet=" + nRet);
			return nRet;
		}
		
		public override int OpenMic()
		{
			PrintLog("GCloudVoice_C# API: OpenMic");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_OpenMic();
			PrintLog("GCloudVoice_C# API: OpenMic  nRet=" + nRet);
			return nRet;
		}

		public override int CloseMic()
		{
			PrintLog("GCloudVoice_C# API: CloseMic");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_CloseMic();
			PrintLog("GCloudVoice_C# API: CloseMic  nRet=" + nRet);
			return nRet;
		}

		public override int OpenSpeaker()
		{
			PrintLog("GCloudVoice_C# API: OpenSpeaker");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_OpenSpeaker();
			PrintLog("GCloudVoice_C# API: OpenSpeaker  nRet=" + nRet);
			return nRet;
		}

		public override int CloseSpeaker()
		{
			PrintLog("GCloudVoice_C# API: CloseSpeaker");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_CloseSpeaker();
			PrintLog("GCloudVoice_C# API: CloseSpeaker  nRet=" + nRet);
			return nRet;
		}


		public override int EnableRoomMicrophone(string roomName, bool enable)
		{
			PrintLog("GCloudVoice_C# API: EnableRoomMicrophone");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_EnableRoomMicrophone(roomName, enable);
			PrintLog("GCloudVoice_C# API: EnableRoomMicrophone  nRet=" + nRet);
			return nRet;
		}

		public override int EnableRoomSpeaker(string roomName, bool enable)
		{
			PrintLog("GCloudVoice_C# API: EnableRoomSpeaker");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_EnableRoomSpeaker(roomName, enable);
			PrintLog("GCloudVoice_C# API: EnableRoomSpeaker  nRet=" + nRet);
			return nRet;

		}

		public override int EnableMultiRoom(bool enable)
		{
			PrintLog("GCloudVoice_C# API: EnableMultiRoom");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_EnableMultiRoom(enable);
			PrintLog("GCloudVoice_C# API: EnableMultiRoom  nRet=" + nRet);
			return nRet;

		}

		public override int UpdateCoordinate (string roomName, long x, long y, long z, long r)
		{
			PrintLog("GCloudVoice_C# API: UpdateCoordinate");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_UpdateCoordinate (roomName, x, y, z, r);
			PrintLog("GCloudVoice_C# API: UpdateCoordinate  nRet=" + nRet);
			return nRet;
		}

		public override int ApplyMessageKey(int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: ApplyMessageKey");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_ApplyMessageKey(msTimeout);
			PrintLog("GCloudVoice_C# API: ApplyMessageKey  nRet=" + nRet);
			return nRet;
		}

		public override int ApplyMessageKey(string token, int timestamp, int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: ApplyMessageKey");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_ApplyMessageKey_Token(token, timestamp, msTimeout);
			PrintLog("GCloudVoice_C# API: ApplyMessageKey  nRet=" + nRet);
			return nRet;
		}

		public override int StartRecording(string filePath)
		{
			PrintLog("GCloudVoice_C# API: StartRecording");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			
			int nRet = GCloudVoice_StartRecording(filePath, false);
			PrintLog("GCloudVoice_C# API: StartRecording  nRet=" + nRet);
			return nRet;
		}

		public override int StopRecording ()
		{
			PrintLog("GCloudVoice_C# API: StopRecording");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_StopRecording();
			PrintLog("GCloudVoice_C# API: StopRecording  nRet=" + nRet);
			return nRet;
		}

#if UNITY_IPHONE
        public override int StartRecording(string filePath, bool bOptimized)
		{
			PrintLog("GCloudVoice_C# API: StartRecording");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_StartRecording(filePath, bOptimized);
			PrintLog("GCloudVoice_C# API: StartRecording  nRet=" + nRet);
			return nRet;
		}

#endif

		public override int UploadRecordedFile(string filePath, int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: UploadRecordedFile");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_UploadRecordedFile(filePath, msTimeout, false);
			PrintLog("GCloudVoice_C# API: UploadRecordedFile  nRet=" + nRet);
			return nRet;
		}

		public override int DownloadRecordedFile(string fileID, string downloadFilePath, int msTimeout)
		{
			PrintLog("GCloudVoice_C# API: DownloadRecordedFile");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_DownloadRecordedFile(fileID, downloadFilePath, msTimeout, false);
			PrintLog("GCloudVoice_C# API: DownloadRecordedFile  nRet=" + nRet);
			return nRet;
		}

        public override int UploadRecordedFile(string filePath, int msTimeout, bool bPermanent)
        {
            PrintLog("GCloudVoice_C# API: UploadRecordedFile");
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }

            int nRet = GCloudVoice_UploadRecordedFile(filePath, msTimeout, bPermanent);
            PrintLog("GCloudVoice_C# API: UploadRecordedFile  nRet=" + nRet);
            return nRet;
        }

        public override int DownloadRecordedFile(string fileID, string downloadFilePath, int msTimeout, bool bPermanent)
        {
            PrintLog("GCloudVoice_C# API: DownloadRecordedFile");
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }

            int nRet = GCloudVoice_DownloadRecordedFile(fileID, downloadFilePath, msTimeout, bPermanent);
            PrintLog("GCloudVoice_C# API: DownloadRecordedFile  nRet=" + nRet);
            return nRet;
        }

		public override int PlayRecordedFile(string downloadFilePath)
		{
			PrintLog("GCloudVoice_C# API: PlayRecordedFile");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_PlayRecordedFile(downloadFilePath);
			PrintLog("GCloudVoice_C# API: PlayRecordedFile  nRet=" + nRet);
			return nRet;
		}

		public override int StopPlayFile()
		{
			PrintLog("GCloudVoice_C# API: StopPlayFile");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_StopPlayFile();
			PrintLog("GCloudVoice_C# API: StopPlayFile  nRet=" + nRet);
			return nRet;
		}

		public override int SpeechToText(string fileID, int language = 0, int msTimeout = 6000)
		{
			PrintLog("GCloudVoice_C# API: SpeechToText");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_SpeechToText(fileID, language, msTimeout);
			PrintLog("GCloudVoice_C# API: SpeechToText  nRet=" + nRet);
			return nRet;
		}

		public override int SpeechToText(string fileID, string token, int timestamp, int language = 0, int msTimeout = 6000)
		{
			PrintLog("GCloudVoice_C# API: SpeechToText");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_SpeechToText_Token(fileID, token, timestamp, language, msTimeout);
			PrintLog("GCloudVoice_C# API: SpeechToText  nRet=" + nRet);
			return nRet;
		}

		public override int ForbidMemberVoice(int member, bool bEnable, string roomName="")
		{
			PrintLog("GCloudVoice_C# API: ForbidMemberVoice");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_ForbidMemberVoice(member, bEnable, roomName);
			PrintLog("GCloudVoice_C# API: ForbidMemberVoice  nRet=" + nRet);
			return nRet;
		}

		public override int EnableLog(bool enable)
		{
			PrintLog("GCloudVoice_C# API: EnableLog");
			
			int nRet = GCloudVoice_EnableLog(enable);
            bPrintLog = enable;
			PrintLog("GCloudVoice_C# API: EnableLog  nRet=" + nRet);
			return nRet;
		}

		public override int SetDataFree(bool enable)
		{
			PrintLog("GCloudVoice_C# API: SetDataFree");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_SetDataFree(enable);
			PrintLog("GCloudVoice_C# API: SetDataFree  nRet=" + nRet);
			return nRet;

		}

		public override int GetMicLevel()
		{
			PrintLog("GCloudVoice_C# API: GetMicLevel");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_GetMicLevel(true);
			PrintLog("GCloudVoice_C# API: GetMicLevel  nRet=" + nRet);
			return nRet;
		}

        public override int GetMicLevel(bool bFadeOut)
        {
            PrintLog("GCloudVoice_C# API: GetMicLevel");
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }

            int nRet = GCloudVoice_GetMicLevel(bFadeOut);
            PrintLog("GCloudVoice_C# API: GetMicLevel  nRet=" + nRet);
            return nRet;
        }

		public override int GetSpeakerLevel()
		{
			PrintLog("GCloudVoice_C# API: GetSpeakerLevel");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_GetSpeakerLevel();
			PrintLog("GCloudVoice_C# API: GetSpeakerLevel  nRet=" + nRet);
			return nRet;
		}

		public override int SetSpeakerVolume(int vol)
		{
			PrintLog("GCloudVoice_C# API: SetSpeakerVolume");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_SetSpeakerVolume(vol);
			PrintLog("GCloudVoice_C# API: SetSpeakerVolume  nRet=" + nRet);
			return nRet;
		}

		public override int TestMic()
		{
			PrintLog("GCloudVoice_C# API: TestMic");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_TestMic();
			PrintLog("GCloudVoice_C# API: TestMic  nRet=" + nRet);
			return nRet;
		}

		public override int GetFileParam(string filepath, int [] bytes, float [] seconds)
		{
			PrintLog("GCloudVoice_C# API: GetFileParam");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_GetFileParam(filepath, bytes, seconds);
			PrintLog("GCloudVoice_C# API: GetFileParam  nRet=" + nRet);
			return nRet;
		}

		public override int invoke( uint nCmd, uint nParam1, uint nParam2, uint [] pOutput )
		{
			PrintLog("GCloudVoice_C# API: invoke");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_invoke(nCmd, nParam1, nParam2, pOutput);
			PrintLog("GCloudVoice_C# API: invoke  nRet=" + nRet);
			return nRet;
		}
		public override int EnableSpeakerOn(bool bEnable)
		{
			PrintLog("GCloudVoice_C# API: EnableSpeakerOn");

			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_EnableSpeakerOn(bEnable);
			PrintLog("GCloudVoice_C# API: GCloudVoice_EnableSpeakerOn  nRet=" + nRet);
			return nRet;

		}

		public override int SetMicVolume(int vol)
		{
			PrintLog("GCloudVoice_C# API: SetMicVol");

			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_SetMicVol(vol);
			PrintLog("GCloudVoice_C# API: GCloudVoice_SetMicVol  nRet=" + nRet);
			return nRet;

		}

		public override int SetVoiceEffects(SoundEffects mode)
		{
			PrintLog("GCloudVoice_C# API: SetVoiceEffects");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_SetVoiceEffects((int)mode);
			PrintLog("GCloudVoice_C# API: SetVoiceEffects  nRet=" + nRet);
			return nRet;
		}

        public override int IsSpeaking()
        {
            if (!bInit)
            {
                return 0;
            }

            return GCloudVoice_IsSpeaking();
        }


		public override int EnableReverb(bool bEnable)
		{
			PrintLog("GCloudVoice_C# API: EnableReverb");

			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_EnableReverb(bEnable);
			PrintLog("GCloudVoice_C# API: GCloudVoice_EnableReverb  nRet=" + nRet);
			return nRet;
		}

		public override int SetReverbMode(int mode)
		{
			PrintLog("GCloudVoice_C# API: SetReverbMode");

			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_SetReverbMode(mode);
			PrintLog("GCloudVoice_C# API: GCloudVoice_SetReverbMode  nRet=" + nRet);
			return nRet;

		}
		public override int GetVoiceIdentify()
		{
			PrintLog("GCloudVoice_C# API: GetVoiceIdentify");

			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_GetVoiceIdentify();
			PrintLog("GCloudVoice_C# API: GCloudVoice_GetVoiceIdentify nRet = "+ nRet);
			return nRet;

		}

		public override int SetBGMPath(string path)
		{
			PrintLog("GCloudVoice_C# API: SetBGMPath");

			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_SetBGMPath(path);
			PrintLog("GCloudVoice_C# API: GCloudVoice_SetBGMPath nRet = "+ nRet);
			return nRet;

		}

		public override int StartBGMPlay()
		{
			PrintLog("GCloudVoice_C# API: StartBGMPlay");

			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_StartBGMPlay();
			PrintLog("GCloudVoice_C# API: GCloudVoice_StartBGMPlay nRet = "+ nRet);
			return nRet;

		}

		public override int StopBGMPlay()
		{
			PrintLog("GCloudVoice_C# API: StopBGMPlay");

			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_StopBGMPlay();
			PrintLog("GCloudVoice_C# API: GCloudVoice_StopBGMPlay nRet = "+ nRet);
			return nRet;

		}

		public override int PauseBGMPlay()
		{
			PrintLog("GCloudVoice_C# API: PauseBGMPlay");

			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_PauseBGMPlay();
			PrintLog("GCloudVoice_C# API: GCloudVoice_PauseBGMPlay nRet = "+ nRet);
			return nRet;

		}

		public override int ResumeBGMPlay()
		{
			PrintLog("GCloudVoice_C# API: ResumeBGMPlay");

			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_ResumeBGMPlay();
			PrintLog("GCloudVoice_C# API: GCloudVoice_ResumeBGMPlay nRet = "+ nRet);
			return nRet;

		}

		public override int SetBGMVol(int vol)
		{
			PrintLog("GCloudVoice_C# API: SetBGMVol");

			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_SetBGMVol(vol);
			PrintLog("GCloudVoice_C# API: GCloudVoice_SetBGMVol nRet = "+ nRet);
			return nRet;

		}

		public override int GetBGMPlayState()
		{
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_GetBGMPlayState();
			return nRet;

		}

		public override int EnableNativeBGMPlay(bool bEnable)
		{
			PrintLog("GCloudVoice_C# API: GetVoiceIdentify");

			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_EnableNativeBGMPlay(bEnable);
			PrintLog("GCloudVoice_C# API: GCloudVoice_EnableNativeBGMPlay nRet = "+ nRet);
			return nRet;

		}

		public override int SetBitRate(int bitrate)
		{
			PrintLog("GCloudVoice_C# API: SetBitRate");

			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_SetBitRate(bitrate);
			PrintLog("GCloudVoice_C# API: GCloudVoice_SetBitRate nRet = "+ nRet);
			return nRet;

		}

        public override int getAudioDeviceConnectionState()
		{
			PrintLog("GCloudVoice_C# API: getAudioDeviceConnectionState");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_GetAudioDeviceConnectionState();
			PrintLog("GCloudVoice_C# API: getAudioDeviceConnectionState  nRet=" + nRet);
			return nRet;
		}

        public override void EnableBluetoothSCO(bool enable)
		{
			PrintLog("GCloudVoice_C# API: EnableBluetoothSCO");
			if (!bInit)
			{
				return;
			}
			GCloudVoice_EnableBluetoothSCO(enable);
		}

        public override int CheckDeviceMuteState()
        {
            PrintLog("GCloudVoice_C# API: CheckDeviceMuteState");
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            return GCloudVoice_CheckDeviceMuteState();
        }

        public override int SetAudience(int []audience, string roomName ="" )
        {
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			return GCloudVoice_SetAudience(audience, audience.Length, roomName);
        }


    	public override int GetMicState()
    	{
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			return GCloudVoice_GetMicState();
    	}
     
    	public override int GetSpeakerState()
    	{
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			return GCloudVoice_GetSpeakerState();
    	}
        

    	public override int GetMuteResult()
    	{
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			return GCloudVoice_GetMuteResult();
    	}
        
        // APIs with token
        public override int JoinTeamRoom_Token(string roomName, string token, int timestamp, int msTimeout)
        {
            PrintLog("GCloudVoice_C# API: JoinTeamRoom_Token"+" mstimeout:"+msTimeout);
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            
            int nRet = GCloudVoice_JoinTeamRoom_Token(roomName, token, timestamp, msTimeout);
            PrintLog("GCloudVoice_C# API: JoinTeamRoom_Token  nRet=" + nRet);
            return nRet;
        }
        
        public override int JoinNationalRoom_Token(string roomName, string token, int timestamp, GCloudVoiceRole role, int msTimeout)
        {
            PrintLog("GCloudVoice_C# API: JoinNationalRoom_Token");
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            
            int nRet = GCloudVoice_JoinNationalRoom_Token(roomName, (int)role, token, timestamp, msTimeout);
            PrintLog("GCloudVoice_C# API: JoinNationalRoom_Token  nRet=" + nRet);
            return nRet;
        }
        
        public override int ApplyMessageKey_Token(string token, int timestamp, int msTimeout)
        {
            PrintLog("GCloudVoice_C# API: ApplyMessageKey_Token");
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            
            int nRet = GCloudVoice_ApplyMessageKey_Token(token, timestamp, msTimeout);
            PrintLog("GCloudVoice_C# API: ApplyMessageKey_Token  nRet=" + nRet);
            return nRet;
        }
        
        public override int SpeechToText_Token(string fileID, string token, int timestamp, int language = 0, int msTimeout = 6000)
        {
            PrintLog("GCloudVoice_C# API: SpeechToText_Token");
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            
            int nRet = GCloudVoice_SpeechToText_Token(fileID, token, timestamp, language, msTimeout);
            PrintLog("GCloudVoice_C# API: SpeechToText_Token  nRet=" + nRet);
            return nRet;
        }
        public override int SetReportBufferTime(int nTimeSec)
        {
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            GCloudVoice_SetReportBufferTime(nTimeSec);
            return (int)GCloudVoiceErr.GCLOUD_VOICE_SUCC;
        }
        public override int SetReportedPlayerInfo(string[] cszOpenID, int[] nMemberID, int nCount)
        {
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            return GCloudVoice_SetReportedPlayerInfo(cszOpenID, nMemberID, nCount);
        }
        public override int ReportPlayer(string[] cszOpenID, int nCount, string strInfo)
        {
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            return GCloudVoice_ReportPlayer(cszOpenID, nCount, strInfo);
        }
		
		public override int GetRoomMembers(string roomName, RoomMembers[] members, int len)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            //return room members num
            if (members == null || len == -1)
            {
                return GCloudVoice_GetRoomMembers(roomName, null, -1);
            }
            int retNum = GCloudVoice_GetRoomMembers(roomName, membersBuf, membersBufLen);
            int usedbuflen = 0;
            int guard = 0;
            int openidlen = 0;
            usedbuflen = BitConverter.ToInt32(membersBuf, guard);
            guard += sizeof(Int32);
            for(int i = 0; i < retNum && i < len && guard < membersBufLen && guard <=usedbuflen; i++)
            {
                members[i].memberid = BitConverter.ToInt32(membersBuf, guard);
                guard += sizeof(Int32);
                openidlen = BitConverter.ToInt32(membersBuf, guard);
                guard += sizeof(Int32);
                members[i].openid = System.Text.Encoding.Default.GetString(membersBuf, guard, openidlen);
                guard += openidlen;
				members[i].micstatus = BitConverter.ToInt32(membersBuf, guard);
				guard += sizeof(Int32);
            }
            return retNum;
        }
		///////////////////////////////////////////////////////////////////////////////

        public override int EnableCivilVoice(bool bEnable)
        {
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            return GCloudVoice_EnableCivilVoice(bEnable);
        }

		///////////////////////////////////////////////////////////////////////////////
       public override int StartSaveVoice()
        {
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            return GCloudVoice_StartSaveVoice();
        }

        public override int StopSaveVoice()
        {
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            return GCloudVoice_StopSaveVoice();
        }

        public override int SetRecSaveTs(int ts)
        {
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            return GCloudVoice_SetRecSaveTs(ts);
        }

        public override int SetPlayFileIndex(string fileid, int fileindex)
        {
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            return GCloudVoice_SetPlayFileIndex(fileid, fileindex);
        }

        public override int StartPlaySaveVoiceTs(int ts)
        {
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            return GCloudVoice_StartPlaySaveVoiceTs(ts);
        }
	
		public override int StopPlaySaveVoice()
		{
	    	if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            return GCloudVoice_StopPlaySaveVoice();
		}

        public override int DelAllSaveVoiceFile(string fileid, int fileindex)
        {
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            return GCloudVoice_DelAllSaveVoiceFile(fileid, fileindex);
        }
		public override int SpeechTranslate(string fileID, SpeechLanguageType srcLang, SpeechLanguageType targetLang, SpeechTranslateType transType, int nTimeoutMS)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_SpeechTranslate(fileID, (int)srcLang, (int)targetLang, (int)transType, nTimeoutMS);
		}
		

		public override int SpeechFileTranslate(string filePath, SpeechLanguageType srcLang, SpeechLanguageType targetLang, int voiceType, float voiceRate, float volume,  int nTimeoutMS)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_SpeechFileTranslate(filePath, (int)srcLang, (int)targetLang, voiceType, voiceRate, volume, nTimeoutMS);
		}

		public override int SpeechFileToText(string filePath, SpeechLanguageType srcLang, SpeechLanguageType targetLang, int nTimeoutMS)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_SpeechFileToText(filePath, (int)srcLang, (int)targetLang, nTimeoutMS);
		}
		
		public override int TextToSpeech(string text, SpeechLanguageType lang, int voiceType, int nTimeoutMS)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_TextToSpeech(text, (int)lang, (int)voiceType, nTimeoutMS);
		}
		
		public override int TextToSpeechFile(string text, SpeechLanguageType lang, string filePath, int voiceType, float voiceRate, float volume, int nTimeoutMS)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_TextToSpeechFile(text, (int)lang, filePath, voiceType, voiceRate, volume, nTimeoutMS);
		}
		
		
		public override int TextTranslate(string text, SpeechLanguageType srcLang, SpeechLanguageType targetLang, int nTimeoutMS)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_TextTranslate(text, (int)srcLang, (int)targetLang, nTimeoutMS);
		}
		
		public override int RSTSStartRecording(SpeechLanguageType srcLang, SpeechLanguageType[] pTargetLangs, int nTargetLangCnt, SpeechTranslateType transType, int nTimeoutMS)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			int[] nTargetLangs = Array.ConvertAll(pTargetLangs, value => (int) value);
			return GCloudVoice_RSTSStartRecording((int)srcLang, nTargetLangs, nTargetLangCnt, (int)transType, nTimeoutMS);
		}


		public override int RSTSSpeechToSpeech(SpeechLanguageType srcLang, SpeechLanguageType[] pTargetLangs, int nTargetLangCnt, string dirPath, int voiceType, float voiceRate, float volume, int nTimeoutMS)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			int[] nTargetLangs = Array.ConvertAll(pTargetLangs, value => (int) value);
			return GCloudVoice_RSTSSpeechToSpeech((int)srcLang, nTargetLangs, nTargetLangCnt, dirPath, voiceType, voiceRate, volume, nTimeoutMS);
		}
		
		public override int RSTSSpeechToText(SpeechLanguageType srcLang, SpeechLanguageType[] pTargetLangs, int nTargetLangCnt, int nTimeoutMS)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			int[] nTargetLangs = Array.ConvertAll(pTargetLangs, value => (int) value);
			return GCloudVoice_RSTSSpeechToText((int)srcLang, nTargetLangs, nTargetLangCnt, nTimeoutMS);
		}
		
		public override int RSTSStopRecording()
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_RSTSStopRecording();
		}

		public override int EnableTranslate(string roomname, bool enable, SpeechLanguageType myLang, RealTimeTranslateType transType)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_EnableTranslate(roomname, enable, (int)myLang, (int)transType);
		}
		
		public override int EnableWXMiniApp(string roomName, bool enable)
		{
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			return GCloudVoice_EnableWXMiniApp(roomName, enable);
		}

		public override int UpdateSelfInfo(string roomName, string info)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_UpdateSelfInfo(roomName, info);
		}		

        
    	public override int  QueryWXMembers(string roomName)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_QueryWXMembers(roomName);
		}
        
    	public override int  QueryUserInfo(string roomName, int memberID, string openID)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_QueryUserInfo(roomName, memberID, openID);
		}

		public string ToBase64String(string str)
		{
			if (String.IsNullOrEmpty(str))
			{
				return "";
			}

			byte[] bytes = Encoding.UTF8.GetBytes(str);
			return Convert.ToBase64String(bytes);
		}

		public string UnBase64String(string str)
		{
			if (String.IsNullOrEmpty(str))
			{
				return "";
			}

			byte[] bytes = Convert.FromBase64String(str);
			return Encoding.UTF8.GetString(bytes);
		}

		public WXMemberInfo[] Parse2MemberInfos(string userInfo)
		{
			if (String.IsNullOrEmpty(userInfo))
			{
				return null;
			}

			string[] userInfos = userInfo.Split('$');
			int count = userInfos.Length / 3;
			WXMemberInfo[] memberInfos = new WXMemberInfo[count];

			for (int i = 0; i < count; i++)
			{
				WXMemberInfo memberInfo = new WXMemberInfo();
				memberInfo.memberID = Convert.ToInt32(UnBase64String(userInfos[3 * i]));
				memberInfo.openID = UnBase64String(userInfos[3 * i + 1]);
				memberInfo.info = UnBase64String(userInfos[3 * i + 2]);
				memberInfos[i] = memberInfo;
			}

			return memberInfos;
		}

		public WXMemberInfo Parse2MemberInfo(string userInfo)
		{
			if (String.IsNullOrEmpty(userInfo))
			{
				return null;
			}

			string[] userInfos = userInfo.Split('$');
			WXMemberInfo memberInfo = new WXMemberInfo();
			memberInfo.memberID = Convert.ToInt32(UnBase64String(userInfos[0]));
			memberInfo.openID = UnBase64String(userInfos[1]);
			memberInfo.info = UnBase64String(userInfos[2]);

			return memberInfo;
		}

		public override int SetCivilBinPath(string path)
		{
			PrintLog("GCloudVoice_C# API: SetCivilBinPath");
	
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
	
			int nRet = GCloudVoice_SetCivilBinPath(path);
			PrintLog("GCloudVoice_C# API: SetCivilBinPath nRet = "+ nRet);
			return nRet;
	
		}
		
				
		public override int SetPlayerVolume(string playerid, uint vol)
		{
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			int nRet = GCloudVoice_SetPlayerVolume(playerid, vol);
			return nRet;
		}
	
	
		public override int GetPlayerVolume(string playerid)
		{
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			int nRet = GCloudVoice_GetPlayerVolume(playerid);
			return nRet;
		}


		public override int EnableKeyWordsDetect(bool bEnable)
		{
			PrintLog("GCloudVoice_C# API: EnableKeyWordsDetect");
	
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
	
			int nRet = GCloudVoice_EnableKeyWordsDetect(bEnable);
            PrintLog("GCloudVoice_C# API: EnableKeyWordsDetect nRet = "+ nRet);
			return nRet;
		}
  
        public override int EnableMagicVoice(string magicType, bool enable){
            PrintLog("GCloudVoice_C# API: EnableMagicVoice");
            
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            
            if (String.IsNullOrEmpty(magicType))
            {
                PrintLog("The parameter magicType is Null or empty.");
                return (int)GCloudVoiceErr.GCLOUD_VOICE_PARAM_NULL;
            }

	        int ret = GCloudVoice_EnableMagicVoice(magicType, enable);

	        string logMsg = string.Format("EnableMagicVoice enable: {0} magicType: {1} ret: {2}", enable, magicType, ret);
            PrintLog(logMsg);
            return ret;
        }
        
        public override int EnableRecvMagicVoice(bool enable){
            PrintLog("GCloudVoice_C# API: EnableRecvMagicVoice");
            
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            
            int ret = GCloudVoice_EnableRecvMagicVoice(enable);

            string logMsg = string.Format("EnableRecvMagicVoice enable: {0} ret: {1}", enable, ret);
            PrintLog(logMsg);
            return ret;
        }

	public override int SetPlayerInfoAbroad(string []cszOpenID, int []nMemberID,SpeechLanguageType []pLang, int nCount)
	{
		if(!bInit)
		{
			return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
		}
            int[] nTargetLangs = Array.ConvertAll(pLang, value => (int)value);
            return GCloudVoice_SetPlayerInfoAbroad(cszOpenID,nMemberID, nTargetLangs, nCount);
	}


		public override int TextToStreamSpeechStart(string text, string voiceType, int nTimeoutMS, string filePath = "")
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
	
			return GCloudVoice_TextToStreamSpeechStart(text, voiceType, nTimeoutMS, filePath);
		}

		public override int TextToStreamSpeechStop()
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_TextToStreamSpeechStop();
		}
		
        public override int RoomGeneralDataChannel(string roomName, string content){
            PrintLog("GCloudVoice_C# API: RoomGeneralDataChannel");
            
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            
            if (String.IsNullOrEmpty(roomName))
            {
                PrintLog("The parameter roomName is Null or empty.");
                return (int)GCloudVoiceErr.GCLOUD_VOICE_PARAM_NULL;
            }

            int ret = GCloudVoice_RoomGeneralDataChannel(roomName, content);

            string logMsg = string.Format("RoomGeneralDataChannel roomName: {0} content: {1}" +
                                          "ret: {2}", roomName, content, ret);
            PrintLog(logMsg);
            return ret;
        }
		public override int APITrace(string api, string callTrace){
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			
			if (String.IsNullOrEmpty(api) || String.IsNullOrEmpty(callTrace))
            {
                PrintLog("The parameter is Null or empty.");
                return (int)GCloudVoiceErr.GCLOUD_VOICE_PARAM_NULL;
            }
			
			return GCloudVoice_APITrace(api, callTrace);
		}
		public override int SetMagicVoiceMsgType(string magicType)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
	
			return GCloudVoice_SetMagicVoiceMsgType(magicType);
		}

		public override int SetTransSecInfo(string secInfo)
		{
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			return GCloudVoice_SetTransSecInfo(secInfo);
		} 

		public override int EnableReportALL(bool bEnable)
		{
			PrintLog("GCloudVoice_C# API: EnableReportALL");
	
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
	
			int nRet = GCloudVoice_EnableReportALL(bEnable);
            PrintLog("GCloudVoice_C# API: GCloudVoice_EnableReportALL nRet = "+ nRet);
			return nRet;
		}
		public override int EnableReportALLAbroad(bool bEnable)
		{
			PrintLog("GCloudVoice_C# API: EnableReportALLAbroad");
	
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
	
			int nRet = GCloudVoice_EnableReportALLAbroad(bEnable);
            PrintLog("GCloudVoice_C# API: GCloudVoice_EnableReportALLAbroad nRet = "+ nRet);
			return nRet;
		}

		public override int EnableCivilFile(bool bEnable)
        {
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            return GCloudVoice_EnableCivilFile(bEnable);
        }

		
		public override int Enable3DVoice(bool bEnable)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_Enable3DVoice(bEnable);
		}


		public override int EnableDualLink(bool enable)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_EnableDualLink(enable);
		}

		public override int Set3DPosition(GVoice3DVector pos)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_Set3DPosition(pos.x, pos.y, pos.z);
		}
		public override int Set3DForward(GVoice3DVector forward)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_Set3DForward(forward.x, forward.y, forward.z);
		}
		public override int Set3DUpward(GVoice3DVector upward)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_Set3DUpward(upward.x, upward.y, upward.z);
		}
		public override int Set3DDistProperties(GVoice3DDistProperties g3dProperties)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_Set3DDistProperties((int)(g3dProperties.AttenuationModel), g3dProperties.MinDistance, g3dProperties.MaxDistance, g3dProperties.Rolloff);
		}
		public override int AuditionFileForMagicType(string filepath,string magicType)
		{
			if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
			return GCloudVoice_AuditionFileForMagicType(filepath, magicType);
		}

	public override int IsSaveMagicFile(bool bSave)
	{
		if (!bInit)
		{
			return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
		}
		return GCloudVoice_IsSaveMagicFile(bSave);
	}
    
    public override int SetServerUrl(GVoiceUrlType urlType, string url){
        PrintLog("GCloudVoice_C# API: SetServerUrl");
    
        if (!bInit)
        {
            return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
        }

        int nRet = GCloudVoice_SetServerUrl((int)urlType, url);
        PrintLog("GCloudVoice_C# API: GCloudVoice_SetServerUrl nRet = "+ nRet);
        return nRet;
    }
	
		public override int SetSoundTag(GCloudVoiceSoundTag vtype, float[]  pParam)
	{
		PrintLog("GCloudVoice_C# API: SetSoundTag");		
		if (!bInit)
		{
			return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
		}
		int nRet = 0;
		if(pParam == null || pParam.Length == 0)
		{
			nRet = GCloudVoice_SetSoundTag((int)vtype, null, 0);
		}else
		{
			nRet = GCloudVoice_SetSoundTag((int)vtype, pParam, pParam.Length);
		}
		return nRet;	
	}
		
    }
}//end namespace
