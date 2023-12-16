using System.Collections.Generic;
using UnityEngine;

internal class ShaderArgs
{
    public Material[] materials;
    public float currentTimer;
    public float maxTimer;
}
public class ShaderSystem : MonoBehaviour
{
    public static ShaderSystem Instance { get; private set; }

    private List<ShaderArgs> shaderArgs = new();

}
