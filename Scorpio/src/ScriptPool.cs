using Scorpio.Function;
using Scorpio.Library;
using Scorpio.Tools;
using Scorpio.Userdata;
using Scorpio.Runtime;
namespace Scorpio {
    public partial class Script {
        private ScriptObjectsPool<ScriptType> typePool;
        private ScriptObjectsPool<ScriptInstance> instancePool;
        private ScriptObjectsPool<ScriptArray> arrayPool;
        private ScriptObjectsPool<ScriptMapObject> mapObjectPool;
        private ScriptObjectsPool<ScriptMapString> mapStringPool;
        private ScriptObjectsPool<ScriptMapPolling> mapPollingPool;
        private ScriptObjectsPool<ScriptHashSet> hashSetPool;
        private ScriptObjectsPool<ScriptStringBuilder> stringBuilderPool;
        private ScriptObjectsPool<ScriptScriptFunction> functionPool;
        private ScriptObjectsPool<ScriptScriptBindFunction> bindFunctionPool;
        private ScriptObjectsPool<ScriptScriptAsyncFunction> asyncFunctionPool;
        private ScriptObjectsPool<ScriptScriptAsyncBindFunction> asyncBindFunctionPool;


        private ScriptObjectsPool<ScriptUserdataObject> userdataObjectPool;
        private ScriptObjectsPool<ScriptUserdataArray> userdataArrayPool;
        private ScriptObjectsPool<ScriptUserdataDelegate> userdataDelegatePool;
        private ScriptObjectsPool<ScriptInstanceMethodFunction> instanceMethodPool;
        private ScriptObjectsPool<ScriptGenericMethodFunction> genericMethodPool;
        private ScriptObjectsPool<ScriptStaticMethodFunction> staticMethodPool;

        private ObjectsPool<InternalValue[]> internalValuesPool;
        private ScriptObjectsPool<InternalValue> internalValuePool;
        private ScorpioJsonSerializer scorpioJsonSerializer;
        private ScorpioJsonDeserializer scorpioJsonDeserializer;
        private void InitPool() {
            typePool = new ScriptObjectsPool<ScriptType>(() => new ScriptType(this));
            instancePool = new ScriptObjectsPool<ScriptInstance>(() => new ScriptInstance(this));
            arrayPool = new ScriptObjectsPool<ScriptArray>(() => new ScriptArray(this));
            mapObjectPool = new ScriptObjectsPool<ScriptMapObject>(() => new ScriptMapObject(this));
            mapStringPool = new ScriptObjectsPool<ScriptMapString>(() => new ScriptMapString(this));
            mapPollingPool = new ScriptObjectsPool<ScriptMapPolling>(() => new ScriptMapPolling(this));

            hashSetPool = new ScriptObjectsPool<ScriptHashSet>(() => new ScriptHashSet(this));
            stringBuilderPool = new ScriptObjectsPool<ScriptStringBuilder>(() => new ScriptStringBuilder(this));
            functionPool = new ScriptObjectsPool<ScriptScriptFunction>(() => new ScriptScriptFunction(this));
            bindFunctionPool = new ScriptObjectsPool<ScriptScriptBindFunction>(() => new ScriptScriptBindFunction(this));
            asyncFunctionPool = new ScriptObjectsPool<ScriptScriptAsyncFunction>(() => new ScriptScriptAsyncFunction(this));
            asyncBindFunctionPool = new ScriptObjectsPool<ScriptScriptAsyncBindFunction>(() => new ScriptScriptAsyncBindFunction(this));

            userdataObjectPool = new ScriptObjectsPool<ScriptUserdataObject>(() => new ScriptUserdataObject(this));
            userdataArrayPool = new ScriptObjectsPool<ScriptUserdataArray>(() => new ScriptUserdataArray(this));
            userdataDelegatePool = new ScriptObjectsPool<ScriptUserdataDelegate>(() => new ScriptUserdataDelegate(this));
            instanceMethodPool = new ScriptObjectsPool<ScriptInstanceMethodFunction>(() => new ScriptInstanceMethodFunction(this));
            genericMethodPool = new ScriptObjectsPool<ScriptGenericMethodFunction>(() => new ScriptGenericMethodFunction(this));
            staticMethodPool = new ScriptObjectsPool<ScriptStaticMethodFunction>(() => new ScriptStaticMethodFunction(this));

            internalValuesPool = new ObjectsPool<InternalValue[]>(() => new InternalValue[64]);
            internalValuePool = new ScriptObjectsPool<InternalValue>(() => new InternalValue(this));

            scorpioJsonSerializer = new ScorpioJsonSerializer();
            scorpioJsonDeserializer = new ScorpioJsonDeserializer(this);
        }
        public ScriptType NewType() {
            return typePool.Alloc();
        }
        public ScriptInstance NewInstance() {
            return instancePool.Alloc();
        }
        public ScriptArray NewArray() {
            return arrayPool.Alloc();
        }
        public ScriptMapObject NewMapObject() {
            return mapObjectPool.Alloc();
        }
        public ScriptMapString NewMapString() {
            return mapStringPool.Alloc();
        }
        public ScriptMapPolling NewMapPolling() {
            return mapPollingPool.Alloc();
        }
        public ScriptHashSet NewHashSet() {
            return hashSetPool.Alloc();
        }
        public ScriptStringBuilder NewStringBuilder() {
            return stringBuilderPool.Alloc();
        }
        public ScriptScriptFunction NewFunction() {
            return functionPool.Alloc();
        }
        public ScriptScriptBindFunction NewBindFunction() {
            return bindFunctionPool.Alloc();
        }
        public ScriptScriptAsyncFunction NewAsyncFunction() {
            return asyncFunctionPool.Alloc();
        }
        public ScriptScriptAsyncBindFunction NewAsyncBindFunction() {
            return asyncBindFunctionPool.Alloc();
        }

