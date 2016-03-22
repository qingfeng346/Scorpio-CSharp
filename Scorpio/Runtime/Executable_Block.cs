namespace Scorpio.Runtime {
    public enum Executable_Block {
        None,
        //上下文
        Context,
        //普通的分块
        Block,
        //函数
        Function,
        //判断语句
        If,
        //for循环开始
        ForBegin,
        //for循环执行
        ForLoop,
        //for语句内容
        For,
        //foreach语句
        Foreach,
        //while语句
        While,
        //swtich语句
        Switch,
    }
}
