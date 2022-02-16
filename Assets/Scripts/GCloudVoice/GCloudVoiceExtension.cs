 ///
 /// GVoice(Game Voice) is a voice service that covers diverse game scenes.
 ///
 /// This file includes the extension APIs in GVoice SDK, which supplies the additional functions
 /// in GVoice, such as playing background music, setting the voice effect mode and so on.
 ///
 

#if UNITY_IOS
#define UNITY_IPHONE
#endif

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;


namespace gcloud_voice
{
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi,Pack = 1)]
public struct RoomMembers
{
	public int memberid;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string openid;
	public int micstatus;
}
public abstract class IGCloudVoiceExtension : IGCloudVoiceNotify
{
	/**
	* Join in a team room.
	* Team room function allows no more than 20 members join in the same room to communicate freely.
	*
	* e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->JoinTeamRoom
	* -->.....-->QuitRoom
	*
	* The result of joining room successful or not can be obtained by the callback method OnJoinRoom.
	* @see OnJoinRoom
	*
	* @param scenesName: The scene of entering the room, Used to classify rooms, 
	*	one scene can only enter one room, will quit the previous room in the same scene automatically, 
	*	one room can only be associated with one scene, will fail to join room with anothor scenes,
	*	limit 127 bytes.
	* @param roomName: The name of The room to join, it should be a string composed by 0-9A-Za-Z._- and less than 127 bytes.
	* @param msTimeout: The length of the timeout for joining, it is a micro second, value range[5000, 60000].
	* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
	* @see GCloudVoiceErrno
	*/
	public abstract int JoinTeamRoom_Scenes(string scenesName, string roomName, int msTimeout);

	/**
	* Join in a LBS room.
	* RangeRoom function allows user to join a LBS room.
	* After joined a RangeRoom, the member can hear the members' voice within a specific range.
	*
	* e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->JoinRangeRoom
	* -->.....-->QuitRoom
	*
	* The result of joining room successful or not can be obtained by the callback method OnJoinRoom.
	* @see OnJoinRoom
	*
	* @param scenesName: The scene of entering the room, Used to classify rooms, 
	*	one scene can only enter one room, will quit the previous room in the same scene automatically, 
	*	one room can only be associated with one scene, will fail to join room with anothor scenes,
	*	limit 127 bytes.
	* @param roomName: The name of The room to join, it should be composed by 0-9A-Za-Z._- and less than 127 bytes.
	* @param msTimeout: The length of the timeout for joining, it is a micro second, value range[5000, 60000].
	* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
	* @see GCloudVoiceErrno
	*/
	public abstract int JoinRangeRoom_Scenes(string scenesName, string roomName, int msTimeout);

	/**
	* Join in a national room.
	* National room function allows more 20 members to join in the same room, and they can choose two different roles to be.
	* The Anchor role can open microphone to speak and open speaker to listen.
	* The Audience role can only open the speaker to listen.
	*
	* e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->JoinNationalRoom
	* -->.....-->QuitRoom
	*
	* The result of joining room successful or not can be obtained by the callback method OnJoinRoom.
	* @see OnJoinRoom
	*
	* @param scenesName: The scene of entering the room, Used to classify rooms, 
	*	one scene can only enter one room, will quit the previous room in the same scene automatically, 
	*	one room can only be associated with one scene, will fail to join room with anothor scenes,
	*	limit 127 bytes.
	* @param roomName: The name of The room to join, it should be composed by 0-9A-Za-Z._- and less than 127 bytes.
	* @param role: A GCloudVoiceMemberRole value illustrates wheather the player can send voice data or not.
	* @param msTimeout: The length of the timeout for joining, it is a micro second, value range[5000, 60000].
	* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
	* @see GCloudVoiceErrno
	*/
	public abstract int JoinNationalRoom_Scenes(string scenesName, string roomName, GCloudVoiceRole role, int msTimeout);


	/**
	* Quit the voice room.
	*
	* QuitRoom method should be called after the member has joined a voice room successfully.
	* e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->JoinXxxRoom
	* -->.....-->QuitRoom
	*
	* The result of quiting room successful or not can be obtained by the callback method OnQuitRoom
	* @see OnQuitRoom
	*
	* @param scenesName: The scene of entering the room
	* @param msTimeout: The length of the timeout for quiting, it is a micro second, value range[5000, 60000].
	* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
	* @see GCloudVoiceErrno
	*/
	public abstract int QuitRoom_Scenes(string scenesName, int msTimeout);
		
    /*************************************************************************
     *                  Multiroom related APIs
     *
     * Multiroom allows a member to join
     * 1~16 room(s) at the same time.
     *
     * The workflow of the Multiroom function:
     * GetEngine-->SetAppInfo-->Init-->Poll-->
     * EnableMultiRoom-->JoinTeamRoom/JoinRangeRoom-->EnableRoomMicrophone/
     * EnableRoomSpeaker-->...-->QuitRoom
     *************************************************************************/
    /// <summary>
    /// Enable a member to join in multi rooms. Notice that this may cause higher bitrate.
    ///
    /// and before you call the JoinXxxRoom method.
    /// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->EnableMultiRoom
    /// -->JoinXxxRoom-->.....-->QuitRoom
    /// </summary>
    ///
    /// <param name="enable">Enable joining in multi rooms if it is ture, and disable joining in multi rooms if it is false.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int EnableMultiRoom(bool enable);
    
