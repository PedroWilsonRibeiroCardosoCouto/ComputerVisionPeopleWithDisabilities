using UnityEngine;
using System.Collections.Generic;

namespace LuckArkman.XR.AI
{
    [System.Serializable]
    public struct DetectionResult
    {
        public Rect box;
        public float confidence;
        public int classId;
        public string label;
    }

    /// <summary>
    /// Decodifica a saída bruta do Sentis em objetos DetectionResult.
    /// </summary>
    public static class YoloOutputParser
    {
        public static List<DetectionResult> Parse(float[] outputData, int outputCount, float threshold)
        {
            List<DetectionResult> results = new List<DetectionResult>();

            // Exemplo de parse genérico para YOLO (Ajustar conforme o modelo específico)
            // YOLOv8 geralmente tem saída [1, 84, 8400]
            
            for (int i = 0; i < outputCount; i++)
            {
                // Lógica de Non-Max Suppression (NMS) e extração de Box/Class
                // ...
            }

            return results;
        }
    }
}
