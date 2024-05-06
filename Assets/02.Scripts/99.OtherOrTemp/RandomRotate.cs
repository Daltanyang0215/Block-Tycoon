using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotate : MonoBehaviour
{
    private Vector3 rotateEuler;

    private void Start()
    {
       rotateEuler = new Vector3 (Random.value, Random.value , Random.value) * Time.deltaTime * Random.value*60;
    }

    private void Update()
    {
        transform.Rotate (rotateEuler);
    }

    private void OnMouseDrag()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;
    }
}