    /// <summary>
    /// Open or close the microphone in a specific room in MultiRoom mode.
    ///
    /// EnableRoomMicrophone method should be called after the member has joined a voice room in MultiRoom mode successfully.
    /// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->EnableMultiRoom(true)
    /// -->JoinXxxRoom-->EnableRoomMicrophone-->.....-->QuitRoom
    /// </summary>
    ///
    /// <param name="roomName">The name of The room to enable microphone, it should be an exist room name.</param>
    /// <param name="enable">Open the microphone in The room if it is true, close the microphone in The room if it is false.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int EnableRoomMicrophone(string roomName, bool enable);
    
    /// <summary>
    /// Open or Close the speaker in a specific room in MultiRoom mode.
    ///
    /// EnableRoomSpeaker method should be called after the member has joined a voice room in MultiRoom mode successfully.
    /// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->EnableMultiRoom(true)
    /// -->JoinXxxRoom-->EnableRoomMicrophone-->.....-->QuitRoom
    /// </summary>
    ///
    /// <param name="roomName">The name of The room to enable speaker, it should be an exist room name.</param>
    /// <param name="enable">Open the speaker in The room if it is true, close the speaker if it is false.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int EnableRoomSpeaker(string roomName, bool enable);
    
    
    /*************************************************************************
     *                  BGM related APIs
     *
     * GVoice supports mp3 format background music.
     * The workflow of the BGM function:
     * getInstance-->SetAppInfo-->Init-->EnableNativeBGMPlay-->SetBGMPath
     * -->StartBGMPlay-->PauseBGMPlay-->ResumeBGMPlay-->StopBGMPlay
     *************************************************************************/
	/// <summary>
	/// Set The path to a BGM file.
    /// SetBGMPath method should be called after you have initialized the voice engine.
	/// </summary>
    ///
    /// <param name="path">The path of the BGM file.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int SetBGMPath(string path);

	/// <summary>
	/// Enable or disable the native play mode.
    /// EnableNativeBGMPlay method should be called after you have initialized the voice engine.
	/// </summary>
    ///
    /// <param name="bEnable">Enable the native play mode if it is true, and disable the native play mode if it is false.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int EnableNativeBGMPlay(bool bEnable);

	/// <summary>
	/// Start playing the BGM.
    /// StartBGMPlay method should be called after you have set The path of the BGM file by SetBGMPath method.
	/// </summary>
    ///
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int StartBGMPlay();

	/// <summary>
	/// Set the play volume of the BGM.
    /// SetBGMVol method should be called after you have initialized the voice engine.
	/// </summary>
    ///
    /// <param name="vol">The play volume of the BGM, which should be an integer between 0~800.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int SetBGMVol(int vol);

	/// <summary>
	/// Pause the BGM.
    /// When you want to pause the playing of BGM or when the application paused, you can call PauseBGMPlay
    /// method to pause the BGM.
	/// </summary>
    ///
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int PauseBGMPlay();

	/// <summary>
	/// Resume the BGM.
    /// When you want to resume the playing of BGM after paused it or when the application resumed, you can call
    /// ResumeBGMPlay method to resume the BGM.
	/// </summary>
    ///
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int ResumeBGMPlay();

	/// <summary>
	/// Get the state of the BGM.
    /// If you want to get the playing state of the BGM, you can call GetBGMPlayState method.
    /// GetBGMPlayState method should be called after you have initialized the voice engine.
	/// </summary>
    ///
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int GetBGMPlayState();

	/// <summary>
	/// Stop the BGM.
    /// StopBGMPlay method should be called after you have initialized the voice engine.
	/// </summary>
    ///
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int StopBGMPlay();


	/*************************************************************
	 *                  Token related APIs
	 *************************************************************/
	/// <summary>
	/// Join in a team room with token.
    /// Team room function allows no more than 20 members join in the same room to communicate freely.
    ///
    /// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinTeamRoom_Token
    /// -->.....-->QuitRoom
    ///
    /// The result of joining room successful or not can be obtained by the event OnJoinRoom.
    /// <see cref="OnJoinRoom"/>
	/// </summary>
    ///
    /// <param name="roomName">The name of The room to join, it should be composed by 0-9A-Za-Z._- and less than 127 bytes.</param>
    /// <param name="token"></param>
    /// <param name="timestamp"></param>
    /// <param name="msTimeout">The length of the timeout for joining, it is a micro second, value range[5000, 60000].</param>
	/// <returns>return GCLOUD_VOICE_SUCC</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int JoinTeamRoom_Token(string roomName, string token, int timestamp, int msTimeout);

    /// <summary>
    /// Join in a national room with token.
    /// National room function allows more than 20 members to join in the same room, and they can choose two different roles to be.
    /// The Anchor role can open microphone to speak and open speaker to listen.
    /// The Audience role can only open the speaker to listen.
    ///
    /// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinNationalRoom_Token
    /// -->.....-->QuitRoom
    ///
    /// The result of joining room successful or not can be obtained by the event OnJoinRoom.
    /// <see cref="OnJoinRoom"/>
    /// </summary>
    ///
    /// <param name="roomName">The room to join, should be less than 127byte, composed by alpha.</param>
    /// <param name="token"></param>
    /// <param name="timestamp"></param>
    /// <param name="role">A GCloudVoiceMemberRole value illustrates wheather the player can send voice data or not.</param>
    /// <param name="msTimeout">The length of the timeout for joining, it is a micro second, value range[5000, 60000]</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int JoinNationalRoom_Token(string roomName, string token, int timestamp, GCloudVoiceRole role, int msTimeout);

