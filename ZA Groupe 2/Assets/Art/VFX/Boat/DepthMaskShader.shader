Shader "Unlit/DepthMaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
     

        Tags{ "Queue" = "Geometry+10" }
        // Don't draw in the RGBA channels; just the depth buffer

        ColorMask 0
        ZWrite On

        // Do nothing specific in the pass:

        Pass{}

    }
}
