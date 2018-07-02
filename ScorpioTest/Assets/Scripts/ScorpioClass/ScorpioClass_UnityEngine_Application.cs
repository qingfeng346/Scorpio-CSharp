using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio;
using Scorpio.Userdata;
using Scorpio.Variable;
public class ScorpioClass_UnityEngine_Application : IScorpioFastReflectClass {
    private Script m_Script;
    public ScorpioClass_UnityEngine_Application(Script script) {
        m_Script = script;
    }
    public FastReflectUserdataMethod GetConstructor() {
        return ScorpioClass_UnityEngine_Application_Constructor.GetMethod(m_Script);
    }
    public object GetValue(object obj, string name) {
        if (name == "absoluteURL") return UnityEngine.Application.absoluteURL;
        if (name == "backgroundLoadingPriority") return UnityEngine.Application.backgroundLoadingPriority;
        if (name == "buildGUID") return UnityEngine.Application.buildGUID;
        if (name == "cloudProjectId") return UnityEngine.Application.cloudProjectId;
        if (name == "companyName") return UnityEngine.Application.companyName;
        if (name == "dataPath") return UnityEngine.Application.dataPath;
        if (name == "genuine") return UnityEngine.Application.genuine;
        if (name == "genuineCheckAvailable") return UnityEngine.Application.genuineCheckAvailable;
        if (name == "identifier") return UnityEngine.Application.identifier;
        if (name == "installerName") return UnityEngine.Application.installerName;
        if (name == "installMode") return UnityEngine.Application.installMode;
        if (name == "internetReachability") return UnityEngine.Application.internetReachability;
        if (name == "isConsolePlatform") return UnityEngine.Application.isConsolePlatform;
        if (name == "isEditor") return UnityEngine.Application.isEditor;
        if (name == "isFocused") return UnityEngine.Application.isFocused;
        if (name == "isMobilePlatform") return UnityEngine.Application.isMobilePlatform;
        if (name == "isPlaying") return UnityEngine.Application.isPlaying;
        if (name == "persistentDataPath") return UnityEngine.Application.persistentDataPath;
        if (name == "platform") return UnityEngine.Application.platform;
        if (name == "productName") return UnityEngine.Application.productName;
        if (name == "runInBackground") return UnityEngine.Application.runInBackground;
        if (name == "sandboxType") return UnityEngine.Application.sandboxType;
        if (name == "streamedBytes") return UnityEngine.Application.streamedBytes;
        if (name == "streamingAssetsPath") return UnityEngine.Application.streamingAssetsPath;
        if (name == "systemLanguage") return UnityEngine.Application.systemLanguage;
        if (name == "targetFrameRate") return UnityEngine.Application.targetFrameRate;
        if (name == "temporaryCachePath") return UnityEngine.Application.temporaryCachePath;
        if (name == "unityVersion") return UnityEngine.Application.unityVersion;
        if (name == "version") return UnityEngine.Application.version;
        if (name == "add_logMessageReceived") return ScorpioClass_UnityEngine_Application_add_logMessageReceived.GetInstance(m_Script, obj);
        if (name == "add_logMessageReceivedThreaded") return ScorpioClass_UnityEngine_Application_add_logMessageReceivedThreaded.GetInstance(m_Script, obj);
        if (name == "add_lowMemory") return ScorpioClass_UnityEngine_Application_add_lowMemory.GetInstance(m_Script, obj);
        if (name == "add_onBeforeRender") return ScorpioClass_UnityEngine_Application_add_onBeforeRender.GetInstance(m_Script, obj);
        if (name == "CancelQuit") return ScorpioClass_UnityEngine_Application_CancelQuit.GetInstance(m_Script, obj);
        if (name == "CanStreamedLevelBeLoaded") return ScorpioClass_UnityEngine_Application_CanStreamedLevelBeLoaded.GetInstance(m_Script, obj);
        if (name == "Equals") return ScorpioClass_UnityEngine_Application_Equals.GetInstance(m_Script, obj);
        if (name == "get_absoluteURL") return ScorpioClass_UnityEngine_Application_get_absoluteURL.GetInstance(m_Script, obj);
        if (name == "get_backgroundLoadingPriority") return ScorpioClass_UnityEngine_Application_get_backgroundLoadingPriority.GetInstance(m_Script, obj);
        if (name == "get_buildGUID") return ScorpioClass_UnityEngine_Application_get_buildGUID.GetInstance(m_Script, obj);
        if (name == "get_cloudProjectId") return ScorpioClass_UnityEngine_Application_get_cloudProjectId.GetInstance(m_Script, obj);
        if (name == "get_companyName") return ScorpioClass_UnityEngine_Application_get_companyName.GetInstance(m_Script, obj);
        if (name == "get_dataPath") return ScorpioClass_UnityEngine_Application_get_dataPath.GetInstance(m_Script, obj);
        if (name == "get_genuine") return ScorpioClass_UnityEngine_Application_get_genuine.GetInstance(m_Script, obj);
        if (name == "get_genuineCheckAvailable") return ScorpioClass_UnityEngine_Application_get_genuineCheckAvailable.GetInstance(m_Script, obj);
        if (name == "get_identifier") return ScorpioClass_UnityEngine_Application_get_identifier.GetInstance(m_Script, obj);
        if (name == "get_installerName") return ScorpioClass_UnityEngine_Application_get_installerName.GetInstance(m_Script, obj);
        if (name == "get_installMode") return ScorpioClass_UnityEngine_Application_get_installMode.GetInstance(m_Script, obj);
        if (name == "get_internetReachability") return ScorpioClass_UnityEngine_Application_get_internetReachability.GetInstance(m_Script, obj);
        if (name == "get_isConsolePlatform") return ScorpioClass_UnityEngine_Application_get_isConsolePlatform.GetInstance(m_Script, obj);
        if (name == "get_isEditor") return ScorpioClass_UnityEngine_Application_get_isEditor.GetInstance(m_Script, obj);
        if (name == "get_isFocused") return ScorpioClass_UnityEngine_Application_get_isFocused.GetInstance(m_Script, obj);
        if (name == "get_isMobilePlatform") return ScorpioClass_UnityEngine_Application_get_isMobilePlatform.GetInstance(m_Script, obj);
        if (name == "get_isPlaying") return ScorpioClass_UnityEngine_Application_get_isPlaying.GetInstance(m_Script, obj);
        if (name == "get_persistentDataPath") return ScorpioClass_UnityEngine_Application_get_persistentDataPath.GetInstance(m_Script, obj);
        if (name == "get_platform") return ScorpioClass_UnityEngine_Application_get_platform.GetInstance(m_Script, obj);
        if (name == "get_productName") return ScorpioClass_UnityEngine_Application_get_productName.GetInstance(m_Script, obj);
        if (name == "get_runInBackground") return ScorpioClass_UnityEngine_Application_get_runInBackground.GetInstance(m_Script, obj);
        if (name == "get_sandboxType") return ScorpioClass_UnityEngine_Application_get_sandboxType.GetInstance(m_Script, obj);
        if (name == "get_streamedBytes") return ScorpioClass_UnityEngine_Application_get_streamedBytes.GetInstance(m_Script, obj);
        if (name == "get_streamingAssetsPath") return ScorpioClass_UnityEngine_Application_get_streamingAssetsPath.GetInstance(m_Script, obj);
        if (name == "get_systemLanguage") return ScorpioClass_UnityEngine_Application_get_systemLanguage.GetInstance(m_Script, obj);
        if (name == "get_targetFrameRate") return ScorpioClass_UnityEngine_Application_get_targetFrameRate.GetInstance(m_Script, obj);
        if (name == "get_temporaryCachePath") return ScorpioClass_UnityEngine_Application_get_temporaryCachePath.GetInstance(m_Script, obj);
        if (name == "get_unityVersion") return ScorpioClass_UnityEngine_Application_get_unityVersion.GetInstance(m_Script, obj);
        if (name == "get_version") return ScorpioClass_UnityEngine_Application_get_version.GetInstance(m_Script, obj);
        if (name == "GetBuildTags") return ScorpioClass_UnityEngine_Application_GetBuildTags.GetInstance(m_Script, obj);
        if (name == "GetHashCode") return ScorpioClass_UnityEngine_Application_GetHashCode.GetInstance(m_Script, obj);
        if (name == "GetStackTraceLogType") return ScorpioClass_UnityEngine_Application_GetStackTraceLogType.GetInstance(m_Script, obj);
        if (name == "GetStreamProgressForLevel") return ScorpioClass_UnityEngine_Application_GetStreamProgressForLevel.GetInstance(m_Script, obj);
        if (name == "GetType") return ScorpioClass_UnityEngine_Application_GetType.GetInstance(m_Script, obj);
        if (name == "HasProLicense") return ScorpioClass_UnityEngine_Application_HasProLicense.GetInstance(m_Script, obj);
        if (name == "HasUserAuthorization") return ScorpioClass_UnityEngine_Application_HasUserAuthorization.GetInstance(m_Script, obj);
        if (name == "OpenURL") return ScorpioClass_UnityEngine_Application_OpenURL.GetInstance(m_Script, obj);
        if (name == "Quit") return ScorpioClass_UnityEngine_Application_Quit.GetInstance(m_Script, obj);
        if (name == "ReferenceEquals") return ScorpioClass_UnityEngine_Application_ReferenceEquals.GetInstance(m_Script, obj);
        if (name == "remove_logMessageReceived") return ScorpioClass_UnityEngine_Application_remove_logMessageReceived.GetInstance(m_Script, obj);
        if (name == "remove_logMessageReceivedThreaded") return ScorpioClass_UnityEngine_Application_remove_logMessageReceivedThreaded.GetInstance(m_Script, obj);
        if (name == "remove_lowMemory") return ScorpioClass_UnityEngine_Application_remove_lowMemory.GetInstance(m_Script, obj);
        if (name == "remove_onBeforeRender") return ScorpioClass_UnityEngine_Application_remove_onBeforeRender.GetInstance(m_Script, obj);
        if (name == "RequestAdvertisingIdentifierAsync") return ScorpioClass_UnityEngine_Application_RequestAdvertisingIdentifierAsync.GetInstance(m_Script, obj);
        if (name == "RequestUserAuthorization") return ScorpioClass_UnityEngine_Application_RequestUserAuthorization.GetInstance(m_Script, obj);
        if (name == "set_backgroundLoadingPriority") return ScorpioClass_UnityEngine_Application_set_backgroundLoadingPriority.GetInstance(m_Script, obj);
        if (name == "set_runInBackground") return ScorpioClass_UnityEngine_Application_set_runInBackground.GetInstance(m_Script, obj);
        if (name == "set_targetFrameRate") return ScorpioClass_UnityEngine_Application_set_targetFrameRate.GetInstance(m_Script, obj);
        if (name == "SetBuildTags") return ScorpioClass_UnityEngine_Application_SetBuildTags.GetInstance(m_Script, obj);
        if (name == "SetStackTraceLogType") return ScorpioClass_UnityEngine_Application_SetStackTraceLogType.GetInstance(m_Script, obj);
        if (name == "ToString") return ScorpioClass_UnityEngine_Application_ToString.GetInstance(m_Script, obj);
        if (name == "Unload") return ScorpioClass_UnityEngine_Application_Unload.GetInstance(m_Script, obj);
        throw new Exception("UnityEngine.Application 找不到变量 : " + name);
    }
    public void SetValue(object obj, string name, ScriptObject value) {
        if (name == "backgroundLoadingPriority") { UnityEngine.Application.backgroundLoadingPriority = (UnityEngine.ThreadPriority)(Util.ChangeType(m_Script, value, typeof(UnityEngine.ThreadPriority))); return; }
        if (name == "runInBackground") { UnityEngine.Application.runInBackground = (System.Boolean)(Util.ChangeType(m_Script, value, typeof(System.Boolean))); return; }
        if (name == "targetFrameRate") { UnityEngine.Application.targetFrameRate = (System.Int32)(Util.ChangeType(m_Script, value, typeof(System.Int32))); return; }
        throw new Exception("UnityEngine.Application 找不到变量 : " + name);
    }

