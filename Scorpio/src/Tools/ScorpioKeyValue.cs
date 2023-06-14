namespace Scorpio.Tools {
    public struct ScorpioKeyValue<TKey, TValue> {
        public TKey Key;
        public TValue Value;
        public ScorpioKeyValue(TKey key, TValue value) {
            Key = key;
            Value = value;
        }
    }
}
