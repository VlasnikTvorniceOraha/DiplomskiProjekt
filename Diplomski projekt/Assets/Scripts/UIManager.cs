using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject ui;

    public FirstPersonController controller;

    public string filename = "";

    public JSONPasrser jSONParser;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    public void SetFilename(string newName)
    {
        filename = newName;
    }

    public void GetJsonButton()
    {
        jSONParser.GetJSONFunc(filename);
        ui.SetActive(false);
        controller.cameraCanMove = true;
        controller.playerCanMove = true;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ui.activeSelf)
            {
                ui.SetActive(false);
                controller.cameraCanMove = true;
                controller.playerCanMove = true;
                Cursor.visible = false;
            }
            else if (!ui.activeSelf)
            {
                ui.SetActive(true);
                controller.cameraCanMove = false;
                controller.playerCanMove = false;
                Cursor.visible = true;
            }
        }
    }

}
