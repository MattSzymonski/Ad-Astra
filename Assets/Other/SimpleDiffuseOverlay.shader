Shader "QuickBait/Simple Diffuse Overlay" {
	
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
	}

	SubShader
	{

		Tags{ "Queue" = "Overlay" }
		LOD 200

	    Pass 
	    {       
			ZTest Always
        }


		CGPROGRAM
		#pragma surface surf Lambert

		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
		}
			Fallback "Diffuse"
}