        public ScriptUserdataObject NewUserdataObject() {
            return userdataObjectPool.Alloc();
        }
        public ScriptUserdataArray NewUserdataArray() {
            return userdataArrayPool.Alloc();
        }
        public ScriptUserdataDelegate NewUserdataDelegate() {
            return userdataDelegatePool.Alloc();
        }
        public ScriptInstanceMethodFunction NewInstanceMethod() {
            return instanceMethodPool.Alloc();
        }
        public ScriptGenericMethodFunction NewGenericMethod() {
            return genericMethodPool.Alloc();
        }
        public ScriptStaticMethodFunction NewStaticMethod() {
            return staticMethodPool.Alloc();
        }
        public InternalValue[] NewIntervalValues() {
            return internalValuesPool.Alloc();
        }
        public InternalValue NewIntervalValue() {
            return internalValuePool.Alloc();
        }


        public void Free(ScriptType value) {
            typePool.Free(value);
        }
        public void Free(ScriptInstance value) {
            instancePool.Free(value);
        }
        public void Free(ScriptArray value) {
            arrayPool.Free(value);
        }
        public void Free(ScriptMapObject value) {
            mapObjectPool.Free(value);
        }
        public void Free(ScriptMapString value) {
            mapStringPool.Free(value);
        }
        public void Free(ScriptMapPolling value) {
            mapPollingPool.Free(value);
        }
        public void Free(ScriptHashSet value) {
            hashSetPool.Free(value);
        }
        public void Free(ScriptStringBuilder value) {
            stringBuilderPool.Free(value);
        }
        public void Free(ScriptScriptFunction value) {
            functionPool.Free(value);
        }
        public void Free(ScriptScriptBindFunction value) {
            bindFunctionPool.Free(value);
        }
        public void Free(ScriptScriptAsyncFunction value) {
            asyncFunctionPool.Free(value);
        }
        public void Free(ScriptScriptAsyncBindFunction value) {
            asyncBindFunctionPool.Free(value);
        }
        public void Free(ScriptUserdataObject value) {
            userdataObjectPool.Free(value);
        }
        public void Free(ScriptUserdataArray value) {
            userdataArrayPool.Free(value);
        }
        public void Free(ScriptUserdataDelegate value) {
            userdataDelegatePool.Free(value);
        }
        public void Free(ScriptInstanceMethodFunction value) {
            instanceMethodPool.Free(value);
        }
        public void Free(ScriptGenericMethodFunction value) {
            genericMethodPool.Free(value);
        }
        public void Free(ScriptStaticMethodFunction value) {
            staticMethodPool.Free(value);
        }
        public void Free(InternalValue[] internalValues) {
            internalValuesPool.Free(internalValues);
        }
        public void Free(InternalValue internalValue) {
            internalValuePool.Free(internalValue);
        }

        public string ToJson(ScriptValue scriptValue) {
            using (scorpioJsonSerializer) {
                return scorpioJsonSerializer.ToJson(scriptValue);
            }
        }
        public string ToJson(ScriptObject scriptObject) {
            using (scorpioJsonSerializer) {
                return scorpioJsonSerializer.ToJson(scriptObject);
            }
        }
        public ScriptValue ParseJson(string buffer, bool supportLong) {
            using (scorpioJsonDeserializer) {
                return scorpioJsonDeserializer.Parse(buffer, supportLong);
            }
        }

        void Check<T>(ObjectsPool<T> pool) {
            var count = pool.Check();
            if (count != 0) ScorpioLogger.error($"当前未释放{pool}变量 数量:{count}");
        }
        void Check<T>(ScriptObjectsPool<T> pool) where T : IPool {
            var count = pool.Check();
            if (count != 0) ScorpioLogger.error($"当前未释放{pool}变量 数量:{count}");
        }
        public void GCCollect() {
            ScriptObjectReference.GCCollect();
        }
        public void CheckGCCollect() {
            ScriptObjectReference.CheckGCCollect();
        }
        public void CheckPool() {
            StringReference.CheckPool();
            ScriptObjectReference.CheckPool();
        }
        public void CheckValuePool() {
            Check(typePool);
            Check(instancePool);
            Check(arrayPool);
            Check(mapObjectPool);
            Check(mapStringPool);
            Check(mapPollingPool);
            Check(hashSetPool);
            Check(stringBuilderPool);
            Check(functionPool);
            Check(bindFunctionPool);
            Check(asyncFunctionPool);
            Check(asyncBindFunctionPool);

            Check(userdataObjectPool);
            Check(userdataArrayPool);
            Check(userdataDelegatePool);
            Check(instanceMethodPool);
            Check(genericMethodPool);
            Check(staticMethodPool);

            Check(internalValuesPool);
            Check(internalValuePool);
        }
    }
}
