Shader "Custom/GeoShader"
{
    Properties
    {
        
        _XColor("X Color", Color) = (1, 0, 0, 1)
        _X2Color("X Color 2", Color) = (1, 0, 0, 1)
        _YColor("Y Color", Color) = (0, 1, 0, 1)
        _Y2Color("Y Color 2", Color) = (0, 1, 0, 1)
        _ZColor("Z Color", Color) = (0, 0, 1, 1)
        _Z2Color("Z Color 2", Color) = (0, 0, 1, 1)
        
        _FogColor("Fog Color", Color) = (1, 1, 1, 1)
        _FogStartY("Fog Start Y", Float) = 1
        _FogEndY("Fog End Y", Float) = 0
        
        [Toggle(_USE_COLOR_GRADIENT)] _UseColorGradient("Use Color Gradient", Float) = 1 
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        Tags { "LightMode"="UniversalForward" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma shader_feature _USE_COLOR_GRADIENT


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 objectNormal : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float4 shadowCoords : TEXCOORD3;
                float3 positionWS : TEXCOORD4;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _XColor;
                half4 _X2Color;
                half4 _YColor;
                half4 _Y2Color;
                half4 _ZColor;
                half4 _Z2Color;
                half4 _FogColor;
                float _FogStartY;
                float _FogEndY;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldNormal = TransformObjectToWorldNormal(IN.objectNormal);
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);

                // From https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/manual/use-built-in-shader-methods-shadows.html
                VertexPositionInputs positions = GetVertexPositionInputs(IN.positionOS.xyz);

                // Convert the vertex position to a position on the shadow map
                float4 shadowCoordinates = GetShadowCoord(positions);

                // Pass the shadow coordinates to the fragment shader
                OUT.shadowCoords = shadowCoordinates;
                OUT.positionWS = positions.positionWS;
                return OUT;
            }
            
            

            half4 frag(Varyings IN) : SV_Target
            {
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
                float3 normal = normalize(IN.worldNormal);
                half3 a = abs(normal);

                half4 x = _XColor;
                half4 y = _YColor;
                half4 z = _ZColor;
                
                #if _USE_COLOR_GRADIENT
                x = lerp(_XColor, _X2Color, screenUV.x);
                y = lerp(_YColor, _Y2Color, screenUV.y);
                z = lerp(_ZColor, _Z2Color, screenUV.x);
                #endif
                
                // Correct weights so sum of xyz is always 1
                // So that the colour on diagonals isnt too bright
                a = a / dot(a, 1);

                // Get the main light direction
                Light mainLight = GetMainLight(IN.shadowCoords);

                // NdotL: 1 = fully facing light, 0 or negative = facing away
                half NdotL = dot(normal, mainLight.direction);

                half shadowAmount = NdotL > 0 ? mainLight.shadowAttenuation : 1;

                shadowAmount = clamp(shadowAmount, 0.6, 1);
                
                half4 color = x * a.x + y * a.y + z * a.z;
                color *= shadowAmount;

                float fogSize = _FogStartY - _FogEndY;
                float fogFraction = (IN.positionWS.y - _FogEndY) / fogSize;
                fogFraction = clamp(fogFraction, 0, 1);
                color = lerp(color, _FogColor, fogFraction);
                return color;
            }
            ENDHLSL
        }
    }
}
