using UnityEngine;
using UnityEngine.UI;

//[ExecuteAlways]
public class DayNightManager : MonoBehaviour
{
    [SerializeField] private Light dLight;
    [SerializeField] private LightingPreset lightPreset;
    [SerializeField, Range(0, 24)] private float timeOfADay;
    [SerializeField] private Text dayText;

    private Transform playerTrans;

    //Real world passed gameTime second = 1 hour time passed in game
    private float gameTime = 5f;
    private int day = 0;
    private bool isNewDay = true;


    private void Start()
    {
        day = 0;
        dayText.text = "Day: " + day;
    }
    private void Awake()
    {
        playerTrans = PlayerManager.instance.player.transform;
    }

    private void Update()
    {
        dayText.text = "Day: " + day;
        if (lightPreset == null)
            return;
        if (timeOfADay>= 6 && isNewDay)
        {
            day++;
            playerTrans.GetComponent<PlayerInfo>().LogInfo("Day "+ day);
            isNewDay = false;
        }else if(timeOfADay>=0 && timeOfADay <= 1)
        {
            isNewDay = true;
        }

        if (timeOfADay >= 8 && timeOfADay<= 18)
        {
            gameTime = 1.5f;
        }
        else
        {
            gameTime = 5f;
        }

        if (Application.isPlaying)
        {
            timeOfADay += (Time.deltaTime / gameTime);
            timeOfADay %= 24;
            UpdateLight(timeOfADay / 24f);
        }
        else
        {
            UpdateLight(timeOfADay / 24f);
        }
    }

    private void UpdateLight(float timePercent)
    {
        //Set ambient and fog
        RenderSettings.ambientLight = lightPreset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = lightPreset.FogColor.Evaluate(timePercent);

        //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
        if (dLight != null)
        {
            dLight.color = lightPreset.DirectionalColor.Evaluate(timePercent);

            dLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }

    }
    private void OnValidate()
    {
        if (!dLight)
            return;

        if (!RenderSettings.sun)
        {
            dLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    dLight = light;
                    return;
                }
            }
        }
    }

    public float GetCurrentTime()
    {
        return timeOfADay;
    }

    public int GetDay()
    {
        return day;
    }

    public void SetTime(float value)
    {
        timeOfADay = value;
    }
}