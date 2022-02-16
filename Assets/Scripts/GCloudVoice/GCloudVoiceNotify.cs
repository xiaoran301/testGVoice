using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Text;


namespace gcloud_voice
{

 public abstract class IGCloudVoiceNotify{

     public class WXMemberInfo {
         public int memberID;
         public string openID;
         public string info;
     }
	
    /*************************************************************
     *                  Real-Time Voice Callbacks
     *************************************************************/
    /// <summary>
    /// Callback after you called JoinXxxRoom, you can get the result of JoinXxxRoom from the parameters.
    /// </summary>
    /// 
    /// <param name="code">A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.</param>
    /// <param name="roomName">Name of the room which you joined, it is the one you set in JoinXxxRoom method.</param>
    /// <param name="memberID">If success, returns the player's ID in this room.</param>
    /// <see cref="JoinTeamRoom"/>
    /// <see cref="JoinNationalRoom"/>
    /// <see cref="JoinRangeRoom"/>
    /// <see cref="GCloudVoiceCompleteCode"/>
    public delegate void JoinRoomCompleteHandler(GCloudVoiceCompleteCode code, string roomName, int memberID);
    
    /// <summary>
    /// Callback when someone in the same room changes saying status, such as begining saying from silence or stopping saying.
    /// </summary>
    ///
    /// <param name="members">An int array composed of [memberid_0, status, memberid_1, status ... memberid_2*count, status],
    /// here, status could be 0, 1, 2. 0 means being silence from saying, 1 means begining saying from silence
    /// and 2 means continue saying.</param>
    /// <param name="count">The count of members who's status has changed.</param>
    public delegate void MemberVoiceHandler(int[] members, int count);
    
    /// <summary>
    /// Callback when someone in the same room changes saying status, such as begining saying from silence or stopping saying.
    /// </summary>
    ///
    /// <param name="roomName">Name of the room which you joined.</param>
    /// <param name="member">The ID of the member who's status has changed.</param>
    /// <param name="status">Status could be 0, 1, 2. 0 means being silence from saying, 1 means begining saying from silence
    /// and 2 means continue saying.</param>
    /// <see cref="GCloudVoiceCompleteCode"/>
    public delegate void RoomMemberVoiceHandler(string roomName, int member, int status);
    
    /// <summary>
    /// Callback after you called ChangeRole, you can get the result of ChangeRole from the parameters.
    /// </summary>
    /// <param name="code">A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.</param>
    /// <param name="roomName">Name of the room which the member joined.</param>
    /// <param name="memberID">The ID of the member who changed role.</param>
    /// <param name="role">Current role of the member, Anchor or Audience.</param>
    /// <see cref="ChangeRole"/>
    /// <see cref="GCloudVoiceCompleteCode"/>
    public delegate void ChangeRoleCompleteHandler(GCloudVoiceCompleteCode code, string roomName, int memberID, int role);
    
    /// <summary>
    /// Callback when dropped from the room. When a member be offline more than 1min, he will be dropped from the room.
    /// </summary>
    ///
    /// <param name="code">A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.</param>
    /// <param name="roomName">Name of the room which the member joined.</param>
    /// <param name="memberID">If success, return the ID of the mermber who has been dropped from the room.</param>
    /// <see cref="GCloudVoiceCompleteCode"/>
    public delegate void StatusUpdateHandler(GCloudVoiceCompleteCode code, string roomName, int memberID);
    
    /// <summary>
    /// Callback after you called QuitRoom, you can get the result of QuitRoom from the parameters.
    /// </summary>
    ///
    /// <param name="code">A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.</param>
    /// <param name="roomName">Name of the room which you quited.</param>
    /// <param name="memberID">If success, return the ID of the mermber who has quitted from the room.</param>
    /// <see cref="QuitRoom"/>
    /// <see cref="GCloudVoiceCompleteCode"/>
    public delegate void QuitRoomCompleteHandler(GCloudVoiceCompleteCode code, string roomName, int memberID);

