// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/DissolveWeapon"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_DisolveGuide("Disolve Guide", 2D) = "white" {}
		_BurnRamp("Burn Ramp", 2D) = "white" {}
		_DissolveAmount("Dissolve Amount", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _DissolveAmount;
		uniform sampler2D _DisolveGuide;
		uniform sampler2D _BurnRamp;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal, uv_Normal ).rgb;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = tex2D( _Albedo, uv_Albedo ).rgb;
			float3 ase_worldPos = i.worldPos;
			float2 appendResult144 = (float2(ase_worldPos.x , ase_worldPos.y));
			float temp_output_73_0 = ( (-0.6 + (( 1.0 - _DissolveAmount ) - 0.0) * (0.6 - -0.6) / (1.0 - 0.0)) + tex2D( _DisolveGuide, appendResult144 ).r );
			float clampResult113 = clamp( (-3.0 + (temp_output_73_0 - 0.0) * (3.0 - -3.0) / (1.0 - 0.0)) , 0.0 , 1.0 );
			float temp_output_130_0 = ( 1.0 - clampResult113 );
			float2 appendResult115 = (float2(temp_output_130_0 , 0.0));
			float4 tex2DNode114 = tex2D( _BurnRamp, appendResult115 );
			o.Emission = ( temp_output_130_0 * tex2DNode114 ).rgb;
			o.Alpha = 1;
			clip( temp_output_73_0 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
1;1;1918;1017;2124.636;159.8552;1.36837;True;True
Node;AmplifyShaderEditor.CommentaryNode;128;-1410.652,558.9573;Float;False;908.2314;498.3652;Dissolve - Opacity Mask;7;4;71;2;73;111;144;141;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1401.322,627.1715;Float;False;Property;_DissolveAmount;Dissolve Amount;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;141;-1457.582,723.7697;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;144;-1205.879,781.3301;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;71;-1136.528,628.0174;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-912.8859,798.7648;Float;True;Property;_DisolveGuide;Disolve Guide;3;0;Create;True;0;0;False;0;2e309bd80f472034fb0a8684fb2b2482;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;111;-973.7111,623.8019;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.6;False;4;FLOAT;0.6;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-733.0505,604.6202;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;129;-1541.894,-241.5952;Float;False;814.5701;432.0292;Burn Effect - Emission;7;113;126;115;114;112;130;145;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;112;-1527.114,-9.797367;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-3;False;4;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;113;-1446.595,-200.3783;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;130;-1276.56,-207.5907;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;115;-1314.4,20.40818;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;114;-1154.105,5.319351;Float;True;Property;_BurnRamp;Burn Ramp;4;0;Create;True;0;0;False;0;516e751d989ccc349a10c51355b1c849;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;145;-790.193,17.52893;Float;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;132;144.1929,26.72195;Float;False;765.1592;493.9802;;1;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;78;-323.0742,-415.978;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;131;-329.1071,-216.2575;Float;True;Property;_Normal;Normal;2;0;Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-851.3253,-164.9277;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;146;-1126.193,215.5289;Float;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;False;0;0.69;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;435.9929,109.222;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Custom/DissolveWeapon;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0.03;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;144;0;141;1
WireConnection;144;1;141;2
WireConnection;71;0;4;0
WireConnection;2;1;144;0
WireConnection;111;0;71;0
WireConnection;73;0;111;0
WireConnection;73;1;2;1
WireConnection;112;0;73;0
WireConnection;113;0;112;0
WireConnection;130;0;113;0
WireConnection;115;0;130;0
WireConnection;114;1;115;0
WireConnection;145;0;114;0
WireConnection;145;1;146;0
WireConnection;126;0;130;0
WireConnection;126;1;114;0
WireConnection;0;0;78;0
WireConnection;0;1;131;0
WireConnection;0;2;126;0
WireConnection;0;10;73;0
ASEEND*/
//CHKSM=FFB9A3135DB0FC07627DD3F40CC7406F1A061086