using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lives : MonoBehaviour
{
    int counter = 3;
    public void Damage()
    {
        Destroy(GameObject.Find("Life (" + counter + ")"));
    }
}
