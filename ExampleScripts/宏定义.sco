#define TEST	//定义一个宏定义
#if TEST
    print("1")
    #if TEST1
        print("6")
    #endif
    print("2")
#elseif TEST2
    print("3")
    #if TEST
        print("4")
        #if TEST
            print("6")
        #endif
    #endif
    print("5")
#endif