    /// <summary>
    /// Apply the key for voice message with token.
    /// In Messages, Translation and RSTT mode, you should first apply the message key before you use the functions.
    ///
    /// ApplyMessageKey_Token method should be called after you have set the voice mode to Messages, Translation or RSTT.
    /// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->SetMode(Messages)-->ApplyMessageKey_Token-->...
    ///
    /// The result of applying message key successful or not can be obtained by the event OnApplyMessageKey.
    /// <see cref="OnApplyMessageKey"/>
    /// </summary>
    ///
    /// <param name="token"></param>
    /// <param name="timestamp"></param>
    /// <param name="msTimeout">The length of the timeout for applying, it is a micro second, value range[5000, 60000].</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
	public abstract int ApplyMessageKey_Token(string token, int timestamp, int msTimeout);

    /// <summary>
    /// Translate the voice data to a piece of text in a specific language with token, the default language is Chinese.
    ///
    /// SpeechToText_Token method should be called in Message mode, and after you have
    /// uploaded a voice message successfully.
    /// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->SetMode(Message)-->ApplyMessageKey
    /// -->StartRecording-->StopRecording-->SpeechToText_Token-->...
    ///
    /// The result of translating successful or not can be obtained by the event OnSpeechToText.
    /// <see cref="OnSpeechToText"/>
    /// </summary>
    ///
    /// <param name="fileID">The ID of the file to be translated. FileID can be obtained from the callback method OnUploadFile.</param>
    /// <param name="token"></param>
    /// <param name="timestamp"></param>
    /// <param name="language">The specific language to be translated to.</param>
    /// <param name="msTimeout">The length of the timeout for translating, it is a micro second, value range[5000, 60000].</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudLanguage"/>
    /// <see cref="GCloudVoiceErr"/>
	public abstract int SpeechToText_Token(string fileID, string token, int timestamp, int language = 0, int msTimeout = 6000);


	/*************************************************************
	 *                  Micphone or speaker related APIs
	 *************************************************************/
	/// <summary>
	/// Open or close the speaker.
    /// EnableSpeakerOn method should be called after you have initialized the voice engine.
	/// </summary>
    ///
    /// <param name="bEnable">Open the speaker if it is true and close the speaker if it is false.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int EnableSpeakerOn(bool bEnable);

	/// <summary>
	/// Set the volume of microphone.
    /// SetMicVolume method should be called after you have initialized the voice engine.
	/// </summary>
    ///
    /// <param name="vol">The volume to set, for windows platform, the vol should in -1000～1000,
    /// and in other platforms, the vol should in -150～150.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int SetMicVolume(int vol);

	/// <summary>
	/// Set the sepaker's volume.
    /// SetSpeakerVolume method should be called after you have initialized the voice engine.
	/// </summary>
    ///
    /// <param name="vol">The volume to set, for windows platform, the vol should in 0～100, and in other platforms,
    /// the vol should in 0～150, the real volume is equals to (the vol / 100 * the original voice volume).
    /// If you set the vol to 120, then the real vol is (1.2*the original voice volume).</param>
	/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int SetSpeakerVolume(int vol) ;

	/// <summary>
    /// Get the microphone's volume.
    /// GetMicLevel method should be called after you have initialized the voice engine.
    /// </summary>
    ///
    /// <returns>The microphone's volume, if return value>0, means you have said something captured by microphone.</returns>
    public abstract int GetMicLevel();

	/// <summary>
	/// Get the microphone's volume.
    /// GetMicLevel method should be called after you have initialized the voice engine.
	/// </summary>
    /// TODO
    /// <param name="bFadeOut"></param>
	/// <returns>The microphone's volume, if return value>0, means you have said something captured by microphone.</returns>
	public abstract int GetMicLevel(bool bFadeOut);

	/// <summary>
	/// Get the speaker's volume.
    /// GetSpeakerLevel method should be called after you have initialized the voice engine.
	/// </summary>
    ///
	/// <returns>The speaker's volume, the value is equal to the parameter when you call SetSpeakerVolume method.
    /// </returns>
	public abstract int GetSpeakerLevel();
    
    /// <summary>
    /// Get the microphone's state, open microphone success, failed or be occupied.
    /// </summary>
    ///
    /// <returns>The microphone's state. -1: microphone is closed; 0: open microphone failed;
    /// 1: open microphone success; 2: microphone has been occupied.
    /// </returns>
    public abstract int GetMicState();
    
    
    /// <summary>
    /// Get the speaker's state, open speaker success, failed or be occupied.
    /// </summary>
    ///
    /// <returns>The speaker's state. -1: speaker is closed; 0: open speaker failed;
    /// 1: open speaker success; 2: speaker has been occupied.
    /// </returns>
    public abstract int GetSpeakerState();

	/// <summary>
    /// Test wheather the microphone is available or not.
    /// Before you want to open microphone, call TestMic method to check whether the microphone is available or not.
    /// TestMic method should be called after you have initialized the voice engine.
    /// </summary>
    ///
    /// <returns>If microphone device is available, returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int TestMic() ;

	/// <summary>
	/// Detect whether the member is speaking or just keep microphone opened.
    /// IsSpeaking method should be called after you have initialized the voice engine.
	/// </summary>
    ///
	/// <returns>If the member is speaking, returns true, otherwise returns false.</returns>
	public abstract int IsSpeaking();

	/// <summary>
    /// Enable or disable the bluetooth SCO mode. When you want to capture the voice via bluetooth, you can call EnableBluetoothSCO(true).
    /// EnableBluetoothSCO method should be called after you have initialized the voice engine.
	/// </summary>
    ///
    /// <param name="enable">Enable the bluetooth SCO mode if it is true, and disable the bluetooth SCO mode if it is false.</param>
	public abstract void EnableBluetoothSCO(bool enable);

	/// <summary>
	/// Identify that whether there is any device connected or not.
	/// </summary>
    ///
    /// <returns>0: no audio device connected; 1: a wiredheadset device connected; 2: a bluetooth device connected.</returns>
    /// <see cref="GCloudVoiceDeviceState"/>
	public abstract int getAudioDeviceConnectionState();

