using System;
using System.Collections.Generic;
using System.Linq;

namespace NLog.Loki.Model
{
    public class LokiLabels : IEquatable<LokiLabels>
    {
        private readonly int hashCode;

        public IReadOnlyCollection<LokiLabel> Labels { get; }

        public LokiLabels(IEnumerable<LokiLabel> labels)
        {
            Labels = new List<LokiLabel>(labels ?? Enumerable.Empty<LokiLabel>());

            unchecked
            {
                hashCode =
                    Labels.Aggregate(0,
                        (current, label) => (current * 397) ^ label.GetHashCode());
            }
        }

        public bool Equals(LokiLabels other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return Labels.SequenceEqual(other.Labels);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((LokiLabels)obj);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public static bool operator ==(LokiLabels left, LokiLabels right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LokiLabels left, LokiLabels right)
        {
            return !Equals(left, right);
        }
    }
}
