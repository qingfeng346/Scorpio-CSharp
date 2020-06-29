using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
//using Commons.Util;
using Scorpio.ScorpioReflect;
public class BuildScorpioClass {
	class Filter : ClassFilter {
		public bool Check(GenerateScorpioClass generate, Type type, FieldInfo fieldInfo) {
			return true;
		}
		public bool Check(GenerateScorpioClass generate, Type type, EventInfo eventInfo) {
			return true;
		}
		public bool Check(GenerateScorpioClass generate, Type type, PropertyInfo propertyInfo) {
            if (propertyInfo.IsDefined(typeof(ObsoleteAttribute), true)) {
                return false;
            } else if (type == typeof(Texture2D) && propertyInfo.Name == "alphaIsTransparency") {
                return false;
            }
            return true;
		}
		public bool Check(GenerateScorpioClass generate, Type type, MethodInfo methodInfo) {
            if (methodInfo.IsDefined(typeof(ObsoleteAttribute), true))
                return false;
            else if (type == typeof(Input) && methodInfo.Name == "IsJoystickPreconfigured")
                return false;
			return true;
		}
	}
    public static string GetTypeFullName(Type type) {
        var fullName = type.FullName;
        if (type.IsGenericType) {
            var index = fullName.IndexOf("`");
            fullName = fullName.Substring(0, index);
            fullName += "<";
            var types = type.GetGenericArguments();
            bool first = true;
            foreach (var t in types) {
                if (first == false) { fullName += ","; } else { first = false; }
                fullName += GetTypeFullName(t);
            }
            fullName += ">";
        } else {
            fullName = fullName.Replace("+", ".");
        }
        return fullName;
    }
	[MenuItem("Assets/生成sco 脚本去反射类")]
	public static void Build() {
        var types = new Type[] {
            //Unity常用类
            typeof(UnityEngine.Application),
            // typeof(UnityEngine.SystemInfo),
            // typeof(UnityEngine.Time),
            // typeof(UnityEngine.Input),
            // //Unity资源类
            // typeof(UnityEngine.AudioClip),
            // typeof(UnityEngine.AssetBundle),
            // typeof(UnityEngine.Texture2D),
            // //Unity基础类
			// typeof(UnityEngine.GameObject),
			// typeof(UnityEngine.Transform),
			
            //Unity组件类
			//typeof(UnityEngine.BoxCollider),
			//typeof(UnityEngine.BoxCollider2D),
			//typeof(UnityEngine.Rigidbody),
			//typeof(UnityEngine.Rigidbody2D),
			//typeof(UnityEngine.UI.Image),
			//typeof(UnityEngine.UI.Text),

        };
        StringBuilder builder = new StringBuilder();
        builder.Append(@"using Scorpio.Userdata;
public class ScorpioClassManager {
    public static void Initialize(Scorpio.Script script) {
");
		foreach (var type in types) {
			var generate = new Scorpio.ScorpioReflect.GenerateScorpioClass(type);
			generate.SetClassFilter(new Filter());
            System.IO.File.WriteAllBytes(Application.dataPath + "/Scripts/ScorpioClass/" + generate.ScorpioClassName + ".cs", System.Text.Encoding.UTF8.GetBytes(generate.Generate()));
            builder.AppendFormat("        TypeManager.PushFastReflectClass(typeof({0}), new {1}(script));\n", GetTypeFullName(type), generate.ScorpioClassName);
        }
        builder.Append(@"    }
}
");
        System.IO.File.WriteAllBytes(Application.dataPath + "/Scripts/ScorpioClass/ScorpioClassManager.cs", System.Text.Encoding.UTF8.GetBytes(builder.ToString()));
        //Commons.Util.FileUtil.CreateFile(Application.dataPath + "/Scripts/Game/ScorpioClass/ScorpioClassManager.cs", builder.ToString(), Encoding.UTF8);
		AssetDatabase.Refresh();
	}
}
