using UnityEngine;
using LuckArkman.XR.Networking;
using LuckArkman.XR.AR;
using LuckArkman.XR.AI;
using LuckArkman.XR.Safety;
using LuckArkman.XR.UI;
using System.Collections.Generic;

namespace LuckArkman.XR.System
{
    /// <summary>
    /// Orquestrador central que une todos os módulos do sistema.
    /// Gerencia o fluxo: Streaming -> AR -> IA -> Heatmap -> UI.
    /// </summary>
    public class MainSystemOrchestrator : MonoBehaviour
    {
        public enum SystemState { Idle, Searching, Connecting, Active, Warning }
        
        [Header("Referências de Módulos")]
        public WifiDiscoveryManager discoveryManager;
        public SignalingClient signalingClient;
        public ARCoordinator arCoordinator;
        public YoloInferenceManager yoloAI;
        public RiskCalculator riskCalculator;
        public HeatmapManager heatmapManager;
        public HudController hudController;

        [Header("Configurações de Execução")]
        public float aiProcessInterval = 0.1f; // 10 FPS de IA para economizar bateria
        
        private SystemState currentState = SystemState.Idle;
        private float nextAiTick = 0f;

        private void Start()
        {
            SetState(SystemState.Searching);
        }

        private void Update()
        {
            if (currentState == SystemState.Active)
            {
                RunActivePipeline();
            }
        }

        private void RunActivePipeline()
        {
            // O pipeline de vídeo é gerenciado pelo WebRTC (Sprint 1)
            // Aqui orquestramos o fluxo de dados para a IA e Segurança
            
            if (Time.time >= nextAiTick)
            {
                // 1. Obter frame atual do streaming (Exemplo: de uma RenderTexture pública)
                // yoloAI.ExecuteInference(streamingTexture);
                
                // 2. Processar riscos baseados nas detecções (Simulado para integração)
                ProcessSystemSafety();
                
                nextAiTick = Time.time + aiProcessInterval;
            }
        }

        private void ProcessSystemSafety()
        {
            // Coleta dados de todos os módulos para gerar o feedback final
            List<RiskCalculator.ObjectRiskProfile> currentProfiles = new List<RiskCalculator.ObjectRiskProfile>();
            
            // Exemplo de lógica de unificação:
            // Para cada objeto detectado pela IA -> Calcular Risco -> Adicionar ao Heatmap
            
            heatmapManager.UpdateHeatmap(currentProfiles);
        }

        public void SetState(SystemState newState)
        {
            currentState = newState;
            Debug.Log($"[Orchestrator] Transição de estado: {newState}");
            // Atualizar HUD opcionalmente aqui
        }

        public void OnConnectionEstablished()
        {
            SetState(SystemState.Active);
        }

        public void OnConnectionLost()
        {
            SetState(SystemState.Searching);
            discoveryManager.StartDiscovery();
        }
    }
}
