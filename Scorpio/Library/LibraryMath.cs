using System;
using System.Collections.Generic;
using System.Text;
namespace Scorpio.Library {
    public class LibraryMath {
		public const float PI = 3.14159274f;
		public const float Infinity = float.PositiveInfinity;
		public const float NegativeInfinity = float.NegativeInfinity;
		public const float Deg2Rad = 0.0174532924f;
		public const float Rad2Deg = 57.29578f;
		public const float Epsilon = 1.401298E-45f;
        public static void Load(Script script) {
            ScriptTable Table = script.CreateTable();
			script.SetObject ("PI", PI);
			script.SetObject ("Infinity", Infinity);
			script.SetObject ("NegativeInfinity", NegativeInfinity);
			script.SetObject ("Deg2Rad", Deg2Rad);
			script.SetObject ("Rad2Deg", Rad2Deg);
			script.SetObject ("Epsilon", Epsilon);
            script.SetObjectInternal("math", Table);
        }
		private class min : ScorpioHandle {
			public object Call(ScriptObject[] args) {
				int num = args.Length;
				if (num == 0) return 0f;
				double num2 = (args[0] as ScriptNumber).ToDouble();
				for (int i = 1; i < num; i++) {
					double num3 = (args[i] as ScriptNumber).ToDouble();
					if (num3 < num2)
						num2 = num3;
				}
				return num2;
			}
		}
		private class max : ScorpioHandle {
			public object Call(ScriptObject[] args) {
				int num = args.Length;
				if (num == 0) return 0f;
				double num2 = (args[0] as ScriptNumber).ToDouble();
				for (int i = 1; i < num; i++) {
					double num3 = (args[i] as ScriptNumber).ToDouble();
					if (num3 > num2)
						num2 = num3;
				}
				return num2;
			}
		}
		private class abs : ScorpioHandle {
			public object Call(ScriptObject[] args) {
				return (args [0] as ScriptNumber).Abs ();
			}
		}
    }
}
