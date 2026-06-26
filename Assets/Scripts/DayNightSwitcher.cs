using System;
using UnityEngine;

public class DayNightSwitcher : MonoBehaviour
{
    public Material nightSky;
    public Material sunriseSky;
    public Material sunsetSky;
    public Material noonSky;

    public Light DirectionalLight;

    public float nightColorTemp;
    public float nightIntensity;
    public Color nightTint;

    public float sunsetColorTemp;
    public float sunsetIntensity;

    public float noonColorTemp;
    public float noonIntensity;

    public bool forceNight;
    public bool forceSunrise;
    public bool forceSunset;
    public bool forceNoon;

    enum TimeOfDay { None, Noon, Sunrise, Sunset, Night }
    TimeOfDay current = TimeOfDay.None;

    void Update()
    {
        int hour = DateTime.Now.Hour;

        TimeOfDay target;

        if (forceNight || hour >= 19 || hour < 5)
            target = TimeOfDay.Night;
        else if (forceSunrise || (hour >= 5 && hour < 8))
            target = TimeOfDay.Sunrise;
        else if (forceSunset || (hour >= 17 && hour < 19))
            target = TimeOfDay.Sunset;
        else if (forceNoon || (hour >= 8 && hour < 17))
            target = TimeOfDay.Noon;
        else
            return;

        if (target == current) return;

        current = target;

        switch (target)
        {
            case TimeOfDay.Night:
                ApplySkybox(nightSky, nightColorTemp, nightIntensity);
                DirectionalLight.color = nightTint;
                break;
            case TimeOfDay.Sunrise:
                ApplySkybox(sunriseSky, sunsetColorTemp, sunsetIntensity);
                DirectionalLight.color = Color.white;
                break;
            case TimeOfDay.Sunset:
                ApplySkybox(sunsetSky, sunsetColorTemp, sunsetIntensity);
                DirectionalLight.color = Color.white;
                break;
            case TimeOfDay.Noon:
                ApplySkybox(noonSky, noonColorTemp, noonIntensity);
                DirectionalLight.color = Color.white;
                break;
        }

    }

    void ApplySkybox(Material sky, float colorTemp, float intensity)
    {
        RenderSettings.skybox = sky;
        DirectionalLight.colorTemperature = colorTemp;
        DirectionalLight.intensity = intensity;
    }
}