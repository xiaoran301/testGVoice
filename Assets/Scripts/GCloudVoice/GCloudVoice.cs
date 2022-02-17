 ///
 /// CSharp API for GCloudVoice
 ///
 /// GVoice(Game Voice) is a voice service that covers diverse game scenes.
 ///
 /// RealTime voice, multiple members can join in the same room to communicate with each other.
 /// There are several different scenes in RealTime voice, they are TeamRoom, NationalRoom, RangeRoom.
 /// The workflow of RealTime voice is like below:
 /// GetEngine-->SetAppInfo-->Init-->Poll-->
 /// JoinXxxRoom-->...-->QuitRoom
 ///
 /// Message voice, a member can quickly record and send a voice message to other members.
 /// The workflow of Messages voice is like below:
 /// For record side:
 /// GetEngine-->SetAppInfo-->Init-->Poll-->SetMode(Messages)-->
 /// ApplyMessageKey-->StartRecording-->StopRecording-->UploadRecordedFile
 /// Or for play side:
 /// GetEngine-->SetAppInfo-->Init-->Poll-->
 /// ApplyMessageKey-->DownloadRecordedFile-->PlayRecordedFile-->StopPlayFile
 ///
 /// Translate Message, members can translate a recorded voice message to a piece of
 /// text in a specific language.
 /// The workflow of Translation is like below:
 /// GetEngine-->SetAppInfo-->Init-->invoke(9004,0,0,null)-->Poll-->SetMode(Messages)-->
 /// ApplyMessageKey-->StartRecording-->StopRecording-->SpeechToText
 /// Then you can get the translation result from the event "OnSpeechToText".
 ///
 /// RSTT, members can translate a recorded voice message to a piece of
 /// text in a specific language in realtime.
 /// The workflow of RSTT is like below:
 /// GetEngine-->SetAppInfo-->Init-->Poll-->SetMode(RSTT)-->
 /// ApplyMessageKey-->StartRecording-->StopRecording
 /// Then you can get the translation result from the event "OnStreamSpeechToText".
 ///
 /// Notice: GVoice SDK uses asynchronous callback mechanism to notify you the result of a
 /// function, so please remember to do the following three things:
 /// 1. Implement the delegates in GCloudVoiceEngineNotify;
 /// 2. Subscrib the events in GCloudVoiceEngineNotify;
 /// 3. Periodically call the Poll method to drive the engine return the callback results.
 ///

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace gcloud_voice
{
	public abstract class IGCloudVoice : IGCloudVoiceExtension
	{

		/*************************************************************
		 *                  Basic common APIs
		 *************************************************************/
		/// <summary>
		/// Set your app's info such as appID/appKey.
		///
		/// SetAppInfo method should be called after you have gotten the voice engine by GetEngine.
		/// e.g. GetEngine-->SetAppInfo
		/// </summary>
		///
		/// <param name="appID">Your game ID after you have registered.</param>
		/// <param name="appKey">Your game key after you have registered.</param>
		/// <param name="openID">A unique user ID, any string which can uniquely identify a user.</param>
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int SetAppInfo(string appID, string appKey, string openID);

		/// <summary>
		/// Initialize the GCloudVoice engine.
		///
		/// Init method should be called after you have set the app information by SetAppInfo.
		/// e.g. GetEngine-->SetAppInfo-->Init
		/// </summary>
		///
		/// <returns> If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int Init();

		/// <summary>
		/// Set the mode for Message/RSTT.
		///
		/// </summary>
		///
		/// <param name="mode">Mode to set. <see cref="GCloudVoiceMode"/>
		/// Messages: voice message to text
		/// RSTT: real-time speech to text </param>
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceMode"/>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int SetMode(GCloudVoiceMode mode);

		/// <summary>
		/// Trigger engine's callback.
		/// You should invoke poll on your loop periodically, such as Update() in Unity.
		///
		/// Poll method should be called after you have initialized the voice engine by Init.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll
		/// </summary>
		///
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int Poll();

		/// <summary>
		/// The Application's Pause.
		/// When your app pause such as goto backend you should invoke this.
		/// </summary>
		///
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int Pause();

		/// <summary>
		/// The Application's Resume.
		/// When your app reuse such as come back from  backend you should invoke this
		/// </summary>
		///
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int Resume();


		/*************************************************************
		 *                  Real-Time Voice APIs
		 *************************************************************/
		/// <summary>
		/// Join in a team room.
		/// Team room function allows no more than 20 members join in the same room to communicate freely.
		///
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinTeamRoom
		/// -->.....-->QuitRoom
		///
		/// The result of joining room successful or not can be obtained by the event OnJoinRoom.
		/// <see cref="OnJoinRoom"/>
		/// </summary>
		///
		/// <param name="roomName">The room to join, should be less than 127byte, composed by alpha.</param>
		/// <param name="msTimeout">The length of the timeout for joining, it is a micro second, value range[5000, 60000].</param>
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int JoinTeamRoom(string roomName, int msTimeout);

		/// <summary>
		/// Join in a LBS team room.
		/// RangeRoom function allows user to join a LBS room.
		/// After joined a RangeRoom, the member can hear the members' voice within a specific range.
		///
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinRangeRoom
		/// -->.....-->QuitRoom
		///
		/// The result of joining room successful or not can be obtained by the event OnJoinRoom.
		/// <see cref="OnJoinRoom"/>
		/// </summary>
		///
		/// <param name="roomName">The room to join, should be less than 127byte, composed by alpha.</param>
		/// <param name="msTimeout">The length of the timeout for joining, it is a micro second, value range[5000, 60000].</param>
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int JoinRangeRoom(string roomName, int msTimeout);

		/// <summary>
		/// Join in a national room.
		/// National room function allows more than 20 members to join in the same room, and they can choose two different roles to be.
		/// The Anchor role can open microphone to speak and open speaker to listen.
		/// The Audience role can only open the speaker to listen.
		///
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinNationalRoom
		/// -->.....-->QuitRoom
		///
		/// The result of joining room successful or not can be obtained by the event OnJoinRoom.
		/// <see cref="OnJoinRoom"/>
		/// </summary>
		///
		/// <param name="roomName">The room to join, should be less than 127byte, composed by alpha.</param>
		/// <param name="role">A GCloudVoiceMemberRole value illustrates wheather the player can send voice data or not.</param>
		/// <param name="msTimeout">The length of the timeout for joining, it is a micro second, value range[5000, 60000]</param>
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int JoinNationalRoom(string roomName, GCloudVoiceRole role, int msTimeout);

		/// <summary>
		/// Update your coordinate.
		///
		/// UpdateCoordinate method should be called after the member has joined a RangeRoom successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinRangeRoom
		/// -->UpdateCoordinate-->.....-->QuitRoom
		/// </summary>
		///
		/// <param name="roomName">The room to update, should be less than 127byte, composed by alpha.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		/// <param name="r">The r coordinate.</param>
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int UpdateCoordinate(string roomName, long x, long y, long z, long r);

		/// <summary>
		/// Change the member's role in a national room.
		/// ChangeRole is a function in NationalRoom, so this method should be called after the member has
		/// joined a NationalRoom successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinNationalRoom
		/// -->.....-->ChangeRole-->.....-->QuitRoom
		///
		/// The result of changing role successful or not can be obtained by the event OnRoleChanged.
		/// <see cref="OnRoleChanged"/>
		/// </summary>
		///
		/// <param name="role">The member's role want to change to.</param>
		/// <param name="roomName">The name of The room to change role, it should be an exist national room name.</param>
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceRole"/>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int ChangeRole(GCloudVoiceRole role, string roomName = "");

		/// <summary>
		/// Open the player's microphone and begin to send the player's voice data.
		///
		/// OpenMic method should be called after the member has joined a voice room successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinXxxRoom
		/// -->.....-->OpenMic-->.....-->QuitRoom
		/// </summary>
		///
		/// <returns> If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int OpenMic();

		/// <summary>
		/// Close the players's microphone and stop sending the player's voice data.
		///
		/// CloseMic method should be called after the member has joined a voice room successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinXxxRoom
		/// -->.....-->OpenMic-->CloseMic-->.....-->QuitRoom
		/// </summary>
		///
		/// <returns> If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int CloseMic();

		/// <summary>
		/// Open the player's speaker and begin to recvie voice data from the network.
		///
		/// OpenSpeaker method should be called after the member has joined a voice room successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinXxxRoom
		/// -->.....-->OpenSpeaker-->.....-->QuitRoom
		/// </summary>
		///
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int OpenSpeaker();

		/// <summary>
		/// Close player's speaker and stop reciving voice data from the network.
		///
		/// CloseSpeaker method should be called after the member has joined a voice room successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinXxxRoom
		/// -->.....-->OpenSpeaker-->CloseSpeaker-->.....-->QuitRoom
		/// </summary>
		///
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int CloseSpeaker();

		/// <summary>
		/// Quit the voice room.
		///
		/// QuitRoom method should be called after the member has joined a voice room successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinXxxRoom
		/// -->.....-->QuitRoom
		///
		/// The result of quiting room successful or not can be obtained by the event OnQuitRoom
		/// <see cref="OnQuitRoom"/>
		/// </summary>
		///
		/// <param name="roomName">The name of The room to quit, it should be composed by 0-9A-Za-Z._- and less than 127 bytes
		/// and should be an exist room names.</param>
		/// <param name="msTimeout">The length of the timeout for quiting, it is a micro second, value range[5000, 60000].</param>
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int QuitRoom(string roomName, int msTimeout);


		/*************************************************************
		 *                 Messages Voice APIs
		 *************************************************************/
		/// <summary>
		/// Apply the key for voice message.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->ApplyMessageKey-->...
		///
		/// The result of applying message key successful or not can be obtained by the event OnApplyMessageKey.
		/// <see cref="OnApplyMessageKey"/>
		/// </summary>
		///
		/// <param name="msTimeout">The length of the timeout for applying, it is a micro second, value range[5000, 60000].</param>
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int ApplyMessageKey(int msTimeout);

		/// <summary>
		/// Open the player's microphone and record the player's voice.
		///
		/// StartRecording method should be called in Messages or RSTT mode, and after you have
		/// applied the message key successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->SetMode(Messages/RSTT)-->ApplyMessageKey
		/// -->StartRecording-->...
		/// </summary>
		///
		/// <param name="filePath">The path of the file to store the voice data, the filePath should like:"your_dir/your_file_name"</param>
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int StartRecording(string filePath);

		/// <summary>
		/// Stop the player's microphone and stop record the player's voice.
		///
		/// StopRecording method should be called in Messages or RSTT mode, and after you have
		/// applied the message key successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->SetMode(Messages/RSTT)-->ApplyMessageKey
		/// -->StartRecording-->StopRecording-->...
		/// </summary>
		///
		///<returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int StopRecording();

		/// <summary>
		/// Upload the player's voice message file to the network.
		///
		/// recorded a voice message successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->SetMode(Messages/RSTT)-->ApplyMessageKey
		/// -->StartRecording-->StopRecording-->UploadRecordedFile-->...
		///
		/// The result of uploading recorded file successful or not can be obtained by the event OnUploadFile.
		/// <see cref="OnUploadFile"/>
		/// </summary>
		///
		/// <param name="filePath">The path of the voice file to upload, the filePath should like:"your_dir/your_file_name".</param>
		/// <param name="msTimeout">The length of the timeout for uploading, it is a micro second, value range[5000, 60000].</param>
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int UploadRecordedFile(string filePath, int msTimeout);

		/// <summary>
		/// Download other players' voice message from the network.
		///
		/// uploaded a voice message successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->ApplyMessageKey
		/// -->...-->DownloadRecordedFile-->...
		///
		/// The result of downloading recorded file successful or not can be obtained by the event OnDownloadFile.
		/// <see cref="OnDownloadFile"/>
		/// </summary>
		///
		/// <param name="fileID">The ID of the file to be downloaded. FileID can be obtained from the callback method OnUploadFile.</param>
		/// <param name="downloadFilePath">The path of the voice file to download, the filePath should like:"your_dir/your_file_name"</param>
		/// <param name="msTimeout">The length of the timeout for downloading, it is a micro second, value range[5000, 60000].</param>
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int DownloadRecordedFile(string fileID, string downloadFilePath, int msTimeout);

		/// <summary>
		/// Play local voice message file.
		///
		/// PlayRecordedFile method should be called after you have
		/// recorded a voice message successfully or downloaded a voice message successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->SetMode(Messages/RSTT)-->ApplyMessageKey
		/// -->StartRecording-->StopRecording-->PlayRecordedFile-->...
		/// or GetEngine-->SetAppInfo-->Init-->Poll-->ApplyMessageKey
		/// -->DownloadRecordedFile-->PlayRecordedFile-->...
		///
		/// If the voice file has been played to the end normally, the event OnPlayRecordedFile will be called.
		/// And if you called StopPlayFile method before the end of the voice file, OnPlayRecordedFile will not be called.
		/// <see cref="OnPlayRecordedFile"/>
		/// </summary>
		///
		/// <param name="downloadFilePath">The path of the voice file to play, the filePath should like:"your_dir/your_file_name".</param>
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int PlayRecordedFile(string downloadFilePath);

		/// <summary>
		/// Stop playing the voice file.
		///
		/// StopPlayFile method should be called before the voice message has been played to the end.
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->ApplyMessageKey
		/// -->...-->PlayRecordedFile-->StopPlayFile
		///
		/// </summary>
		///
		/// <returns>If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int StopPlayFile();


		/*************************************************************
		 *                 Translation APIs
		 *************************************************************/
		/// <summary>
		/// Translate the voice data to a piece of text in a specific language, the default language is Chinese.
		///
		/// SpeechToText method should be called after you have
		/// uploaded a voice message successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->invoke(9004,0,0,null)-->Poll-->SetMode(Message)-->ApplyMessageKey
		/// -->StartRecording-->StopRecording-->SpeechToText-->...
		///
		/// The result of translating successful or not can be obtained by the event OnSpeechToText.
		/// <see cref="OnSpeechToText"/>
		/// </summary>
		///
		/// <param name="fileID">The ID of the file to be translated. FileID can be obtained from the callback method OnUploadFile.</param>
		/// <param name="language">The specific language to be translated to.</param>
		/// <param name="msTimeout">The length of the timeout for translating, it is a micro second, value range[5000, 60000].</param>
		/// <returns> If success returns GCLOUD_VOICE_SUCC, otherwise returns other errno.</returns>
		/// <see cref="GCloudLanguage"/>
		/// <see cref="GCloudVoiceErr"/>
		public abstract int SpeechToText(string fileID, int language = 0, int msTimeout = 6000);


		/*************************************************************
		 *                  Token related APIs
		 * Deprecated APIs, please move to the APIs ends with token
		 * in IGCloudVoiceExtension class
		 *************************************************************/
		/// <summary>
		/// Join in a team room with token.
		/// Team room function allows no more than 20 members join in the same room to communicate freely.
		///
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinTeamRoom
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
		public abstract int JoinTeamRoom(string roomName, string token, int timestamp, int msTimeout);

		/// <summary>
		/// Join in a national room with token.
		/// National room function allows more than 20 members to join in the same room, and they can choose two different roles to be.
		/// The Anchor role can open microphone to speak and open speaker to listen.
		/// The Audience role can only open the speaker to listen.
		///
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->JoinNationalRoom
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
		public abstract int JoinNationalRoom(string roomName, string token, int timestamp, GCloudVoiceRole role, int msTimeout);

		/// <summary>
		/// Apply the key for voice message with token.
		///
		/// e.g. GetEngine-->SetAppInfo-->Init-->Poll-->ApplyMessageKey-->...
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
		public abstract int ApplyMessageKey(string token, int timestamp, int msTimeout);

		/// <summary>
		/// Translate the voice data to a piece of text in a specific language with token, the default language is Chinese.
		///
		/// SpeechToText method should be called after you have
		/// uploaded a voice message successfully.
		/// e.g. GetEngine-->SetAppInfo-->Init-->invoke(9004,0,0,null)-->Poll-->SetMode(Message)-->ApplyMessageKey
		/// -->StartRecording-->StopRecording-->SpeechToText-->...
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
		public abstract int SpeechToText(string fileID, string token, int timestamp, int language = 0, int msTimeout = 6000);

		public abstract int Enable3DVoice(bool bEnable);
		public abstract int Set3DPosition(GVoice3DVector pos);
		public abstract int Set3DForward(GVoice3DVector forward);
		public abstract int Set3DUpward(GVoice3DVector upward);
		public abstract int Set3DDistProperties(GVoice3DDistProperties g3dProperties);
		public abstract int EnableDualLink(bool enable);


		//class CGCloudVoiceSys
		public class GCloudVoice
		{
			/// <summary>
			/// Get the voice engine instance.
			/// </summary>
			///
			/// <returns>If success returns the voice engine instance, otherwise returns null.</returns>
			public static IGCloudVoice GetEngine()
			{
				if (instance == null)
				{
					instance = new GCloudVoiceEngine();
#if UNITY_ANDROID && !UNITY_EDITOR
					GCloudVoiceEngine.PrintLog("GCloudVoice_C# API: Call java from c sharp before");
					var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
					var currentActivity = activity.GetStatic<AndroidJavaObject>("currentActivity");
					var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

					AndroidJavaClass jarVoice =
						new AndroidJavaClass("com.gcloudsdk.gcloud.voice.GCloudVoiceEngine");
					AndroidJavaObject ins = jarVoice.CallStatic<AndroidJavaObject>("getInstance");
					ins.Call<int>("init", context, currentActivity);
					GCloudVoiceEngine.PrintLog("GCloudVoice_C# API: Call java from c sharp after");
#endif
				}

				return instance;
			}

			private static IGCloudVoice instance = null;
		}
	}
}//end namespace
