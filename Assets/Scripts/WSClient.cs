using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WSClient : MonoBehaviour
{
    WebSocket ws;
	//CardManager cardManager;

    void Start()
    {
		//cardManager = GameObject.Find( "CardManager" )
		//					.GetComponent<CardManager>();	
        ws = new WebSocket( "ws://127.0.0.1:5000" );
		// 接続直後のイベントハンドラ
		ws.OnOpen += ( sender, e ) =>
		{
			Debug.Log( "WsOpen" );
		};

		// データ受信した時のイベントハンドラ
		ws.OnMessage += ( sender, e ) =>
		{
			// e.Dataに送られてきたデータが入る
			Debug.Log( "WsMessage: " + e.Data );
			//cardManager.SpawnCard( e.Data );
		};

		// エラーが発生した時のイベントハンドラ
		ws.OnError += ( sender, e ) =>
		{
			// e.Messageにエラーメッセージが入る
			Debug.Log( "WsError: " + e.Message );
		};

		// 接続切断時のイベントハンドラ
		ws.OnClose += ( sender, e ) =>
		{
			// e.Messageにエラーメッセージが入る
			Debug.Log( "WsClose" );
		};

		// 接続開始
		ws.Connect();
	}

	private void OnDestroy()
	{
		ws.Close();
	}

	// データ送信
	public void Send( string s )
	{
		ws.Send( s );
	}

	// Update is called once per frame
	void Update()
    {
  //      if ( Input.GetMouseButtonDown( 0 ) ) {
		//	Send( "Hello" );
		//}
    }
}

