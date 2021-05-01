using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{

    public float backSpeed = .1f;
    private MeshRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Background " + backSpeed);
        float x = Time.time * backSpeed;
        Vector2 offset = new Vector2(x, 0);
        renderer.sharedMaterial.SetTextureOffset("_MainTex",offset);
    }
}
