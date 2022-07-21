using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    public class ShakeEvent
    {
        private float duration;
        private float timeRemaning;

        private CameraShakeScriptable data;

        public CameraShakeScriptable.Target target
        {
            get
            {
                return data.target;
            }
        }

        private Vector3 noiseOffset;
        public Vector3 noise;

        public ShakeEvent(CameraShakeScriptable data)
        {
            this.data = data;

            duration = data.duration;
            timeRemaning = duration;

            float rand = 32.0f;

            noiseOffset.x = Range(0.0f, rand);
            noiseOffset.y = Range(0.0f, rand);
            noiseOffset.z = Range(0.0f, rand);
        }
        
        public void Update()
        {
            float deltaTime = Time.deltaTime;

            timeRemaning -= deltaTime;

            float noiseOffsetDelta = deltaTime * data.frequency;

            noiseOffset.x += noiseOffsetDelta;
            noiseOffset.y += noiseOffsetDelta;
            noiseOffset.z += noiseOffsetDelta;

            noise.x = Mathf.PerlinNoise(noiseOffset.x, 0.0f);
            noise.y = Mathf.PerlinNoise(noiseOffset.x, 1.0f);
            noise.z = Mathf.PerlinNoise(noiseOffset.x, 2.0f);

            noise -= Vector3.one * 0.5f;

            noise *= data.amplitude;

            float agePercent = 1.0f - (timeRemaning / duration);
            noise *= data.blendOverLifetime.Evaluate(agePercent);
        }

        public bool IsAlive()
        {
            return timeRemaning > 0.0f;
        }
    }

    public List<ShakeEvent> shakeEvents = new List<ShakeEvent>();

    public void AddShakeEvent(CameraShakeScriptable data)
    {
        shakeEvents.Clear();
        shakeEvents.Add(new ShakeEvent(data));
    }

    public void AddShakeEvent(float amplitude, float frequency, float duration, AnimationCurve blendOverLifetime,
        CameraShakeScriptable.Target target)
    {
        CameraShakeScriptable data = ScriptableObject.CreateInstance<CameraShakeScriptable>();
        data.Init(amplitude, frequency, duration, blendOverLifetime, target);
        
        AddShakeEvent(data);
    }

    private void LateUpdate()
    {
        Vector3 positionOffset = Vector3.zero;
        Vector3 rotationOffset = Vector3.zero;

        foreach (var shakeEvent in shakeEvents)
        {
                ShakeEvent se = shakeEvent; se.Update();

                if (se.target == CameraShakeScriptable.Target.Position)
                {
                    positionOffset += se.noise;
                }
                else
                {
                    rotationOffset += se.noise;
                }
        }
        transform.localPosition = positionOffset;
        transform.localEulerAngles = rotationOffset;
    }

    private void Awake()
    {
        Instance = this;
    }
}
