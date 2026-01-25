// Copyright (c) 2013-2017 Pablo J. Martínez. All rights reserved.
// Licensed under the MIT License.
// Part of the legacy PabloJMartinez.AStar implementation.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ComingLights
{
    public class ApplyMeshChanges : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private InputField inputFieldType;
        [SerializeField]
        private InputField inputFieldcolorR;
        [SerializeField]
        private InputField inputFieldcolorG;
        [SerializeField]
        private InputField inputFieldcolorB;

        public void OnPointerClick(PointerEventData eventData)
        {
            int meshesType = int.Parse(inputFieldType.text);
            Color meshesColor = Navmesh.NavmeshMeshesTypesColors[Navmesh.Active][0];
            bool shouldTheColorChange = false;
            if(inputFieldcolorR.text != null && inputFieldcolorG.text != null && inputFieldcolorB.text != null)
            {
                meshesColor = new Color(float.Parse(inputFieldcolorR.text), float.Parse(inputFieldcolorG.text), float.Parse(inputFieldcolorB.text));
                Navmesh.NavmeshMeshesTypesColors[Navmesh.Active][meshesType] = meshesColor;
                shouldTheColorChange = true;
            }
            int selectedMesh;
            NavmeshMesh selectedNavmeshMesh;
            int selectedMeshesCount = NavmeshGenerator.SelectedMeshes.Count;
            for(int i = 0; i < selectedMeshesCount; i++)
            {
                selectedMesh = NavmeshGenerator.SelectedMeshes[i];
                selectedNavmeshMesh = Navmesh.NavmeshMeshes[Navmesh.Active][selectedMesh];
                Navmesh.NavmeshMeshes[Navmesh.Active][selectedMesh] = new NavmeshMesh(selectedNavmeshMesh.Navmesh,
                                                                                      selectedNavmeshMesh.Mesh,
                                                                                      meshesType,
                                                                                      selectedNavmeshMesh.Weight,
                                                                                      selectedNavmeshMesh.GameObject);
                if(shouldTheColorChange == true)
                {
                    selectedNavmeshMesh.GameObject.GetComponent<Renderer>().material.color = meshesColor;
                }
            }
        }
    }
}