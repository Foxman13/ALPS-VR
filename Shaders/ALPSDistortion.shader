Shader "Custom/ALPSDistortion" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	Subshader 
	{
 	Pass 
 	{
	 	ZTest Always Cull Off ZWrite Off
	  	Fog { Mode off }     
	  	
	  	CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"
        sampler2D _MainTex;
        float2 _SHIFT;
        float2 _CONVERGE;
        float2 _SCALE;

        struct appdata {
            float4 pos : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };
        
        v2f vert (appdata v) {
            v2f o;
            o.pos = v.pos ;
            o.uv.x = (v.uv.x * 0.5f)+_SHIFT;
            o.uv.y = (v.uv.y);
            return o;
        }
        
        
        uniform float _AberrationOffset;
 		uniform float4 _Center;
 		uniform float _ChromaticConstant;
 		
        float4 frag(v2f i):COLOR{
        
        	float2 coords = i.uv.xy;
           
            _AberrationOffset /= 300.0f;
            _ChromaticConstant /= 300.0f;
           	float radius = (coords.x - _Center.x)*(coords.x - _Center.x) + (coords.y - _Center.y)*(coords.y - _Center.y);
      
            float radialOffsetX = (step(0, coords.x - _Center.x) * 2 - 1) * _ChromaticConstant + (coords.x - _Center.x) * _AberrationOffset;
            float radialOffsetY = (step(0, coords.y - _Center.y) * 2 - 1) * _ChromaticConstant + (coords.y - _Center.y) * _AberrationOffset;
            
            float4 red = tex2D(_MainTex , float2(coords.x + radialOffsetX, coords.y + radialOffsetY));
            //Green Channel
            float4 green = tex2D(_MainTex, coords.xy );
            //Blue Channel
            float4 blue = tex2D(_MainTex, float2(coords.x - radialOffsetX, coords.y - radialOffsetY));
           
            //final color
            float4 finalColor = float4(red.r, green.g, blue.b, 1.0f);
            return finalColor;
        }
        
        ENDCG
        
  	}
}

Fallback off
}
