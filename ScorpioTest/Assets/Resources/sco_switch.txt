//switch使用
function sw(a) {
	var value = 200
    switch (a) {
        case 1: 
            print("1")
            break;
        case 2:
        case 3:
            print("2 or 3")
            break;
        case "a":
        case "b":
            print("a or b")
        //上一个 case 不加break 可以穿透 case
        case "c":
            print("c")
            break
        //case 也可以使用变凉 直接加 reutrn 可以跳出函数
		case value:
			print(value)
			return;
        //默认 case
        default:
            print("default : " + a)
            break;
    }
}
sw(1)
sw(2)
sw(3)
sw("a")
sw("b")
sw("c")
sw(200)
sw("scorpio")