using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class GPSToUnity : MonoBehaviour
{

    //Skripta koja pretvara geografske koordinate u Unity koordinate

    //kutevi
    private UnityEngine.Vector2 topRightUnity = new UnityEngine.Vector2(143.1423f, 17.6407f);
    private UnityEngine.Vector2 topLeftUnity = new UnityEngine.Vector2(-1.445096f, 17.6407f);
    private UnityEngine.Vector2 bottomLeftUnity = new UnityEngine.Vector2(-1.445096f, -73.738f);
    private UnityEngine.Vector2 bottomRightUnity = new UnityEngine.Vector2(143.1423f, -73.738f);

    private UnityEngine.Vector2 topRightGPS = new UnityEngine.Vector2(45.813171f, 15.957474f);
    private UnityEngine.Vector2 topLeftGPS= new UnityEngine.Vector2(45.813171f, 15.955117f);
    private UnityEngine.Vector2 bottomRightGPS = new UnityEngine.Vector2(45.812075f, 15.957474f);
    private UnityEngine.Vector2 bottomLeftGPS = new UnityEngine.Vector2(45.812075f, 15.955117f);

    public UnityEngine.Vector2 ConvertGPSToUnity(UnityEngine.Vector2 GPS)
    {
        //default pozicija na koju ce se spawnati
        UnityEngine.Vector2 unityCoordinates = new UnityEngine.Vector2(76, -14);

        //provjeri je li pozicija unutar kutije, inace postavi na default poziciju
        if (GPS.y < topLeftGPS.y || GPS.y > topRightGPS.y || GPS.x < bottomLeftGPS.x || GPS.x > topRightGPS.x)
        {
            Debug.Log("Default koordinate jer je GPS neispravan");
            return unityCoordinates;
        }

        Debug.Log("Izracun novih koordinata");
        //144.5874
        float unityWidth = Mathf.Abs(topLeftUnity.x - topRightUnity.x);
        //91.3787
        float unityHeight = Mathf.Abs(topLeftUnity.y - bottomLeftUnity.y);

        //za gps su osi flipane
        //0.002357
        float GPSWidth = Mathf.Abs(topLeftGPS.y - topRightGPS.y);
        //0.001096
        float GPSHeight = Mathf.Abs(topLeftGPS.x - bottomLeftGPS.x);

        UnityEngine.Vector2 relativeVector = new UnityEngine.Vector2((GPS.x - bottomLeftGPS.x) / GPSHeight, (GPS.y - bottomLeftGPS.y) / GPSWidth);

        //Debug.Log("Relative vector " + relativeVector);

        unityCoordinates = new UnityEngine.Vector2(bottomLeftUnity.x + relativeVector.y * unityWidth, bottomLeftUnity.y + relativeVector.x * unityHeight);

        //Debug.Log("Vracam poziciju " + unityCoordinates);

        return unityCoordinates;
    }

    
}
