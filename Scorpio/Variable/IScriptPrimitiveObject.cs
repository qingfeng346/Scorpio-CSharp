using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.Variable
{
    public abstract class IScriptPrimitiveObject : ScriptObject
    {
        public abstract object ObjectValue { get; }
    }
}
