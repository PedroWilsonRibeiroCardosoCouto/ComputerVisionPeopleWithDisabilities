using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace LuckArkman.XR.Networking
{
    public class WifiDiscoveryManager : MonoBehaviour
    {
        [Header("Configurações de Rede")]
        public int discoveryPort = 8888;
        public string broadcastMessage = "XR_HEADSET_DISCOVERY";
        
        private UdpClient udpListener;
        private IPEndPoint groupEP;
        
        public struct HeadsetInfo
        {
            public string Name;
            public string IP;
            public DateTime LastSeen;
        }

        public Dictionary<string, HeadsetInfo> detectedHeadsets = new Dictionary<string, HeadsetInfo>();
        
        public event Action OnHeadsetFound;

        private void Start()
        {
            StartDiscovery();
        }

        public void StartDiscovery()
        {
            try
            {
                udpListener = new UdpClient(discoveryPort);
                groupEP = new IPEndPoint(IPAddress.Any, discoveryPort);
                Debug.Log($"[WifiDiscovery] Ouvindo na porta {discoveryPort}...");
            }
            catch (Exception e)
            {
                Debug.LogError($"[WifiDiscovery] Falha ao iniciar UDP: {e.Message}");
            }
        }

        private void Update()
        {
            if (udpListener == null) return;

            while (udpListener.Available > 0)
            {
                byte[] bytes = udpListener.Receive(ref groupEP);
                string message = Encoding.UTF8.GetString(bytes);
                
                if (message.StartsWith(broadcastMessage))
                {
                    ProcessDiscoveryMessage(message, groupEP.Address.ToString());
                }
            }
        }

        private void ProcessDiscoveryMessage(string msg, string ip)
        {
            // Esperado: XR_HEADSET_DISCOVERY|DeviceName
            string[] parts = msg.Split('|');
            string deviceName = parts.Length > 1 ? parts[1] : "Oculus Unknown";

            if (!detectedHeadsets.ContainsKey(ip))
            {
                detectedHeadsets[ip] = new HeadsetInfo 
                { 
                    Name = deviceName, 
                    IP = ip, 
                    LastSeen = DateTime.Now 
                };
                Debug.Log($"[WifiDiscovery] Novo Headset encontrado: {deviceName} em {ip}");
                OnHeadsetFound?.Invoke();
            }
            else
            {
                var info = detectedHeadsets[ip];
                info.LastSeen = DateTime.Now;
                detectedHeadsets[ip] = info;
            }
        }

        private void OnDestroy()
        {
            udpListener?.Close();
        }
    }
}
