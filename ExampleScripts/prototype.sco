//原生对象支持的 prototype 类请查看 Proto 文件夹下所有类
//可以修改原生对象的 prototype 例如 
Number.testFunc = () => {
    print(this)
}
100.testFunc()

String.end = (str) => {
    return this.endsWith(str)
}
print("str".end("r"))