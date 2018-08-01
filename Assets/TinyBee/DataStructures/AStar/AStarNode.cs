namespace TinyBee
{
	using System;
	using UnityEngine;

	public class AStarNode : IComparable
	{
		public float NodeTotalCost; // G 它是从开始节点到当前节点的代价值
		public float EstimatedCost; // H 它是从当前节点到目标节点的估计值
		public bool IsObstacle;
		public AStarNode Parent;
		public Vector3 Position;

		public AStarNode()
		{
			EstimatedCost = 0.0f;
			NodeTotalCost = 1.0f;
			IsObstacle = false;
			Parent = null;
		}

		public AStarNode(Vector3 pos)
		{
			EstimatedCost = 0.0f;
			NodeTotalCost = 1.0f;
			IsObstacle = false;
			Parent = null;
			Position = pos;
		}

		public void MarkAsObstacle()
		{
			IsObstacle = true;
		}

		public int CompareTo(object obj)
		{
			AStarNode node = obj as AStarNode;
			return EstimatedCost.CompareTo(node.EstimatedCost);
		}
	}
}