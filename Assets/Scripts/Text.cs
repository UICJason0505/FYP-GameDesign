using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Text : MonoBehaviour
{
    [SerializeField] public Chess chess;
    private TextMeshPro text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        if (text != null){
            text.text = chess.number.ToString();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!string.Equals(text.text, chess.number.ToString()))
        {
            text.text = chess.number.ToString();
        }
    }
}
