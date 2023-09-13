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
		// �ڑ�����̃C�x���g�n���h��
		ws.OnOpen += ( sender, e ) =>
		{
			Debug.Log( "WsOpen" );
		};

		// �f�[�^��M�������̃C�x���g�n���h��
		ws.OnMessage += ( sender, e ) =>
		{
			// e.Data�ɑ����Ă����f�[�^������
			Debug.Log( "WsMessage: " + e.Data );
			//cardManager.SpawnCard( e.Data );
		};

		// �G���[�������������̃C�x���g�n���h��
		ws.OnError += ( sender, e ) =>
		{
			// e.Message�ɃG���[���b�Z�[�W������
			Debug.Log( "WsError: " + e.Message );
		};

		// �ڑ��ؒf���̃C�x���g�n���h��
		ws.OnClose += ( sender, e ) =>
		{
			// e.Message�ɃG���[���b�Z�[�W������
			Debug.Log( "WsClose" );
		};

		// �ڑ��J�n
		ws.Connect();
	}

	private void OnDestroy()
	{
		ws.Close();
	}

	// �f�[�^���M
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

