//json库 json.encode  json.decode 两个函数
var b = 
{
    b1 = 1,
    b2 = 2,
    b3 = 3,
}
var c = [100,200,300,400,500]
var d = ["a","b","c","d"]
var test =
{
    a = "100",
    b = b
    c = c
    d = d,
    e = 100,
    f = false,
}
var str = json.encode(test)
print(str)
var dec = json.decode(str)
foreach (pair in pairs(dec))
{
    print("${pair.key} = ${pair.value}")
}