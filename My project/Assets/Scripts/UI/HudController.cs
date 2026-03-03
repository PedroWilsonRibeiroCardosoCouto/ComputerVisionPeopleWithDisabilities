using UnityEngine;
using UnityEngine.UIElements;
using LuckArkman.XR.Networking;
using System.Linq;

namespace LuckArkman.XR.UI
{
    public class HudController : MonoBehaviour
    {
        private UIDocument uiDocument;
        private VisualElement root;
        private ScrollView deviceList;
        private Label statusLabel;
        private VisualElement statusTag;

        [SerializeField] private WifiDiscoveryManager discoveryManager;
        [SerializeField] private SignalingClient signalingClient;
        [SerializeField] private LatencyMonitor latencyMonitor;

        private Label latencyLabel;
        private Label bitrateLabel;

        private void OnEnable()
        {
            uiDocument = GetComponent<UIDocument>();
            root = uiDocument.rootVisualElement;

            deviceList = root.Q<ScrollView>("DeviceList");
            statusLabel = root.Q<Label>("StatusLabel");
            statusTag = root.Q<VisualElement>("StatusTag");
            
            // Novos elementos (opcionais na UI inicial)
            latencyLabel = root.Q<Label>("LatencyValue");
            bitrateLabel = root.Q<Label>("BitrateValue");

            deviceList.Clear();
            discoveryManager.OnHeadsetFound += UpdateDeviceList;
        }

        private void Update()
        {
            if (latencyLabel != null)
                latencyLabel.text = $"{latencyMonitor.LastRtt:F1} ms";
        }

        private void UpdateDeviceList()
        {
            deviceList.Clear();

            foreach (var headset in discoveryManager.detectedHeadsets.Values)
            {
                VisualElement item = CreateDeviceItem(headset.Name, headset.IP);
                deviceList.Add(item);
            }

            if (discoveryManager.detectedHeadsets.Count > 0)
            {
                statusLabel.text = "DISPOSITIVOS ENCONTRADOS";
                statusTag.AddToClassList("status-connected");
                statusTag.RemoveFromClassList("status-searching");
            }
        }

        private VisualElement CreateDeviceItem(string name, string ip)
        {
            var container = new VisualElement();
            container.AddToClassList("device-item");

            var nameLabel = new Label($"{name} ({ip})");
            nameLabel.AddToClassList("device-name");

            var connectBtn = new Button(async () => await ConnectToHeadset(ip));
            connectBtn.text = "CONECTAR";
            connectBtn.AddToClassList("control-btn");

            container.Add(nameLabel);
            container.Add(connectBtn);

            return container;
        }

        private async System.Threading.Tasks.Task ConnectToHeadset(string ip)
        {
            Debug.Log($"[HUD] Tentando conectar ao IP: {ip}");
            statusLabel.text = $"CONECTANDO A {ip}...";
            
            await signalingClient.Connect(ip);
            
            if (signalingClient.IsConnected)
            {
                statusLabel.text = $"CONECTADO A {ip}";
                statusTag.style.backgroundColor = new StyleColor(Color.green);
            }
            else
            {
                statusLabel.text = "FALHA NA CONEXÃO";
                statusTag.style.backgroundColor = new StyleColor(Color.red);
            }
        }
    }
}
