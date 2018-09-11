using System.Collections.Generic;
using UnityEngine;

namespace Zob.Internal
{
    public class LogFilterHandler
    {
        private HashSet<ILogFilter> _filters = new HashSet<ILogFilter>();

        public void AddFilter(ILogFilter filter)
        {
            if (!_filters.Contains(filter))
            {
                _filters.Add(filter);
            }
        }

        public void RemoveFilter(ILogFilter filter)
        {
            _filters.Remove(filter);
        }
    }
}