using UnityEngine;
using System;
using System.Collections.Generic;

public class RequirementSystem {
    private NodeR start;

    public RequirementSystem()
    {
        NodeR depth0 = new NodeR(typeof(HallCenter))
        {
            owned = true
        };
        NodeR depth1 = new NodeR(typeof(Windmill));
        AddUnlocksAndLocks(depth0, depth1);

        //Barracks
        depth1 = new NodeR(typeof(Barracks));
        AddUnlocksAndLocks(depth0, depth1);

        NodeR depth2 = new NodeR(typeof(Warrior));
        AddUnlocksAndLocks(depth1, depth2);
        depth2 = new NodeR(typeof(Wizard));
        AddUnlocksAndLocks(depth1, depth2);
        depth2 = new NodeR(typeof(Brute));
        AddUnlocksAndLocks(depth1, depth2);

        //Magic Center
        depth1 = new NodeR(typeof(MagicCenter));
        AddUnlocksAndLocks(depth0, depth1);

        depth2 = new NodeR(typeof(Arcaneblast));
        AddUnlocksAndLocks(depth1, depth2);
        depth2 = new NodeR(typeof(ArcaneIntellect));
        AddUnlocksAndLocks(depth1, depth2);
        depth2 = new NodeR(typeof(Shrine));
        AddUnlocksAndLocks(depth1, depth2);

        //Shrine
        NodeR depth3 = new NodeR(typeof(Fireblast));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(FireHammer));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(FlammingSwords));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(Frostblast));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(Frostlance));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(Naturewrath));
        AddUnlocksAndLocks(depth2, depth3);

        start = depth0;
    }

    public void SetUnlocked(Type type)
    {
        NodeR n = start.Search(type);
        if(n == null)
        {
            throw new NotImplementedException("Not implemented in the requirement system: " + type.ToString());
        }
        n.owned = true;
    }
    public void SetTier2(Type type)
    {
        NodeR n = start.Search(type);
        if (n == null)
        {
            throw new NotImplementedException("Not implemented in the requirement system: " + type.ToString());
        }
        n.t2 = true;
    }
    public bool IsAlreadyUnlocked(Type type)
    {
        NodeR n = start.Search(type);
        if (n == null)
        {
            throw new NotImplementedException("Not implemented in the requirement system: " + type.ToString());
        }
        return n.owned;
    }
    public bool IsAlreadyT2(Type type)
    {
        NodeR n = start.Search(type);
        if (n == null)
        {
            throw new NotImplementedException("Not implemented in the requirement system: " + type.ToString());
        }
        return n.t2;
    }

    public void debugString()
    {
        start.debugString(0);
    }

    public bool CheckIfRequirementAreSatisfied(Type type, bool ist2)
    {
        NodeR n = start.Search(type);
        if (n == null)
        {
            throw new NotImplementedException("Not implemented in the requirement system: " + type.ToString());
        }
        bool allLocksOwned = true;
        bool allT2 = true;
        foreach(NodeR parent in n.locks)
        {
            if (!parent.owned)
            {
                allLocksOwned = false;
            }
            if (!parent.t2)
            {
                allT2 = false;
            }
        }
        return allLocksOwned && (ist2 && allT2 || !ist2);
    }

    private void AddUnlocksAndLocks(NodeR parent, NodeR child)
    {
        parent.unlocks.Add(child);
        child.locks.Add(parent);
    }

    private class NodeR
    {
        public Type element;
        public List<NodeR> unlocks;
        public List<NodeR> locks;
        public bool owned;
        public bool t2;
        public NodeR(Type element_)
        {
            element = element_;
            owned = false;
            t2 = false;
            unlocks = new List<NodeR>();
            locks = new List<NodeR>();
        }
        public void debugString(int depth)
        {
            string prefix = "";
            for (int i = 0; i < depth; i++)
            {
                prefix += "|";
            }
            Debug.Log(prefix + " " + element.ToString() + ":"+owned.ToString());
            foreach(NodeR n in unlocks)
            {
                n.debugString(depth + 1);
            }
    }

        public NodeR Search(Type type)
        {
            if (type == element)
            {
                return this;
            }
            else
            {
                if(unlocks.Count == 0)
                {
                    return null;
                }
                foreach (NodeR child in unlocks) {
                    NodeR result = child.Search(type);
                    if(result != null)
                    {
                        return result;
                    }
                }
                return null;
            }
        }
    }
} 