	/// <summary>
	/// Check mute switch state; iPhone is valiable; iOS simulator and android will return non-mute.
	/// </summary>
    ///
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="event MuteSwitchResultHandler OnMuteSwitchState"/>
    /// <see cref="GCloudVoiceErr"/>
	public abstract int CheckDeviceMuteState();
    
    /// <summary>
    /// Get the mute state of the device.
    /// </summary>
    ///
    /// <returns> The device is muted or not. non-zero:mute state; 0: not in mute state; -1:error.
    /// </returns>
    public abstract int GetMuteResult();


	/*************************************************************
	 *                  Voice algorithm related APIs
	 *************************************************************/
	/// <summary>
    /// This method supports setting sound effect mode.
    /// SetVoiceEffects method should be called after you have initialized the voice engine.
    /// </summary>
    ///
    /// <param name="mode">The path of the BGM file.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
	public abstract int SetVoiceEffects(SoundEffects mode);

    /// <summary>
    /// This method supports enabling sound reverb function.
    /// EnableReverb method should be called after you have initialized the voice engine.
    /// </summary>
    ///
    /// <param name="bEnable">Enable the sound reverb if it is true, and disable the sound reverb if it is false.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int EnableReverb(bool bEnable);

	/// <summary>
    /// This method supports setting sound reverb mode.
    /// SetReverbMode method should be called after you have initialized the voice engine.
    /// </summary>
    ///
    /// <param name="mode">The reverb mode which you want to set, the value should in 0~5, and default is 0.
    /// 0: strong vocal; 1: vocal; 2: small room; 3: large room; 4: church; 5: theater</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
	public abstract int SetReverbMode(int mode);

	/// <summary>
    /// Identify the type of the voice.
    /// GetVoiceIdentify method should be called after you have initialized the voice engine.
    /// </summary>
    ///
    /// <returns>0: boy's sound; 1: girl's sound; 2: non human sound; -1: error.</returns>
	public abstract int GetVoiceIdentify();

	/*************************************************************
	 *                  Other APIs
	 *************************************************************/
    /// <summary>
    /// Set the server's address, only needed for games which published in foreign contries, such as Korea, Europe...
    ///
    /// </summary>
    ///
    /// <param name="URL">url of server, you can get the url from gcloud console after you have registered.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int SetServerInfo(string URL, string defaultipsvr ="");
    
	/// <summary>
    /// Set the bit rate of the voice code.
    /// When you want to change the voice's bit rate, you can call this method.
    /// SetBitRate method should be called after you have initialized the voice engine.
    /// </summary>
    ///
    /// <param name="bitrate">The bit rate you want to set, it should be an integer between 8~256K.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
	public abstract int SetBitRate(int bitrate);

    /// <summary>
    /// Set if it is datafree.
    /// SetDataFree method should be called after you have initialized the voice engine.
    /// </summary>
    ///
    /// <param name="enable">Enable datafree if it is true, and disable datafree if it is false.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int SetDataFree(bool enable);

	/// <summary>
    /// Open Voice Engine's logcat.
    /// EnableLog method should be called after you have initialized the voice engine.
	/// </summary>
    ///
	/// <param name="enable">Open logcat if it is true, and disable logcate if it is false.</param>
	/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int EnableLog(bool enable);
    
    
    /// <summary>
    /// Set the audience list who can hear, that is, members not in this list can not hear the voice from the members in the same room.
    /// </summary>
    ///
    /// <param name="audience">The IDs of the members who can hear the voice.</param>
    /// <param name="roomName">The room to set the audience list.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    /// </summary>
    public abstract int SetAudience(int []audience, string roomName ="" );
    
    /// <summary>
    /// Don't play the member's voice.
    ///
    /// ForbidMemberVoice method should be called after the member has joined a voice room successfully.
    /// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinXxxRoom
    /// -->ForbidMemberVoice-->.....-->QuitRoom
    /// </summary>
    ///
    /// <param name="member">The ID of the member who you want to forbid his voice.</param>
    /// <param name="bEnable">Forbid the member's voice if it is true, and listen the member's voice if it is false.</param>
    /// <param name="roomName">The name of The room to forbid member's voice, it should be an exist room name.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int ForbidMemberVoice(int member, bool bEnable, string roomName="");
    
    #if UNITY_IPHONE
    /// <summary>
    /// Open the player's microphone and record the player's voice.
    ///
    /// StartRecording method should be called in Messages or RSTT mode, and after you have
    /// applied the message key successfully.
    /// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->SetMode(Messages)-->ApplyMessageKey
    /// -->StartRecording-->...
    /// </summary>
    ///
    /// <param name="filePath">The path of the file to store the voice data, the filePath should like:"your_dir/your_file_name".</param>
    /// <param name="bOptimized">Optimization flag, it means record audio not enter voip mode if it is true, and it means appears the same as before if it is false.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int StartRecording(string filePath, bool bOptimized);
    #endif
    
    #region overload
    /// <summary>
    /// Upload the player's voice message file to the network.
    /// It is not recommended to call this method.
    ///
    /// UploadRecordedFile method should be called after you have
    /// recorded a voice message successfully.
    /// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->SetMode(Messages)-->ApplyMessageKey
    /// -->StartRecording-->StopRecording-->UploadRecordedFile-->...
    ///
    /// The result of uploading recorded file successful or not can be obtained by the event OnUploadFile.
    /// <see cref="OnUploadFile"/>
    /// </summary>
    ///
    /// <param name="filePath">The path of the voice file to upload, the filePath should like:"your_dir/your_file_name".</param>
    /// <param name="msTimeout">The length of the timeout for uploading, it is a micro second, value range[5000, 60000].</param>
    /// <param name="bPermanent">if set true, server will never delete upload-file but limit the NO. of uploads, if set false, upload-file will keep 7 days and not limited.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int UploadRecordedFile(string filePath, int msTimeout, bool bPermanent);
    
