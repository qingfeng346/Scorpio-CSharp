var TestClass = import_type("TestClass")
var a = new TestClass()
var b = a.TestFunc1
var c = userdata.bind(b, a)
print(c())