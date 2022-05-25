Shader "CustomRenderTexture/Cloud Light Cookie shader"
{
    Properties
    {
    	[Header(Main Parameter 1)]
        _Color1("Color 1", Color) = (1,1,1,1)
    	_ColorIntensity1("Color 1 Intensity", Range(1.0,10.0)) = 2.0
        _Tex1("Texture Light 1", 2D) = "white" {}
		
    	
    	[Header(Main Parameter 2)]
    	_Color2("Color 2", Color) = (1,1,1,1)
        _ColorIntensity2("Color 2 Intensity", Range(1.0,10.0)) = 2.0
		_Tex2("Texture Light 2", 2D) = "white" {}
        
    	[Header(Background)]
    	_BackgroundColor("Background Color", Color) = (1,1,1,1)
	    _BackgroundColorIntensity("Background Color Intensity", Range(1.0,10.0)) = 2.0
    	
    	//Extra Parameter
    	_InvertTexture("Invert Texture X = Tex1, Y = Tex 2, Z = Tex 3, W = Tex 4", Vector) = (0.0, 0.0, 0.0, 0.0)
    	_ExtraTexture("Extra Texture", int) = 0
    	_Noise("Noise Texture", 2D) = "white" {}
    	_NoiseColor("Noise Color", Color) = (1,1,1,1)
        _NoiseColorColorIntensity("Color Noise Intensity", Range(0.0,10.0)) = 2.0
    	
    	[Header(Special Parameter)]
    	_Strength("Strength", Range(0.0,1.0)) = 0.8
    	_SineRange("Sine Range", Range(0.0,1.0)) = 0.2
    	_SineSpeedT1("Sine Speed Tex 1", Vector) = (0.25, 0.5,0,0)
    	_SineSpeedT2("Sine Speed Tex 2", Vector) = (0.25, 0.5,0,0)
    	_SineNoiseSpeed("Sine Noise Speed", Vector) = (0.25, 0.5,0,0)
    	_OffsetThirdTexture("Offset Third Texture", Vector) = (0.25, 0.5,0,0)
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
            sampler2D  _Noise;
			float4     _Noise_ST;
            float4      _InvertTexture;
            float      _ExtraTexture;

            //Color Parameter
            float4     _Color1;
            float      _ColorIntensity1;
            float4     _Color2;
            float      _ColorIntensity2;
            float4     _BackgroundColor;
            float      _BackgroundColorIntensity;
            float4     _NoiseColor;
            float      _NoiseColorColorIntensity;
            

            //Speed Parameter
            float      _Strength;
			float      _SineRange;
			float2     _SineSpeedT1;
			float2     _SineSpeedT2;
			float2     _SineNoiseSpeed;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
            	//Final Texture
            	float4 tex = 0;
            	
            	//UV Texture
                float2 uv1 = IN.globalTexcoord.xy * _Tex1_ST.xy + _Tex1_ST.zw;
				float2 uv2 = IN.globalTexcoord.xy * _Tex2_ST.xy + _Tex2_ST.zw;
				float2 uv3 = IN.globalTexcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
            	
				//Combine texture
            	float4 tex1 = tex2D(_Tex1, uv1 * _Noise_ST.xy + _SineSpeedT1 * 0.1 * _Time.xy);
				float4 tex2 = tex2D(_Tex2, uv2 * _Noise_ST.xy + _SineSpeedT2 * 0.1 * _Time.xy);
				float4 texNoise1 = tex2D(_Noise, uv3 + _SineNoiseSpeed * 0.1 * _Time.xy);
            	float4 invertTex = 1 - (tex1 + tex2);
            	float4 tex3 = tex1 * tex2;

            	//Invert Texture
            	if(_InvertTexture.x == 1)
            	{
            		tex1 = 1 - tex1;
            	}

            	if(_InvertTexture.y == 1)
            	{
            		tex2 = 1 - tex2;
            	}


            	//Add Texture
            	if(_ExtraTexture == 1)
            	{
            		tex = tex1 * _Color1 * _ColorIntensity1
            		+ tex2 * _Color2 * _ColorIntensity2
            		+ invertTex * _BackgroundColor * _BackgroundColorIntensity ;
            	}
            	else
            	{
            		tex = (tex1 * _Color1 + tex2 * _Color2) + invertTex * _BackgroundColor;
            	}
				return tex;
            }
            ENDCG
        }
    }
}