    /// <summary>
    /// Download other players' voice message from the network.
    /// It is not recommended to call this method.
    ///
    /// DownloadRecordedFile method should be called after the other member has
    /// uploaded a voice message successfully.
    /// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->SetMode(Messages)-->ApplyMessageKey
    /// -->...-->DownloadRecordedFile-->...
    ///
    /// The result of downloading recorded file successful or not can be obtained by the event OnDownloadFile.
    /// <see cref="OnDownloadFile"/>
    /// </summary>
    ///
    /// <param name="fileID">The ID of the file to be downloaded. FileID can be obtained from the callback method OnUploadFile.</param>
    /// <param name="downloadFilePath">The path of the voice file to download, the filePath should like:"your_dir/your_file_name".</param>
    /// <param name="msTimeout">The length of the timeout for downloading, it is a micro second, value range[5000, 60000].</param>
    /// <param name="bPermanent">if the file is permanently saved on the server, set true, if not, set false</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int DownloadRecordedFile(string fileID, string downloadFilePath, int msTimeout, bool bPermanent);
    #endregion

	/// <summary>
	/// Get the voice message's file size and last time.
	/// </summary>
    ///
	/// <param name="filepath">The path of the voice file to get infomation, the filePath should like:"your_dir/your_file_name".</param>
	/// <param name="bytes">For returning the file's size.</param>
	/// <param name="seconds">For returning the voice's length.</param>
	/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
	/// <see cref="GCloudVoiceErr"/>
	public abstract int GetFileParam(string filepath, int [] bytes, float [] seconds);

    /// @brief set time for GVoice set the buffer size of player's voice data
    ///
    /// @param[in] nTimeSe set time for the buffer thae is used to reporter，default value is 20
    public abstract int SetReportBufferTime(int nTimeSec);

    /// @brief set players information that may be reported by yourself
    ///
    /// @param[in] nResult the reported result, 0 means server receive your reporter succ
    /// @brief Report a uncivilized player in your game
    ///
    /// @param[in] cszOpenID all players openid you may report
    /// @param[in] nMemberID all players memberid you may report
    /// @param[in] nCount element count in array
    ///
    /// @return an integer result, @enum GCloudVoiceErr
    public abstract int SetReportedPlayerInfo(string[] cszOpenID, int[] nMemberID, int nCount);

    /// @brief Report uncivilized players in your game
    ///
    /// @param[in] strInfo information that will be sent to server
    /// @return an integer result, @enum GCloudVoiceErr
    public abstract int ReportPlayer(string[] cszOpenID, int nCount, string strInfo);
	
	/// @brief Get room members(openid-memberid) with roomName
    ///
    /// @param[in] roomName, which room members you want to get
	/// @param[in] members, roommbers struct array, to save the room member info, the value of null will return the number of RoomMembers.
	/// @param[in] len, roommbers struct len, the value of -1 return the number of RoomMembers. 
    /// @return  the room members num of the room
    public abstract int GetRoomMembers(string roomName, RoomMembers[] members, int len);
	
	/// *********************************************************************************************************
	/// * !!!This API is not recommended, SpeechFileTranslate/SpeechFileToText is recommended
	/// *********************************************************************************************************
	/// @brief Translate speech from one language to another.
	///
	/// @param[in] fileID, ID of speech file which to be translated.
	/// @param[in] srcLang, speech language associated with fileID.
	/// @param[in] targetLang, target language that we want to translate to.
	/// @param[in] transType, if set SPEECH_TRANSLATE_STST, targetLang will be ignored
	/// @param[in] nTimeoutMS, length of speech translate perform timeout, the unit is milliseconds, recommended >= 10000
	///
	/// @return an integer result, @enum GCloudVoiceErr
	public abstract int SpeechTranslate(string fileID, SpeechLanguageType srcLang, SpeechLanguageType targetLang, SpeechTranslateType transType, int nTimeoutMS=10000);

	/// @brief Translate speech file from one language to another, OnSpeechFileTranslate return translated speech file.
	///
	/// @param[in] filePath, sts local file path, should like:"your_dir/your_file_name"
	/// @param[in] srcLang, speech language associated with fileID.
	/// @param[in] targetLang, target language that we want to translate to
	/// @param[in] voiceType, 0:normal woman, 1:normal man
	/// @param[in] voiceRate, [0.25, 4.0], 1.0 for normal voice speed rate,0.25 for min speed, 4.0 for max speed 
	/// @param[in] volume,[-10.0,10.0], 0.0 for normal volume, -10.0 for min volume, 10.0 for max volume 
	/// @param[in] nTimeoutMS, length of speech translate perform timeout, the unit is milliseconds, recommended >= 10000
	///
	/// @return an integer result, @enum GCloudVoiceErr
	public abstract int SpeechFileTranslate(string filePath, SpeechLanguageType srcLang, SpeechLanguageType targetLang, int voiceType = 0, float voiceRate = 1.0f, float volume = 0.0f, int nTimeoutMS = 10000);
	
