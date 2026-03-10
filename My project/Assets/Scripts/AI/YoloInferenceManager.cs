using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace LuckArkman.XR.AI
{
    /// <summary>
    /// Gerencia a inferência do modelo YOLO usando Unity Sentis na GPU.
    /// </summary>
    public class YoloInferenceManager : MonoBehaviour
    {
        [Header("Configurações do Modelo")]
        public Unity.InferenceEngine.ModelAsset modelAsset;
        public ComputeShader preProcessShader;
        
        private Unity.InferenceEngine.Worker worker;
        private Unity.InferenceEngine.Model model;
        
        [Header("Parâmetros de Detecção")]
        [Range(0, 1)] public float confidenceThreshold = 0.5f;
        public int targetWidth = 640;
        public int targetHeight = 640;

        private void Start()
        {
            if (modelAsset != null)
            {
                LoadModel();
            }
        }

        private void LoadModel()
        {
            model = Unity.InferenceEngine.ModelLoader.Load(modelAsset);
            
            // Em Sentis 2.x / Inference Engine, WorkerFactory.CreateWorker foi substituído pelo construtor de Worker
            worker = new Unity.InferenceEngine.Worker(model, Unity.InferenceEngine.BackendType.GPUCompute);
            
            Debug.Log("[YoloAI] Modelo carregado e Worker inicializado na GPU.");
        }

        /// <summary>
        /// Executa a inferência em uma RenderTexture proveniente do streaming.
        /// </summary>
        public void ExecuteInference(RenderTexture sourceTexture)
        {
            // 1. Pré-processamento e normalização
            Unity.InferenceEngine.Tensor<float> inputTensor = Unity.InferenceEngine.TextureConverter.ToTensor(sourceTexture, targetWidth, targetHeight, 3);
            
            // 2. Agendar Inferência (Assíncrono)
            worker.Schedule(inputTensor);
            
            // 3. Obter Saída (Referência não bloqueante)
            Unity.InferenceEngine.Tensor<float> outputTensor = worker.PeekOutput() as Unity.InferenceEngine.Tensor<float>;
            
            // 4. Processar resultados
            ProcessResults(outputTensor);
            
            // Liberar tensores temporários
            inputTensor.Dispose();
        }

        private void ProcessResults(Unity.InferenceEngine.Tensor<float> output)
        {
            // Aqui entra a lógica de decodificação das Bounding Boxes específicas do YOLO
            // Como cada versão do YOLO (v8, v10, v11) tem um formato de saída,
            // esta parte é customizada conforme o arquivo .onnx utilizado.
            
            // Debug básico de saída
            // Debug.Log($"[YoloAI] Shape de saída: {output.shape}");
        }

        private void OnDestroy()
        {
            worker?.Dispose();
        }
    }
}
