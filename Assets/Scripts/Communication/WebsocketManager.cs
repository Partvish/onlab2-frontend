using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using System.Text.Json.Serialization;

public class WebsocketManager : MonoBehaviour
{
    class RegisterPlayerMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    };




    private SocketIO socket;
    public int port = 5000;
    public string playerId = "";
    // Start is called before the first frame update
    async void Start()
    {
        Debug.Log("start WebSocketManager");
        socket = new SocketIO($"ws://localhost:{port}");

        socket.OnError += (o, e) =>
        {
            Debug.Log("ERROR:" + e.ToString());
            Debug.Log("ERROR:" + o.ToString());
        };

        socket.OnConnected += async (socketO, eventArgs) =>
        {
            Debug.Log("CONNECTED:" + socketO.ToString());
            Debug.Log("CONNECTED:" + eventArgs.ToString());
            RegisterPlayerMessage o = new RegisterPlayerMessage() { Id = playerId };
            await socket.EmitAsync("register_player", o);
        };

        socket.On("update_room_state", response =>
        {
            Debug.Log("kisfasz");
        });

        await socket.ConnectAsync();
    }

    async void registerHit(string victimId) {
        RegisterPlayerMessage o = new RegisterPlayerMessage() { Id = victimId };
        await this.socket.EmitAsync(GameMessage.HIT , o);
    }

     async void OnDestroy()
    {
        Debug.Log("stop WebsocketManager");
        await socket.DisconnectAsync();
    }
}
