using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCookieMotion : MonoBehaviour
{
    [SerializeField] Material m_lightCookieMaterial;

    [Header("Texture 1")]
    [SerializeField] Vector2 m_cycleDuration1UV = new Vector2(20f, 20f);
    [SerializeField] AnimationCurve m_movementPath1U;
    [SerializeField] AnimationCurve m_movementPath1V;
    [SerializeField] Vector2 m_movementMagnitude1UV = new Vector2(0.1f, 0.1f);
    [SerializeField] Vector2 m_movementTimeOffset1UV = new Vector2();
    [SerializeField] Vector2 m_tex1TilingUV = new Vector2(1f, 1f);
    [SerializeField] Vector2 m_tex1OffsetUV = new Vector2();

    [Header("Texture 2")]
    [SerializeField] Vector2 m_cycleDuration2UV = new Vector2(20f, 20f);
    [SerializeField] AnimationCurve m_movementPath2U;
    [SerializeField] AnimationCurve m_movementPath2V;
    [SerializeField] Vector2 m_movementMagnitude2UV = new Vector2(0.1f, 0.1f);
    [SerializeField] Vector2 m_movementTimeOffset2UV = new Vector2();
    [SerializeField] Vector2 m_tex2TilingUV = new Vector2(2f, 2f);
    [SerializeField] Vector2 m_tex2OffsetUV = new Vector2();

    private float m_time1U;
    private float m_time1V;

    private float m_time2U;
    private float m_time2V;


    void Update()
    {
        m_time1U = Time.time % m_cycleDuration1UV.x;
        m_time1U /= m_cycleDuration1UV.x;

        m_time1V = Time.time % m_cycleDuration1UV.y;
        m_time1V /= m_cycleDuration1UV.y;

        m_time2U = Time.time % m_cycleDuration2UV.x;
        m_time2U /= m_cycleDuration2UV.x;

        m_time2V = Time.time % m_cycleDuration2UV.y;
        m_time2V /= m_cycleDuration2UV.y;

        UpdateMaterial(); 
    }


    private void UpdateMaterial()
    {
        float newU1 = m_movementPath1U.Evaluate(m_time1U + m_movementTimeOffset1UV.x) * m_movementMagnitude1UV.x;
        float newV1 = m_movementPath1V.Evaluate(m_time1V + m_movementTimeOffset1UV.y) * m_movementMagnitude1UV.y;

        var newUV1 = new Vector4(m_tex1TilingUV.x, m_tex1TilingUV.y, newU1 + m_tex1OffsetUV.x, newV1 + m_tex1OffsetUV.y);

        m_lightCookieMaterial.SetVector("_Tex1_ST", newUV1);

        float newU2 = m_movementPath2U.Evaluate(m_time2U + m_movementTimeOffset2UV.x) * m_movementMagnitude2UV.x;
        float newV2 = m_movementPath2V.Evaluate(m_time2V + m_movementTimeOffset2UV.y) * m_movementMagnitude2UV.y;

        var newUV2 = new Vector4(m_tex2TilingUV.x, m_tex2TilingUV.y, newU2 + m_tex2OffsetUV.x, newV2 + m_tex2OffsetUV.y);

        m_lightCookieMaterial.SetVector("_Tex2_ST", newUV2);
    }


    private void OnValidate()
    {
        UpdateMaterial();
    }
}