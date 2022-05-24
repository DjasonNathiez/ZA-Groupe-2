Shader "CustomRenderTexture/Light cookie shader"
{
    Properties
    {
    	[Header(Main Parameter)]
        _Color1("Color1", Color) = (1,1,1,1)
        _Tex1("Tex1", 2D) = "white" {}
		_Color2("Color1", Color) = (1,1,1,1)
		_Tex2("Tex2", 2D) = "white" {}
    	_InvertTexture("Invert Texture", int) = 0
    	
    	
    	[Header(Special Parameter)]
    	_Strength("Strength", Range(0.0,1.0)) = 0.8
    	_SineRange("Sine Range", Range(0.0,1.0)) = 0.2
    	_SineSpeedT1("Sine Speed Tex 1", Vector) = (0.25, 0.5,0,0)
    	_SineSpeedT2("Sine Speed Tex 2", Vector) = (0.25, 0.5,0,0)
    }

     SubShader
     { 
        Blend One Zero

        Pass
        {
            Name "Light cookie shader"

            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0


            //Texture Base Parameter

            sampler2D  _Tex1;
			float4     _Tex1_ST;
			sampler2D  _Tex2;
			float4     _Tex2_ST;
            float      _InvertTexture;

            //Color Parameter
            float4     _Color1;
            float4     _Color2;
			float      _Strength;
			float      _SineRange;
			float2      _SineSpeedT1;
			float2      _SineSpeedT2;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
            	//UV Texture
                float2 uv1 = IN.globalTexcoord.xy * _Tex1_ST.xy + _Tex1_ST.zw;
				float2 uv2 = IN.globalTexcoord.xy * _Tex2_ST.xy + _Tex2_ST.zw;

            	//Speed UV
            	
            	
				//Combine texture
            	float4 tex = tex2D(_Tex1, (uv1 + _SineSpeedT1 * 0.1 * _Time.xy) * _Color1)
            	+ tex2D(_Tex2, (uv2 + _SineSpeedT2 * 0.1 * _Time.xy) * _Color2);
				if(_InvertTexture == 1)
				{
					tex = 1 - tex;	
				}
				return tex;
            }
            ENDCG
        }
    }
}