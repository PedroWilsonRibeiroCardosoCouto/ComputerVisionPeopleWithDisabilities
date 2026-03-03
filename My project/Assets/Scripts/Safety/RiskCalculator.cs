using UnityEngine;
using System.Collections.Generic;
using LuckArkman.XR.AI;

namespace LuckArkman.XR.Safety
{
    /// <summary>
    /// Calcula o nível de risco de colisão baseado em distância, velocidade e direção.
    /// Implementa a fórmula: Risco = f(distância, velocidade, direção)
    /// </summary>
    public class RiskCalculator : MonoBehaviour
    {
        [Header("Parâmetros de Risco")]
        public float safetyRadius = 1.0f; // Verde
        public float cautionRadius = 2.5f; // Amarelo
        public float dangerRadius = 0.5f;  // Vermelho

        public struct ObjectRiskProfile
        {
            public int objectId;
            public float riskScore; // 0.0 a 1.0
            public Vector3 position;
            public Vector3 velocity;
        }

        /// <summary>
        /// Avalia o risco de um objeto detectado pela IA.
        /// </summary>
        public float CalculateScore(Vector3 userPos, Vector3 objectPos, Vector3 objectVelocity)
        {
            float distance = Vector3.Distance(userPos, objectPos);
            
            // 1. Fator Proximidade
            float proximityFactor = 1.0f - Mathf.Clamp01((distance - dangerRadius) / cautionRadius);
            
            // 2. Fator Vetor de Movimento (Se o objeto está vindo em direção ao usuário)
            Vector3 directionToUser = (userPos - objectPos).normalized;
            float alignmentFactor = Mathf.Max(0, Vector3.Dot(objectVelocity.normalized, directionToUser));
            
            // 3. Score Final
            float score = (proximityFactor * 0.7f) + (alignmentFactor * objectVelocity.magnitude * 0.3f);
            
            return Mathf.Clamp01(score);
        }
    }
}
