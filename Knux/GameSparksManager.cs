using UnityEngine;
using System.Collections;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.Api.Messages;
using System.Collections.Generic;
using GameSparks.RT;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using GameSparks.Api.Requests;


/// <summary>
/// Game sparks manager.
/// Created by Sean Durkan for GameSparks Inc 2016 cc
/// </summary>
public class GameSparksManager : MonoBehaviour {

	/// <summary>The GameSparks Manager singleton</summary>
	private static GameSparksManager instance = null;
	/// <summary>This method will return the current instance of this class </summary>
	public static GameSparksManager Instance(){
		if (instance != null) {
			return instance; // return the singleton if the instance has been setup
		} else { // otherwise return an error
			Debug.LogError ("GSM| GameSparksManager Not Initialized...");
		}
		return null;
	}

    public string GameScene;

	void Awake() {
		instance = this; // if not, give it a reference to this class...
		DontDestroyOnLoad(this.gameObject); // and make this object persistent as we load new scenes
	}
	private GameSparksRTUnity gameSparksRTUnity;
	public GameSparksRTUnity GetRTSession(){
		return gameSparksRTUnity;
	}
	private RTSessionInfo sessionInfo;
	public RTSessionInfo GetSessionInfo(){
		return sessionInfo;
	}
	private ChatManager chatManager; // this is a refrence to the chat-manager so that any packets that contain chat-messages can be sent to the chat-manager

	#region Login & Registration
	public delegate void AuthCallback(AuthenticationResponse _authresp2);
	public delegate void RegCallback(RegistrationResponse _authResp);
	/// <summary>
	/// Sends an authentication request or registration request to GS.
	/// </summary>
	/// <param name="_callback1">Auth-Response</param>
	/// <param name="_callback2">Registration-Response</param>
	public void AuthenticateUser (string _userName, string _password, RegCallback _regcallback, AuthCallback _authcallback)
	{
		new GameSparks.Api.Requests.RegistrationRequest()
		// this login method first attempts a registration //
		// if the player is not new, we will be able to tell as the registrationResponse has a bool 'NewPlayer' which we can check //
		// for this example we use the user-name was the display name also //
			.SetDisplayName(_userName)
			.SetUserName(_userName)
			.SetPassword(_password)
			.Send((regResp) => {
				if(!regResp.HasErrors){ // if we get the response back with no errors then the registration was successful
					Debug.Log("GSM| Registration Successful..."); 
					_regcallback(regResp);
				}else{
					// if we receive errors in the response, then the first thing we check is if the player is new or not
					if(!(bool)regResp.NewPlayer) // player already registered, lets authenticate instead
					{
						Debug.LogWarning("GSM| Existing User, Switching to Authentication");
						new GameSparks.Api.Requests.AuthenticationRequest()
							.SetUserName(_userName)
							.SetPassword(_password)
							.Send((authResp) => {
								if(!authResp.HasErrors){
									Debug.Log("Authentication Successful...");
									_authcallback(authResp);
								}else{
									Debug.LogWarning("GSM| Error Authenticating User \n"+authResp.Errors.JSON);
								}
							});
					}else{
						Debug.LogWarning("GSM| Error Authenticating User \n"+regResp.Errors.JSON); // if there is another error, then the registration must have failed
					}
				}
			});
	}
	#endregion

	bool isConnected, isAuthenticated;

	void Start(){

//		GS.GameSparksAuthenticated += (isAv)=>{
//			Debug.Log (">>> GS AUTHENTICATED <<<  "+isAv);
//			isAuthenticated = true;
//		};
//
//		GS.GameSparksAvailable += (isCon)=>{
//			Debug.Log (">>> GS AVAILABLE "+isCon+" <<<");
//			isConnected = isCon;
//			if(!isCon){
//				isAuthenticated = false;
//			}
//		};
	}
		

//	void Update(){
//			
//		if(isAuthenticated){
//			Debug.Log (">>> IS AUTHENTICATED <<<");
//		}
//		else if(isConnected){
//			Debug.Log (">>> IS AVAILABLE <<<");
//		}
//	}

	#region Matchmaking Request
	/// <summary>
	/// This will request a match between as many players you have set in the match.
	/// When the max number of players is found each player will receive the MatchFound message
	/// </summary>
	public void FindPlayers(string matchShortCode){


		Debug.Log ("GSM| Attempting Matchmaking...");
		new GameSparks.Api.Requests.MatchmakingRequest ()
            .SetMatchShortCode (matchShortCode) // set the shortcode to be the same as the one we created in the first tutorial
			.SetSkill (0) // in// this case we want anyone to be able to join so the skill is set to zero by default

			.Send ((response) => {
				if(response.HasErrors){ // check for errors
					Debug.LogError("GSM| MatchMaking Error \n"+response.Errors.JSON);
				}
			});
	}
	#endregion

