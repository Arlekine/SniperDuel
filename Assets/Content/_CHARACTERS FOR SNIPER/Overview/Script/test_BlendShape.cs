using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test_BlendShape : MonoBehaviour
{
    public Slider slider1,slider2;
    SkinnedMeshRenderer mySkin;
    // Start is called before the first frame update
    void Start()
    {
        mySkin = GetComponent<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        mySkin.SetBlendShapeWeight(0, slider1.value*100);
        mySkin.SetBlendShapeWeight(1, slider2.value*100);
    }
}
