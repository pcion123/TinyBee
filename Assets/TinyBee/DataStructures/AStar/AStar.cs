namespace TinyBee
{
    using System.Collections.Generic;
    using UnityEngine;

    public class AStar
    {
        public static PriorityQueue ClosedList, OpenList;

        private static float HeuristicEstimateCost(AStarNode curNode, AStarNode goalNode)
        {
            Vector3 vecCost = curNode.Position - goalNode.Position;
            return vecCost.magnitude;
        }

        public static List<AStarNode> FindPath(AStarNode start, AStarNode goal)
        {
            OpenList = new PriorityQueue();
            OpenList.Push(start);
            start.NodeTotalCost = 0.0f;
            start.EstimatedCost = HeuristicEstimateCost(start, goal);
            ClosedList = new PriorityQueue();
            AStarNode node = null;
            while (OpenList.Length != 0)
            {
                node = OpenList.First();
                //Check if the current node is the goal node  
                if (node.Position == goal.Position)
                {
                    return CalculatePath(node);
                }
                //Create an ArrayList to store the neighboring nodes  
                List<AStarNode> neighbours = new List<AStarNode>();
                GridManager.Instance.GetNeighbours(node, neighbours);
                for (int i = 0; i < neighbours.Count; i++)
                {
                    AStarNode neighbourNode = neighbours[i] as AStarNode;
                    if (!ClosedList.Contains(neighbourNode))
                    {
                        float cost = HeuristicEstimateCost(node,
                            neighbourNode);
                        float totalCost = node.NodeTotalCost + cost;
                        float neighbourNodeEstCost = HeuristicEstimateCost(
                            neighbourNode, goal);
                        neighbourNode.NodeTotalCost = totalCost;
                        neighbourNode.Parent = node;
                        neighbourNode.EstimatedCost = totalCost +
                                                      neighbourNodeEstCost;
                        if (!OpenList.Contains(neighbourNode))
                        {
                            OpenList.Push(neighbourNode);
                        }
                    }
                }
                //Push the current node to the closed list  
                ClosedList.Push(node);
                //and remove it from openList  
                OpenList.Remove(node);
            }
            if (node.Position != goal.Position)
            {
                UnityEngine.Debug.LogError("Goal Not Found");
                return null;
            }
            return CalculatePath(node);
        }

        private static List<AStarNode> CalculatePath(AStarNode node)
        {
            List<AStarNode> list = new List<AStarNode>();
            while (node != null)
            {
                list.Add(node);
                node = node.Parent;
            }
            list.Reverse();
            return list;
        }
    }
}