	public void StartNewRTSession(RTSessionInfo _info){
		if (gameSparksRTUnity == null) {
			Debug.Log ("GSM| Creating New RT Session Instance...");
			sessionInfo = _info;
			gameSparksRTUnity = this.gameObject.AddComponent<GameSparksRTUnity> (); // Adds the RT script to the game
			// In order to create a new RT game we need a 'FindMatchResponse' //
			// This would usually come from the server directly after a sucessful FindMatchRequest //
			// However, in our case, we want the game to be created only when the first player decides using a button //
			// therefore, the details from the response is passed in from the gameInfo and a mock-up of a FindMatchResponse //
			// is passed in. In normal operation this mock-response may not be needed //
			GSRequestData mockedResponse = new GSRequestData ().AddNumber ("port", (double)_info.GetPortID ()).AddString ("host", _info.GetHostURL ()).AddString ("accessToken", _info.GetAccessToken ()); // construct a dataset from the game-details
			FindMatchResponse response = new FindMatchResponse (mockedResponse); // create a match-response from that data and pass it into the game-config
			// So in the game-config method we pass in the response which gives the instance its connection settings //
			// In this example i use a lambda expression to pass in actions for 
			// OnPlayerConnect, OnPlayerDisconnect, OnReady and OnPacket actions //
			// These methods are self-explanitory, but the important one is the OnPacket Method //
			// this gets called when a packet is received //

			gameSparksRTUnity.Configure (response, 
				(peerId) => {
					OnPlayerConnectedToGame (peerId);
				},
				(peerId) => {
					OnPlayerDisconnected (peerId);
				},
				(ready) => {
					OnRTReady (ready);
				},
				(packet) => {
					OnPacketReceived (packet);
				});
			gameSparksRTUnity.Connect (); // when the config is set, connect the game
           
//		Debug.LogError (_info.GetAccessToken()+"|"+_info.GetMatchID());
		} else {
			Debug.LogError ("Session Already Started");
		}

	}

	private void OnPlayerConnectedToGame(int _peerId){
		Debug.Log ("GSM| Player "+_peerId+" Connected");
	}

	private void OnPlayerDisconnected(int _peerId){
		Debug.Log ("GSM| Player "+_peerId+" Disconnected");
		GameController.Instance.OnOpponentDisconnected (_peerId);
	}

	private void OnRTReady(bool _isReady){
		if (_isReady) {
			Debug.LogError ("GSM| RT Session Connected...");
			Nux.SceneManager.Instance.LoadScene(GameScene);

            StartCoroutine (SendPackets());
		}
	}



    IEnumerator SendPackets ()
    {
        for (int i = 1; i <= 150; i++) {
            GameSparksRTUnity.Instance.SendData (1, GameSparksRT.DeliveryIntent.RELIABLE, new RTData ().SetInt (1, i));
            yield return new WaitForSeconds (2f);
        }
    }

	private void OnPacketReceived(RTPacket _packet){

        Debug.Log("GSM| PacketSize: " + _packet.PacketSize);

		switch (_packet.OpCode) {
		// op-code 1 refers to any chat-messages being received by a player //
		// from here, we will send them to the chat-manager //
		//case 1:

		//	if (chatManager == null) { // if the chat manager is not yet setup, then assign the reference in the scene
		//		chatManager = GameObject.Find ("Chat Manager").GetComponent<ChatManager> ();
		//	}
		//	chatManager.OnMessageReceived (_packet); // send the whole packet to the chat-manager
		//	break;
		case 2:
			// contains information about the rotation, positiona and 'isInvincible' bool
			GameController.Instance.UpdateOpponents (_packet);
			break;
		case 3:
			// contains information about the id of the shell that needs to be created
			// so that the sender and recipient a corresonding id for the shell
			GameController.Instance.InstantiateOpponentProjetile(_packet);
			break;
		case 4:
			// contains information about the position and rotation of the opponent shells
			GameController.Instance.UpdateOpponentProjectiles (_packet);
		    break;
        case 5:
            // contains information about the shell that hit, the player it hit, and the owner of the shell
            GameController.Instance.RegisterOpponentCollision(_packet);
            break;

//		// REGISTER NETWORK INFO //
//		case 100:
//			Debug.Log ("GSM| Loading Level...");
//			SceneManager.LoadScene ("GameScene");
//			break;
//		case 101:
//			GameController.Instance ().CalculateTimeDelta (_packet);
//			break;
//		case 102:
//			GameController.Instance ().SyncClock (_packet);
//			break;
        }
    }
}

public class RTSessionInfo
{
	private string hostURL;
	public string GetHostURL(){	return this.hostURL;	}
	private string acccessToken;
	public string GetAccessToken(){	return this.acccessToken;	}
	private int portID;
	public int GetPortID(){	return this.portID;	}
	private string matchID;
	public string GetMatchID(){	return this.matchID;	}

	private List<RTPlayer> playerList = new List<RTPlayer> ();
	public List<RTPlayer> GetPlayerList(){
		return playerList;
	}

	/// <summary>
	/// Creates a new RTSession object which is held untill a new RT session is created
	/// </summary>
	/// <param name="_message">Message.</param>
	public RTSessionInfo (MatchFoundMessage _message){
		portID = (int)_message.Port;
		hostURL = _message.Host;
		acccessToken = _message.AccessToken;
		matchID = _message.MatchId;
		// we loop through each participant and get thier peerId and display name //
		foreach(MatchFoundMessage._Participant p in _message.Participants){
			playerList.Add(new RTPlayer(p.DisplayName, p.Id, (int)p.PeerId));
		}
	}

	public class RTPlayer
	{
		public RTPlayer(string _displayName, string _id, int _peerId){
			this.displayName = _displayName;
			this.id = _id;
			this.peerID = _peerId;
		}

		public string displayName;
		public string id;
		public int peerID;
		public bool isOnline;
	}
}


