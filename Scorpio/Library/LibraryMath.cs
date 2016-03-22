using Scorpio;
namespace Scorpio.Library {
    public class LibraryMath {
		public const float PI = 3.14159274f;
		public const float Deg2Rad = 0.0174532924f;
		public const float Rad2Deg = 57.29578f;
		public const float Epsilon = 1.401298E-45f;
        public static void Load(Script script) {
            ScriptTable Table = script.CreateTable();
			Table.SetValue("PI", script.CreateNumber(PI));
			Table.SetValue("Deg2Rad", script.CreateNumber(Deg2Rad));
			Table.SetValue("Rad2Deg", script.CreateNumber(Rad2Deg));
			Table.SetValue("Epsilon", script.CreateNumber(Epsilon));
			Table.SetValue("min", script.CreateFunction(new min()));
			Table.SetValue("max", script.CreateFunction(new max()));
			Table.SetValue("abs", script.CreateFunction(new abs()));
			Table.SetValue("floor", script.CreateFunction(new floor()));
			Table.SetValue("clamp", script.CreateFunction(new clamp()));
			Table.SetValue("sqrt", script.CreateFunction(new sqrt()));
			Table.SetValue("pow", script.CreateFunction(new pow()));
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
		private class floor : ScorpioHandle {
			public object Call(ScriptObject[] args) {
				return (args [0] as ScriptNumber).Floor ();
			}
		}
		private class clamp : ScorpioHandle {
			public object Call(ScriptObject[] args) {
				return (args [0] as ScriptNumber).Clamp (args [1] as ScriptNumber, args [2] as ScriptNumber);
			}
		}
		private class sqrt : ScorpioHandle {
			public object Call(ScriptObject[] args) {
				return (args [0] as ScriptNumber).Sqrt ();
			}
		}
		private class pow : ScorpioHandle {
			public object Call(ScriptObject[] args) {
				return (args [0] as ScriptNumber).Pow (args [1] as ScriptNumber);
			}
		}
    }
}
