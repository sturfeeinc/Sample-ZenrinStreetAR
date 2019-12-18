using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Constants;

namespace Sturfee.Unity.XR.Package.Utilities
{
    public class ToggleBuildings : MonoBehaviour {

        [SerializeField]
        private GameObject ToggleButton;


        private List<MeshFilter> _buildings;
        private List<MeshFilter> _terrain;

        private bool _isToggleOn = false;

        private void Awake()
        {
            SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
        }

        private void OnDestroy()
        {
            SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
        }


        public void ToggleDebugBuildings()
        {
            _isToggleOn = !_isToggleOn;

            foreach(MeshFilter mf in _buildings)
            {
                if(_isToggleOn)
                {
                    mf.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/BuildingDebugMaterial");
                    mf.GetComponent<Renderer>().material.color = GetRandomColor();
                }
                else
                {
                    mf.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/BuildingHide");
                }
            }
            foreach(MeshFilter mf in _terrain)
            {
                if (_isToggleOn)
                {
                    mf.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/BuildingDebugMaterial");
                    mf.GetComponent<Renderer>().material.color = GetRandomColor();
                }
                else
                {
                    mf.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/BuildingHide");
                }
            }

        }

        private void OnSessionReady()
        {

            _buildings = new List<MeshFilter>();
            _terrain = new List<MeshFilter>();
            foreach (MeshFilter mf in FindObjectsOfType<MeshFilter>())
            {
                if (mf.gameObject.layer == LayerMask.NameToLayer(SturfeeLayers.Building))
                {
                    _buildings.Add(mf);
                }

                else if (mf.gameObject.layer == LayerMask.NameToLayer(SturfeeLayers.Terrain))
                {
                    _terrain.Add(mf);
                }

            }

            //Uncomment this, if we want debug buildings to show up when session starts
            //ToggleDebugBuildings();

        }

        private Color GetRandomColor()
        {
            var colors = new List<Color>() {
                new Color(0, 0.7176f, 0.9608f, 0.4f),           // R: 0 G: 182 B: 245
                new Color(0, 0.55f, 0.851f, 0.4f),              // R: 0 G: 140 B: 217
                new Color(0, 0.298f, 0.655f, 0.4f),             // R: 0 G: 76 B: 167
                new Color(0.408f, 0.1608f, 0.5804f, 0.4f),      // R: 104 G: 41 B: 148
                new Color(0, 0.7176f, 0.149f, 0.4f),            // R: 151 G: 39 B: 123
                new Color(0, 0.1373f, 0.3686f, 0.4f),           // R: 192 G: 35 B: 94
                new Color(0.933f, 0.1373f, 0.149f, 0.4f),       // R: 238 G: 35 B: 38
                new Color(0.953f, 0.4039f, 0.024f, 0.4f),       // R: 243 G: 103 B: 6
                new Color(1, 0.7686f, 0, 0.4f),                 // R: 255 G: 196 B: 0
                new Color(1, 0.9294f, 0, 0.4f),                 // R: 255 G: 237 B: 2
                new Color(0.8f, 0.8588f, 0, 0.4f),              // R: 204 G: 219 B: 2
                new Color(0.54f, 0.78f, 0.1804f, 0.4f),         // R: 138 G: 199 B: 46
                new Color(0, 0.655f, 0.216f, 0.4f),             // R: 0 G: 167 B: 55
                new Color(0, 0.54f, 0.29f, 0.4f),               // R: 0 G: 138 B: 74
            };

            var rnd = new System.Random();

            return colors[rnd.Next(0, colors.Count)];
        }

    }
}