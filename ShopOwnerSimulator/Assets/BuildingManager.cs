using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using System.Threading;

[System.Serializable]
public class BuildingPreset
{
    [Header("Information")]
    public GameObject BuildingPrefab;
    public Sprite BuildingImage;
    public int Cost;
    public string buildingName;

    [Header("Classes")]
    public Classes BuildingClass;
    public enum Classes
    {
        Building,
        Station,
        Decoration,
        Fun
    };
}



public class BuildingManager : MonoBehaviour
{
    [Header("Presets")]
    public BuildingPreset[] buildingPresets;

    [Header("Stored Information")]
    private Vector3 MousePosition;
    Quaternion rotation;
    [SerializeField] private GameObject pendingObject;
    int Index;
    public bool gridActive;
    public float gridSize;

    [Header("Keybinds")]
    public KeyCode ConfirmBuildKey;
    public KeyCode ToggleGridKey;

    [Header("Components")]
    public Camera cam;
    private RaycastHit hit;
    public LayerMask layerMask;

    private void Update()
    {
        GetMousePosition();

        if(pendingObject != null)
        {
            if (gridActive)
            {
                pendingObject.transform.position = new Vector3(
                    RoundToNearestGrid(MousePosition.x),
                    RoundToNearestGrid(MousePosition.y),
                    RoundToNearestGrid(MousePosition.z));
            }
            else
            {
                pendingObject.transform.position = MousePosition;
            }



            if (Input.GetKeyDown(ConfirmBuildKey))
            {
                PlaceBuilding();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                pendingObject.transform.Rotate(0, +90, 0);
            }

            if (Input.GetKeyDown(ToggleGridKey))
            {
                ToggleGrid();
            }
        }
    }


    void PlaceBuilding()
    {
        pendingObject = null;
        Debug.Log(buildingPresets[Index].Cost);
    }

    public void GetMousePosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            MousePosition = hit.point;
        }
    }

    public void SelectBuilding(int ObjectIndex)
    {
        Index = ObjectIndex;
        BuildingPreset preset = buildingPresets[ObjectIndex];
        pendingObject = Instantiate(preset.BuildingPrefab, MousePosition, rotation);
    }

    public void ToggleGrid()
    {
        if (!gridActive)
        {
            gridActive = true;
        }
        else
        {
            gridActive = false;
        }
    }

    float RoundToNearestGrid(float pos)
    {
        float xDiff = pos % gridSize;
        pos -= xDiff;

        if (xDiff > (gridSize / 2))
        {
            pos += gridSize;
        }

        if (xDiff * (-1f) > (gridSize / 2))
        {
            pos -= gridSize;
        }

        return pos;
    }
}

