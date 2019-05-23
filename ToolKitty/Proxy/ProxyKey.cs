namespace System.Reflection.Emit
{
    internal struct ProxyKey : IEquatable<ProxyKey>
    {
        public Type ParentType;
        public Type InterfaceType;

        public bool Equals(ProxyKey other)
        {
            return ParentType.Equals(other.ParentType)
                && InterfaceType.Equals(other.InterfaceType);
        }

        public override int GetHashCode()
        {
            unchecked {
                var hashCode = 17;

                hashCode = hashCode * 23 + ParentType.GetHashCode();
                hashCode = hashCode * 23 + InterfaceType.GetHashCode();

                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is ProxyKey other) {
                return Equals(other);
            }

            return base.Equals(obj);
        }
    }
}