        /// <summary>
        /// Callback when some room members in or out room
        /// </summary>
        ///
        /// <param name="code">A GCloudVoiceCompleteCode code. for member had join or quit room .</param>
        /// <param name="roomName">Name of the room which members changed.</param>
        /// <param name="memid">the changed members memberid.</param>
        /// <param name="openID">the changed members openid.</param>
        /// <see cref="GCloudVoiceCompleteCode"/>
        public delegate void RoomMemberChangedCompleteHandler(GCloudVoiceCompleteCode code, string roomName, int memid, string openID);
    
    
    /*************************************************************
     *                  Voice Messages Callbacks
     *************************************************************/
    /// <summary>
    /// Callback after you called ApplyMessageKey, you can get the result of ApplyMessageKey from the parameters.
    /// </summary>
    ///
    /// <param name="code">A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.</param>
    /// <see cref="ApplyMessageKey"/>
    /// <see cref="GCloudVoiceCompleteCode"/>
    public delegate void ApplyMessageKeyCompleteHandler(GCloudVoiceCompleteCode code);
    
    /// <summary>
    /// Callback after you called UploadRecordedFile, you can get the result of UploadRecordedFile from the parameters.
    /// </summary>
    ///
    /// <param name="code">A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.</param>
    /// <param name="filepath">The path of the voice file uploaded.</param>
    /// <param name="fileid">If success, return the ID of the file.</param>
    /// <see cref="UploadRecordedFile"/>
    /// <see cref="GCloudVoiceCompleteCode"/>
    public delegate void UploadReccordFileCompleteHandler(GCloudVoiceCompleteCode code, string filepath, string fileid);
    
    /// <summary>
    /// Callback after you called DownloadRecordedFile, you can get the result of DownloadRecordedFile from the parameters.
    /// </summary>
    ///
    /// <param name="code">A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.</param>
    /// <param name="filepath">The path of the file which the voice download to.</param>
    /// <param name="fileid">If success,return the ID of the file.</param>
    /// <see cref="DownloadRecordedFile"/>
    /// <see cref="GCloudVoiceCompleteCode"/>
    public delegate void DownloadRecordFileCompleteHandler(GCloudVoiceCompleteCode code, string filepath, string fileid);
    
    /// <summary>
    /// Callback after you called PlayRecordedFile and the voice file has been played to the end, you can get the result of PlayRecordedFile from the parameters.
    /// </summary>
    ///
    /// <param name="code">A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.</param>
    /// <param name="filepath">The path of the file which had been played.</param>
    /// <see cref="PlayRecordedFile"/>
    /// <see cref="GCloudVoiceCompleteCode"/>
    public delegate void PlayRecordFilCompleteHandler(GCloudVoiceCompleteCode code, string filepath);
    
    
    /*************************************************************
     *                  Translation Callbacks
     *************************************************************/
    /// <summary>
    /// Callback after you called SpeechToText, you can get the result of SpeechToText from the parameters.
    /// </summary>
    ///
    /// <param name="code">A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.</param>
    /// <param name="fileID">The ID of the file which had been translated.</param>
    /// <param name="result">If success, return the translation result, which is a piece of text in a specific language.</param>
    /// <see cref="SpeechToText"/>
    /// <see cref="GCloudVoiceCompleteCode"/>
    public delegate void SpeechToTextHandler(GCloudVoiceCompleteCode code, string fileID, string result);
    
    /// <summary>
    /// Callback after you called StopRecording in RSTT mode, you can get the result of stream speech to text from the parameters.
    /// </summary>
    ///
    /// <param name="code">A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.</param>
    /// <param name="error">An error code for internal use, you can ignore it.</param>
    /// <param name="result">If success, return the translation result, which is a piece of text in a specific language.</param>
    /// <param name="voicePath">The path of the voice file.</param>
    /// <see cref="GCloudVoiceCompleteCode"/>
    public delegate void StreamSpeechToTextHandler(GCloudVoiceCompleteCode code, int error, string result, string voicePath);
    
    /// <summary>
    /// Event Callback. e.g. the device connect Event, the device disconcet Event
    /// </summary>
    ///
    /// <param name="code">A event code.</param>
    /// <param name="info">The event info.</param>
    /// <see cref="GCloudVoiceEvent"/>
    public delegate void EventUpdateHandler(GCloudVoiceEvent code, string info) ;
    
    /// <summary>
    /// Callback after you called CheckDeviceMuteState, you can get the result of CheckDeviceMuteState from the parameters.
    /// </summary>
    /// 
    /// <param name="result">Mute state flag. Non-zero means mute state.</param>
    /// <see cref="GCloudVoiceCompleteCode"/>
    public delegate void MuteSwitchResultHandler(int result) ;
	
