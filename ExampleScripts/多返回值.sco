function func() {
    return { a : 100, b : 200 }
}
function main() {
    var { a , b } = func()
    print (a, b)
}
main()