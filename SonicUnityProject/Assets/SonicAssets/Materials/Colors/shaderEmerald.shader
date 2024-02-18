// Shader created with Shader Forge v1.40 
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,cpap:True,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:5745,x:33671,y:32616,varname:node_5745,prsc:2|diff-3987-OUT,diffpow-726-OUT,spec-726-OUT,gloss-726-OUT,normal-6687-RGB,emission-3811-OUT,olcol-1002-RGB;n:type:ShaderForge.SFN_Fresnel,id:3182,x:32242,y:32818,varname:node_3182,prsc:2|EXP-5328-OUT;n:type:ShaderForge.SFN_Color,id:1002,x:32150,y:32627,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1002,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Divide,id:3973,x:32672,y:32646,varname:node_3973,prsc:2|A-237-OUT,B-3099-OUT;n:type:ShaderForge.SFN_Vector1,id:3099,x:32111,y:32771,varname:node_3099,prsc:2,v1:2.2;n:type:ShaderForge.SFN_Vector1,id:726,x:33498,y:32655,varname:node_726,prsc:2,v1:1;n:type:ShaderForge.SFN_Tex2d,id:7400,x:32057,y:32946,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_7400,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:656e8dad6d8d87a45bcf01b2ea57e287,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector1,id:5328,x:32057,y:32877,varname:node_5328,prsc:2,v1:1.9;n:type:ShaderForge.SFN_OneMinus,id:5632,x:32576,y:32977,varname:node_5632,prsc:2|IN-7400-RGB;n:type:ShaderForge.SFN_Multiply,id:3987,x:33325,y:32621,varname:node_3987,prsc:2|A-2025-OUT,B-1002-RGB;n:type:ShaderForge.SFN_Clamp,id:6577,x:32509,y:32803,varname:node_6577,prsc:2|IN-3182-OUT,MIN-8906-OUT,MAX-1699-OUT;n:type:ShaderForge.SFN_Vector1,id:8906,x:32362,y:32879,varname:node_8906,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:1699,x:32362,y:32929,varname:node_1699,prsc:2,v1:0.6;n:type:ShaderForge.SFN_RemapRange,id:6652,x:32672,y:32803,varname:node_6652,prsc:2,frmn:0,frmx:0.6,tomn:0,tomx:1|IN-6577-OUT;n:type:ShaderForge.SFN_RemapRange,id:3811,x:33296,y:32745,varname:node_3811,prsc:2,frmn:0,frmx:1,tomn:-0.2,tomx:1.5|IN-3987-OUT;n:type:ShaderForge.SFN_Clamp,id:2025,x:33055,y:32729,varname:node_2025,prsc:2|IN-645-OUT,MIN-3973-OUT,MAX-1002-RGB;n:type:ShaderForge.SFN_Lerp,id:645,x:32962,y:32996,varname:node_645,prsc:2|A-5632-OUT,B-8270-OUT,T-6652-OUT;n:type:ShaderForge.SFN_RemapRange,id:8270,x:32287,y:32993,varname:node_8270,prsc:2,frmn:0.3,frmx:1,tomn:0,tomx:1.3|IN-7400-RGB;n:type:ShaderForge.SFN_Tex2d,id:6687,x:32001,y:33161,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_6687,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:94f3371b0a3359144b415a1495bd06bc,ntxv:3,isnm:True;n:type:ShaderForge.SFN_AmbientLight,id:6232,x:32509,y:32473,varname:node_6232,prsc:2;n:type:ShaderForge.SFN_Normalize,id:237,x:32685,y:32473,varname:node_237,prsc:2|IN-6232-RGB;proporder:1002-7400-6687;pass:END;sub:END;*/

Shader "Custom/shaderEmerald" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Texture ("Texture", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float4, _Color)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float node_726 = 1.0;
                float gloss = node_726;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float3 specularColor = float3(node_726,node_726,node_726);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = pow(max( 0.0, NdotL), node_726) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float4 _Color_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Color );
                float3 node_3987 = (clamp(lerp((1.0 - _Texture_var.rgb),(_Texture_var.rgb*1.857143+-0.5571429),(clamp(pow(1.0-max(0,dot(normalDirection, viewDirection)),1.9),0.0,0.6)*1.666667+0.0)),(normalize(UNITY_LIGHTMODEL_AMBIENT.rgb)/2.2),_Color_var.rgb)*_Color_var.rgb);
                float3 diffuseColor = node_3987;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = (node_3987*1.7+-0.2);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float4, _Color)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float node_726 = 1.0;
                float gloss = node_726;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float3 specularColor = float3(node_726,node_726,node_726);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = pow(max( 0.0, NdotL), node_726) * attenColor;
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float4 _Color_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Color );
                float3 node_3987 = (clamp(lerp((1.0 - _Texture_var.rgb),(_Texture_var.rgb*1.857143+-0.5571429),(clamp(pow(1.0-max(0,dot(normalDirection, viewDirection)),1.9),0.0,0.6)*1.666667+0.0)),(normalize(UNITY_LIGHTMODEL_AMBIENT.rgb)/2.2),_Color_var.rgb)*_Color_var.rgb);
                float3 diffuseColor = node_3987;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
