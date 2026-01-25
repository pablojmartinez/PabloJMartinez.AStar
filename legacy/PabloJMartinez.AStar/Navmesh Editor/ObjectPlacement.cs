// Copyright (c) 2013-2017 Pablo J. Martínez. All rights reserved.
// Licensed under the MIT License.
// Part of the legacy PabloJMartinez.AStar implementation.

using UnityEngine;
using System.Collections;

namespace ComingLights
{
    public class ObjectPlacement : MonoBehaviour
    {
        private static ObjectPlacement objectPlacement;
        private Ray cameraToMouseRay;
        private RaycastHit raycastHit;
        public static GameObject TheObject { get; private set; }

        private void Awake()
        {
            objectPlacement = GetComponent<ObjectPlacement>();
        }

        private void Update()
        {
            if(TheObject != null)
            {
                if(Input.GetKeyDown(KeyCode.Mouse0))
                {
                    TheObject = GameObject.Instantiate<GameObject>(TheObject);
                }
                //!cameraToMouseRay = CLCamera.MainCamera.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(cameraToMouseRay, out raycastHit, 100);//?, CLCamera.StaticCollidersLayerMask);
                Vector3 mousePosition = new Vector3(raycastHit.point.x, raycastHit.point.y + 0.1f, raycastHit.point.z);
                TheObject.transform.position = mousePosition;
            }
        }

        public static void Enable(GameObject model)
        {
            if(model != null)
            {
                if(TheObject != null)
                {
                    Destroy(TheObject);
                }
                //TheObject = GameObject.Instantiate<GameObject>(model);
                objectPlacement.gameObject.SetActive(true);
            }
        }

        public static void Disable()
        {
            if(TheObject != null)
            {
                Destroy(TheObject);
                objectPlacement.gameObject.SetActive(false);
            }
        }
    }
}