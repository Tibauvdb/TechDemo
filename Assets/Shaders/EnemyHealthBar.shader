// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "UI/HealthBar"
{
	Properties
	{
		_Opacity("Opacity", Range( 0 , 1)) = 1
		_FrontColor("FrontColor", Color) = (0.8113208,0.3184051,0.1951762,0)
		_BackColor("BackColor", Color) = (0.8396226,0.2495105,0.2495105,0)
		_HealthRemaining("HealthRemaining", Range( 0 , 1)) = 0.5764296
		_EdgeColor("EdgeColor", Color) = (1,1,1,0)
		_EdgeLine("EdgeLine", Float) = 0.09
		_MinY("MinY", Float) = 0
		_MaxY("MaxY", Float) = 1
		[Toggle]_ShowBackColor("ShowBackColor", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _BackColor;
		uniform float _HealthRemaining;
		uniform float _MinY;
		uniform float _MaxY;
		uniform float4 _FrontColor;
		uniform float _EdgeLine;
		uniform float4 _EdgeColor;
		uniform float _ShowBackColor;
		uniform float _Opacity;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			//Calculate new billboard vertex position and normal;
			float3 upCamVec = float3( 0, 1, 0 );
			float3 forwardCamVec = -normalize ( UNITY_MATRIX_V._m20_m21_m22 );
			float3 rightCamVec = normalize( UNITY_MATRIX_V._m00_m01_m02 );
			float4x4 rotationCamMatrix = float4x4( rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1 );
			v.normal = normalize( mul( float4( v.normal , 0 ), rotationCamMatrix )).xyz;
			v.vertex.x *= length( unity_ObjectToWorld._m00_m10_m20 );
			v.vertex.y *= length( unity_ObjectToWorld._m01_m11_m21 );
			v.vertex.z *= length( unity_ObjectToWorld._m02_m12_m22 );
			v.vertex = mul( v.vertex, rotationCamMatrix );
			v.vertex.xyz += unity_ObjectToWorld._m03_m13_m23;
			//Need to nullify rotation inserted by generated surface shader;
			v.vertex = mul( unity_WorldToObject, v.vertex );
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float temp_output_7_0 = ( ase_vertex3Pos.y + (-1.0 + (( 1.0 - _HealthRemaining ) - _MinY) * (1.0 - -1.0) / (_MaxY - _MinY)) );
			float temp_output_12_0 = step( temp_output_7_0 , 0.0 );
			float temp_output_14_0 = ( 1.0 - temp_output_12_0 );
			float temp_output_21_0 = step( temp_output_7_0 , _EdgeLine );
			o.Emission = ( ( ( _BackColor * temp_output_14_0 ) + ( _FrontColor * temp_output_12_0 ) ) + ( ( temp_output_21_0 - temp_output_12_0 ) * _EdgeColor ) ).rgb;
			float ForeGroundMask61 = temp_output_21_0;
			float BackgroundMask59 = temp_output_14_0;
			float ifLocalVar54 = 0;
			if( lerp(0.0,1.0,_ShowBackColor) > 0.0 )
				ifLocalVar54 = _Opacity;
			else if( lerp(0.0,1.0,_ShowBackColor) == 0.0 )
				ifLocalVar54 = ( ( ForeGroundMask61 * _Opacity ) + ( _Opacity * ( BackgroundMask59 * 0.0 ) ) );
			o.Alpha = ifLocalVar54;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
-1905;26;1906;1004;2595.59;-157.9133;2.229789;True;True
Node;AmplifyShaderEditor.RangedFloatNode;1;-2146.739,222.981;Float;False;Property;_HealthRemaining;HealthRemaining;3;0;Create;True;0;0;False;0;0.5764296;0.762;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-2012.067,347.5326;Float;False;Property;_MinY;MinY;6;0;Create;True;0;0;False;0;0;-0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-2018.684,430.9016;Float;False;Property;_MaxY;MaxY;7;0;Create;True;0;0;False;0;1;1.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;15;-1836.797,227.9821;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;6;-1827.971,41.78732;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;8;-1659.583,228.0274;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-1423.37,89.16502;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;12;-801.4911,92.62774;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;14;-532.6532,96.95007;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1362.211,449.5809;Float;False;Property;_EdgeLine;EdgeLine;5;0;Create;True;0;0;False;0;0.09;0.025;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;59;-297.2807,92.17876;Float;False;BackgroundMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;21;-1188.01,428.7811;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;61;-871.4181,549.1741;Float;False;ForeGroundMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;-1277.191,1525.663;Float;False;59;BackgroundMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-1091.213,-136.7624;Float;False;Property;_FrontColor;FrontColor;1;0;Create;True;0;0;False;0;0.8113208,0.3184051,0.1951762,0;0.8113208,0.3184051,0.1951762,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-952.5648,1531.365;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;62;-1168.646,1017.242;Float;False;61;ForeGroundMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-1183.428,1231.739;Float;True;Property;_Opacity;Opacity;0;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-844.5861,-360.4688;Float;False;Property;_BackColor;BackColor;2;0;Create;True;0;0;False;0;0.8396226,0.2495105,0.2495105,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-595.9202,1509.163;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-792.1768,1021.04;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-315.1974,-352.863;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-559.3362,-135.3971;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;22;-438.0075,426.7394;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;23;-422.4084,695.8383;Float;False;Property;_EdgeColor;EdgeColor;4;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;55;-32.41917,1180.814;Float;False;Property;_ShowBackColor;ShowBackColor;9;0;Create;True;0;0;False;0;1;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-293.9566,1486.75;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-84.40759,582.739;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;18.23183,-158.0465;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ConditionalIfNode;54;263.311,1185.593;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;279.5931,229.139;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;27;607.1157,195.6909;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;UI/HealthBar;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;True;Cylindrical;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;15;0;1;0
WireConnection;8;0;15;0
WireConnection;8;1;46;0
WireConnection;8;2;47;0
WireConnection;7;0;6;2
WireConnection;7;1;8;0
WireConnection;12;0;7;0
WireConnection;14;0;12;0
WireConnection;59;0;14;0
WireConnection;21;0;7;0
WireConnection;21;1;20;0
WireConnection;61;0;21;0
WireConnection;51;0;60;0
WireConnection;49;0;48;0
WireConnection;49;1;51;0
WireConnection;52;0;62;0
WireConnection;52;1;48;0
WireConnection;17;0;3;0
WireConnection;17;1;14;0
WireConnection;16;0;2;0
WireConnection;16;1;12;0
WireConnection;22;0;21;0
WireConnection;22;1;12;0
WireConnection;53;0;52;0
WireConnection;53;1;49;0
WireConnection;24;0;22;0
WireConnection;24;1;23;0
WireConnection;18;0;17;0
WireConnection;18;1;16;0
WireConnection;54;0;55;0
WireConnection;54;2;48;0
WireConnection;54;3;53;0
WireConnection;25;0;18;0
WireConnection;25;1;24;0
WireConnection;27;2;25;0
WireConnection;27;9;54;0
ASEEND*/
//CHKSM=13CB1927E1928DDFA0E18CD4C8666A0D29AE8764