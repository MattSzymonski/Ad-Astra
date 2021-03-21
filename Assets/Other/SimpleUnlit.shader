Shader "QuickBait/Simple Unlit" {

		Properties{
			_Color("Color", Color) = (1,1,1)
		}

			SubShader{
			Tags{ "RenderType" = "Opaque" }
			LOD 100
			Color[_Color]
			Pass{}
		}

	}