    /// <summary>
    /// callback function @see ReportPlayer
    /// </summary>
    /// <param name="nCode">the reported result, 0 means server receive your reporter succ</param>
    /// <param name="strInfo">the information send to server, json string description, jwt
    /// <returns></returns>
    public delegate void ReportPlayerHandler(GCloudVoiceCompleteCode nCode, string strInfo);

    /// <summary>
    /// For LGame save recdata and callback the fileid and fielindex
    /// </summary>
    /// <param name="code">a GCloudVoiceCompleteCode code . You should check this first.</param>
    /// <param name="fileid">fileid of cdn server for file download</param>
    /// <param name="fileindex">the rec file of index for this video</param>
    /// <returns></returns>
    public delegate void SaveRecFileIndexHandler(GCloudVoiceCompleteCode code, string fileid, int fileindex) ;    
    
	/// @brief Callback function for speech translate
	///
	/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param srcText, text that the source speech file translate to.
	/// @param targetText, target text that translated from source text.
	/// @param targetFileID, ID of the target speech file.
	/// @param srcFileDuration, duration of the source speech file, the unit is milliseconds.
    public delegate void SpeechTranslateHandler(GCloudVoiceCompleteCode nCode, string srcText, string targetText, string targetFileID, int srcFileDuration);

	/// @brief Callback function for speech translate
	///
	/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param srcText, text that the source speech file translate to.
	/// @param targetText, target text that translated from source text.
	/// @param filePath, target speech file.
	/// @param srcFileDuration, duration of the source speech file, the unit is milliseconds.
    public delegate void SpeechFileTranslateHandler(GCloudVoiceCompleteCode nCode, string srcText, string targetText, string filePath, int srcFileDuration);
	
	/// @brief Callback function for speech translate
	///
	/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param srcText, text that the source speech file translate to.
	/// @param targetText, target text that translated from source text.
	/// @param srcFileDuration, duration of the source speech file, the unit is milliseconds.
	public delegate void SpeechFileToTextHandler(GCloudVoiceCompleteCode nCode, string srcText, string targetText, int srcFileDuration);
	
	/// @brief Callback function for tts
	///
	/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param text, source text, from request.
	/// @param lang, source text language.
	/// @param fileID, ID of the target speech file.
	public delegate void TextToSpeechHandler(GCloudVoiceCompleteCode nCode, string text, SpeechLanguageType lang, string fileID);
		
	/// @brief Callback function for TextToSpeech
	///
	/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param text, source text, from request.
	/// @param lang, source text language.
	/// @param fileID, ID of the target speech file.
	public delegate void TextToSpeechFileHandler(GCloudVoiceCompleteCode nCode, string text, SpeechLanguageType lang, string filePath);
	
	/// @brief Callback function for stream tts
	///
	/// @param nCode, GV_ON_OK when TTS succeed, else GV_ON_FAIL.
	/// @param text, source text, from request.
	/// @param error code, defail of fail.
	public delegate void TextToStreamSpeechHandler(GCloudVoiceCompleteCode nCode, string text, int err);
		
	/// @brief Callback function for ttt
	///
	/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param srcLang, speech language associated with fileID.
	/// @param srcText, text that the source speech file translate to.
	/// @param targetLang, target language that we want to translate to.
	/// @param targetText, text that the source text translate to.
    public delegate void TextTranslateHandler(GCloudVoiceCompleteCode nCode, SpeechLanguageType srcLang, string srcText, SpeechLanguageType targetLang, string targetText);
	
    /// @brief Callback function for speech QueryUserInfo
    ///
    /// @param code, this operation's result @enum GCloudVoiceCompleteCode.
    /// @param roomName, name of the room .
    /// @param member, member to query.
    public delegate void QueryUserInfoHandler(GCloudVoiceCompleteCode code, string roomName, WXMemberInfo member);

    /// @brief Callback function for  WXMemberVoiceHandler
    ///
    /// @param members,An int array composed of [memberid_0, status, memberid_1, status ... memberid_2*count, status],
    /// here, status could be 0, 1, 2. 0 means being silence from saying, 1 means begining saying from silence
    /// and 2 means continue saying.
    /// @param roomName, name of the room .
    public delegate void WXMemberVoiceHandler(string roomName, int []members, int count);
        
