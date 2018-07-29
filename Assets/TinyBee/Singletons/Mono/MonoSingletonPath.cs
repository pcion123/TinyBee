namespace TinyBee
{
    using System;

    public class TMonoSingletonPath : MonoSingletonPath
    {
		public TMonoSingletonPath(string pathInHierarchy) : base(pathInHierarchy)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class MonoSingletonPath : Attribute
    {
		private string mPathInHierarchy;

        public MonoSingletonPath(string pathInHierarchy)
        {
            mPathInHierarchy = pathInHierarchy;
        }

        public string PathInHierarchy
        {
            get { return mPathInHierarchy; }
        }
    }
}