	/// @brief Translate speech file to text, OnSpeechFileToText return translated text.
	///
	/// @param[in] filePath, stt local file path, should like:"your_dir/your_file_name"
	/// @param[in] srcLang, speech language associated with fileID.
	/// @param[in] targetLang, target language that we want to translate to,If don't need language conversion, targetLang should set same as srcLang.
	/// @param[in] nTimeoutMS, length of speech translate perform timeout, the unit is milliseconds, recommended >= 10000
	///
	/// @return an integer result, @enum GCloudVoiceErr
	public abstract int SpeechFileToText(string filePath, SpeechLanguageType srcLang, SpeechLanguageType targetLang, int nTimeoutMS = 10000);
	
	/// @brief Text to speech, OnTextToSpeech return fileID.
	///
	/// @param[in] text, utf-8, MAX 255 length
	/// @param[in] lang, text's language.
	/// @param[in] voice type, 0:normal woman, 1:normal man
	/// @param[in] nTimeoutMS, length of stt perform timeout, the unit is milliseconds, recommended >= 10000
	///
	/// @return an integer result, @enum GCloudVoiceErr
	public abstract int TextToSpeech(string text, SpeechLanguageType lang, int voiceType = 0, int nTimeoutMS = 10000);

	/// @brief Text to speech file, OnTextToSpeechFile return local file path.
	///
	/// @param[in] text, utf-8, MAX 255 length
	/// @param[in] lang, text's language.
	/// @param[in] voiceType, 0:normal woman, 1:normal man
	/// @param[in] filePath, tts local file path, should like:"your_dir/your_file_name"
	/// @param[in] voiceRate, [0.25, 4.0], 1.0 for normal voice speed rate,0.25 for min speed, 4.0 for max speed 
	/// @param[in] volume,[-10.0,10.0], 0.0 for normal volume, -10.0 for min volume, 10.0 for max volume 
	/// @param[in] nTimeoutMS, tts perform timeout, the unit is milliseconds, recommended >= 10000
	///
	/// @return an integer result, @enum GCloudVoiceErr
	public abstract int TextToSpeechFile(string text, SpeechLanguageType lang, string filePath, int voiceType = 0, float voiceRate = 1.0f, float volume = 0.0f, int nTimeoutMS = 10000);
	
	/// @brief Text translate
	///
	/// @param[in] text, utf-8, MAX 2000 length
	/// @param[in] srclang, text's language.
	/// @param[in] targetLang, target language that we want to translate to.
	/// @param[in] nTimeoutMS, length of text-translating perform timeout, the unit is milliseconds, recommended >= 10000
	///
	/// @return an integer result, @enum GCloudVoiceErr
	public abstract int TextTranslate(string text, SpeechLanguageType srcLang, SpeechLanguageType targetLang, int nTimeoutMS=10000);
	
	/// *********************************************************************************************************
	/// * !!!This API is not recommended, RSTSSpeechToSpeech/RSTSSpeechToText is recommended
	/// *********************************************************************************************************
	/// @brief start real-time speech to speech.
	///
	/// @param[in] srcLang, the speech language of the recorder.
	/// @param[in] pTargetLangs, target languages that we want to translate to.
	/// @param[in] nTargetLangCnt, number of target languages that we want to translate to.
	/// @param[in] transType, if set SPEECH_TRANSLATE_STST, pTargetLangs and nTargetLangCnt will be ignored
	/// @param[in] nTimeoutMS, length of speech translate perform timeout, the unit is milliseconds, recommended >= 3000
	///
	/// @return an integer result, @enum GCloudVoiceErr
	public abstract int RSTSStartRecording(SpeechLanguageType srcLang, SpeechLanguageType[] pTargetLangs, int nTargetLangCnt, SpeechTranslateType transType, int nTimeoutMS = 5000);
	
	/**
	* Recording voice in real time and converting it to other languages' speech, finally save the translated speech to file.
	*
	* Realization:Microphone->voice frame->translate->other language's voice frame->file
	*
	* e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->RSTSSpeechToSpeech->RSTSStopRecording->OnRSTSSpeechToSpeech
	*
	* @param[in] srcLang, the speech language of the recorder.
	* @param[in] pTargetLangs, target languages that we want to translate to.
	* @param[in] nTargetLangCnt, number of target languages that we want to translate to.
	* @param[in] dirPath, the directory is used to save the translated voice files
	* @param[in] voiceType, could be 0:normal woman, 1:normal man
	* @param[in] voiceRate, [0.25, 4.0], 1.0 for normal voice speed rate,0.25 for min speed, 4.0 for max speed
	* @param[in] volume,[-10.0,10.0], 0.0 for normal volume, -10.0 for min volume, 10.0 for max volume
	* @param[in] nTimeoutMS, perform timeout, the unit is milliseconds, recommended >= 3000
	* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
	* @see GCloudVoiceErrno
	*/
	public abstract int RSTSSpeechToSpeech(SpeechLanguageType srcLang, SpeechLanguageType[] pTargetLangs, int nTargetLangCnt, string dirPath, int voiceType = 0, float voiceRate = 1.0f, float volume = 0.0f, int nTimeoutMS = 10000);

	/**
	* Recording voice in real time and converting it to other languages's text, finally return the translated text.
	*
	* Realization:Microphone->voice frame->translate->other language's voice text
	*
	* e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->RSTSSpeechToText->RSTSStopRecording->OnRSTSSpeechToText
	*
	* @param[in] srcLang, the speech language of the recorder.
	* @param[in] pTargetLangs, target languages that we want to translate to.If don't need language conversion, pTargetLangs should set NULL and nTargetLangCnt set 0.
	* @param[in] nTargetLangCnt, number of target languages that we want to translate to.
	* @param[in] nTimeoutMS, length of RSTS perform timeout, the unit is milliseconds, recommended >= 3000
	* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
	* @see GCloudVoiceErrno
	*/
	public abstract int RSTSSpeechToText(SpeechLanguageType srcLang, SpeechLanguageType[] pTargetLangs, int nTargetLangCnt, int nTimeoutMS = 10000);
	
