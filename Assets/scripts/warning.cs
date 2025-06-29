using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class warning : MonoBehaviour
{
    public void CloseThis()
    {
        Destroy(this.gameObject);
    }
}
