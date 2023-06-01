using Scorpio.Function;
using Scorpio.Tools;
using Scorpio.Userdata;
namespace Scorpio {
    public partial class Script {
        private ObjectsPool<ScriptInstance> instancePool;
        private ObjectsPool<ScriptArray> arrayPool;
        private ObjectsPool<ScriptMapObject> mapObjectPool;
        private ObjectsPool<ScriptHashSet> hashSetPool;
        private ObjectsPool<ScriptStringBuilder> stringBuilderPool;
        private ObjectsPool<ScriptScriptFunction> functionPool;
        private ObjectsPool<ScriptScriptLambdaFunction> lambdaFunctionPool;
        private ObjectsPool<ScriptScriptAsyncFunction> asyncFunctionPool;
        private ObjectsPool<ScriptScriptAsyncLambdaFunction> asyncLambdaFunctionPool;


        private ObjectsPool<ScriptUserdataObject> userdataObjectPool;
        private ObjectsPool<ScriptInstanceMethodFunction> instanceMethodPool;
        private ObjectsPool<ScriptGenericMethodFunction> genericMethodPool;
        private ObjectsPool<ScriptStaticMethodFunction> staticMethodPool;

        private void InitPool() {
            instancePool = new ObjectsPool<ScriptInstance>(() => new ScriptInstance(this));
            arrayPool = new ObjectsPool<ScriptArray>(() => new ScriptArray(this));
            mapObjectPool = new ObjectsPool<ScriptMapObject>(() => new ScriptMapObject(this));
            hashSetPool = new ObjectsPool<ScriptHashSet>(() => new ScriptHashSet(this));
            stringBuilderPool = new ObjectsPool<ScriptStringBuilder>(() => new ScriptStringBuilder(this));
            functionPool = new ObjectsPool<ScriptScriptFunction>(() => new ScriptScriptFunction(this));
            lambdaFunctionPool = new ObjectsPool<ScriptScriptLambdaFunction>(() => new ScriptScriptLambdaFunction(this));
            asyncFunctionPool = new ObjectsPool<ScriptScriptAsyncFunction>(() => new ScriptScriptAsyncFunction(this));
            asyncLambdaFunctionPool = new ObjectsPool<ScriptScriptAsyncLambdaFunction>(() => new ScriptScriptAsyncLambdaFunction(this));

            userdataObjectPool = new ObjectsPool<ScriptUserdataObject>(() => new ScriptUserdataObject(this));
            instanceMethodPool = new ObjectsPool<ScriptInstanceMethodFunction>(() => new ScriptInstanceMethodFunction(this));
            genericMethodPool = new ObjectsPool<ScriptGenericMethodFunction>(() => new ScriptGenericMethodFunction(this));
            staticMethodPool = new ObjectsPool<ScriptStaticMethodFunction>(() => new ScriptStaticMethodFunction(this));
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
        public ScriptHashSet NewHashSet() {
            return hashSetPool.Alloc();
        }
        public ScriptStringBuilder NewStringBuilder() {
            return stringBuilderPool.Alloc();
        }
        public ScriptScriptFunction NewFunction() {
            return functionPool.Alloc();
        }
        public ScriptScriptLambdaFunction NewLambdaFunction() {
            return lambdaFunctionPool.Alloc();
        }
        public ScriptScriptAsyncFunction NewAsyncFunction() {
            return asyncFunctionPool.Alloc();
        }
        public ScriptScriptAsyncLambdaFunction NewAsyncLambdaFunction() {
            return asyncLambdaFunctionPool.Alloc();
        }

        public ScriptUserdataObject NewUserdataObject() {
            return userdataObjectPool.Alloc();
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


        public void Free(ScriptInstance value) {
            instancePool.Free(value);
        }
        public void Free(ScriptArray value) {
            arrayPool.Free(value);
        }
        public void Free(ScriptMapObject value) {
            mapObjectPool.Free(value);
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
        public void Free(ScriptScriptLambdaFunction value) {
            lambdaFunctionPool.Free(value);
        }
        public void Free(ScriptScriptAsyncFunction value) {
            asyncFunctionPool.Free(value);
        }
        public void Free(ScriptScriptAsyncLambdaFunction value) {
            asyncLambdaFunctionPool.Free(value);
        }
        public void Free(ScriptUserdataObject value) {
            userdataObjectPool.Free(value);
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
    }
}
