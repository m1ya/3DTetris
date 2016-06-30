using UnityEngine;
using System.Collections;

public class SoundScript : MonoBehaviour {

    public AudioSource bgmSource;

    //有無のチェックフラグ
    private static bool created = false;

    void Awake()
    {

        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    void Update()
    {
        if (Application.loadedLevelName == "Main")
        {
            Destroy(this.gameObject);
        }
    }
}