    /// @brief Callback function for speech QueryUserInfo
    ///
    /// @param code, this operation's result @enum GCloudVoiceCompleteCode.
    /// @param roomName, name of the room .
    /// @param members, all members' info.
    public delegate void QueryWXMembersHandler(GCloudVoiceCompleteCode code, string roomName, WXMemberInfo []members);

    /// @brief Callback function for UploadUserInfo
    ///
    /// @param code, this operation's result @enum GCloudVoiceCompleteCode.
    /// @param roomName, name of the room .
    /// @param memberID, memberID to query.
    public delegate void UpdateUserInfoHandler(GCloudVoiceCompleteCode code, string roomName, int memberID);    

    /// @brief Callback function for UpdateMicLevelHandler
    /// @param level  mic data amp
	public delegate void UpdateMicLevelHandler(int level);	
	
	/// @brief Callback function for speech translate
	///
	/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param srcLang, the speech language of the recorder.
	/// @param targetLang, target language that we want to translate to.
	/// @param srcText, text that the source speech file translate to.
	/// @param targetText, target text that translated from source text.
	/// @param targetFileID, ID of the target speech file.
	/// @param srcFileDuration, duration of the source speech file, the unit is milliseconds.
    public delegate void RSTSHandler(GCloudVoiceCompleteCode nCode, SpeechLanguageType srcLang, SpeechLanguageType targetLang, string srcText, string targetText, string targetFileID, int srcFileDuration);

	/// @brief Callback function for RSTSSpeechToSpeech
	///
	/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param srcLang, speech language associated with fileID.
	/// @param targetLang, target language that we want to translate to.
	/// @param srcText, text that the source speech file translate to.
	/// @param targetText, target text that translated from source text.
	/// @param filePath, target speech file.
	/// @param srcFileDuration, duration of the source speech file, the unit is milliseconds.
	public delegate void RSTSSpeechToSpeechHandler(GCloudVoiceCompleteCode nCode, SpeechLanguageType srcLang, SpeechLanguageType targetLang, string srcText, string targetText, string filePath, int srcFileDuration);

	/// @brief Callback function for RSTSSpeechToText
	///
	/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param srcLang, speech language.
	/// @param targetLang, target language that we want to translate to.
	/// @param srcText, text that the source speech file translate to.
	/// @param targetText, target text that translated from source text.
	/// @param srcFileDuration, duration of the source speech file, the unit is milliseconds.
	public delegate void RSTSSpeechToTextHandler(GCloudVoiceCompleteCode nCode, SpeechLanguageType srcLang, SpeechLanguageType targetLang, string srcText, string targetText, int srcFileDuration);
	
	/// @brief Callback function for EnableTranslate
	///
	/// @param code, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param roomName, name of the room .
	/// @param transType, refer to EnableTranslate.
    public delegate void EnableTranslateHandler(GCloudVoiceCompleteCode code, string roomName, RealTimeTranslateType transType);
    
	/// @brief After call EnableTranslate function to enable realtime voice STT, we can get realtime translated text by this callback
	///
	/// @param sessionID, ID of the Session for one sentence.
	/// @param seq, The sequence of each segment in a sentence
	/// @param roomID, ID of the room which a player joined in.
	/// @param memberID, the player's ID in this room.
	/// @param text, the result of realtime voice stt, utf-8.
	public delegate void RealTimeTranslateTextHandler(string roomName, int memberID, string sessionID, int seq, string text);
		
    /// <summary>
    /// @brief Callback method of EnableMagicVoice.
    /// @see EnableMagicVoice
    /// </summary>
    ///
    /// <param name="code">you should check this first the get the result of successful or not</param>
    /// <param name="magicType">MagicVoice type the player want to translate to</param>
    /// <param name="enable">true for enable MagicVoice function, and false for disable MagicVoice function</param>
    /// <see cref="GCloudVoiceCompleteCode"/>
    public delegate void EnableMagicVoiceHandler(GCloudVoiceCompleteCode code, string magicType, bool enable);
    
    /**
    * Callback method of EnableRecvMagicVoice.
    * @see EnableRecvMagicVoice
    *
    * @param code: A GCloudVoiceCompleteCode code. You should check this first the get the result of successful or not.
    * @param enable: true for enable RecvMagicVoice function, and false for disable RecvMagicVoice function
    * @see GCloudVoiceCompleteCode
    */
    public delegate void EnableRecvMagicVoiceHandler(GCloudVoiceCompleteCode code, bool enable);

