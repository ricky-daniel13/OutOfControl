using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource normalMusic, possMusic;
    bool toPoss=false;
    public float duration;
    float currentLerpTime, perc;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    void Start()
    {
        currentLerpTime = duration;
    }

    // Update is called once per frame
    void Update()
    {
        currentLerpTime += Time.unscaledDeltaTime;
        if (currentLerpTime > duration)
        {
            currentLerpTime = duration;
        }
        perc = currentLerpTime / duration;

        if (!toPoss)
        {
            normalMusic.volume = perc;
            possMusic.volume = 1 - perc;
        }
        else
        {
            normalMusic.volume = 1 - perc;
            possMusic.volume = perc;
        }

    }

    public void toPossMusic()
    {
        if (!toPoss)
        {
            toPoss = true;
            currentLerpTime = 0;
        }
    }
    public void toNormMusic()
    {
        if (toPoss)
        {
            toPoss = false;
            currentLerpTime = 0;
        }
    }
}
