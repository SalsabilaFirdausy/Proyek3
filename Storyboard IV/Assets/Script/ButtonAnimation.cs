using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    public void Animation()
    {
        GetComponent<Animation>().Play("Button");
        KumpulanSuara.instance.Panggil_Suara(0);
    }
}
