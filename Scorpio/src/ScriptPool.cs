using Scorpio.Function;
using Scorpio.Tools;
namespace Scorpio {
    public partial class Script {
        private ObjectsPool<ScriptArray> arrayPool;
        private ObjectsPool<ScriptMapObject> mapObjectPool;
        private ObjectsPool<ScriptHashSet> hashSetPool;
        private ObjectsPool<ScriptStringBuilder> stringBuilderPool;
        private ObjectsPool<ScriptScriptFunction> functionPool;
        private ObjectsPool<ScriptScriptLambdaFunction> lambdaFunctionPool;
        private ObjectsPool<ScriptScriptAsyncFunction> asyncFunctionPool;
        private ObjectsPool<ScriptScriptAsyncLambdaFunction> asyncLambdaFunctionPool;
        private void InitPool() {
            arrayPool = new ObjectsPool<ScriptArray>(() => new ScriptArray(this));
            mapObjectPool = new ObjectsPool<ScriptMapObject>(() => new ScriptMapObject(this));
            hashSetPool = new ObjectsPool<ScriptHashSet>(() => new ScriptHashSet(this));
            stringBuilderPool = new ObjectsPool<ScriptStringBuilder>(() => new ScriptStringBuilder(this));
            functionPool = new ObjectsPool<ScriptScriptFunction>(() => new ScriptScriptFunction(this));
            lambdaFunctionPool = new ObjectsPool<ScriptScriptLambdaFunction>(() => new ScriptScriptLambdaFunction(this));
            asyncFunctionPool = new ObjectsPool<ScriptScriptAsyncFunction>(() => new ScriptScriptAsyncFunction(this));
            asyncLambdaFunctionPool = new ObjectsPool<ScriptScriptAsyncLambdaFunction>(() => new ScriptScriptAsyncLambdaFunction(this));
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
    }
}
