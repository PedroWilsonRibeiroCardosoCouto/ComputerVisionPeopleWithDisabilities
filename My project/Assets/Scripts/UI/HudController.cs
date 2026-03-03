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
        [SerializeField] private LuckArkman.XR.AR.SpatialSyncManager spatialSync;
        [SerializeField] private LuckArkman.XR.AI.YoloInferenceManager yoloAI;

        private Label latencyLabel;
        private Label bitrateLabel;
        private Label arStatusLabel;
        private Label aiStatusLabel;

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
            arStatusLabel = root.Q<Label>("ArStatusText");
            aiStatusLabel = root.Q<Label>("AiStatusText");

            deviceList.Clear();
            discoveryManager.OnHeadsetFound += UpdateDeviceList;
        }

        private void Update()
        {
            if (latencyLabel != null)
                latencyLabel.text = $"{latencyMonitor.LastRtt:F1} ms";

            if (arStatusLabel != null)
            {
                arStatusLabel.text = spatialSync.IsSynced ? "ESPACIAL: SINCRONIZADO" : "ESPACIAL: AGUARDANDO CALIBRAÇÃO";
                arStatusLabel.style.color = spatialSync.IsSynced ? new StyleColor(Color.green) : new StyleColor(Color.yellow);
            }

            if (aiStatusLabel != null)
            {
                aiStatusLabel.text = "IA: PRONTA (SENTIS GPU)";
                aiStatusLabel.style.color = new StyleColor(new Color(0.22f, 0.74f, 0.97f)); // Cyan
            }
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
