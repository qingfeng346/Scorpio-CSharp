var eee = 0
for (var i = 0, 1000000) {
    var a = i + 1
    var b = 2.3
    if(a < b) {
        a = a + 1
    } else {
        b = b + 1
    }
    if(a == b){
        b = b + 1
    }
    eee = eee + a * b + a / b
}
print(eee)