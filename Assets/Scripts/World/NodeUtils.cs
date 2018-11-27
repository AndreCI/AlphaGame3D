using System.Collections.Generic;
using System.Linq;
using UnityEngine;



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
        public override string ToString()
        {
            return root.ToString() + " d=" + distance.ToString() + " p=" + (parent==null? "null" :parent.ToString());
        }
        public List<Node> GetChildrens()
        {
            List<Node> possibleMoves = new List<Node>
            {
                root
            };
            if (children.Count == 0)
            {
                return possibleMoves;
            }
            foreach (NodeWrapper n in children)
            {
                possibleMoves.AddRange(n.GetChildrens());
            }
            return possibleMoves;
        }

        public List<Node> GetPath(Node target)
        {
            List<Node> path = new List<Node>();
            if (root == target)
            {
                path.Add(root);
            }
            else
            {
                foreach (NodeWrapper child in children)
                {
                    path.AddRange(child.GetPath(target));
                }
                if (path.Count != 0)
                {
                    path.Add(root);
                }
            }
            return path;
        }


        
    }

    public static NodeWrapper BFSNodesAdj(Node sourceNode, int depth, bool selectOnlyWalkable=false)
    {
        NodeWrapper source = new NodeWrapper(sourceNode)
        {
            distance = 0
        };
        Queue<NodeWrapper> queue = new Queue<NodeWrapper>();
        queue.Enqueue(source);
        List<NodeWrapper> knownNodes = new List<NodeWrapper> { { source} };
        while (queue.Count > 0)
        {
            NodeWrapper currentNode = queue.Dequeue();
            if(currentNode.distance < depth)
            {
                foreach (Node n in currentNode.root.adjacentNodes)
                {
                    if(knownNodes.Find(n_ => n_.root.Equals(n)) == null)
                    {
                        if ((!selectOnlyWalkable || n.walkable))
                        {
                            NodeWrapper adjWrapped = new NodeWrapper(n)
                            {
                                parent = currentNode,
                                distance = currentNode.distance + 1
                            };
                            currentNode.children.Add(adjWrapped);
                            knownNodes.Add(adjWrapped);
                            queue.Enqueue(adjWrapped);
                        }else if (n.Attackable(sourceNode)) {
                            NodeWrapper adjWrapped = new NodeWrapper(n)
                            {
                                parent = currentNode,
                                distance = currentNode.distance + 1
                            };
                            currentNode.children.Add(adjWrapped);
                            knownNodes.Add(adjWrapped);
                        }
                    }
                }
            }
        }
        return source;
    }
    public static List<Node> GetAdjNodes(NodeWrapper currentNode)
    {
        List<Vector3> dirs = new List<Vector3> {
        { Vector3.forward },
        {Vector3.right },
        { -Vector3.forward },
        {-Vector3.right }
        };
        List<Node> adjNodes = new List<Node>();
        Vector3 halfExtend = new Vector3(1f, 1f, 1f);
        foreach (Vector3 dir in dirs)
        {
            Collider[] colliders = Physics.OverlapBox(currentNode.root.transform.position + dir, halfExtend);
            foreach (Collider collider in colliders)
            {
                Node node = collider.GetComponent<Node>();
                if (node != null && !node.Equals(currentNode.root) && !node.Equals(currentNode.parent))
                {
                    adjNodes.Add(node);
                }
            }
        }
        return adjNodes;
    }

   }