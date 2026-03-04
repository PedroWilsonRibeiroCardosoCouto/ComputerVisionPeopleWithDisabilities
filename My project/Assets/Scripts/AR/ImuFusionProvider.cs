using UnityEngine;
using UnityEngine.InputSystem;

namespace LuckArkman.XR.AR
{
    /// <summary>
    /// Fornece fusão de dados de sensores (IMU) para predição de pose suave.
    /// </summary>
    public class ImuFusionProvider : MonoBehaviour
    {
        private Quaternion currentRotation = Quaternion.identity;
        private Vector3 gravityVector = Vector3.down;

        [Header("Configurações de Suavização")]
        [Range(0.01f, 1.0f)] public float filterCoefficient = 0.1f;

        private Accelerometer accelerometer;
        private AttitudeSensor attitudeSensor;

        private void Start()
        {
            accelerometer = InputSystem.GetDevice<Accelerometer>();
            attitudeSensor = InputSystem.GetDevice<AttitudeSensor>();

            if (accelerometer != null) InputSystem.EnableDevice(accelerometer);
            if (attitudeSensor != null) InputSystem.EnableDevice(attitudeSensor);
        }

        private void Update()
        {
            if (attitudeSensor != null)
            {
                Quaternion rawRot = attitudeSensor.attitude.ReadValue();
                Quaternion convertedRot = new Quaternion(rawRot.x, rawRot.y, -rawRot.z, -rawRot.w);
                currentRotation = Quaternion.Slerp(currentRotation, convertedRot, filterCoefficient);
            }

            if (accelerometer != null)
            {
                Vector3 rawAccel = accelerometer.acceleration.ReadValue();
                gravityVector = Vector3.Lerp(gravityVector, rawAccel, filterCoefficient);
            }
        }

        public Quaternion GetSmoothedRotation() => currentRotation;
        public Vector3 GetGravityVector() => gravityVector;
    }
}
