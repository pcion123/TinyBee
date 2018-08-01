namespace TinyBee
{
    using System.Collections.Generic;

    public class PriorityQueue
    {
        private List<AStarNode> mNodes = new List<AStarNode>();

        public int Length
        {
            get { return this.mNodes.Count; }
        }

        public bool Contains(object node)
        {
            return this.mNodes.Contains(node as AStarNode);
        }

        public AStarNode First()
        {
            if (mNodes.Count > 0)
            {
                return mNodes[0];
            }
            return null;
        }

        public void Push(AStarNode node)
        {
            mNodes.Add(node);
            mNodes.Sort();
        }

        public void Remove(AStarNode node)
        {
            mNodes.Remove(node);
            mNodes.Sort();
        }
    }
}