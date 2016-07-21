Shader "Custom/StandardVertex"
{
	Properties
	{	
	}
	
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard vertex:vert noforwardadd
		#pragma target 3.0

		struct Input
		{
			float3 vertexColor; // Vertex color stored here by vert() method
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{	
			o.Albedo = IN.vertexColor;
		}

		ENDCG
	}
	FallBack "Diffuse"
}