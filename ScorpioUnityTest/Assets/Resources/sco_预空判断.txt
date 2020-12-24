var a = null
print(a?.test)      //?.会提前判断父级是否为null,是null直接返回null
print(a?.test?.())  //函数也可以预空判断需要使用 ?.()
print(a?.["bbb"])   //?.[]