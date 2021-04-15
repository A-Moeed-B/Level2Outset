using UnityEngine;

public class TileMap : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer renderer;
    
    void Start()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
       

    }
    public void changeColor()
    {
        Debug.Log("in change color");
        renderer.color = Color.blue;
    }
    // Update is called once per frame
    void Update()
    {
       if(Input.GetMouseButtonDown(0))
        {
            changeColor();
        }
    }
}
