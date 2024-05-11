// RawImage 四边圆角
Shader "QBCore/RadiusCorner"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		// required for UI.Mask
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
		_ColorMask("Color Mask", Float) = 15

		_Radius("Radius", Float) = 0.2
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent""IgnoreProjector" = "True""RenderType" = "Transparent""PreviewType" = "Plane""CanUseSpriteAtlas" = "True"
		}
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
		// 源rgba*源a + 背景rgba*(1-源A值)   
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

		Pass
		{

			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}
			ColorMask[_ColorMask]

			CGPROGRAM
			#pragma vertex vert     
			#pragma fragment frag     
			#include "UnityCG.cginc"     
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
			};

			sampler2D _MainTex;
			fixed4 _Color;
			float4 _ClipRect;
			fixed _Radius;
			void ClipRadius(float2 uv, float2 center);
			v2f vert(appdata_t IN)
			{
				v2f OUT;
                OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				#ifdef UNITY_HALF_TEXEL_OFFSET     
				OUT.vertex.xy -= (_ScreenParams.zw - 1.0);
				#endif     
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				// 先缩放，再平移
				half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;

				#ifdef UNITY_UI_CLIP_RECT
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				#endif

				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif
				// 左下
				if(IN.texcoord.x < _Radius && IN.texcoord.y < _Radius){
					ClipRadius(IN.texcoord,float2(_Radius,_Radius));
				}
				// 右下
				if(IN.texcoord.x > 1-_Radius && IN.texcoord.y < _Radius){
					ClipRadius(IN.texcoord,float2(1-_Radius,_Radius));
				}
				// 左上
				if(IN.texcoord.x < _Radius && IN.texcoord.y > 1-_Radius){
					ClipRadius(IN.texcoord,float2(_Radius,1-_Radius));
				}
				// 右上
				if(IN.texcoord.x > 1-_Radius && IN.texcoord.y > 1-_Radius){
					ClipRadius(IN.texcoord,float2(1-_Radius,1-_Radius));
				}
				return color;
			}

			// 圆形裁剪
			void ClipRadius(float2 uv, float2 center)
			{
				float2 delta = uv - center;
				if(delta.x * delta.x + delta.y * delta.y >_Radius * _Radius){
					discard;
				}
			}
			ENDCG


		}
	}
}