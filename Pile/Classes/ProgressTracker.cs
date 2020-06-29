using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Pile
{
    public class ProgressTracker
    {
        private static List<ProgressItem> _items = new List<ProgressItem>();

        public static ProgressItem Create(string name, int startVal, int targetVal)
        {
            //Let's clean up
            //var deletes = Trackers.Where(x => x.Current == x.Target && (x.CreatedAt.AddHours(6) > DateTime.Now || (x.CompletedTime.HasValue && x.CompletedTime.Value.AddHours(1) > DateTime.Now)));
            //foreach (var del in deletes)
            //    Trackers.Remove(del);

            if (targetVal < startVal)
                throw new Exception("Cannot create progress tracker with startVal greater than targetVal");

            _items.RemoveAll(x => x.Completed && x.Name == name);

            if (_items.Any(x => x.Name == name && x.Completed == false))
                return null;

            var tracker = new ProgressItem(name, startVal, targetVal);

            _items.Add(tracker);
            return tracker;
        }

        public static List<ProgressItem> Items { get { return _items; } }

        public static ProgressItem Get(string name)
        {
            return _items.FirstOrDefault(x => x.Name == name);
        }

    }

    public class ProgressItem
    {
        private int _current;

        public ProgressItem(string name, int start, int target)
        {
            Name = name;
            CreatedAt = DateTime.Now;
            _current = start;
            Target = target;
            Completed = Current >= Target;
            if (Completed)
                CompletedTime = DateTime.Now;
        }

        public void Increment()
        {
            Interlocked.Increment(ref _current);
            Completed = Current >= Target;
            if (Completed)
                CompletedTime = DateTime.Now;
        }

        public void StopWithError(string error)
        {
            Completed = true;
            CompletedTime = DateTime.Now;
            Error = error;
        }

        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Current { get { return _current; } }
        public int Target { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletedTime { get; set; }
        public string Error { get; set; }
    }
}