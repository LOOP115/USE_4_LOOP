Shader "Custom/Terrian"
{
    Properties
    {
        testTexture("Texture", 2D) = "white"{}
        testScale("Scale", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        const static int maxLayerCount = 8;
        const static float epsilon = 1E-4;

        int layerCount;
        float3 baseColours[maxLayerCount];
        float baseStartHeight[maxLayerCount];
        float baseBlends[maxLayerCount];
        float baseColourStrength[maxLayerCount];
        float baseTextureScales[maxLayerCount];
        UNITY_DECLARE_TEX2DARRAY(baseTextures);

        float minHeight;
        float maxHeight;

        sampler2D testTexture;
        float testScale;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        float inverseLerp(float a, float b, float value) {
            return saturate((value - a) / (b - a));
        }

        //https://blog.csdn.net/liu_if_else/article/details/73833656
        float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex) {
            float3 scaledWorldPos = worldPos / scale;
            float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
            float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
            float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
            return xProjection + yProjection + zProjection;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 blendAxes = abs(IN.worldNormal);
            //Making sure that the colour won't exceed 1 (too bright)
            blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;
            float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
            for (int i = 0; i < layerCount; i++) {
                // no division by zero so we minus a small number
                float drawStrength = inverseLerp(-baseBlends[i]/2 - epsilon, baseBlends[i] / 2, (heightPercent - baseStartHeight[i]));
                
                float3 baseColour = baseColours[i] * baseColourStrength[i];
                float3 textureColour = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i) * (1 - baseColourStrength[i]);

                o.Albedo = o.Albedo * (1 - drawStrength) + (baseColour + textureColour) * drawStrength;
            }

        }
        ENDCG
    }
    FallBack "Diffuse"
}
