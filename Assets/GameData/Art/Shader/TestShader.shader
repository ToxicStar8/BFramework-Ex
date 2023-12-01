//"test"是Shader真正的名字，支持目录结构
Shader "test/test"
{
	/*材质球参数及UI面板
	https://docs.unity3d.com/cn/current/Manual/SL-Properties.html
	https://docs.unity3d.com/cn/current/ScriptReference/MaterialPropertyDrawer.html
	https://zhuanlan.zhihu.com/p/93194054
	*/

    //面板变量声明
    Properties
    {
        //下划线前缀 约定俗成的名字声明
        _Float("Float",Float) = 0.5
        _Range("Range",Range(0,1)) = 0.1
        _Color("Color",Color) = (1,1,1,1)
        _Vector("Vector",Vector) = (1,1,1,1)
        //white是默认无贴图的颜色
        _MainTex("MainTex",2D) = "white"{}
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode",float) = 2
    }

	/*
	这是为了让你可以在一个Shader文件中写多种版本的Shader，但只有一个会被使用。
	提供多个版本的SubShader，Unity可以根据对应平台选择最合适的Shader
	或者配合LOD机制一起使用。
	一般写一个即可
	*/
    SubShader
    {
		/*
		标签属性，有两种：一种是SubShader层级，一种在Pass层级
		https://docs.unity3d.com/cn/current/Manual/SL-SubShaderTags.html
		https://docs.unity3d.com/cn/current/Manual/SL-PassTags.html
		*/
		Tags { "RenderType"="Opaque" } 

		/*
		Pass里面的内容Shader代码真正起作用的地方，
		一个Pass对应一个真正意义上运行在GPU上的完整着色器(Vertex-Fragment Shader)
		一个SubShader里面可以包含多个Pass，每个Pass会被按顺序执行，每一个Pass渲染一次
		*/
        Pass
        {
            //背面剔除开启/关闭 Off=双面显示=0 Back=默认状态背面剔除=1 Front=正面剔除=2
            //Cull Off
            Cull [_CullMode]
            //Shader代码从这里开始
            CGPROGRAM

            //顶点Shader
            #pragma vertex vert
            //片元Shader
            #pragma fragment frag
            //Unity库，很多内置api可调用
            #include "UnityCG.cginc"

            //CPU向顶点Shader提供的模型数据 名字一般为appdata
			//https://docs.unity3d.com/Manual/SL-VertexProgramInputs.html
            struct appdata
            {
				//冒号后面的是特定语义词，告诉CPU需要哪些类似的数据
                //POSITION表示模型的坐标
                float4 vertex : POSITION;
                //TEXCOORD0表示贴图的第一套uv 最多可以写4套uv：TEXCOORD0 ~ TEXCOORD3
                half2 uv : TEXCOORD0;
                //half2 uv2 : TEXCOORD1;
                //顶点法线
                half3 normal : NORMAL;
                //顶点颜色
                half4 color : COLOR;
                //顶点切线(模型导入Unity后自动计算得到)
				half4 tangent : TANGENT; 
            };

            //自定义数据结构体vertex to fragment，顶点Shader的输出数据，也是片元Shader的输入数据
            struct v2f
            {
                //输出裁剪空间下的顶点坐标数据，给光栅化使用，必须要写的数据
                float4 vertex : SV_POSITION;
				//注意跟上方的TEXCOORD的意义是不一样的，上方代表的是UV，这里可以是任意数据。
				//插值器：输出后会被光栅化进行插值，而后作为输入数据，进入片元Shader
				//最多可以写16个：TEXCOORD0 ~ TEXCOORD15。
                float2 uv  : TEXCOORD0;
                float3 normal  : NORMAL;
            };

            //Shader内的变量声明，如果是与Properties内部变量一样名字的参数，参数可以链接
			//Unity内置变量：https://docs.unity3d.com/560/Documentation/Manual/SL-UnityShaderVariables.html
			//Unity内置函数：https://docs.unity3d.com/560/Documentation/Manual/SL-BuiltinFunctions.html
            float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Vector;

            //顶点Shader，输入参数为模型数据，返回顶点数据v2f
            v2f vert(appdata v)
            {
                v2f o;

                /*
                //模型空间转换到世界空间
                float4 pos_world = mul(unity_ObjectToWorld,v.vertex);
                //世界空间转换到相机空间
                float4 pos_view = mul(UNITY_MATRIX_V,pos_world);
                //相机空间转换到裁剪空间
                float4 pos_clip = mul(UNITY_MATRIX_P,pos_view);
                //赋值
                o.vertex = pos_clip;
                */

                //Unity库自带的模型空间到裁剪空间转换的Api
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                //xy=平铺 zw=偏移(uv流动)
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            };
            
            //float精度32位用于坐标点 half精度16位用于uv和大部分向量 fixed精度8位用于颜色
            //片元Shader，输入参数为顶点数据v2f，返回颜色信息
            //SV_Target表示为：片元Shader输出的目标地（渲染目标）
            float4 frag(v2f o) : SV_Target
            {
                //使用_Time.y计算时间流逝
				fixed4 col = tex2D(_MainTex, o.uv + _Time.y * _Vector.xy);
                //alpha测试 但是使用clip方式进行
                //clip(col - _Float);
                return col;
            };

            //Shader代码从这里结束
            ENDCG
        }
    }
}
