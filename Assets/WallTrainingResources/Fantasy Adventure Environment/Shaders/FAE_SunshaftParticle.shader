// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FAE/Sunshaft Particle"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_Alpha("Alpha", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		_Fadedistance("Fade distance", Range( 0 , 150)) = 6
		_Intersectionfade("Intersection fade", Range( 0 , 20)) = 2
	}

	Category 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane"  }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off
		
		SubShader
		{
			

			Pass {
			
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "Lighting.cginc"
				#include "UnityShaderVariables.cginc"
				#include "UnityCG.cginc"


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 ase_texcoord3 : TEXCOORD3;
					float4 ase_texcoord4 : TEXCOORD4;
				};
				
				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform sampler2D _Alpha;
				uniform float4 _Alpha_ST;
				uniform float _Fadedistance;
				uniform sampler2D _CameraDepthTexture;
				uniform float _Intersectionfade;

				v2f vert ( appdata_t v  )
				{
					v2f o;
					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.ase_texcoord3.xyz = worldPos;
					float4 clipPos = UnityObjectToClipPos(v.vertex);
					float4 screenPos = ComputeScreenPos(clipPos);
					o.ase_texcoord4 = screenPos;
					
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.ase_texcoord3.w = 0;

					o.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.texcoord = v.texcoord;
					o.texcoord.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{

					float2 uv_Alpha = i.texcoord * _Alpha_ST.xy + _Alpha_ST.zw;
					float temp_output_34_0 = ( i.color.a * tex2D( _Alpha, uv_Alpha ).a );
					float3 worldPos = i.ase_texcoord3.xyz;
					float4 screenPos = i.ase_texcoord4;
					float4 ase_screenPosNorm = screenPos / screenPos.w;
					ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
					float screenDepth20 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(screenPos))));
					float distanceDepth20 = abs( ( screenDepth20 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Intersectionfade ) );
					float clampResult40 = clamp( distanceDepth20 , 0.001 , 1.0 );
					float4 appendResult42 = (float4(( _LightColor0.rgb * (i.color).rgb ).x , ( _LightColor0.rgb * (i.color).rgb ).y , ( _LightColor0.rgb * (i.color).rgb ).z , saturate( ( (_LightColor0).a * ( temp_output_34_0 * ( temp_output_34_0 * ( distance( _WorldSpaceCameraPos , worldPos ) / _Fadedistance ) ) * clampResult40 ) ) )));
					

					fixed4 col = appendResult42;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
}
/*ASEBEGIN
Version=13401
1926;23;1906;1004;-427.6341;473.0468;1.55846;True;False
Node;AmplifyShaderEditor.WorldSpaceCameraPos;13;564.3344,361.8737;Float;False;0;1;FLOAT3
Node;AmplifyShaderEditor.WorldPosInputsNode;14;611.3344,478.8737;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;30;1049.204,-107.348;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;21;1200.152,672.7226;Float;False;Property;_Intersectionfade;Intersection fade;2;0;2;0;20;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;17;765.3344,676.8737;Float;False;Property;_Fadedistance;Fade distance;1;0;6;0;150;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;29;1031.586,91.48868;Float;True;Property;_Alpha;Alpha;3;0;Assets/Fantasy Adventure Environment/Resources/Substances/FAE_Particles.sbsar;True;0;False;white;Auto;False;Object;-1;Auto;ProceduralTexture;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DistanceOpNode;15;925.3344,409.8737;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.DepthFade;20;1545.152,554.7226;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;16;1180.334,482.8737;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;1420.45,122.9503;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;40;1829.449,562.1527;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.001;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;1668.366,348.2147;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LightColorNode;31;1070.598,-245.7783;Float;False;0;3;COLOR;FLOAT3;FLOAT
Node;AmplifyShaderEditor.SwizzleNode;38;1543.779,-55.75092;Float;False;FLOAT;3;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;1937.677,300.393;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SwizzleNode;33;1293.346,-122.4494;Float;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;1553.846,-197.957;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0,0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;2198.178,241.2455;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SaturateNode;39;2404.565,258.864;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;43;2204.279,-33.56119;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;42;2648.439,142.5447;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.TemplateMasterNode;41;2872.652,146.0635;Float;False;True;2;Float;ASEMaterialInspector;0;5;FAE/Sunshaft Particle;0b6a9f8b4f707c74ca64c0be8e590de0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;15;0;13;0
WireConnection;15;1;14;0
WireConnection;20;0;21;0
WireConnection;16;0;15;0
WireConnection;16;1;17;0
WireConnection;34;0;30;4
WireConnection;34;1;29;4
WireConnection;40;0;20;0
WireConnection;35;0;34;0
WireConnection;35;1;16;0
WireConnection;38;0;31;0
WireConnection;36;0;34;0
WireConnection;36;1;35;0
WireConnection;36;2;40;0
WireConnection;33;0;30;0
WireConnection;32;0;31;1
WireConnection;32;1;33;0
WireConnection;37;0;38;0
WireConnection;37;1;36;0
WireConnection;39;0;37;0
WireConnection;43;0;32;0
WireConnection;42;0;43;0
WireConnection;42;1;43;1
WireConnection;42;2;43;2
WireConnection;42;3;39;0
WireConnection;41;0;42;0
ASEEND*/
//CHKSM=D1055BEB979F2F415731294DC41F9D8B756C0D1F