using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models
{
    public class TreeCollection
    {
        public bool IsSelected { get; set; }

        public long ID { get; set; }

        public string Name { get; set; }

        public IEnumerable<TreeCollection> _sub { get; set; }
    }

    public static class IEnumerableTreeCollectionExtention
    {
        public static IEnumerable<long> GetValues(this IEnumerable<TreeCollection> collection)
        {
            var results = new List<long>();
            foreach (var node in collection)
            {
                results.AddRange(node._getNodeValues());
            }

            return results;
        }

        private static IEnumerable<long> _getNodeValues(this TreeCollection node)
        {
            var results = new List<long>();
            if (node.IsSelected)
            {
                results.Add(node.ID);
                foreach (var sub in node._sub)
                {
                    results.AddRange(sub._getNodeValues());
                }
            }

            return results;
        }
    }
}
