using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sistem : MonoBehaviour
{
    public static Sistem instance;

    public int ID; //id memanggil urutan buah
    public GameObject TempatSpawn; //tempat spawn object
    public GameObject[] KoleksiBuah; //array koleksi buah
    public GameObject Gui_Utama;
    public AudioClip[] SuaraBuah;
    AudioSource Suara;
    
    //akan diakses sebelum start
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ID = 0;
        //SpawnObject();
        Gui_Utama.SetActive(false);
        Suara = gameObject.AddComponent<AudioSource>();
    }

    public void SpawnObject()
    {
        //menghancurkan benda sebelumnya dengan tag "buah"
        GameObject BedaSebelumnya = GameObject.FindGameObjectWithTag("Buah");
        //menghancurkan benda sebelumnya jika ada bendanya
        if(BedaSebelumnya != null) Destroy(BedaSebelumnya);

        //memunculkan benda gameoject
        GameObject Object = Instantiate(KoleksiBuah[ID]);
        //memasukkan gambar yang ingin dimunculkan dalam TempatSpawn
        Object.transform.SetParent(TempatSpawn.transform, false);
        //setting ukuran gambar
        Object.transform.localScale = new Vector3 (2906,2906,2906);

        TempatSpawn.GetComponent<Animation>().Play("PopUp");
        KumpulanSuara.instance.Panggil_Suara(1);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeFruit(true);
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeFruit(false);
        }
    }

    //jika tombol arrow keyboard ditekan, gambar akan berubah
    public void ChangeFruit(bool kanan)
    {
        if(kanan)
        {
            if(ID >= KoleksiBuah.Length - 1)
            {
                ID = 0;
            }
            else
            {
                ID++;
            }
        }
        else
        {
            if(ID <= 0)
            {
                ID = KoleksiBuah.Length - 1;
            }
            else
            {
                ID--;
            }
        }

        SpawnObject();
        PanggilSuaraBuah();
    }
    public void PanggilSuaraBuah()
    {
        Suara.clip = SuaraBuah[ID];
        Suara.Play();
    }
}
