using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LanternManager : MonoBehaviour
{
    [SerializeField] private Sprite lightSprite;
    [SerializeField] private Sprite darkSprite;
    SpriteRenderer[] lanterns;
    Light2D[] lights;
    void Start()
    {
        lanterns = GetComponentsInChildren<SpriteRenderer>();
        lights = GetComponentsInChildren<Light2D>();
    }

    public void DimLanterns() {
        StartCoroutine(ToggleLanterns(darkSprite));
    }

    public void LightLanterns() {
        StartCoroutine(ToggleLanterns(lightSprite));
    }

    private IEnumerator ToggleLanterns(Sprite newSprite) {
        foreach(Light2D l in lights) {
            l.enabled = !l.enabled;
        }
        foreach(SpriteRenderer r in lanterns) {
            r.sprite = newSprite;
        }
        yield return null;
    }
}
