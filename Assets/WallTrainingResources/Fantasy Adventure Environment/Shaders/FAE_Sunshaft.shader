// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FAE/Sunshaft"
{
	Properties
	{
		_Alpha("Alpha", 2D) = "white" {}
		_PanningSpeed("PanningSpeed", Range( 0 , 1)) = 0.33
		_Fadedistance("Fade distance", Range( 0 , 150)) = 6
		_Intersectionfade("Intersection fade", Range( 0 , 20)) = 2
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite On
		Offset  0 , 0.1
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf StandardCustomLighting keepalpha noshadow nolightmap  nodynlightmap nodirlightmap vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
			float3 worldPos;
			float4 screenPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _Alpha;
		uniform float _PanningSpeed;
		uniform float _Fadedistance;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Intersectionfade;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float4 temp_output_22_0 = _LightColor0;
			float temp_output_3_0 = ( _Time.x * _PanningSpeed );
			float2 panner4 = ( i.texcoord_0 + temp_output_3_0 * float2( 0,0 ));
			float2 panner5 = ( i.texcoord_0 + temp_output_3_0 * float2( 0,1 ));
			float lerpResult12 = lerp( 0.0 , ( tex2D( _Alpha, panner4 ).r * tex2D( _Alpha, panner5 ).g ) , tex2D( _Alpha, i.texcoord_0 ).a);
			float3 ase_worldPos = i.worldPos;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth20 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth20 = abs( ( screenDepth20 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Intersectionfade ) );
			float clampResult28 = clamp( distanceDepth20 , 0.001 , 1.0 );
			c.rgb = temp_output_22_0.rgb;
			c.a = saturate( ( (_LightColor0).a * ( lerpResult12 * ( distance( _WorldSpaceCameraPos , ase_worldPos ) / _Fadedistance ) * clampResult28 ) ) );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=13401
1926;23;1906;1004;-848.1519;56.2774;1;True;False
Node;AmplifyShaderEditor.TimeNode;1;-1099,-27;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;2;-1168,202;Float;False;Property;_PanningSpeed;PanningSpeed;2;0;0.33;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-853,387;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-785,91;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;4;-500,70;Float;False;3;0;FLOAT2;1,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.TexturePropertyNode;7;-480,-227;Float;True;Property;_Alpha;Alpha;1;0;Assets/Fantasy Adventure Environment/Resources/Substances/FAE_Particles.sbsar;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.PannerNode;5;-498,215;Float;False;3;0;FLOAT2;1,0;False;2;FLOAT2;0,1;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.WorldPosInputsNode;14;611.3344,478.8737;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;21;1200.152,672.7226;Float;False;Property;_Intersectionfade;Intersection fade;4;0;2;0;20;0;1;FLOAT
Node;AmplifyShaderEditor.WorldSpaceCameraPos;13;564.3344,361.8737;Float;False;0;1;FLOAT3
Node;AmplifyShaderEditor.SamplerNode;8;-142,-4;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;ProceduralTexture;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;9;-157,213;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;ProceduralTexture;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;319.4025,163.132;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DepthFade;20;1545.152,554.7226;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;10;-147,434;Float;True;Property;_TextureSample2;Texture Sample 2;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;ProceduralTexture;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;17;765.3344,676.8737;Float;False;Property;_Fadedistance;Fade distance;3;0;6;0;150;0;1;FLOAT
Node;AmplifyShaderEditor.DistanceOpNode;15;925.3344,409.8737;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;28;1814.152,539.7226;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.001;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.LightColorNode;22;1784.152,-30.2774;Float;False;0;3;COLOR;FLOAT3;FLOAT
Node;AmplifyShaderEditor.LerpOp;12;575.3344,161.8737;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;16;1180.334,482.8737;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SwizzleNode;24;1986.152,-16.2774;Float;False;FLOAT;3;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;1882.334,197.8737;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;2230.152,158.7226;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;25;2434.152,148.7226;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;27;2754.45,115.362;Float;False;True;2;Float;;0;0;CustomLighting;FAE/Sunshaft;False;False;False;False;False;False;True;True;True;False;False;False;False;False;True;True;True;Off;1;0;True;0;0.1;Custom;0.5;True;False;0;True;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;False;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;1;1
WireConnection;3;1;2;0
WireConnection;4;0;6;0
WireConnection;4;1;3;0
WireConnection;5;0;6;0
WireConnection;5;1;3;0
WireConnection;8;0;7;0
WireConnection;8;1;4;0
WireConnection;9;0;7;0
WireConnection;9;1;5;0
WireConnection;11;0;8;1
WireConnection;11;1;9;2
WireConnection;20;0;21;0
WireConnection;10;0;7;0
WireConnection;10;1;6;0
WireConnection;15;0;13;0
WireConnection;15;1;14;0
WireConnection;28;0;20;0
WireConnection;12;1;11;0
WireConnection;12;2;10;4
WireConnection;16;0;15;0
WireConnection;16;1;17;0
WireConnection;24;0;22;0
WireConnection;18;0;12;0
WireConnection;18;1;16;0
WireConnection;18;2;28;0
WireConnection;23;0;24;0
WireConnection;23;1;18;0
WireConnection;25;0;23;0
WireConnection;27;2;22;0
WireConnection;27;9;25;0
ASEEND*/
//CHKSM=1DA3C05CCDEFD980A2C9D00842E3FB7BB5B0167D