	/// @brief Callback function for report speech translate
	///
	/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param transText, translation text that translated from report speech.
	/// @param openID, ID of the reported player's openid.
	public delegate  void	STTReportHandler(GCloudVoiceCompleteCode nCode,string transText, string openID, string fileid);


	/// @brief Callback method of magic voice message function.
	///
	/// @param nCode, this operation's result @enum GCloudVoiceCompleteCode.
	/// @param filePath, magic voice file.
	public delegate  void MagicVoiceMsgHandler(GCloudVoiceCompleteCode nCode,string filePath);

	/// @brief Callback function for Realtime security of voice punish
	///
	/// @param nRet, the value from security department, >0 suggest forbid player talk ability
	/// @param roomName, the roomName
    /// @param secInfo, the string from security.
    public delegate void RTSecInfoHandler(int nRet, string roomName, string secInfo);

    /// @brief Callback function for recognition info to gameclient forbide/punish user voice ablitiy
    ///
    /// @param roomname, roomname
    /// @param info, the info from security department.
    public delegate void AIRecognitionHandler(string roomName, string secInfo);


    
    public abstract event JoinRoomCompleteHandler            OnJoinRoomComplete;
    public abstract event MemberVoiceHandler                 OnMemberVoice;
    public abstract event RoomMemberVoiceHandler             OnRoomMemberVoice;
    public abstract event ChangeRoleCompleteHandler          OnRoleChangeComplete;
    public abstract event StatusUpdateHandler                OnStatusUpdate;
    public abstract event QuitRoomCompleteHandler            OnQuitRoomComplete;
    public abstract event ApplyMessageKeyCompleteHandler     OnApplyMessageKeyComplete;
    public abstract event UploadReccordFileCompleteHandler   OnUploadReccordFileComplete;
    public abstract event DownloadRecordFileCompleteHandler  OnDownloadRecordFileComplete;
    public abstract event PlayRecordFilCompleteHandler       OnPlayRecordFilComplete;
    public abstract event SpeechToTextHandler                OnSpeechToText;
    public abstract event StreamSpeechToTextHandler          OnStreamSpeechToText;
    public abstract event EventUpdateHandler                 OnEvent;
    public abstract event MuteSwitchResultHandler            OnMuteSwitchState;
    public abstract event ReportPlayerHandler                OnReportPlayer;
    public abstract event SaveRecFileIndexHandler            OnSaveRecFileIndex;
	public abstract event RoomMemberChangedCompleteHandler   OnRoomMemberInfo;
	public abstract event SpeechTranslateHandler             OnSpeechTranslate;
	public abstract event SpeechFileTranslateHandler         OnSpeechFileTranslate;
	public abstract event SpeechFileToTextHandler            OnSpeechFileToText;
	public abstract event TextToSpeechHandler				 OnTextToSpeech;
	public abstract event TextToSpeechFileHandler            OnTextToSpeechFile;
	public abstract event TextToStreamSpeechHandler			 OnTextToStreamSpeech;
	public abstract event TextTranslateHandler				 OnTextTranslate;
	public abstract event RSTSHandler                        OnRSTS;
	public abstract event RSTSSpeechToSpeechHandler          OnRSTSSpeechToSpeech;
	public abstract event RSTSSpeechToTextHandler            OnRSTSSpeechToText;
	public abstract event EnableTranslateHandler             OnEnableTranslate;
	public abstract event RealTimeTranslateTextHandler       OnRealTimeTranslateText;
    public abstract event QueryUserInfoHandler OnQueryUserInfo;
    public abstract event WXMemberVoiceHandler OnWXMemberVoice;
    public abstract event QueryWXMembersHandler OnQueryWXMembers;
    public abstract event UpdateUserInfoHandler OnUpdateUserInfo;
	public abstract event UpdateMicLevelHandler OnUpdateMicLevel;
    public abstract event EnableMagicVoiceHandler OnEnableMagicVoice;
    public abstract event EnableRecvMagicVoiceHandler OnEnableRecvMagicVoice;
	public abstract event STTReportHandler       OnSTTReport;
	public abstract event MagicVoiceMsgHandler	OnMagicVoiceMsg;
    public abstract event RTSecInfoHandler OnRTSecInfo;
    public abstract event AIRecognitionHandler OnAIRecognition;
 }
}//end namespace
