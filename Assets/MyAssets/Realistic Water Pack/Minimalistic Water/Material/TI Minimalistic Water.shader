Shader "Tauri Interactive/Water/Minimalistic Water" 
  {
    Properties 
    {
     _Color ("Color", Color) = (0.412,0.412,0.412,1)
     _MainTex ("Texture", 2D) = "white" {}
     _BumpMap ("Bumpmap", 2D) = "bump" {}
     _Cube ("Cubemap", CUBE) = "" {}
     _Value ("Reflection Power", Range(0,1)) = 0.3
    }
    
    SubShader 
    {
     Tags 
     { 
     	"RenderType" = "Opaque" 
     	"Queue" = "Transparent"
     }
  
     Cull Off
     Blend SrcAlpha OneMinusSrcAlpha
     Blend DstColor SrcColor
     
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
     float _Value2;
     float _Value3;

     void surf (Input IN, inout SurfaceOutput o) 
      {
       float4 tex = tex2D (_MainTex, IN.uv_MainTex) * _Color;
       o.Albedo = tex.rgb;
       o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
       float4 refl = texCUBE (_Cube, WorldReflectionVector (IN, o.Normal));
       o.Emission = refl.rgb * _Value * refl.a;
      }
     ENDCG
     }
  Fallback "Diffuse"
} 