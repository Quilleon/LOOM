using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        
        transform.GetChild(Random.Range(0, transform.childCount)).gameObject.SetActive(true);
    }
}
