//switch使用
var value = 100
var c = 400
switch (value) {
    case 100:
        print("100");
        //如果case 不加 break 则会向下穿透
    case 200:
        print("200");
        break;
    case 300:
        print("300");
        break
    case c:     //case 支持变量和表达式
        print("400");
        break
    default:
        print("default")
        break
}