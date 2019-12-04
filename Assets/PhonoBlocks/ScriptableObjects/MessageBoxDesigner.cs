using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu (menuName = "MessageBox/NewDesign")]
public class MessageBoxDesigner : ScriptableObject
{
    [System.Serializable]
    private struct IconSet
    {
        public Icons icon;
        public Sprite IconRenderer;
    }
    
    
    [System.Serializable]
    private struct ColorSet
    {
        public Colors color;
        public Color colorValue;
    }
    
    public enum Colors
    {
        Red,
        Green,
        Blue,
    }
    
    [HideInInspector]
    public Colors color;

    public enum Icons
    {
        Ok,
        Close,
        Info,
    }


    [SerializeField]
    private IconSet[] icons;
    
    [SerializeField]
    private ColorSet[] colors;

    private Dictionary<Icons, Sprite> iconDictionary;
    private Dictionary<Colors, Color> colorDictionary;
    private void OnEnable()
    {
         iconDictionary = new Dictionary<Icons, Sprite>();
        for (int i = 0; i < icons.Length; i++)
        {
            iconDictionary.Add(icons[i].icon,icons[i].IconRenderer);
        }
        
        colorDictionary = new Dictionary<Colors, Color>();
        for (int i = 0; i < colors.Length; i++)
        {
            colorDictionary.Add(colors[i].color, colors[i].colorValue);

        }
        
    }

    public Color SetColor(Colors c)
    {
        return colorDictionary[c];
    }

    public Sprite Icon(Icons icon)
    {
        return iconDictionary[icon];
    }
    
    
    
}
