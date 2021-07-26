function main1() {
    return  { a : 100, b: 200}
}
function main() {
    var { a, b } = @{ a : 100, b: 200}
    print(a, b)
    // var b = function() {
    //     a = 200
    // }
}
main()