using System;
using System.IO;

namespace Rackspace.CloudFiles
{
    public class ProgressFactory
    {
        private static Action _action;

        private static readonly object lockobject = new object();
        public static Action GetAction
        {
            get
            {
                lock (lockobject)
                {
                    return _action;

                }
            }
            set
            {
                lock (lockobject)
                {
                    _action = value;
                }
            }

        }
    }
}