using UnityEngine;
using LuckArkman.XR.Networking;

namespace LuckArkman.XR.AR
{
    /// <summary>
    /// Sincroniza o sistema de coordenadas do Headset VR com a sessão AR local.
    /// </summary>
    public class SpatialSyncManager : MonoBehaviour
    {
        [SerializeField] private ARCoordinator arCoordinator;
        [SerializeField] private SignalingClient signalingClient;

        private Matrix4x4 vrToArMatrix = Matrix4x4.identity;
        private bool isSynced = false;

        public bool IsSynced => isSynced;

        /// <summary>
        /// Define a origem do mundo AR baseada em um ponto de calibração enviado pelo Headset.
        /// </summary>
        public void SyncWithHeadset(Vector3 vrPosition, Quaternion vrRotation)
        {
            if (!arCoordinator.TryGetPlacementPoint(out Pose arPose))
            {
                Debug.LogWarning("[SpatialSync] Falha ao sincronizar: Nenhum plano AR detectado para ancoragem.");
                return;
            }

            // Calcula a transformação necessária para alinhar VR com AR
            Matrix4x4 vrMatrix = Matrix4x4.TRS(vrPosition, vrRotation, Vector3.one);
            Matrix4x4 arMatrix = Matrix4x4.TRS(arPose.position, arPose.rotation, Vector3.one);

            vrToArMatrix = arMatrix * vrMatrix.inverse;
            isSynced = true;

            Debug.Log("[SpatialSync] Sincronização concluída com sucesso.");
        }

        /// <summary>
        /// Converte uma posição do espaço VR para o espaço AR local.
        /// </summary>
        public Vector3 VRToARPosition(Vector3 vrPos)
        {
            return vrToArMatrix.MultiplyPoint3x4(vrPos);
        }

        /// <summary>
        /// Converte uma rotação do espaço VR para o espaço AR local.
        /// </summary>
        public Quaternion VRToARRotation(Quaternion vrRot)
        {
            return vrToArMatrix.rotation * vrRot;
        }
    }
}
