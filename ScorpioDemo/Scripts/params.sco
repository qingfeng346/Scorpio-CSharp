function test(a,...b)
{
    print(a)
    print(array.count(b))
    foreach (pair in pairs(b))
    {
        print(pair.value)
    }
}
test(100,200,300,400)