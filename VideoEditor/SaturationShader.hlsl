struct VS_INPUT
{
    float2 pos : POSITION;
    float2 tex : TEXCOORD;
};

struct PS_INPUT
{
    float4 pos : SV_POSITION;
    float2 tex : TEXCOORD;
};

cbuffer SaturationBuffer : register(b0)
{
    float saturation;
};

Texture2D texture0 : register(t0);
SamplerState sampler0 : register(s0);

PS_INPUT VSMain(VS_INPUT input)
{
    PS_INPUT output;
    output.pos = float4(input.pos, 0.0, 1.0);
    output.tex = input.tex;
    return output;
}

float4 PSMain(PS_INPUT input) : SV_Target
{
    float4 color = texture0.Sample(sampler0, input.tex);
    
    // Convert to grayscale
    float luminance = dot(color.rgb, float3(0.299, 0.587, 0.114));
    
    // Interpolate between grayscale and original color
    color.rgb = lerp(float3(luminance, luminance, luminance), color.rgb, saturation);
    
    return color;
}
