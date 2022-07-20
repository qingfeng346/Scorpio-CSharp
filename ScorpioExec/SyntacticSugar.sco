var files = io.getFiles("../ExampleScripts", "*.sco")
foreach (var pair in pairs(files)) {
    require(pair.value)
    print("${pair.value} 测试通过")
}