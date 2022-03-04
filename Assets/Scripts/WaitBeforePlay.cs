using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitBeforePlay : MonoBehaviour
{
    void Start()
    {
        GetComponent<AudioSource>()?.PlayDelayed(5.0f);
    }
}
