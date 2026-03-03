using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

namespace LuckArkman.XR.AR
{
    /// <summary>
    /// Gerencia a sessão AR, detecção de planos e ancoragem inicial.
    /// </summary>
    [RequireComponent(typeof(ARSessionOrigin))]
    [RequireComponent(typeof(ARPlaneManager))]
    [RequireComponent(typeof(ARRaycastManager))]
    public class ARCoordinator : MonoBehaviour
    {
        private ARSessionOrigin sessionOrigin;
        private ARPlaneManager planeManager;
        private ARRaycastManager raycastManager;

        public bool isInitialized { get; private set; }

        private void Awake()
        {
            sessionOrigin = GetComponent<ARSessionOrigin>();
            planeManager = GetComponent<ARPlaneManager>();
            raycastManager = GetComponent<ARRaycastManager>();
        }

        private void OnEnable()
        {
            planeManager.planesChanged += OnPlanesChanged;
        }

        private void OnDisable()
        {
            planeManager.planesChanged -= OnPlanesChanged;
        }

        private void OnPlanesChanged(ARPlanesChangedEventArgs args)
        {
            if (args.added.Count > 0 && !isInitialized)
            {
                Debug.Log("[ARCoordinator] Primário plano detectado. Sistema AR pronto.");
                isInitialized = true;
            }
        }

        /// <summary>
        /// Realiza um Raycast contra os planos detectados para posicionar elementos no mundo real.
        /// </summary>
        public bool TryGetPlacementPoint(out Pose pose)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

            if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                pose = hits[0].pose;
                return true;
            }

            pose = Pose.identity;
            return false;
        }
    }
}
