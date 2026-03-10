using UnityEngine;
using System.Collections.Generic;

namespace LuckArkman.XR.Safety
{
    /// <summary>
    /// Gerencia os dados para o Shader de Mapa de Calor.
    /// Alimenta o Global Shader Buffer com as posições de risco.
    /// </summary>
    public class HeatmapManager : MonoBehaviour
    {
        [Header("Configurações do Heatmap")]
        public Material heatmapMaterial;
        public int maxPoints = 50;

        private Vector4[] points;
        private float[] intensities;

        private void Start()
        {
            points = new Vector4[maxPoints];
            intensities = new float[maxPoints];
        }

        /// <summary>
        /// Atualiza o shader com os pontos de calor atuais.
        /// Vector4 format: (x, y, z, intensity)
        /// </summary>
        public void UpdateHeatmap(List<RiskCalculator.ObjectRiskProfile> profiles)
        {
            for (int i = 0; i < maxPoints; i++)
            {
                if (i < profiles.Count)
                {
                    points[i] = new Vector4(
                        profiles[i].position.x, 
                        profiles[i].position.y, 
                        profiles[i].position.z, 
                        profiles[i].riskScore
                    );
                }
                else
                {
                    points[i] = Vector4.zero;
                }
            }

            // Envia para o pipeline de renderização (Shader Global)
            Shader.SetGlobalVectorArray("_HeatmapPoints", points);
            Shader.SetGlobalInt("_HeatmapCount", Mathf.Min(profiles.Count, maxPoints));
        }
    }
}
