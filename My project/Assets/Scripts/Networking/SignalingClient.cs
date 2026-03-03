using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckArkman.XR.Networking
{
    /// <summary>
    /// Cliente de sinalização para troca de SDP e Candidates via WebSocket.
    /// </summary>
    public class SignalingClient : MonoBehaviour
    {
        private ClientWebSocket webSocket;
        private CancellationTokenSource cts;

        public event Action<string> OnMessageReceived;
        public event Action OnConnected;
        public event Action OnDisconnected;

        public bool IsConnected => webSocket?.State == WebSocketState.Open;

        public async Task Connect(string ip, int port = 8080)
        {
            try
            {
                webSocket = new ClientWebSocket();
                cts = new CancellationTokenSource();
                
                string uri = $"ws://{ip}:{port}/signaling";
                Debug.Log($"[Signaling] Conectando a {uri}...");
                
                await webSocket.ConnectAsync(new Uri(uri), cts.Token);
                
                Debug.Log("[Signaling] Conectado!");
                OnConnected?.Invoke();
                
                _ = ReceiveLoop();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Signaling] Erro na conexão: {e.Message}");
            }
        }

        private async Task ReceiveLoop()
        {
            var buffer = new byte[4096];
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cts.Token);
                        OnDisconnected?.Invoke();
                    }
                    else
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        OnMessageReceived?.Invoke(message);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Signaling] Loop de recebimento encerrado: {e.Message}");
                OnDisconnected?.Invoke();
            }
        }

        public async Task SendMessage(string message)
        {
            if (!IsConnected) return;
            
            var bytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cts.Token);
        }

        private void OnDestroy()
        {
            cts?.Cancel();
            webSocket?.Dispose();
        }
    }

    [Serializable]
    public class SignalingMessage
    {
        public string type; // "offer", "answer", "candidate"
        public string sdp;
        public IceCandidateData candidate;
    }

    [Serializable]
    public class IceCandidateData
    {
        public string candidate;
        public string sdpMid;
        public int sdpMLineIndex;
    }
}
