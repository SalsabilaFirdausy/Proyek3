using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//agar kita bisa pindah ke scene lain

public class CanvasTransisi : MonoBehaviour
{
   public static string NamaScene;

    public void btn_move(string nama)
    {
        //KumpulanSuara.instance.Panggil_Suara(0);
        this.gameObject.SetActive(true);
        NamaScene = nama;
        GetComponent<Animator>().Play("end");
    }

    public void Object_InActive()
    {
        this.gameObject.SetActive(false);
    }

    public void Pindah_Scene()
    {
        SceneManager.LoadScene(NamaScene);
    }

    public void btn_exit()
    {
        Application.Quit();
    }
}
