namespace Scorpio.LibraryV1 {
    public partial class LibraryBasis {
        public static void Load(Script script) {
            script.NewNameGlobal("isBoolean", "is_bool");
            script.NewNameGlobal("isNumber", "is_number");
            script.NewNameGlobal("isDouble", "is_double");
            script.NewNameGlobal("isLong", "is_long");
            script.NewNameGlobal("isString", "is_string");
            script.NewNameGlobal("isFunction", "is_function");
            script.NewNameGlobal("isArray", "is_array");
            script.NewNameGlobal("isMap", "is_table");
            script.NewNameGlobal("isUserdata", "is_userdata");

            script.NewNameGlobal("toString", "tostring");
            
            script.NewNameGlobal("toInt8", "tosbyte");
            script.NewNameGlobal("toUint8", "tobyte");
            script.NewNameGlobal("toInt16", "toshort");
            script.NewNameGlobal("toUint16", "toushort");
            script.NewNameGlobal("toInt32", "toint");
            script.NewNameGlobal("toUint32", "touint");
            script.NewNameGlobal("toInt64", "tolong");
            script.NewNameGlobal("toUint64", "toulong");

            
            script.NewNameGlobal("toFloat", "tofloat");
            script.NewNameGlobal("toNumber", "todouble");
            script.NewNameGlobal("toNumber", "tonumber");
            script.NewNameGlobal("toEnum", "toenum");

            script.NewNameGlobal("typeOf", "typeof");

            script.NewNameGlobal("pushSearch", "push_search");
            script.NewNameGlobal("pushAssembly", "push_assembly");
            script.NewNameGlobal("importType", "import_type");
            script.NewNameGlobal("importNamespace", "import_namespace");
            script.NewNameGlobal("importExtension", "import_extension");
            script.NewNameGlobal("genericType", "generic_type");
            script.NewNameGlobal("genericMethod", "generic_method");
        }
    }
}