    public class ScorpioClass_UnityEngine_Application_Constructor : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_Constructor() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo(".ctor", false, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(false, script, typeof(UnityEngine.Application), "Constructor", _methods, new ScorpioClass_UnityEngine_Application_Constructor()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(false, script, typeof(UnityEngine.Application), "Constructor", _methods, new ScorpioClass_UnityEngine_Application_Constructor()); 
            }
            if (obj == null) {
                if (_instance == null) {
                    _instance = new ScorpioTypeMethod(script, "Constructor", _method, typeof(UnityEngine.Application));
                }
                return _instance;
            }
            return new ScorpioObjectMethod(obj, "Constructor", _method);
        }
        public object Call(object obj, string type, object[] args) {
		    if (type == "") return new UnityEngine.Application();
            throw new Exception("UnityEngine.Application 找不到合适的函数 : Constructor    type : " + type);
        }
    }

    public class ScorpioClass_UnityEngine_Application_add_logMessageReceived : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_add_logMessageReceived() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("add_logMessageReceived", true, new Type[] {typeof(UnityEngine.Application.LogCallback)}, false, null, "UnityEngine.Application.LogCallback+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "add_logMessageReceived", _methods, new ScorpioClass_UnityEngine_Application_add_logMessageReceived()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "add_logMessageReceived", _methods, new ScorpioClass_UnityEngine_Application_add_logMessageReceived()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("add_logMessageReceived", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.Application.LogCallback+") { UnityEngine.Application.logMessageReceived += (UnityEngine.Application.LogCallback)args[0]; return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : add_logMessageReceived    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_add_logMessageReceivedThreaded : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_add_logMessageReceivedThreaded() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("add_logMessageReceivedThreaded", true, new Type[] {typeof(UnityEngine.Application.LogCallback)}, false, null, "UnityEngine.Application.LogCallback+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "add_logMessageReceivedThreaded", _methods, new ScorpioClass_UnityEngine_Application_add_logMessageReceivedThreaded()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "add_logMessageReceivedThreaded", _methods, new ScorpioClass_UnityEngine_Application_add_logMessageReceivedThreaded()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("add_logMessageReceivedThreaded", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.Application.LogCallback+") { UnityEngine.Application.logMessageReceivedThreaded += (UnityEngine.Application.LogCallback)args[0]; return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : add_logMessageReceivedThreaded    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_add_lowMemory : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_add_lowMemory() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("add_lowMemory", true, new Type[] {typeof(UnityEngine.Application.LowMemoryCallback)}, false, null, "UnityEngine.Application.LowMemoryCallback+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "add_lowMemory", _methods, new ScorpioClass_UnityEngine_Application_add_lowMemory()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "add_lowMemory", _methods, new ScorpioClass_UnityEngine_Application_add_lowMemory()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("add_lowMemory", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.Application.LowMemoryCallback+") { UnityEngine.Application.lowMemory += (UnityEngine.Application.LowMemoryCallback)args[0]; return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : add_lowMemory    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_add_onBeforeRender : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_add_onBeforeRender() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("add_onBeforeRender", true, new Type[] {typeof(UnityEngine.Events.UnityAction)}, false, null, "UnityEngine.Events.UnityAction+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "add_onBeforeRender", _methods, new ScorpioClass_UnityEngine_Application_add_onBeforeRender()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "add_onBeforeRender", _methods, new ScorpioClass_UnityEngine_Application_add_onBeforeRender()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("add_onBeforeRender", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.Events.UnityAction+") { UnityEngine.Application.onBeforeRender += (UnityEngine.Events.UnityAction)args[0]; return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : add_onBeforeRender    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_CancelQuit : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_CancelQuit() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("CancelQuit", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "CancelQuit", _methods, new ScorpioClass_UnityEngine_Application_CancelQuit()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "CancelQuit", _methods, new ScorpioClass_UnityEngine_Application_CancelQuit()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("CancelQuit", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { UnityEngine.Application.CancelQuit(); return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : CancelQuit    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_CanStreamedLevelBeLoaded : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_CanStreamedLevelBeLoaded() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("CanStreamedLevelBeLoaded", true, new Type[] {typeof(System.String)}, false, null, "System.String+"));
            methods.Add(new ScorpioMethodInfo("CanStreamedLevelBeLoaded", true, new Type[] {typeof(System.Int32)}, false, null, "System.Int32+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "CanStreamedLevelBeLoaded", _methods, new ScorpioClass_UnityEngine_Application_CanStreamedLevelBeLoaded()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "CanStreamedLevelBeLoaded", _methods, new ScorpioClass_UnityEngine_Application_CanStreamedLevelBeLoaded()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("CanStreamedLevelBeLoaded", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "System.String+") { return UnityEngine.Application.CanStreamedLevelBeLoaded((System.String)args[0]); }
            if (type == "System.Int32+") { return UnityEngine.Application.CanStreamedLevelBeLoaded((System.Int32)args[0]); }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : CanStreamedLevelBeLoaded    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_Equals : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_Equals() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("Equals", false, new Type[] {typeof(System.Object)}, false, null, "System.Object+"));
            methods.Add(new ScorpioMethodInfo("Equals", true, new Type[] {typeof(System.Object),typeof(System.Object)}, false, null, "System.Object+System.Object+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "Equals", _methods, new ScorpioClass_UnityEngine_Application_Equals()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "Equals", _methods, new ScorpioClass_UnityEngine_Application_Equals()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("Equals", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "System.Object+") { return ((UnityEngine.Application)obj).Equals((System.Object)args[0]); }
            if (type == "System.Object+System.Object+") { return UnityEngine.Application.Equals((System.Object)args[0],(System.Object)args[1]); }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : Equals    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_absoluteURL : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_absoluteURL() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_absoluteURL", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_absoluteURL", _methods, new ScorpioClass_UnityEngine_Application_get_absoluteURL()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_absoluteURL", _methods, new ScorpioClass_UnityEngine_Application_get_absoluteURL()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_absoluteURL", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.absoluteURL; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_absoluteURL    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_backgroundLoadingPriority : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_backgroundLoadingPriority() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_backgroundLoadingPriority", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_backgroundLoadingPriority", _methods, new ScorpioClass_UnityEngine_Application_get_backgroundLoadingPriority()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_backgroundLoadingPriority", _methods, new ScorpioClass_UnityEngine_Application_get_backgroundLoadingPriority()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_backgroundLoadingPriority", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.backgroundLoadingPriority; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_backgroundLoadingPriority    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_buildGUID : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_buildGUID() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_buildGUID", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_buildGUID", _methods, new ScorpioClass_UnityEngine_Application_get_buildGUID()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_buildGUID", _methods, new ScorpioClass_UnityEngine_Application_get_buildGUID()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_buildGUID", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.buildGUID; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_buildGUID    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_cloudProjectId : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_cloudProjectId() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_cloudProjectId", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_cloudProjectId", _methods, new ScorpioClass_UnityEngine_Application_get_cloudProjectId()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_cloudProjectId", _methods, new ScorpioClass_UnityEngine_Application_get_cloudProjectId()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_cloudProjectId", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.cloudProjectId; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_cloudProjectId    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_companyName : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_companyName() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_companyName", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_companyName", _methods, new ScorpioClass_UnityEngine_Application_get_companyName()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_companyName", _methods, new ScorpioClass_UnityEngine_Application_get_companyName()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_companyName", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.companyName; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_companyName    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_dataPath : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_dataPath() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_dataPath", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_dataPath", _methods, new ScorpioClass_UnityEngine_Application_get_dataPath()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_dataPath", _methods, new ScorpioClass_UnityEngine_Application_get_dataPath()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_dataPath", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.dataPath; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_dataPath    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_genuine : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_genuine() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_genuine", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_genuine", _methods, new ScorpioClass_UnityEngine_Application_get_genuine()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_genuine", _methods, new ScorpioClass_UnityEngine_Application_get_genuine()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_genuine", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.genuine; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_genuine    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_genuineCheckAvailable : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_genuineCheckAvailable() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_genuineCheckAvailable", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_genuineCheckAvailable", _methods, new ScorpioClass_UnityEngine_Application_get_genuineCheckAvailable()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_genuineCheckAvailable", _methods, new ScorpioClass_UnityEngine_Application_get_genuineCheckAvailable()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_genuineCheckAvailable", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.genuineCheckAvailable; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_genuineCheckAvailable    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_identifier : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_identifier() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_identifier", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_identifier", _methods, new ScorpioClass_UnityEngine_Application_get_identifier()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_identifier", _methods, new ScorpioClass_UnityEngine_Application_get_identifier()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_identifier", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.identifier; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_identifier    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_installerName : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_installerName() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_installerName", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_installerName", _methods, new ScorpioClass_UnityEngine_Application_get_installerName()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_installerName", _methods, new ScorpioClass_UnityEngine_Application_get_installerName()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_installerName", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.installerName; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_installerName    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_installMode : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_installMode() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_installMode", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_installMode", _methods, new ScorpioClass_UnityEngine_Application_get_installMode()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_installMode", _methods, new ScorpioClass_UnityEngine_Application_get_installMode()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_installMode", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.installMode; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_installMode    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_internetReachability : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_internetReachability() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_internetReachability", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_internetReachability", _methods, new ScorpioClass_UnityEngine_Application_get_internetReachability()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_internetReachability", _methods, new ScorpioClass_UnityEngine_Application_get_internetReachability()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_internetReachability", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.internetReachability; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_internetReachability    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_isConsolePlatform : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_isConsolePlatform() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_isConsolePlatform", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_isConsolePlatform", _methods, new ScorpioClass_UnityEngine_Application_get_isConsolePlatform()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_isConsolePlatform", _methods, new ScorpioClass_UnityEngine_Application_get_isConsolePlatform()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_isConsolePlatform", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.isConsolePlatform; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_isConsolePlatform    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_isEditor : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_isEditor() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_isEditor", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_isEditor", _methods, new ScorpioClass_UnityEngine_Application_get_isEditor()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_isEditor", _methods, new ScorpioClass_UnityEngine_Application_get_isEditor()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_isEditor", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.isEditor; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_isEditor    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_isFocused : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_isFocused() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_isFocused", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_isFocused", _methods, new ScorpioClass_UnityEngine_Application_get_isFocused()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_isFocused", _methods, new ScorpioClass_UnityEngine_Application_get_isFocused()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_isFocused", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.isFocused; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_isFocused    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_isMobilePlatform : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_isMobilePlatform() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_isMobilePlatform", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_isMobilePlatform", _methods, new ScorpioClass_UnityEngine_Application_get_isMobilePlatform()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_isMobilePlatform", _methods, new ScorpioClass_UnityEngine_Application_get_isMobilePlatform()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_isMobilePlatform", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.isMobilePlatform; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_isMobilePlatform    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_isPlaying : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_isPlaying() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_isPlaying", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_isPlaying", _methods, new ScorpioClass_UnityEngine_Application_get_isPlaying()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_isPlaying", _methods, new ScorpioClass_UnityEngine_Application_get_isPlaying()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_isPlaying", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.isPlaying; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_isPlaying    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_persistentDataPath : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_persistentDataPath() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_persistentDataPath", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_persistentDataPath", _methods, new ScorpioClass_UnityEngine_Application_get_persistentDataPath()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_persistentDataPath", _methods, new ScorpioClass_UnityEngine_Application_get_persistentDataPath()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_persistentDataPath", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.persistentDataPath; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_persistentDataPath    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_platform : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_platform() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_platform", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_platform", _methods, new ScorpioClass_UnityEngine_Application_get_platform()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_platform", _methods, new ScorpioClass_UnityEngine_Application_get_platform()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_platform", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.platform; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_platform    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_productName : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_productName() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_productName", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_productName", _methods, new ScorpioClass_UnityEngine_Application_get_productName()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_productName", _methods, new ScorpioClass_UnityEngine_Application_get_productName()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_productName", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.productName; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_productName    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_runInBackground : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_runInBackground() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_runInBackground", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_runInBackground", _methods, new ScorpioClass_UnityEngine_Application_get_runInBackground()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_runInBackground", _methods, new ScorpioClass_UnityEngine_Application_get_runInBackground()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_runInBackground", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.runInBackground; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_runInBackground    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_sandboxType : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_sandboxType() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_sandboxType", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_sandboxType", _methods, new ScorpioClass_UnityEngine_Application_get_sandboxType()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_sandboxType", _methods, new ScorpioClass_UnityEngine_Application_get_sandboxType()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_sandboxType", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.sandboxType; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_sandboxType    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_streamedBytes : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_streamedBytes() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_streamedBytes", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_streamedBytes", _methods, new ScorpioClass_UnityEngine_Application_get_streamedBytes()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_streamedBytes", _methods, new ScorpioClass_UnityEngine_Application_get_streamedBytes()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_streamedBytes", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.streamedBytes; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_streamedBytes    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_streamingAssetsPath : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_streamingAssetsPath() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_streamingAssetsPath", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_streamingAssetsPath", _methods, new ScorpioClass_UnityEngine_Application_get_streamingAssetsPath()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_streamingAssetsPath", _methods, new ScorpioClass_UnityEngine_Application_get_streamingAssetsPath()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_streamingAssetsPath", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.streamingAssetsPath; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_streamingAssetsPath    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_systemLanguage : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_systemLanguage() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_systemLanguage", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_systemLanguage", _methods, new ScorpioClass_UnityEngine_Application_get_systemLanguage()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_systemLanguage", _methods, new ScorpioClass_UnityEngine_Application_get_systemLanguage()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_systemLanguage", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.systemLanguage; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_systemLanguage    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_targetFrameRate : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_targetFrameRate() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_targetFrameRate", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_targetFrameRate", _methods, new ScorpioClass_UnityEngine_Application_get_targetFrameRate()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_targetFrameRate", _methods, new ScorpioClass_UnityEngine_Application_get_targetFrameRate()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_targetFrameRate", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.targetFrameRate; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_targetFrameRate    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_temporaryCachePath : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_temporaryCachePath() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_temporaryCachePath", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_temporaryCachePath", _methods, new ScorpioClass_UnityEngine_Application_get_temporaryCachePath()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_temporaryCachePath", _methods, new ScorpioClass_UnityEngine_Application_get_temporaryCachePath()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_temporaryCachePath", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.temporaryCachePath; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_temporaryCachePath    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_unityVersion : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_unityVersion() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_unityVersion", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_unityVersion", _methods, new ScorpioClass_UnityEngine_Application_get_unityVersion()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_unityVersion", _methods, new ScorpioClass_UnityEngine_Application_get_unityVersion()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_unityVersion", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.unityVersion; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_unityVersion    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_get_version : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_get_version() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("get_version", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_version", _methods, new ScorpioClass_UnityEngine_Application_get_version()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "get_version", _methods, new ScorpioClass_UnityEngine_Application_get_version()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("get_version", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.version; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : get_version    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_GetBuildTags : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_GetBuildTags() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("GetBuildTags", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "GetBuildTags", _methods, new ScorpioClass_UnityEngine_Application_GetBuildTags()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "GetBuildTags", _methods, new ScorpioClass_UnityEngine_Application_GetBuildTags()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("GetBuildTags", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.GetBuildTags(); }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : GetBuildTags    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_GetHashCode : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_GetHashCode() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("GetHashCode", false, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(false, script, typeof(UnityEngine.Application), "GetHashCode", _methods, new ScorpioClass_UnityEngine_Application_GetHashCode()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(false, script, typeof(UnityEngine.Application), "GetHashCode", _methods, new ScorpioClass_UnityEngine_Application_GetHashCode()); 
            }
            if (obj == null) {
                if (_instance == null) {
                    _instance = new ScorpioTypeMethod(script, "GetHashCode", _method, typeof(UnityEngine.Application));
                }
                return _instance;
            }
            return new ScorpioObjectMethod(obj, "GetHashCode", _method);
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return ((UnityEngine.Application)obj).GetHashCode(); }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : GetHashCode    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_GetStackTraceLogType : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_GetStackTraceLogType() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("GetStackTraceLogType", true, new Type[] {typeof(UnityEngine.LogType)}, false, null, "UnityEngine.LogType+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "GetStackTraceLogType", _methods, new ScorpioClass_UnityEngine_Application_GetStackTraceLogType()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "GetStackTraceLogType", _methods, new ScorpioClass_UnityEngine_Application_GetStackTraceLogType()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("GetStackTraceLogType", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.LogType+") { return UnityEngine.Application.GetStackTraceLogType((UnityEngine.LogType)args[0]); }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : GetStackTraceLogType    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_GetStreamProgressForLevel : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_GetStreamProgressForLevel() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("GetStreamProgressForLevel", true, new Type[] {typeof(System.String)}, false, null, "System.String+"));
            methods.Add(new ScorpioMethodInfo("GetStreamProgressForLevel", true, new Type[] {typeof(System.Int32)}, false, null, "System.Int32+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "GetStreamProgressForLevel", _methods, new ScorpioClass_UnityEngine_Application_GetStreamProgressForLevel()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "GetStreamProgressForLevel", _methods, new ScorpioClass_UnityEngine_Application_GetStreamProgressForLevel()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("GetStreamProgressForLevel", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "System.String+") { return UnityEngine.Application.GetStreamProgressForLevel((System.String)args[0]); }
            if (type == "System.Int32+") { return UnityEngine.Application.GetStreamProgressForLevel((System.Int32)args[0]); }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : GetStreamProgressForLevel    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_GetType : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_GetType() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("GetType", false, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(false, script, typeof(UnityEngine.Application), "GetType", _methods, new ScorpioClass_UnityEngine_Application_GetType()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(false, script, typeof(UnityEngine.Application), "GetType", _methods, new ScorpioClass_UnityEngine_Application_GetType()); 
            }
            if (obj == null) {
                if (_instance == null) {
                    _instance = new ScorpioTypeMethod(script, "GetType", _method, typeof(UnityEngine.Application));
                }
                return _instance;
            }
            return new ScorpioObjectMethod(obj, "GetType", _method);
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return ((UnityEngine.Application)obj).GetType(); }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : GetType    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_HasProLicense : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_HasProLicense() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("HasProLicense", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "HasProLicense", _methods, new ScorpioClass_UnityEngine_Application_HasProLicense()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "HasProLicense", _methods, new ScorpioClass_UnityEngine_Application_HasProLicense()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("HasProLicense", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return UnityEngine.Application.HasProLicense(); }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : HasProLicense    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_HasUserAuthorization : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_HasUserAuthorization() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("HasUserAuthorization", true, new Type[] {typeof(UnityEngine.UserAuthorization)}, false, null, "UnityEngine.UserAuthorization+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "HasUserAuthorization", _methods, new ScorpioClass_UnityEngine_Application_HasUserAuthorization()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "HasUserAuthorization", _methods, new ScorpioClass_UnityEngine_Application_HasUserAuthorization()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("HasUserAuthorization", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.UserAuthorization+") { return UnityEngine.Application.HasUserAuthorization((UnityEngine.UserAuthorization)args[0]); }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : HasUserAuthorization    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_OpenURL : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_OpenURL() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("OpenURL", true, new Type[] {typeof(System.String)}, false, null, "System.String+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "OpenURL", _methods, new ScorpioClass_UnityEngine_Application_OpenURL()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "OpenURL", _methods, new ScorpioClass_UnityEngine_Application_OpenURL()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("OpenURL", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "System.String+") { UnityEngine.Application.OpenURL((System.String)args[0]); return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : OpenURL    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_Quit : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_Quit() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("Quit", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "Quit", _methods, new ScorpioClass_UnityEngine_Application_Quit()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "Quit", _methods, new ScorpioClass_UnityEngine_Application_Quit()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("Quit", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { UnityEngine.Application.Quit(); return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : Quit    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_ReferenceEquals : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_ReferenceEquals() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("ReferenceEquals", true, new Type[] {typeof(System.Object),typeof(System.Object)}, false, null, "System.Object+System.Object+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "ReferenceEquals", _methods, new ScorpioClass_UnityEngine_Application_ReferenceEquals()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "ReferenceEquals", _methods, new ScorpioClass_UnityEngine_Application_ReferenceEquals()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("ReferenceEquals", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "System.Object+System.Object+") { return UnityEngine.Application.ReferenceEquals((System.Object)args[0],(System.Object)args[1]); }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : ReferenceEquals    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_remove_logMessageReceived : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_remove_logMessageReceived() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("remove_logMessageReceived", true, new Type[] {typeof(UnityEngine.Application.LogCallback)}, false, null, "UnityEngine.Application.LogCallback+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "remove_logMessageReceived", _methods, new ScorpioClass_UnityEngine_Application_remove_logMessageReceived()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "remove_logMessageReceived", _methods, new ScorpioClass_UnityEngine_Application_remove_logMessageReceived()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("remove_logMessageReceived", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.Application.LogCallback+") { UnityEngine.Application.logMessageReceived -= (UnityEngine.Application.LogCallback)args[0]; return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : remove_logMessageReceived    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_remove_logMessageReceivedThreaded : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_remove_logMessageReceivedThreaded() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("remove_logMessageReceivedThreaded", true, new Type[] {typeof(UnityEngine.Application.LogCallback)}, false, null, "UnityEngine.Application.LogCallback+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "remove_logMessageReceivedThreaded", _methods, new ScorpioClass_UnityEngine_Application_remove_logMessageReceivedThreaded()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "remove_logMessageReceivedThreaded", _methods, new ScorpioClass_UnityEngine_Application_remove_logMessageReceivedThreaded()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("remove_logMessageReceivedThreaded", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.Application.LogCallback+") { UnityEngine.Application.logMessageReceivedThreaded -= (UnityEngine.Application.LogCallback)args[0]; return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : remove_logMessageReceivedThreaded    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_remove_lowMemory : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_remove_lowMemory() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("remove_lowMemory", true, new Type[] {typeof(UnityEngine.Application.LowMemoryCallback)}, false, null, "UnityEngine.Application.LowMemoryCallback+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "remove_lowMemory", _methods, new ScorpioClass_UnityEngine_Application_remove_lowMemory()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "remove_lowMemory", _methods, new ScorpioClass_UnityEngine_Application_remove_lowMemory()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("remove_lowMemory", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.Application.LowMemoryCallback+") { UnityEngine.Application.lowMemory -= (UnityEngine.Application.LowMemoryCallback)args[0]; return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : remove_lowMemory    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_remove_onBeforeRender : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_remove_onBeforeRender() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("remove_onBeforeRender", true, new Type[] {typeof(UnityEngine.Events.UnityAction)}, false, null, "UnityEngine.Events.UnityAction+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "remove_onBeforeRender", _methods, new ScorpioClass_UnityEngine_Application_remove_onBeforeRender()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "remove_onBeforeRender", _methods, new ScorpioClass_UnityEngine_Application_remove_onBeforeRender()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("remove_onBeforeRender", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.Events.UnityAction+") { UnityEngine.Application.onBeforeRender -= (UnityEngine.Events.UnityAction)args[0]; return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : remove_onBeforeRender    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_RequestAdvertisingIdentifierAsync : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_RequestAdvertisingIdentifierAsync() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("RequestAdvertisingIdentifierAsync", true, new Type[] {typeof(UnityEngine.Application.AdvertisingIdentifierCallback)}, false, null, "UnityEngine.Application.AdvertisingIdentifierCallback+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "RequestAdvertisingIdentifierAsync", _methods, new ScorpioClass_UnityEngine_Application_RequestAdvertisingIdentifierAsync()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "RequestAdvertisingIdentifierAsync", _methods, new ScorpioClass_UnityEngine_Application_RequestAdvertisingIdentifierAsync()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("RequestAdvertisingIdentifierAsync", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.Application.AdvertisingIdentifierCallback+") { return UnityEngine.Application.RequestAdvertisingIdentifierAsync((UnityEngine.Application.AdvertisingIdentifierCallback)args[0]); }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : RequestAdvertisingIdentifierAsync    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_RequestUserAuthorization : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_RequestUserAuthorization() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("RequestUserAuthorization", true, new Type[] {typeof(UnityEngine.UserAuthorization)}, false, null, "UnityEngine.UserAuthorization+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "RequestUserAuthorization", _methods, new ScorpioClass_UnityEngine_Application_RequestUserAuthorization()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "RequestUserAuthorization", _methods, new ScorpioClass_UnityEngine_Application_RequestUserAuthorization()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("RequestUserAuthorization", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.UserAuthorization+") { return UnityEngine.Application.RequestUserAuthorization((UnityEngine.UserAuthorization)args[0]); }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : RequestUserAuthorization    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_set_backgroundLoadingPriority : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_set_backgroundLoadingPriority() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("set_backgroundLoadingPriority", true, new Type[] {typeof(UnityEngine.ThreadPriority)}, false, null, "UnityEngine.ThreadPriority+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "set_backgroundLoadingPriority", _methods, new ScorpioClass_UnityEngine_Application_set_backgroundLoadingPriority()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "set_backgroundLoadingPriority", _methods, new ScorpioClass_UnityEngine_Application_set_backgroundLoadingPriority()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("set_backgroundLoadingPriority", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.ThreadPriority+") { UnityEngine.Application.backgroundLoadingPriority = (UnityEngine.ThreadPriority)args[0]; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : set_backgroundLoadingPriority    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_set_runInBackground : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_set_runInBackground() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("set_runInBackground", true, new Type[] {typeof(System.Boolean)}, false, null, "System.Boolean+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "set_runInBackground", _methods, new ScorpioClass_UnityEngine_Application_set_runInBackground()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "set_runInBackground", _methods, new ScorpioClass_UnityEngine_Application_set_runInBackground()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("set_runInBackground", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "System.Boolean+") { UnityEngine.Application.runInBackground = (System.Boolean)args[0]; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : set_runInBackground    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_set_targetFrameRate : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_set_targetFrameRate() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("set_targetFrameRate", true, new Type[] {typeof(System.Int32)}, false, null, "System.Int32+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "set_targetFrameRate", _methods, new ScorpioClass_UnityEngine_Application_set_targetFrameRate()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "set_targetFrameRate", _methods, new ScorpioClass_UnityEngine_Application_set_targetFrameRate()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("set_targetFrameRate", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "System.Int32+") { UnityEngine.Application.targetFrameRate = (System.Int32)args[0]; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : set_targetFrameRate    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_SetBuildTags : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_SetBuildTags() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("SetBuildTags", true, new Type[] {typeof(System.String[])}, false, null, "System.String[]+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "SetBuildTags", _methods, new ScorpioClass_UnityEngine_Application_SetBuildTags()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "SetBuildTags", _methods, new ScorpioClass_UnityEngine_Application_SetBuildTags()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("SetBuildTags", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "System.String[]+") { UnityEngine.Application.SetBuildTags((System.String[])args[0]); return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : SetBuildTags    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_SetStackTraceLogType : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_SetStackTraceLogType() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("SetStackTraceLogType", true, new Type[] {typeof(UnityEngine.LogType),typeof(UnityEngine.StackTraceLogType)}, false, null, "UnityEngine.LogType+UnityEngine.StackTraceLogType+"));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "SetStackTraceLogType", _methods, new ScorpioClass_UnityEngine_Application_SetStackTraceLogType()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "SetStackTraceLogType", _methods, new ScorpioClass_UnityEngine_Application_SetStackTraceLogType()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("SetStackTraceLogType", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "UnityEngine.LogType+UnityEngine.StackTraceLogType+") { UnityEngine.Application.SetStackTraceLogType((UnityEngine.LogType)args[0],(UnityEngine.StackTraceLogType)args[1]); return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : SetStackTraceLogType    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_ToString : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_ToString() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("ToString", false, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(false, script, typeof(UnityEngine.Application), "ToString", _methods, new ScorpioClass_UnityEngine_Application_ToString()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(false, script, typeof(UnityEngine.Application), "ToString", _methods, new ScorpioClass_UnityEngine_Application_ToString()); 
            }
            if (obj == null) {
                if (_instance == null) {
                    _instance = new ScorpioTypeMethod(script, "ToString", _method, typeof(UnityEngine.Application));
                }
                return _instance;
            }
            return new ScorpioObjectMethod(obj, "ToString", _method);
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { return ((UnityEngine.Application)obj).ToString(); }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : ToString    type : " + type);
        }
    }
    public class ScorpioClass_UnityEngine_Application_Unload : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static ScorpioClass_UnityEngine_Application_Unload() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
            methods.Add(new ScorpioMethodInfo("Unload", true, new Type[] {}, false, null, ""));
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "Unload", _methods, new ScorpioClass_UnityEngine_Application_Unload()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(true, script, typeof(UnityEngine.Application), "Unload", _methods, new ScorpioClass_UnityEngine_Application_Unload()); 
            }
            if (_instance == null) {
                _instance = new ScorpioStaticMethod("Unload", _method);
            }
            return _instance;
        }
        public object Call(object obj, string type, object[] args) {
            if (type == "") { UnityEngine.Application.Unload(); return null; }
            throw new Exception("UnityEngine.Application 找不到合适的函数 : Unload    type : " + type);
        }
    }
}