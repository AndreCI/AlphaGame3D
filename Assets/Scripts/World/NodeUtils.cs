using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class NodeUtils
{
    public class NodeWrapper
    {
        public enum STATE
        {
            EMPTY,
            NONEMPTY
        }
        public Node root;
        public List<NodeWrapper> children;
        public NodeWrapper parent;
        public STATE state;
        public int distance;
        public NodeWrapper(Node root_)
        {
            root = root_;
            children = new List<NodeWrapper>();
            distance = 9001;
        }

        public List<NodeWrapper> GetNodeChildren()
        {
            List<NodeWrapper> possibleMoves = new List<NodeWrapper>
            {
                this
            };
            if (children.Count == 0)
            {
                return possibleMoves;
            }
            foreach (NodeWrapper n in children)
            {
                possibleMoves.AddRange(n.GetNodeChildren());
            }
            return possibleMoves;
        }

        public List<NodeWrapper> Search(Node target)
        {
            List<NodeWrapper> path = new List<NodeWrapper>();
            if (root == target)
            {
                path.Add(this);
            }
            else
            {
                foreach(NodeWrapper child in children)
                {
                    path.AddRange(child.Search(target));
                }
                if (path.Count != 0)
                {
                    path.Add(this);
                }
            }
            return path;
        }
    }

    public static NodeWrapper GetNeighborsNode(Node sourceNode, int depth)
    {
        List<Node> availableNodes = new List<Node>();
        List<NodeWrapper> unvisitedNeighborsNodes = new List<NodeWrapper>();
        List<NodeWrapper> nextUnvisitedNeighborsNodes = new List<NodeWrapper>();

        NodeWrapper currentNodeWrapped = new NodeWrapper(sourceNode)
        {
            parent = null,
            distance = 0,
            state = NodeWrapper.STATE.NONEMPTY
        };
        if (depth == 0)
        {
            return currentNodeWrapped;
        }
        availableNodes.Add(sourceNode);
        AddAdjacentNodes(currentNodeWrapped, availableNodes, unvisitedNeighborsNodes, sourceNode, true);
        for (int i = 1; i < depth; i++)
        {
            nextUnvisitedNeighborsNodes = new List<NodeWrapper>();
            foreach (NodeWrapper unvisited in unvisitedNeighborsNodes)
            {
                AddAdjacentNodes(unvisited, availableNodes, nextUnvisitedNeighborsNodes, sourceNode, true);
            }
            unvisitedNeighborsNodes = nextUnvisitedNeighborsNodes;
        }
        return currentNodeWrapped;
    }

    public static NodeWrapper GetPossibleNodes(Node sourceNode, int depth)
    {
        List<Node> availableNodes = new List<Node>();
        List<NodeWrapper> unvisitedNeighborsNodes = new List<NodeWrapper>();
        List<NodeWrapper> nextUnvisitedNeighborsNodes = new List<NodeWrapper>();

        NodeWrapper currentNodeWrapped = new NodeWrapper(sourceNode);
        currentNodeWrapped.parent = null;
        currentNodeWrapped.distance = 0;
        currentNodeWrapped.state = NodeWrapper.STATE.NONEMPTY;
        if (depth == 0)
        {
            return currentNodeWrapped;
        }
        availableNodes.Add(sourceNode);
        AddAdjacentNodes(currentNodeWrapped, availableNodes, unvisitedNeighborsNodes, sourceNode, false);
        for (int i=1; i<depth; i++)
        {
            nextUnvisitedNeighborsNodes = new List<NodeWrapper>();
            foreach (NodeWrapper unvisited in unvisitedNeighborsNodes)
            {
                AddAdjacentNodes(unvisited, availableNodes, nextUnvisitedNeighborsNodes, sourceNode, false);
            }
            unvisitedNeighborsNodes = nextUnvisitedNeighborsNodes;
        }
        return currentNodeWrapped;
    }
    private static void AddAdjacentNodes(NodeWrapper currentNode, List<Node> knownNeighbors, List<NodeWrapper> unvisitedNeighborsNodes, Node sourceNode, bool explorationOnly)
    {

        AddNodeToListsFromDirection(Vector3.forward, currentNode, unvisitedNeighborsNodes, knownNeighbors, sourceNode, explorationOnly);
        AddNodeToListsFromDirection(-Vector3.forward, currentNode, unvisitedNeighborsNodes, knownNeighbors, sourceNode, explorationOnly);
        AddNodeToListsFromDirection(Vector3.right, currentNode, unvisitedNeighborsNodes, knownNeighbors, sourceNode, explorationOnly);
        AddNodeToListsFromDirection(-Vector3.right, currentNode, unvisitedNeighborsNodes, knownNeighbors, sourceNode, explorationOnly);

    }
    private static void AddNodeToListsFromDirection(Vector3 dir, NodeWrapper currentNode, List<NodeWrapper> unvisitedNeighborsNode, List<Node> knownNeighbors, Node sourceNode, bool explorationOnly)
    {
        Vector3 halfExtend = new Vector3(1f, 1f, 1f);
        Collider[] colliders = Physics.OverlapBox(currentNode.root.transform.position + dir, halfExtend);
        foreach (Collider collider in colliders)
        {
            Node node = collider.GetComponent<Node>();
            if (node != null && !knownNeighbors.Contains(node))
            {
                bool found = false;
                foreach (NodeWrapper n in unvisitedNeighborsNode)
                {
                    if (n.root == (node))
                    {
                        found = true;
                        if(currentNode.distance + 1 < n.distance)
                        {
                            n.distance = currentNode.distance + 1;
                            currentNode.children.Add(n);
                            n.parent.children.Remove(n);
                            n.parent = currentNode;
                        }
                    }
                }
                if (!found)
                {
                    if (explorationOnly || TurnManager.Instance.currentPlayer.visibleNodes.Contains(node))
                    {
                        NodeWrapper nodeWrap = new NodeWrapper(node);
                        if (node.walkable || explorationOnly)
                        {
                            nodeWrap.state = NodeWrapper.STATE.EMPTY;
                            unvisitedNeighborsNode.Add(nodeWrap);
                            currentNode.children.Add(nodeWrap);
                            knownNeighbors.Add(node);
                            nodeWrap.distance = currentNode.distance + 1;
                            nodeWrap.parent = currentNode;
                        }
                        else if (node.Attackable(sourceNode))
                        {
                            nodeWrap.state = NodeWrapper.STATE.NONEMPTY;
                            currentNode.children.Add(nodeWrap);
                            knownNeighbors.Add(node);
                            nodeWrap.distance = currentNode.distance + 1;
                            nodeWrap.parent = currentNode;
                        }
                        else if (node.SpellInteractable(sourceNode))
                        {
                            nodeWrap.state = NodeWrapper.STATE.NONEMPTY;
                            unvisitedNeighborsNode.Add(nodeWrap);
                            currentNode.children.Add(nodeWrap);
                            knownNeighbors.Add(node);
                            nodeWrap.distance = currentNode.distance + 1;
                            nodeWrap.parent = currentNode;
                        }
                    }
                }
            }
        }
    }
}