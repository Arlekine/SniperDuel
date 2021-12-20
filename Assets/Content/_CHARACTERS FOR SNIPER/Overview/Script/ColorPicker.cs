using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public Texture currentSkin;


    public delegate void ChangeEvent(Texture newTexture); //I do declare!
    public static event ChangeEvent SkinChangeEvent;  // create an event variable 
    public static event ChangeEvent HairChangeEvent;

    public void SendColor(Texture image)
    {
       // SkinChangeEvent?.Invoke(image);
    }

    public void SendHairColor(Texture image)
    {
       // HairChangeEvent?.Invoke(image);
    }

}
