using System;
using System.Collections.Generic;
using System.Linq;

namespace NLog.Loki.Model
{
    public class LokiLabels : IEquatable<LokiLabels>
    {
        private readonly int hashCode;

        public ISet<LokiLabel> Labels { get; }

        public LokiLabels(params LokiLabel[] labels) :
            this((IEnumerable<LokiLabel>)labels)
        {
        }

        public LokiLabels(IEnumerable<LokiLabel> labels)
        {
            Labels = new HashSet<LokiLabel>(labels ?? Enumerable.Empty<LokiLabel>());

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
            return Labels.SetEquals(other.Labels);
        }

        public override bool Equals(object other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            if(other.GetType() != this.GetType()) return false;
            return Equals((LokiLabels)other);
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
