using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizableCharacter : MonoBehaviour
{
    public ColorPicker skinColorPicker, hairColorPicker;
    public Material skin_Mat, hair_Mat;

    void OnEnable()
    {
        if (skinColorPicker)
        {
            ColorPicker.SkinChangeEvent += SetSkinColor; // subscribing to the event. 
        }
        if (hairColorPicker)
        {
            ColorPicker.HairChangeEvent += SetHairColor; // subscribing to the event. 
        }
    }
    void SetSkinColor(Texture newTex)
    {
        skin_Mat.mainTexture = newTex;
    }

    void SetHairColor(Texture newTex)
    {
        hair_Mat.mainTexture = newTex;
    }
}
