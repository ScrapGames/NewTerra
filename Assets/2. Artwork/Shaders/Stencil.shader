Shader "Scrap Games/Stencil"
{
    Properties
    {
        
    }
    SubShader
    {
        LOD 100
        Tags { "Queue" = "Geometry-2" }  // Write to the stencil buffer before drawing any geometry to the screen
        ColorMask 0 // Don't write to any colour channels
        ZWrite Off // Don't write to the Depth buffer
        // Write the value 1 to the stencil buffer
        
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace            
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                return float4(0,0,0,0);
            }
            ENDCG
        }

        
    }
}
