using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Transform;
using Query;
using Rhino.Commons;

namespace StoneHaven.Models.Filters
{
    public abstract class AbstractSearchFilter
    {
        protected IList<Action<DetachedCriteria>> actions = new List<Action<DetachedCriteria>>();

        public void Apply(DetachedCriteria dc)
        {
            foreach (var action in actions)
            {
                action(dc);
            }
        }

        public string OrderBy { get; set; }

        public bool Asc { get; set; }
    }
}