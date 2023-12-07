using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SetColor : MonoBehaviour {

    public Color color;
    public UnityEvent<Color> setColor;

    public void DoSetColor() {
        setColor?.Invoke(color);
    }

    
    public void DoSetColor(Color _color) {
        color = _color;
        setColor?.Invoke(color);
    }
}