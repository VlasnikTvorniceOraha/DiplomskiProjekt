using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class Generator : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private JSONPasrser jsonParser;

    [Header("Prefabs")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject wallOuterPrefab;
    [SerializeField] private GameObject wallInnerPrefab;
    [SerializeField] private GameObject roofPrefab;
    [SerializeField] private GameObject ceilingPrefab;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject doorOuterPrefab;
    [SerializeField] private GameObject doorInnerPrefab;
    [SerializeField] private GameObject windowPrefab;
    [SerializeField] private GameObject framePrefab;

    [Header("Parent")]
    [SerializeField] private GameObject parentGO;
    [SerializeField] private GameObject wallParentGO;
    [SerializeField] private GameObject atticParentGO;
    [SerializeField] private GameObject floorParentGO;

    private float antiFlickerOffset = 0.001f;
    private HouseInfo HouseInfo;

    public GPSToUnity gpsToUnity;


    void Start()
    {
        jsonParser.JSONParseAction -= CreateHouse;
        jsonParser.JSONParseAction += CreateHouse;
    }

    private void OnDestroy()
    {
        jsonParser.JSONParseAction -= CreateHouse;
    }

    /// <summary>
    /// Calls methods for creating house parts
    /// Calls CenterHouse after 0.5 seconds
    /// </summary>
    /// <param name="houseInfo"> HouseInfo object with data about the house - got from action subscription </param>
    private void CreateHouse(HouseInfo houseInfo)
    {
        //unisti proslu kucu ako postoji
        DestroyHouse();

        HouseInfo = houseInfo;
        CreateFloor(houseInfo.Floor);
        CreateWalls(HouseInfo.Walls);
        CreateAttic(HouseInfo.Attic);
        //call CenterHouse after 0.5 seconds
        Invoke("CenterHouse", 0.5f);
    }

    /// <summary>
    /// Centers the house in the scene
    /// </summary>
    private void CenterHouse()
    {
        //ocitaj GPS koordinate kuce i pomakni na tu lokaciju
        
        
        Vector2 XZHouse = gpsToUnity.ConvertGPSToUnity(new Vector2(HouseInfo.GPS.X, HouseInfo.GPS.Z));

        parentGO.transform.position = new Vector3(XZHouse.x, 0.01f, XZHouse.y);
        
        
    }

    /// <summary>
    /// Creates floor
    /// </summary>
    /// <param name="floor">Data about the floor</param>
    private void CreateFloor(Floor floor)
    {
        GameObject go = Instantiate(floorPrefab, new Vector3(floor.Position.X + floor.Dimension.X / 2, floor.Position.Z + floor.Dimension.Z / 2, floor.Position.Y + floor.Dimension.Y / 2), Quaternion.identity);
        Transform rect = go.GetComponent<Transform>();
        rect.SetParent(floorParentGO.transform);
        rect.localScale = new Vector3(floor.Dimension.X, floor.Dimension.Z, floor.Dimension.Y);
        Debug.Log("Floor position: " + go.GetComponent<Transform>().position);
    }

    /// <summary>
    /// Creates wall with windows and doors
    /// </summary>
    /// <param name="walls">Data about all walls, windows and doors</param>
    private void CreateWalls(List<Wall> walls)
    {
        for (int i = 0; i < walls.Count; i++)
        {
            if (walls[i].Position.R == 0)
                walls[i].Position.R = -180;

            if (walls[i].Position.R == 180)
                walls[i].Position.R = 0;
            
            // assumes that the first 4 walls in the list are the outer walls TODO po novom je u JSONu eksplicitno označeno koji su vanjski zidovi
            GameObject wallPrefab = i < 4 ? wallOuterPrefab : wallInnerPrefab;
            GameObject doorPrefab = i < 4 ? doorOuterPrefab : doorInnerPrefab;

            //  sort the doors and windows by local X position
            foreach (Door door in walls[i].Doors)
                walls[i].BuildingBlocks.Add(door);
            foreach (Window window in walls[i].Windows)
                walls[i].BuildingBlocks.Add(window);
            walls[i].sortDoorsAndWindows();

            GameObject wallEmpty = new GameObject("wall empty");
            
            // builds wall by stacking wall segments between windows and doors
            float currentPos = 0f;
            int counter = 0;
            foreach (BuildingBlock buildingBlock in walls[i].BuildingBlocks)    // buildingBlock is Door or Window
            {
                counter++;
                float bBlockPosition = buildingBlock.Position.X;
                
                GameObject wall = Instantiate(wallPrefab, wallEmpty.transform);
                wall.name = "wall"+counter;
                wall.transform.position -= new Vector3(currentPos, 0f, 0f);
                wall.transform.localScale = new Vector3(bBlockPosition - currentPos, walls[i].Dimension.Z, walls[i].Dimension.Y);

                GameObject opening; // opening = door or window
                if (buildingBlock.GetType() == typeof(Door))    // if door
                {
                    opening = Instantiate(doorPrefab, wallEmpty.transform);
                    opening.name = "door"+counter;
                    Transform doorHole = opening.transform.GetChild(0);
                    Transform topWall = opening.transform.GetChild(1);

                    //float antiFlickerOffset = 0.001f;   // prevents Z fighting/flicker
                    opening.transform.position -= new Vector3(bBlockPosition, 0f, buildingBlock.Position.Y);

                    topWall.position = doorHole.position + new Vector3(0f, buildingBlock.Dimension.Z, 0f);
                    topWall.localScale = new Vector3(buildingBlock.Dimension.X, walls[i].Dimension.Z - buildingBlock.Dimension.Z, walls[i].Dimension.Y);
                    
                    float doorThickness = 0.1f;
                    doorHole.localScale = new Vector3(buildingBlock.Dimension.X, buildingBlock.Dimension.Z, doorThickness);
                    doorHole.localRotation = Quaternion.Euler(0f, -90f, 0f);    // open the door
                }
                else  // if window
                {
                    opening = Instantiate(windowPrefab, wallEmpty.transform);
                    opening.name = "window"+counter;

                    Transform windowPane = opening.transform.GetChild(0);
                    Transform bottomWall = opening.transform.GetChild(1);
                    Transform topWall = opening.transform.GetChild(2);
                    
                    opening.transform.position -= new Vector3(bBlockPosition, 0f, buildingBlock.Position.Y);
                    bottomWall.localScale = new Vector3(buildingBlock.Dimension.X, buildingBlock.Position.Z,walls[i].Dimension.Y);

                    windowPane.position = opening.transform.position + new Vector3(0f, buildingBlock.Position.Z, 0f);
                    windowPane.localScale = new Vector3(buildingBlock.Dimension.X, buildingBlock.Dimension.Z, walls[i].Dimension.Y);

                    topWall.position = windowPane.position + new Vector3(0f, buildingBlock.Dimension.Z, 0f);
                    topWall.localScale = new Vector3(buildingBlock.Dimension.X,

                        walls[i].Dimension.Z - (buildingBlock.Position.Z + buildingBlock.Dimension.Z),
                        walls[i].Dimension.Y);
                    //opening.transform.localScale = new Vector3(buildingBlock.Dimension.X, buildingBlock.Dimension.Z, buildingBlock.Dimension.Y);
                }
                // positioning doorframe or windowframe
                {
                    GameObject frame = Instantiate(framePrefab, opening.transform);
                    frame.name = "frame";

                    float frameThickness = 0.06f;
                    float frameExtrusionDepth = 0.02f; // how much the frame sticks out from the wall
                    
                    Transform rightFrame = frame.transform.GetChild(0);
                    Transform leftFrame = frame.transform.GetChild(1);
                    Transform topFrame = frame.transform.GetChild(2);
                    Transform bottomFrame = frame.transform.GetChild(3);
                    
                    frame.transform.localScale = new Vector3(1, 1, walls[i].Dimension.Y + 2*frameExtrusionDepth);
                    frame.transform.position -= new Vector3(0, 0, frameExtrusionDepth);

                    rightFrame.localScale = leftFrame.localScale = new Vector3(frameThickness, buildingBlock.Dimension.Z, 1f);
                    topFrame.localScale = bottomFrame.localScale = new Vector3(buildingBlock.Dimension.X + 2*frameThickness, frameThickness, 1f);

                    rightFrame.localPosition = new Vector3(-antiFlickerOffset, 0, 0);
                    leftFrame.localPosition = new Vector3(-buildingBlock.Dimension.X + antiFlickerOffset, 0, 0);
                    bottomFrame.localPosition = new Vector3(frameThickness, antiFlickerOffset, 0);
                    topFrame.localPosition = new Vector3(frameThickness, buildingBlock.Dimension.Z - antiFlickerOffset, 0);
                    
                    
                    if (buildingBlock.GetType() == typeof(Door))
                        bottomFrame.gameObject.SetActive(false);
                    else if (buildingBlock.GetType() == typeof(Window))
                    {
                        frame.transform.position += new Vector3(0, buildingBlock.Position.Z, 0);
                    }
                }
                
                currentPos = bBlockPosition + buildingBlock.Dimension.X;
            }

            
            // cap it all off with and final wall segment
            GameObject endWall = Instantiate(wallPrefab, wallEmpty.transform);
            endWall.name = "wallEnd";
            endWall.transform.position -= new Vector3(currentPos, 0f, 0f);
            endWall.transform.localScale = new Vector3(walls[i].Dimension.X - currentPos, walls[i].Dimension.Z, walls[i].Dimension.Y);

            //  position and rotate the whole wall structure into place
            wallEmpty.transform.position = new Vector3(walls[i].Position.X, walls[i].Position.Z, walls[i].Position.Y);
            wallEmpty.transform.localRotation = Quaternion.Euler(0, walls[i].Position.R, 0);
            wallEmpty.transform.SetParent(wallParentGO.transform);
        }
    }

    /// <summary>
    /// Creates attic
    /// </summary>
    /// <param name="attic">Data about the roof</param>
    private void CreateAttic(Attic attic)
    {
        //attic floor
        // GameObject go = Instantiate(roofPrefab, new Vector3(attic.Floor.Position.X + attic.Floor.Dimension.X / 2, attic.Floor.Position.Z + attic.Floor.Dimension.Z / 2, attic.Floor.Position.Y + attic.Floor.Dimension.Y / 2), Quaternion.identity);
        GameObject go = Instantiate(ceilingPrefab, new Vector3(attic.Floor.Position.X + attic.Floor.Dimension.X / 2, attic.Floor.Position.Z + attic.Floor.Dimension.Z / 2, attic.Floor.Position.Y + attic.Floor.Dimension.Y / 2), Quaternion.identity);
        Transform rect = go.GetComponent<Transform>();
        rect.transform.SetParent(atticParentGO.transform);
        rect.localScale = new Vector3(attic.Floor.Dimension.X, attic.Floor.Dimension.Z, attic.Floor.Dimension.Y);

        for (int i = 0; i < attic.AtticSegments.Count; i++)
        {
            float x = attic.AtticSegments[i].Position.X - attic.AtticSegments[i].Dimension.X / 2;
            float y = attic.AtticSegments[i].Position.Z - attic.AtticSegments[i].Dimension.Y / 2;
            float z = attic.AtticSegments[i].Position.Y + attic.AtticSegments[i].Dimension.Z / 2;

            go = Instantiate(roofPrefab, new Vector3(x, y, z), Quaternion.identity);
            rect = go.GetComponent<Transform>();
            rect.transform.SetParent(atticParentGO.transform);
            rect.localScale = new Vector3(attic.AtticSegments[i].Dimension.X, attic.AtticSegments[i].Dimension.Z, attic.AtticSegments[i].Dimension.Y);
            rect.localRotation = Quaternion.Euler(-attic.AtticSegments[i].Position.R, 0, 0);
        }

        //attic roof
        go = Instantiate(roofPrefab, new Vector3(attic.Roof.Position.X + attic.Roof.Dimension.X / 2, attic.Roof.Position.Z + attic.Roof.Dimension.Z / 2, attic.Roof.Position.Y + attic.Roof.Dimension.Y / 2), Quaternion.identity);
        rect = go.GetComponent<Transform>();
        rect.transform.SetParent(atticParentGO.transform);
        rect.localScale = new Vector3(attic.Roof.Dimension.X, attic.Roof.Dimension.Z, attic.Roof.Dimension.Y);
        rect.localRotation = Quaternion.Euler(-attic.Roof.Pitch, 0, 0);
    }

    public void DestroyHouse()
    {
        foreach (Transform child in wallParentGO.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in atticParentGO.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in floorParentGO.transform)
        {
            Destroy(child.gameObject);
        }

        parentGO.transform.position = Vector3.zero;
    }

    


}