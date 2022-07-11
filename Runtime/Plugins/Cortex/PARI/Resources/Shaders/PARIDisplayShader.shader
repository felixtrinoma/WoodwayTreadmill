Shader "Cortex/PARI Display Shader"
{
	Properties 
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_Color("_Color", Color) = (1,1,1,1)
		[Header(Edge blending)]
		_LeftEdgeBlending("_LeftEdgeBlending", Range(0.01,1)) = 0.1
		_RightEdgeBlending("_RightEdgeBlending", Range(0.01,1)) = 0.1
		_InternalColorCorrectionFactor("_InternalColorCorrectionFactor", Range(0,4)) = 1.0
		_InternalColorCorrectionCenter("_InternalColorCorrectionCenter", Range(0.1,0.9)) = 0.5
		_GammaCorrectionLeftEdgeBlendingR("_GammaCorrectionLeftEdgeBlendingR", Range(0,3)) = 1.0
		_GammaCorrectionLeftEdgeBlendingG("_GammaCorrectionLeftEdgeBlendingG", Range(0,3)) = 1.0
		_GammaCorrectionLeftEdgeBlendingB("_GammaCorrectionLeftEdgeBlendingB", Range(0,3)) = 1.0
		_GammaCorrectionRightEdgeBlendingR("_GammaCorrectionRightEdgeBlendingR", Range(0,3)) = 1.0
		_GammaCorrectionRightEdgeBlendingG("_GammaCorrectionRightEdgeBlendingG", Range(0,3)) = 1.0
		_GammaCorrectionRightEdgeBlendingB("_GammaCorrectionRightEdgeBlendingB", Range(0,3)) = 1.0
		_EdgeLayer("_EdgeLayer", 2D) = "black" {}
		_EdgeLayerPower("_EdgeLayerPower", Range(0,1)) = 0
		[Header(Colorimetry correction)]
		_GammaCorrection("_GammaCorrection", Range(0,3)) = 1.0
		_Brightness("_Brightness", Range(-1, 1)) = 0
		_Contrast("_Contrast", Range(-2, 2)) = 0
		_Saturation("_Saturation", Range(0, 8)) = 1
		_TopLeftLinearCorrection("_TopLeftLinearCorrection", Color) = (1,1,1,1)
		_TopRightLinearCorrection("_TopRightLinearCorrection", Color) = (1,1,1,1)
		_BottomLeftLinearCorrection("_BottomLeftLinearCorrection", Color) = (1,1,1,1)
		_BottomRightLinearCorrection("_BottomRightLinearCorrection", Color) = (1,1,1,1)
	}
	
	SubShader 
	{
		Tags { "IgnoreProjector"="True" }

		
	//Cull Back
	Cull Off
	ZTest Always
	Lighting Off


		CGPROGRAM

		#pragma surface surf Lambert nolightmap noshadow // Lambert is faster than Standard 
		#pragma target 3.0

		sampler2D _MainTex;
		float4 _Color;
		float _LeftEdgeBlending = 0.1;
		float _RightEdgeBlending = 0.1;
		float _InternalColorCorrectionFactor = 1.0;
		float _InternalColorCorrectionCenter = 0.5;
		float _GammaCorrectionLeftEdgeBlendingR;
		float _GammaCorrectionLeftEdgeBlendingG;
		float _GammaCorrectionLeftEdgeBlendingB;
		float _GammaCorrectionRightEdgeBlendingR;
		float _GammaCorrectionRightEdgeBlendingG;
		float _GammaCorrectionRightEdgeBlendingB;
		sampler2D _EdgeLayer;
		float _EdgeLayerPower;
		float _GammaCorrection;
		float _Brightness;
		float _Contrast;
		float _Saturation;
		float4 _TopLeftLinearCorrection;
		float4 _TopRightLinearCorrection;
		float4 _BottomLeftLinearCorrection;
		float4 _BottomRightLinearCorrection;


		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_EdgeLayer;
		};
			
		void surf (Input IN, inout SurfaceOutput o) 
		{
			// IMAGE PROCESSING //////////////////////////////////////////////////////////////////////////////////////////////////

			float4	col = tex2D(_MainTex, IN.uv_MainTex);

			// if not using gamma color space, convert from linear to gamma (sRGB) (functions are defined in UnityCG.cginc)
#ifndef UNITY_COLORSPACE_GAMMA
			col.rgb = LinearToGammaSpace(col.rgb);
#endif

			// global color correction (multiply by _Color, contrast, brightness, saturation, global gamma)
			// (must be the same on all videoprojectors)
			col = col * _Color;

			col.rgb = (col.rgb - 0.5) *(_Contrast + 1.0) + 0.5;	// contrast
			col.rgb = col.rgb + _Brightness;					// brightness
			float3 intensity = float(dot(col.rgb, unity_ColorSpaceLuminance)); // unity_ColorSpaceLuminance is defined in UnityCG.cginc
			col.rgb = lerp(intensity, col.rgb, _Saturation);	// saturation
			col = pow(col, _GammaCorrection);					// global gamma
			
			// linear intensity correction between corners
			// (specific to each videoprojector, to lower the colorimetry differences between them)
			float4	topInterpolation, bottomInterpolation, intensityCorrection;
			topInterpolation	= IN.uv_MainTex.x * _TopRightLinearCorrection 		+ (1 - IN.uv_MainTex.x) * _TopLeftLinearCorrection;
			bottomInterpolation = IN.uv_MainTex.x * _BottomRightLinearCorrection 	+ (1 - IN.uv_MainTex.x) * _BottomLeftLinearCorrection;
			
			intensityCorrection = IN.uv_MainTex.y *  topInterpolation 				+ (1 - IN.uv_MainTex.y) *  bottomInterpolation;
			
			// the intensity correction is not gamma corrected since we don't want it to change with the gamma (the colorimetry calibration would be much more difficult)
			col = intensityCorrection * col;


			// EDGE-BLENDING /////////////////////////////////////////////////////////////////////////////////////////////////////

			float EdgeBlendingFactorLeftN = 1;
			float4 EdgeBlendingFactorLeft;
			EdgeBlendingFactorLeft.rgba = 1.0;

			float EdgeBlendingFactorRightN = 1;
			float4 EdgeBlendingFactorRight;
			EdgeBlendingFactorRight.rgba = 1.0;

			float  pos = 1;
			float4 colLayer = 0; // for zebras

			float InternalColorCorrection = 0;

			// compute the blending curve on the edges
			if (_LeftEdgeBlending > 0)
			{
				pos = IN.uv_MainTex.x / _LeftEdgeBlending;
				if (pos < 1.0)
				{
					if (pos <= 0.5)
						EdgeBlendingFactorLeftN = 2.0 * pos * pos;
						//EdgeBlendingFactorLeftN = _LeftMiddle * pow(2.0 * pos, _LeftCurveFactor);
					else
						EdgeBlendingFactorLeftN = 1.0 - 2.0 * (1.0 - pos) * (1.0 - pos);
						//EdgeBlendingFactorLeftN = 1.0 - (1.0 - _LeftMiddle) * pow(2.0 * (1.0 - pos), _LeftCurveFactor);

					EdgeBlendingFactorLeft.r = pow(EdgeBlendingFactorLeftN, _GammaCorrectionLeftEdgeBlendingR);
					EdgeBlendingFactorLeft.g = pow(EdgeBlendingFactorLeftN, _GammaCorrectionLeftEdgeBlendingG);
					EdgeBlendingFactorLeft.b = pow(EdgeBlendingFactorLeftN, _GammaCorrectionLeftEdgeBlendingB);

					colLayer = tex2D(_EdgeLayer, IN.uv_EdgeLayer);

					if (pos <= 0.25)
						InternalColorCorrection = _InternalColorCorrectionCenter * pow(4.0 * pos, 2);
					else
						if (pos <= 0.75)
							InternalColorCorrection = 1.0 - (1.0 - _InternalColorCorrectionCenter) * pow(2.0 * (1.0 - 2.0 * pos), 2);
						else 
							InternalColorCorrection = _InternalColorCorrectionCenter * pow(4.0 * (pos - 1.0), 2);
				}
			}

			if (_RightEdgeBlending > 0)
			{
				pos = (1 - IN.uv_MainTex.x) / _RightEdgeBlending;
				if (pos < 1.0)
				{
					if (pos <= 0.5)
						EdgeBlendingFactorRightN = 2.0 * pos * pos;
						//EdgeBlendingFactorRightN = _RightMiddle * pow(2.0 * pos, _RightCurveFactor);
					else
						EdgeBlendingFactorRightN = 1.0 - 2.0 * (1.0 - pos) * (1.0 - pos);
						//EdgeBlendingFactorRightN = 1.0 - (1.0 - _RightMiddle) * pow(2.0 * (1.0 - pos), _RightCurveFactor);

					EdgeBlendingFactorRight.r = pow(EdgeBlendingFactorRightN, _GammaCorrectionRightEdgeBlendingR);
					EdgeBlendingFactorRight.g = pow(EdgeBlendingFactorRightN, _GammaCorrectionRightEdgeBlendingG);
					EdgeBlendingFactorRight.b = pow(EdgeBlendingFactorRightN, _GammaCorrectionRightEdgeBlendingB);

					float2 uvEdge = IN.uv_EdgeLayer;

					// UNITY_MATRIX_TEXTURE1 is the texture transformation matrix of the second texture (_EdgeLayer)
					// (UNITY_MATRIX_TEXTURE1[0].x, UNITY_MATRIX_TEXTURE1[1].y) is the texture tiling vector set under Unity for the second texture parameters of the shader
					// We use this because of a Unity bug: _EdgeLayer_ST.x is not usable inside a surface shader...
					uvEdge.x -= (1 - _LeftEdgeBlending) * UNITY_MATRIX_TEXTURE1[0].x;

					colLayer = tex2D(_EdgeLayer, uvEdge);

					if (pos <= 0.25)
						InternalColorCorrection = _InternalColorCorrectionCenter * pow(4.0 * pos, 2);
					else
						if (pos <= 0.75)
							InternalColorCorrection = 1.0 - (1.0 - _InternalColorCorrectionCenter) * pow(2.0 * (1.0 - 2.0 * pos), 2);
						else
							InternalColorCorrection = _InternalColorCorrectionCenter * pow(4.0 * (pos - 1.0), 2);
				}
			}

			// INTERNAL VP COLOR CORRECTIONS ///////////////////////////////////////////////////////////////////////////////////
			col = pow(col, 1.0 + InternalColorCorrection * (_InternalColorCorrectionFactor - 1.0));


			// FINAL COLOR /////////////////////////////////////////////////////////////////////////////////////////////////////

			// blending between the image and a gamma corrected version of the edge ramps
			col = EdgeBlendingFactorLeft * EdgeBlendingFactorRight * col;

			// zebras or not zebras
			col = colLayer.r * (colLayer*_EdgeLayerPower) + (1 - colLayer.r*_EdgeLayerPower) * col;

			// un-gamma
#ifndef UNITY_COLORSPACE_GAMMA
			col.rgb = GammaToLinearSpace(col.rgb);
#endif

			o.Emission = col;

			o.Alpha = 1;
		}
		ENDCG
	}
	Fallback "Diffuse"
}