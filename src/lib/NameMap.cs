using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Ockham.Data
{
    public class NameMap
    {
        public string SourceName { get; }
        public string TargetName { get; }

        public NameMap(string source, string target)
        {
            this.SourceName = source;
            this.TargetName = target;
        }

        public override string ToString()
        {
            return SourceName + " => " + TargetName;
        }
    }

    public class NameMapCollection : IEnumerable<NameMap>
    {
        public StringComparer Comparer { get; }

        private readonly Dictionary<string, NameMap> _bySource;
        private readonly Dictionary<string, NameMap> _byTarget;

        public string GetTarget(string source) => _bySource[source].TargetName;

        public string GetSource(string target) => _byTarget[target].SourceName;

        public NameMapCollection(StringComparer comparer)
        {
            this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _bySource = new Dictionary<string, NameMap>(comparer);
            _byTarget = new Dictionary<string, NameMap>(comparer);
        }

        public NameMap Add(string source, string target)
        {
            if (string.IsNullOrWhiteSpace(source)) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(target)) throw new ArgumentNullException(nameof(source));

            if (_bySource.TryGetValue(source, out NameMap existing)) throw new ArgumentException($"Source name {source} is already mapped to {existing.TargetName}");
            if (_byTarget.TryGetValue(source, out existing)) throw new ArgumentException($"Target name {target} is already mapped to {existing.SourceName}");

            var item = new NameMap(source, target);
            _bySource.Add(source, item);
            _byTarget.Add(target, item);
            return item;
        }

        public IEnumerator<NameMap> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
