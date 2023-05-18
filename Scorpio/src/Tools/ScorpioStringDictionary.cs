namespace Scorpio.Tools {
    public class ScorpioStringDictionary<Value> : ScorpioPollingDictionary<string, Value> {
        public ScorpioStringDictionary() : base(0) { }
        public ScorpioStringDictionary(int capacity) : base(capacity) { }
    }
}
