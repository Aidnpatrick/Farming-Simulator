using UnityEngine;

public class DirtScript : MonoBehaviour
{
    public bool isFull = false;
    public string currentPlant = "";
    public float growth = 0;
    private float growthStop = 70f;

    private Transform plantChild;
    private float baseY;

    void Update()
    {
        if (!isFull || plantChild == null)
            return;

        GrowPlant();
    }

    void GrowPlant()
    {
        if (growth >= growthStop)
            return;

        float growAmount = 0.001f;

        Vector3 scale = plantChild.localScale;
        scale.y = Mathf.Clamp(scale.y + growAmount, 0f, 1f);
        scale.x = 0.3f;
        scale.z = 1f;
        plantChild.localScale = scale;

        plantChild.position = new Vector3(
            plantChild.position.x,
            baseY + scale.y / 2f,
            plantChild.position.z
        );
        
        growth+=0.1f;
    }

    public void NewPlant()
    {
        if (transform.childCount == 0)
        {
            Debug.LogError("No plant child found on Dirt object.");
            return;
        }

        Debug.Log("Planted Plant");

        isFull = true;
        plantChild = transform.GetChild(0);

        plantChild.localScale = new Vector3(0.3f, 0f, 1f);

        plantChild.position += new Vector3(0f, -0.35f, 0f);

        baseY = plantChild.position.y;

        growth = 0;
    }
}
