namespace QuickScript.Typing
{
    public class ValueType
    {
        public string Val;
        public ValueType()
        {
            Val = "";
        }
        public ValueType(string val)
        {
            Val = val;
        }

        public static implicit operator string(ValueType val) => val.Val;
        public static explicit operator ValueType(string val) => new ValueType(val);

        public string AsString() { return Val; }
        public override int GetHashCode()
        {
            return Val.GetHashCode();
        }
        public override bool Equals(object o)
        {
            if (!(o is ValueType))
                return false;
            return Val == ((ValueType)o).Val;
        }
        public bool IsEmpty()
        {
            return Val == "";
        }
    }
}
