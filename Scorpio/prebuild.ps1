$contextExcute = Get-Content -Path ./src/Runtime/ScriptContextExecute.cs -Encoding utf8
$contextExcute = $contextExcute.Replace("InternalValue[] internalValues", "InternalValue[] internalValues, ScriptType baseType")
$contextExcute = $contextExcute.Replace("thisObject.Get<ScriptInstance>().Prototype.Get<ScriptType>().Prototype", "baseType.Prototype")
$contextExcute | Out-File -Encoding utf8 ./src/Runtime/ScriptContextExecuteBase.cs

$contextExcute = Get-Content -Path ./src/Runtime/ScriptContextExecuteUnsafe.cs -Encoding utf8
$contextExcute = $contextExcute.Replace("InternalValue[] internalValues", "InternalValue[] internalValues, ScriptType baseType")
$contextExcute = $contextExcute.Replace("thisObject.Get<ScriptInstance>().Prototype.Get<ScriptType>().Prototype", "baseType.Prototype")
$contextExcute | Out-File -Encoding utf8 ./src/Runtime/ScriptContextExecuteUnsafeBase.cs