/*
 * Copyright (C) 2012 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

Shader "BitmapFont" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Font Texture", 2D) = "white" {}
	}

	SubShader {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
		}
		Lighting Off
		Cull Off
		ZWrite On
		Fog {Mode Off}
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			half4 _MainTex_ST;
			fixed4 _Color;
			struct v2f {
				half4 pos: SV_POSITION;
				half2 uv: TEXCOORD0;
				fixed4 color: COLOR;
			};
			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				return o;
			}
			fixed4 frag(v2f i): COLOR
			{
				fixed4 textureColor = tex2D(_MainTex, i.uv.xy);
				fixed3 rgb = i.color.xyz * _Color.xyz;
				fixed a = i.color.w * textureColor.w * _Color.w;
				return fixed4(rgb, a);
			}
			ENDCG
		}
	}
}
