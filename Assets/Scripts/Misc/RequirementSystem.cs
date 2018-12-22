using UnityEngine;
using System;
using System.Collections.Generic;

public class RequirementSystem {
    private NodeR start;

    public RequirementSystem()
    {
        NodeR depth0;
        NodeR depth1;
        NodeR depth2;
        
        depth0 = new NodeR(typeof(HallCenter)){ owned = true};
        depth1 = new NodeR(typeof(Scout));
        AddUnlocksAndLocks(depth0, depth1);

        //shrine
        NodeR shrine = new NodeR(typeof(Shrine)) { maxAllowed = 12,
                                                    needT2 = false,
                                                    oneUnlockInEnough=true};

        //Market
        depth1 = new NodeR(typeof(Market));
        AddUnlocksAndLocks(depth0, depth1);

        depth2 = new NodeR(typeof(Windmill)) { maxAllowed = 2};
        AddUnlocksAndLocks(depth1, depth2);
        AddUnlocksAndLocks(depth1, shrine);
        //Stables
        depth1 = new NodeR(typeof(Stables));
        AddUnlocksAndLocks(depth0, depth1);
        depth2 = new NodeR(typeof(Horseman));
        AddUnlocksAndLocks(depth1, depth2);
        depth2 = new NodeR(typeof(Dragon));
        AddUnlocksAndLocks(depth1, depth2);


        //Barracks
        depth1 = new NodeR(typeof(Barracks));
        AddUnlocksAndLocks(depth0, depth1);

        depth2 = new NodeR(typeof(Warrior));
        AddUnlocksAndLocks(depth1, depth2);
        depth2 = new NodeR(typeof(Wizard));
        AddUnlocksAndLocks(depth1, depth2);
        depth2 = new NodeR(typeof(Archer));
        AddUnlocksAndLocks(depth1, depth2);
        depth2 = new NodeR(typeof(Brute));
        AddUnlocksAndLocks(depth1, depth2);
        depth2 = new NodeR(typeof(Bandit));
        AddUnlocksAndLocks(depth1, depth2);
        AddUnlocksAndLocks(depth1, shrine);


        //Magic Center
        depth1 = new NodeR(typeof(MagicCenter));
        AddUnlocksAndLocks(depth0, depth1);

        depth2 = new NodeR(typeof(Arcaneblast));
        AddUnlocksAndLocks(depth1, depth2);
        depth2 = new NodeR(typeof(ArcaneIntellect));
        AddUnlocksAndLocks(depth1, depth2);
        depth2 = new NodeR(typeof(ArcaneMissile));
        AddUnlocksAndLocks(depth1, depth2);
        depth2 = new NodeR(typeof(ArcaneMirage));
        AddUnlocksAndLocks(depth1, depth2);
        AddUnlocksAndLocks(depth1, shrine);

        //Shrine
        depth2 = shrine;
        NodeR depth3 = new NodeR(typeof(Fireblast));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(BerserkerSpirit));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(FlammingSwords));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(Frostblast));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(Frostlance));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(CropFreeze));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(ManaFreeze));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(Naturewrath));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(EarthLink));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(WaveOfVigor));
        AddUnlocksAndLocks(depth2, depth3);
        depth3 = new NodeR(typeof(NaturesBlessing));
        AddUnlocksAndLocks(depth2, depth3);

        start = depth0;
    }

    public void AddCopy(Type type)
    {
        NodeR n = start.Search(type);
        if(n == null)
        {
            throw new NotImplementedException("Not implemented in the requirement system: " + type.ToString());
        }
        n.owned = true;
        n.currentOwned += 1;
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

    public bool MaxCopyOwned(Type type)
    {
        NodeR n = start.Search(type);
        if (n == null)
        {
            throw new NotImplementedException("Not implemented in the requirement system: " + type.ToString());
        }
        return n.currentOwned >= n.maxAllowed;
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
        if (n.oneUnlockInEnough)
        {
            bool aLockIsOwned = false;
            bool theLockIsT2 = false;
            foreach(NodeR parent in n.locks)
            {
                if (parent.owned)
                {
                    aLockIsOwned =true;
                    if(parent.t2 )
                    {
                        theLockIsT2 = true;
                    }
                }
            }
            return aLockIsOwned && (ist2 && theLockIsT2 || !ist2 || !n.needT2);
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
        return allLocksOwned && (ist2 && allT2 || !ist2 || !n.needT2);
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
        public bool oneUnlockInEnough;
        public bool owned;
        public bool t2;
        public bool needT2;
        public int maxAllowed;
        public int currentOwned;
        public NodeR(Type element_)
        {
            element = element_;
            owned = false;
            t2 = false;
            needT2 = true;
            maxAllowed = 1;
            currentOwned = 0;
            unlocks = new List<NodeR>();
            locks = new List<NodeR>();
            oneUnlockInEnough = false;
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