	/// @brief stop real-time speech to speech.
	///
	/// @return an integer result, @enum GCloudVoiceErr
	public abstract int RSTSStopRecording();
	
	/// 
	///  Enable or Disable real-time voice translate. for example, an American, a Chinese, and a German enter the same room and set their own language,
	/// the background service will translate Chinese speech into English speech and German speech, and send to corresponding person , and so do others .
	/// 
	/// The result of enableTranslate successful or not can be obtained by the callback method OnEnableTranslate.
	/// @see OnEnableTranslate
	/// 
	/// @param roomName: The name of The room to join, it should be a string composed by 0-9A-Za-Z._- and less than 127 bytes.
	/// @param bEnable: true for enable translate, false for disable translate.
	/// @param myLang: speaker language, refer to SpeechLanguageType.
	/// @param transType: realtime translate type, refer to RealTimeTranslateType.
	/// @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
	/// @see GCloudVoiceErrno
	public abstract int EnableTranslate(string roomName, bool enable, SpeechLanguageType myLang, RealTimeTranslateType transType);

	/// @brief Start Text to stream speech,auto play,only support chinese speech,voice type support customization.
	///
	/// @param[in] text, utf-8, MAX 255 length
	/// @param[in] voice type, as below:
	///				db1, General Sweet Girl Voice
	///				female0, General female voice
	///				femalen, emotional female voice
	///				kefu, Customer Service Voice
	///				male0, General male voice
	///				xdgz, emotional male voice
	///				txnews, news female voice
	///				wepay, payment dedicated female voice
	///				xiaowei, assistant female voice
	/// @param[in] nTimeoutMS, length of tts perform timeout, the unit is milliseconds, recommended >= 10000
	/// @param[in] filePath: The path of the file to store the voice data, the filePath should like:"your_dir/your_file_name" or "" witch means no need to save
	///
	/// @return an integer result, @enum GCloudVoiceErr
	public abstract int TextToStreamSpeechStart(string text, string voiceType, int nTimeoutMS = 10000, string filePath = "");

	/// @brief Stop Text to stream speech.
	///
	/// @return an integer result, @enum GCloudVoiceErr
	public abstract int TextToStreamSpeechStop();
		

	/// <summary>
    /// It is not recommended to call this method.
    /// If you want to use this, please contact with the GVoice team.
	/// </summary>
	public abstract int invoke( uint nCmd, uint nParam1, uint nParam2, uint [] pOutput );

    /*************************************************************
     *                  LGame APIs
     *************************************************************/
    //For LGame Rec Interface
    //for rec
    /// <summary>
    ///start save anchor rec voice data
    /// @return : if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno
    /// @see : GCloudVoiceErrno
    /// </summary>
    public abstract int StartSaveVoice();
    /// <summary>
    ///stop save anchor rec voice data
    /// @return : if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno
    /// @see : GCloudVoiceErrno
    /// </summary>
    public abstract int StopSaveVoice();
    /// <summary>
    ///Set Rec Save data timestamp  millionseconds
    /// @return : if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno
    /// @see : GCloudVoiceErrno
    /// </summary>
    public abstract int SetRecSaveTs(int ts);
    //for play
    /// <summary>
    ///Set Rec voice Data, fileid & it's index in this session(video)
    /// @return : if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno
    /// @see : GCloudVoiceErrno
    /// </summary>
    public abstract int SetPlayFileIndex(string fileid, int fileindex);
    /// <summary>
    ///start play voice in timestamp(milliono seconds), -1, will predownload first voice data
    /// @param ts: millinsecond for timestamp, -1:predownload first voice data
    /// @return : if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno
    /// @see : GCloudVoiceErrno
    /// </summary>
    public abstract int StartPlaySaveVoiceTs(int ts);
    
    /// <summary>
    ///when you not need play record savevoice, please call it.
    /// @return : if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno
    /// @see : GCloudVoiceErrno
    /// </summary>
    public abstract int StopPlaySaveVoice();
    /// <summary>
    ///not implement yet
    /// @return : if success return GCLOUD_VOICE_SUCC, failed return other errno @see GCloudVoiceErrno
    /// @see : GCloudVoiceErrno
    /// </summary>
    public abstract int DelAllSaveVoiceFile(string fileid, int fileindex);    
    /// <summary>
    /// enable civilization Voice detect
    /// 
    /// </summary>
    ///
    /// <param name="enable">Enable civilization detect if it is true, and disable  if it is false.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>

	public abstract int EnableCivilVoice(bool bEnable);

 /** api for MiniApp */

    /// @brief Enable MiniApp plugin for room.
    ///
    /// @param[in] roomName, room to set.
    /// @param[in] enable, true for enable and false reverse.
    ///
    /// @return an integer result, @enum GCloudVoiceErr
    public abstract int EnableWXMiniApp(string roomName, bool enable);

        
    /// <summary>
    /// @brief Query members from MiniApp.
    /// </summary>
    /// <param name="roomName">Room to set</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int  QueryWXMembers(string roomName);
        
    /// <summary>
    /// @brief Query member's info from MiniApp.
    /// </summary>
    ///
    /// <param name="roomName">Room to set</param>
    /// <param name="memberID">member ID to query.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int  QueryUserInfo(string roomName, int memberID, string openID);  

    /// <summary>
    /// @brief Send info to MiniApp.
    /// </summary>
    ///
    /// <param name="roomName">Room to set</param>
    /// <param name="info">info, info data for MiniApp.</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErr"/>
    public abstract int UpdateSelfInfo(string roomName, string info);  


