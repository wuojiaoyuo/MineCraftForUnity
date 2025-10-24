using System.Collections.Generic;
using MC;
using MC.Configurations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Test : MonoBehaviour
{
    void Update()
    {
        World.UpdateWorld(transform.position);
    }
}
