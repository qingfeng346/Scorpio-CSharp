Object.objFunc = function() {
    print("obj " + this)
}
Bool.testFunc = function(value) {
    print(this)
    return this == value
}
Number.testFunc = function() {
    print(this)
    return this
}
String.testFunc = function() {
    print(this)
    return this
}
"str".testFunc().objFunc()
print(true.testFunc(false))
100.testFunc().objFunc()