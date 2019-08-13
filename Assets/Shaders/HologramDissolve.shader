// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/HologramDissolve"
{
	Properties
	{
		_AlbedoColor("Albedo Color", Color) = (1,0,0,0)
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_FresnelColor("Fresnel Color", Color) = (0.03516423,0.9811321,0.004627992,0)
		_FresnelBias("Fresnel Bias", Float) = 0
		_FresnelScale("Fresnel Scale", Float) = 1
		_FresnelPower("Fresnel Power", Float) = 3
		_Opacity("Opacity", Float) = 0.8
		_DissolveTexture("Dissolve Texture", 2D) = "white" {}
		_DissolveAmount("Dissolve Amount", Range( 0 , 1)) = 0.5758584
		_DissolveEdgeColor("DissolveEdge Color", Color) = (1,0.5694697,0,0)
		_PannerTexture("PannerTexture", 2D) = "white" {}
		_PanDirection("Pan Direction", Vector) = (-0.5,0.5,0,0)
		_PanSpeed("Pan Speed", Float) = 0.6
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _DissolveEdgeColor;
		uniform sampler2D _DissolveTexture;
		uniform float4 _DissolveTexture_ST;
		uniform float _DissolveAmount;
		uniform float4 _AlbedoColor;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _PannerTexture;
		uniform float _PanSpeed;
		uniform float2 _PanDirection;
		uniform float4 _FresnelColor;
		uniform float _FresnelBias;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_DissolveTexture = i.uv_texcoord * _DissolveTexture_ST.xy + _DissolveTexture_ST.zw;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float DissolveValue62 = _DissolveAmount;
			float LocalPosGradient45 = ( ase_vertex3Pos.y + (-1.0 + (DissolveValue62 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) );
			float4 temp_output_56_0 = ( tex2D( _DissolveTexture, uv_DissolveTexture ) + ( LocalPosGradient45 * -5.0 ) );
			float4 smoothstepResult70 = smoothstep( ( 1.0 - _DissolveEdgeColor ) , float4( 1,1,1,0 ) , temp_output_56_0);
			float4 temp_output_66_0 = step( temp_output_56_0 , float4( 1,1,1,0 ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = ( smoothstepResult70 * ( temp_output_66_0 + ( _AlbedoColor + tex2D( _Albedo, uv_Albedo ) ) ) ).rgb;
			float mulTime118 = _Time.y * _PanSpeed;
			float3 ase_worldPos = i.worldPos;
			float2 panner114 = ( mulTime118 * _PanDirection + ase_worldPos.xy);
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV21 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode21 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNdotV21, _FresnelPower ) );
			o.Emission = ( ( ( ( tex2D( _PannerTexture, panner114 ) * _FresnelColor ) + ( _FresnelColor * fresnelNode21 ) ) + smoothstepResult70 ) * temp_output_66_0 ).rgb;
			float4 clampResult93 = clamp( ( temp_output_66_0 * ( 1.0 - _Opacity ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			o.Alpha = ( 1.0 - clampResult93 ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred 

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
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
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
-1849;89;1906;993;2707.39;165.9646;2.057397;True;True
Node;AmplifyShaderEditor.CommentaryNode;64;-1692.058,-215.9353;Float;False;606.2542;165.2527;Dissolve Value;2;62;38;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-1665.745,-154.2355;Float;False;Property;_DissolveAmount;Dissolve Amount;10;0;Create;True;0;0;False;0;0.5758584;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;60;-1981.63,-652.7475;Float;False;1061.858;403.1178;Top To Bottom Dissolve ;5;40;57;44;63;45;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;62;-1320.54,-148.327;Float;False;DissolveValue;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;63;-1871.745,-457.2329;Float;False;62;DissolveValue;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;40;-1595.676,-451.6301;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;57;-1608.182,-610.7474;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;44;-1363.469,-490.0355;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;125;-3770.129,-663.7737;Float;False;1174.697;455.4246;Panning Texture;6;124;123;118;119;114;115;;1,0.7311321,0.7311321,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-1143.774,-436.6592;Float;False;LocalPosGradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-3720.129,-324.5908;Float;False;Property;_PanSpeed;Pan Speed;14;0;Create;True;0;0;False;0;0.6;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;118;-3491.535,-318.3491;Float;False;1;0;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;126;-3712.904,-82.95026;Float;False;996.3994;520.5262;Fresnel - Rim Light;6;28;27;26;21;25;61;;1,0.6179246,0.6179246,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-2792.318,910.2526;Float;False;45;LocalPosGradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;123;-3511.84,-609.7737;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;54;-2791.532,1000.052;Float;False;Constant;_GradientPower;Gradient Power;9;0;Create;True;0;0;False;0;-5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;119;-3511.535,-465.3489;Float;False;Property;_PanDirection;Pan Direction;13;0;Create;True;0;0;False;0;-0.5,0.5;-0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;114;-3199.637,-532.1136;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;1.67,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-2526.646,941.0768;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-3664.904,224.0497;Float;False;Property;_FresnelScale;Fresnel Scale;6;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-3660.904,144.0497;Float;False;Property;_FresnelBias;Fresnel Bias;5;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-3664.904,301.0497;Float;False;Property;_FresnelPower;Fresnel Power;7;0;Create;True;0;0;False;0;3;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;51;-2814.052,699.374;Float;True;Property;_DissolveTexture;Dissolve Texture;9;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;61;-3264.904,-34.95026;Float;False;Property;_FresnelColor;Fresnel Color;4;0;Create;True;0;0;False;0;0.03516423,0.9811321,0.004627992,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;115;-2916.432,-521.6967;Float;True;Property;_PannerTexture;PannerTexture;12;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;84;-971.4595,1043.308;Float;False;Property;_Opacity;Opacity;8;0;Create;True;0;0;False;0;0.8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;132;-2406.044,336.0011;Float;False;Property;_DissolveEdgeColor;DissolveEdge Color;11;0;Create;True;0;0;False;0;1,0.5694697,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;56;-2308.306,1043.195;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;21;-3321.904,173.0497;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;3.67;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-1647.465,1025.935;Float;True;Property;_Albedo;Albedo;2;0;Create;True;0;0;False;0;516e751d989ccc349a10c51355b1c849;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-2960.904,149.0947;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;-2476.605,-49.45947;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;142;-1597.446,803.0482;Float;False;Property;_AlbedoColor;Albedo Color;1;0;Create;True;0;0;False;0;1,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;66;-1873.094,568.6393;Float;True;2;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;133;-2159.546,585.3586;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;141;-783.3074,1049.167;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;121;-2218.054,372.196;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-596.5522,1002.634;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;70;-1888,304;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0.9755559,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;143;-1278.446,901.0482;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;93;-339.2164,1003.424;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;77;-528.6155,574.8199;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;-1623.431,132.2687;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;128;-1122.35,441.7013;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-249.8465,324.812;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;94;-89.37325,1004.228;Float;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RelayNode;73;-1449.691,132.2687;Float;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-959.5156,127.2892;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;16;-100.5205,-329.5604;Float;True;Property;_Normal;Normal;3;0;Create;True;0;0;False;0;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RelayNode;95;94.22017,1004.277;Float;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RelayNode;129;2.661772,98.56613;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;292.588,101.2128;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Custom/HologramDissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;AlphaTest;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.1;0,0,0,0;VertexScale;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;62;0;38;0
WireConnection;40;0;63;0
WireConnection;44;0;57;2
WireConnection;44;1;40;0
WireConnection;45;0;44;0
WireConnection;118;0;124;0
WireConnection;114;0;123;0
WireConnection;114;2;119;0
WireConnection;114;1;118;0
WireConnection;55;0;53;0
WireConnection;55;1;54;0
WireConnection;115;1;114;0
WireConnection;56;0;51;0
WireConnection;56;1;55;0
WireConnection;21;1;26;0
WireConnection;21;2;27;0
WireConnection;21;3;28;0
WireConnection;25;0;61;0
WireConnection;25;1;21;0
WireConnection;122;0;115;0
WireConnection;122;1;61;0
WireConnection;66;0;56;0
WireConnection;133;0;132;0
WireConnection;141;0;84;0
WireConnection;121;0;122;0
WireConnection;121;1;25;0
WireConnection;88;0;66;0
WireConnection;88;1;141;0
WireConnection;70;0;56;0
WireConnection;70;1;133;0
WireConnection;143;0;142;0
WireConnection;143;1;14;0
WireConnection;93;0;88;0
WireConnection;77;0;66;0
WireConnection;77;1;143;0
WireConnection;71;0;121;0
WireConnection;71;1;70;0
WireConnection;128;0;66;0
WireConnection;81;0;70;0
WireConnection;81;1;77;0
WireConnection;94;0;93;0
WireConnection;73;0;71;0
WireConnection;80;0;73;0
WireConnection;80;1;128;0
WireConnection;95;0;94;0
WireConnection;129;0;81;0
WireConnection;0;0;129;0
WireConnection;0;1;16;0
WireConnection;0;2;80;0
WireConnection;0;9;95;0
ASEEND*/
//CHKSM=810BF3D11BAEB424D54D8DB58787FCCA3FFAE641