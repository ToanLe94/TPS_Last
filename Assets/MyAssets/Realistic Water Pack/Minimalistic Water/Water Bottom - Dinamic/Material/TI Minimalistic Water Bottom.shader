Shader "Tauri Interactive/Water/Bottom/Minimalistic Water Bottom" 
  {
    Properties 
    {
     _Color ("Color", Color) = (0.647,0.647,0.647,1)
     _MainTex ("Texture", 2D) = "white" {}
     _BumpMap ("Bumpmap", 2D) = "bump" {}
    }
    
    SubShader 
    {
     Tags { "RenderType" = "Opaque" }
     Cull Off
     CGPROGRAM
      
     #pragma surface surf Lambert
     struct Input 
      {
       float2 uv_MainTex;
       float2 uv_BumpMap;
       float3 worldRefl;
       INTERNAL_DATA
      };
          
     float4 _Color;
     sampler2D _MainTex;
     sampler2D _BumpMap;
     samplerCUBE _Cube;
     float _Value;

     void surf (Input IN, inout SurfaceOutput o) 
      {
       float4 tex = tex2D (_MainTex, IN.uv_MainTex) * _Color;
       o.Albedo = tex.rgb;
       float4 refl = texCUBE (_Cube, WorldReflectionVector (IN, o.Normal));
      }
     ENDCG
    } 
  Fallback "Diffuse"
} 