using UnityEngine;
using UnityEngine.InputSystem;

namespace LuckArkman.XR.AR
{
    /// <summary>
    /// Fornece fusão de dados de sensores (IMU) para predição de pose suave.
    /// Complementa o rastreamento visual do ARCore.
    /// </summary>
    public class ImuFusionProvider : MonoBehaviour
    {
        private Quaternion currentRotation = Quaternion.identity;
        private Vector3 gravityVector = Vector3.down;

        [Header("Configurações de Suavização")]
        [Range(0.01f, 1.0f)] public float filterCoefficient = 0.1f;

        private void Start()
        {
            if (AttitudeSensor.current != null)
                InputSystem.EnableDevice(AttitudeSensor.current);
            
            if (Accelerometer.current != null)
                InputSystem.EnableDevice(Accelerometer.current);
        }

        private void Update()
        {
            UpdateRotation();
            UpdateGravity();
        }

        private void UpdateRotation()
        {
            if (AttitudeSensor.current != null)
            {
                Quaternion rawRot = AttitudeSensor.current.attitude.ReadValue();
                // Conversão de coordenadas (Device to Unity)
                Quaternion convertedRot = new Quaternion(rawRot.x, rawRot.y, -rawRot.z, -rawRot.w);
                currentRotation = Quaternion.Slerp(currentRotation, convertedRot, filterCoefficient);
            }
        }

        private void UpdateGravity()
        {
            if (Accelerometer.current != null)
            {
                Vector3 rawAccel = Accelerometer.current.acceleration.ReadValue();
                gravityVector = Vector3.Lerp(gravityVector, rawAccel, filterCoefficient);
            }
        }

        public Quaternion GetSmoothedRotation() => currentRotation;
        public Vector3 GetGravityVector() => gravityVector;
    }
}
