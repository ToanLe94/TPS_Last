Shader "Tauri Interactive/Metals/Aluminium" {
    Properties 
    {
     _MainColor ("Main Color", Color) = (1,1,1,1)
     _MainTex ("Texture", 2D) = "white" {}
     _BumpMap ("Bumpmap", 2D) = "bump" {}
     _Cube ("Cubemap", CUBE) = "" {}
     _Albedo ("Albedo", Range(0,1)) = 0.2       
     _EmissionColor ("Emission Color", Color) = (1,1,1,1)
     _Emission("Emission", Range(0,0.5)) = 0.2
     _Shininess("Shininess", Range(5,0.1)) = 1 
    }
    
    SubShader 
    {
     Tags 
     { 
     	"RenderType" = "Opaque" 
     	"Queue" = "Transparent"
     }
     		
     Cull Off
     
     CGPROGRAM
      #pragma surface surf Lambert
      struct Input {
          float2 uv_MainTex;
          float2 uv_BumpMap;
          float3 viewDir;
          float3 worldRefl;
	       INTERNAL_DATA
      };
      float4 _MainColor;
      float4 _EmissionColor;
      sampler2D _MainTex;
      sampler2D _BumpMap;      
	  samplerCUBE _Cube;
      float _Albedo;
      float _Emission;
      float _Shininess;
     
      void surf (Input IN, inout SurfaceOutput o) {      
      	  float4 tex = tex2D (_MainTex, IN.uv_MainTex) * _MainColor;
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Albedo + tex.rgb * _Shininess;
          o.Alpha = tex.rgb * _Shininess;
          o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
          float4 refl = pow( texCUBE (_Cube, WorldReflectionVector (IN, o.Normal)),  _Shininess);
       	  o.Emission = refl.rgb * _Emission * refl.a * _EmissionColor;
      }
      ENDCG
     
    } 
  Fallback "Diffuse"
}