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
        
        private IWorker worker;
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
            
            // Backend recomendado para mobile: GPU (Vulkan/Metal)
            worker = WorkerFactory.CreateWorker(Unity.InferenceEngine.BackendType.GPUCompute, model);
            
            Debug.Log("[YoloAI] Modelo carregado e Worker inicializado na GPU.");
        }

        /// <summary>
        /// Executa a inferência em uma RenderTexture proveniente do streaming.
        /// </summary>
        public void ExecuteInference(RenderTexture sourceTexture)
        {
            // 1. Pré-processamento (Redimensionamento e Normalização via Compute Shader)
            using TensorFloat inputTensor = Unity.InferenceEngine.TextureConverter.ToTensor(sourceTexture, targetWidth, targetHeight, 3);
            
            // 2. Executar Inferência
            worker.Execute(inputTensor);
            
            // 3. Obter Saída
            TensorFloat outputTensor = worker.PeekOutput() as TensorFloat;
            
            // 4. Agendar processamento dos resultados (Async para não travar a main thread)
            ProcessResults(outputTensor);
        }

        private void ProcessResults(TensorFloat output)
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