	/*
	*set civil voice source path
	*param:[in]path :bin absolutive path
	*return:<see cref="GCloudVoiceErr"/>
	*/
	public abstract int SetCivilBinPath(string path);	
	
	
	/*
	*set Player's volume by playerid which value set by SetAppInfo's Openid
	*param:[in]playerid :the playerid who you want to set his volume
	*param:[in]vol: the volume value range[0-100]
	*return:<see cref="GCloudVoiceErr"/>
	*/
	public abstract int SetPlayerVolume(string playerid, uint vol);	
	
	/*
	*Get Player's volume
	*param:[in]playerid :the playerid's volume value,which set by SetPlayerVolume API, default value is 100.
	*return:the playerid's volume value [0-100], default value is 100.
	*/
	public abstract int GetPlayerVolume(string playerid);	


	/*
	*enable key words detect
	*param:[in]bEnable : true enable,false disable
	*return:<see cref="GCloudVoiceErr"/>
	*/
	public abstract int EnableKeyWordsDetect(bool bEnable);
 
    /// <summary>
    /// @brief Enable or disable MagicVoice function in realtime voice room.
    /// If a player enabled the MagicVoice function in a game room, then the play's voice will be translated to
    /// another voice, which is specified by the magicType parameter, and send to other players in the same room.
    ///
    /// EnableMagicVoice method should be called at anytime after JoinXxxRoom success.
    /// e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->JoinTeamRoom(RangeRoom/NationalRoom)
    /// -->EnableMagicVoice-->.....-->QuitRoom
    ///
    /// EnableMagicVoice is an Asynchronous interface, the result of EnableMagicVoice should be obtained by the callback method OnEnableMagicVoice.
    /// @see OnEnableMagicVoice
    /// </summary>
    ///
    /// <param name="magicType">MagicVoice type the player want to translate to</param>
    /// <param name="enable">true for enable MagicVoice function, and false for disable MagicVoice function</param>
    /// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
    /// <see cref="GCloudVoiceErrno"/>
    public abstract int EnableMagicVoice(string magicType, bool enable);
    
    /**
    * Enable or disable receive MagicVoice from other players in the same realtime voice room.
    * If a player A enabled the RecvMagicVoice function in a game room, and anyone B in this room enabled MagicVoice function, then the play A will receive magic voice of B.
    * In default, a player A will receive magic voice from B, if B enabled MagicVoice function.
    *
    * EnableRecvMagicVoice method should be called at anytime after JoinXxxRoom success.
    * e.g. GetVoiceEngine-->SetAppInfo-->Init-->SetNotify-->Poll-->JoinTeamRoom(RangeRoom/NationalRoom)
    * -->EnableRecvMagicVoice-->.....-->QuitRoom
    *
    * EnableRecvMagicVoice is an Asynchronous interface, the result of EnableRecvMagicVoice should be obtained by the callback method OnEnableRecvMagicVoice.
    * @see OnEnableRecvMagicVoice
    *
    * @param enable: true for enable RecvMagicVoice function, and false for disable RecvMagicVoice function
    * @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
    * @see GCloudVoiceErrno
    */
    public abstract int EnableRecvMagicVoice(bool enable);



	/**
	* Room data channel, business can transfer private data.
	*
	* @param roomName, the room which you want to transfer private data.
	* @param content: the content of the room transmission, JSON format, utf-8, limit 1024 bytes
	* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
	* @see GCloudVoiceErrno
	*/
	public abstract int RoomGeneralDataChannel(string roomName, string content);	

	/**
	* Report API call information for problem analysis.
	*
	* @param api, API that you want to trace.
	* @param callInfo: descriptive information of the API call, for example, in which scenario the API is called
	* @return If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.
	* @see GCloudVoiceErrno
	*/
	public abstract int APITrace(string api, string callInfo);	
		
	/// @brief set players information that may be reported by yourself
    ///
    /// @param[in] nResult the reported result, 0 means server receive your reporter succ
    /// @brief Report a uncivilized player in your game
    ///
    /// @param[in] cszOpenID all players openid you may report
    /// @param[in] nMemberID all players memberid you may report
    ///@param[in] pLang  every players voice country
    /// @param[in] nCount element count in array
    ///
    /// @return an integer result, @enum GCloudVoiceErr

	public abstract int SetPlayerInfoAbroad(string []szopenID,int []nMemberID,SpeechLanguageType []pLang,int nCount);
	 

	public abstract int SetMagicVoiceMsgType(string magicType);

    public abstract int SetTransSecInfo(string secInfo); 

	/*
	*@function:report all player
	*@param:bEnable:true /false
		
	*/
	public abstract int EnableReportALL(bool bEnable);


	/*
	*@function:report all player for abroad
	*@param:bEnable:true /false
		
	*/
	public abstract int EnableReportALLAbroad(bool bEnable);


	/// <summary>
/// enable civilization Voice detect for offline
/// 
/// </summary>
///
/// <param name="enable">Enable civilization detect if it is true, and disable	if it is false.</param>
/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
/// <see cref="GCloudVoiceErr"/>

public abstract int EnableCivilFile(bool bEnable);

/*funciton:audition file for specify magic type
*@param :[in] filepath	:which file need play used specify magic type
*		[in] magicType:specify magic type for play
*
*/
public abstract int AuditionFileForMagicType(string filepath,string magicType);


public abstract int IsSaveMagicFile(bool bSave);


/**
 * Set the URLs used in GVoice, you can only set the URLs defined in GVoiceUrlType enum.
 */
public abstract int SetServerUrl(GVoiceUrlType urlType, string url);

public abstract int SetSoundTag(GCloudVoiceSoundTag vtype, float[]  pParam);
	 
}
}//end namespace
