//switch 目前 case 必须加break 否则会解析失败
function sw(a) {
	var c = 200
    switch (a) {
        case 1:
        case 2:
            print("1 or 2")
            break;
        case "a":
        case "b":
            print("a or b")
            break;
		case c:
			print("200")
			return;
			break;
        default:
            print("default")
            break;
    }
	print("finish")
}
sw(1)
sw(2)
sw("a")
sw("b")
sw(200)
sw("scorpio")