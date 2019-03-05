using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBox : MonoBehaviour
{
    public GameObject infoBox;
    private void OnTriggerEnter(Collider other)
    {
        infoBox.SetActive(!infoBox.activeInHierarchy);
    }
}
