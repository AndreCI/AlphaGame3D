using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class Unit : Selectable
{
    [Header("Prefab, animations utils, and UI")]
    public GameObject prefab;
    public Transform movementSphere;
    public Canvas healthCanvas;
    public Image healthDisplay;
    public Sprite sprite;
    public UnitEffectAnimations effectAnimations;
    public float speed;
    public Transform animTransform;
    public GameObject meleeAttackAnimation;
    public RangedAttackAnimation rangedAttackAnimation;
    public Vector3 attackOffset;

    [Header("Unit Information")]
    public int maxHealth;
    public int attack;
    public int maxMovementPoints;
    public int attackRange;
    public int visionRange;
    public string effectDescription;

    [Header("Internal variables (no preset is needed)")]
    public int currentHealth;
    public int currentMovementPoints;
    public int currentAttackModifier;
    public List<UnitEffect> currentEffect;
    public List<UnitAbility> abilities;
    public int armor;
    public int foodConso;
    public Vector3 direction;

    protected Animator anim;
    protected List<Node> potentialPath; //Current path for the target node
    protected List<Node> path; //selected path to node
    protected bool moving;
    protected NodeUtils.NodeWrapper currentPositionWrapped; //NodeWrapper containing the root of the movement tree
    protected List<Node> possibleMoves; //List of all the nodes where the unit can go
    protected List<Node> rangedAttackableMoves; //List of all the nodes where the unit can go


    public virtual void Setup()
    {
        anim = GetComponentInChildren<Animator>();
        anim.logWarnings = false;
        currentAttackModifier = 0;
        currentHealth = maxHealth;
        currentMovementPoints = maxMovementPoints;
        healthDisplay.fillAmount = (float)currentHealth / (float)(maxHealth);
        TurnManager.Instance.StartTurnSubject.AddObserver(this);
        moving = false;
        visible = true;
        currentEffect = new List<UnitEffect>();
        abilities = new List<UnitAbility>();
        armor = 0;
        movementSphere.localPosition = new Vector3(0, movementSphere.localPosition.y, 0);
    }

    void Update()
    {
        if (moving)
        {
            MoveStep();
        }
    }

    public override void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
        if (player.Equals(owner))
        {
            if (type == TurnSubject.NOTIFICATION_TYPE.START_OF_TURN)
            {
                currentMovementPoints = maxMovementPoints;
                currentAttackModifier = 0;
                armor = 0;
                StartCoroutine(DisplayAndApplyCurrentEffects(owner, currentEffect));
            }
        }
        

    }

    public IEnumerator DisplayAndApplyCurrentEffects(Player currentPlayer, List<UnitEffect> currentEffects)
    {

        currentEffects.RemoveAll(ue => ue.duration<=0); //safe removing of elements (direct apply buffs, which sticks even to 0 duration for a turn)
        notificationPanel.SetActive(true);
        notificationPanel.transform.rotation = Camera.main.transform.rotation;

        for (int i =0; i<currentEffects.Count; i++) {
            UnitEffect ue = currentEffects[i]; //No foreach as effects can be added during opening phase.
            //However, maybe this should be fixed... See NaturesBlessingBuilding
            System.Object[] data = ue.ApplyEffect();
            if (data == null || !visible)
            {
                yield return null;
            }
            else
            {
                yield return StartCoroutine(FadeNotification((string)data[1], (Utils.NotificationTypes)data[0]));
            }
        }
        notificationPanel.SetActive(false);
        currentEffects.RemoveAll(ue => ue.effectEnded); //safe removing of elements
        if (currentHealth <= 0)
        {
            Death();
        }
        yield return null;
    }
    public IEnumerator DisplayNotifications(Dictionary<Utils.NotificationTypes, int> notifications)
    {
        notificationPanel.SetActive(true);
        notificationPanel.transform.rotation = Camera.main.transform.rotation;
        foreach (Utils.NotificationTypes type in notifications.Keys)
        {
            yield return StartCoroutine(FadeNotification(notifications[type].ToString(),type));
        }
        notificationPanel.SetActive(false);
        yield return null;
    }

    private void LateUpdate()
    {
        healthCanvas.transform.rotation = Camera.main.transform.rotation;
    }
    public void SetVisible(bool v)
    {
        foreach(Renderer r in movementSphere.GetComponentsInChildren<Renderer>())
        {
            if (r.name != "Selector")
            {
                r.enabled = v;
            }
        }
        //prefab.SetActive(v);
        healthCanvas.enabled = v;
        visible = v;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        StartCoroutine(DisplayNotifications(new Dictionary<Utils.NotificationTypes, int> {
            { Utils.NotificationTypes.HEAL, amount } }));
        healthDisplay.fillAmount = (float)currentHealth / (float)(maxHealth);


    }

    public void IsAttacked(int amount, Unit source, bool riposte)
    {
        FaceNextNode(source.currentPosition);
        TakesDamage(amount);
        if(currentPosition.adjacentNodes.Contains(source.currentPosition) && attackRange==0 && !riposte)
        {
            StartCoroutine(Attack(source.currentPosition, true));
        }
    }

    public void TakesDamage(int amount, bool unsafeDeath=false)
    {
        int amountReduced = amount - armor;
        if (amountReduced <= 0)
        {
            amountReduced = 0;
        }
        currentHealth = currentHealth - amountReduced;
        StartCoroutine(DisplayNotifications(new Dictionary<Utils.NotificationTypes, int> { {Utils.NotificationTypes.DAMAGE, amountReduced } }));
        if (currentHealth <= 0 && !unsafeDeath)
        {
            Death();
        }
        
        healthDisplay.fillAmount = (float)currentHealth/(float)(maxHealth);
    }

    public void Death(bool AIcall=false)
    {
        Debug.Log("A unit died with AIcall:" + AIcall);
        if (owner.GetType() != typeof(ArtificialIntelligence) || AIcall || !TurnManager.Instance.currentPlayer.Equals(owner))
        {
            foreach (UnitEffect ue in currentEffect)
            {
                Debug.Log("         Ending effects " + ue.ToString());
                ue.End();
            }
            Debug.Log("       Effect ended");

            currentEffect = new List<UnitEffect>();
            TurnManager.Instance.StartTurnSubject.RemoveObserver(this);
            if (!(owner.isAi && TurnManager.Instance.currentPlayer.Equals(owner))){
            
                owner.currentUnits.Remove(this);
                Destroy(prefab);
                currentPosition.ResetNode(); //No double reset as a new unit could stand here now
            } //For riposte,safe remove after
            TurnManager.Instance.currentPlayer.UpdateVisibleNodes();
            Debug.Log("     reset correctly");
        }
        else
        {
            currentPosition.ResetNode();
            SetVisible(false);
        }
        Debug.Log("Unit died correctly");

    }
    public override void Select()
    {
        UpdateCardDisplayInfo();
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in rends)
        {
            if (rend.name == "Selector")
            {
                rend.enabled = true;
            }
        }
        ShowPossibleMoves();
    }

    public List<Node> ShowPossibleMoves()
    {
        currentPositionWrapped = NodeUtils.BFSNodesAdj(currentPosition, currentMovementPoints, true);
        possibleMoves = currentPositionWrapped.GetChildrens();


        foreach (Node node in possibleMoves)
        {
            if (TurnManager.Instance.currentPlayer.visibleNodes.Contains(node)) { 

                if (node == currentPosition)
                {
                    node.MakeIdle();
                }else if (node.Attackable(currentPosition))
                {
                    node.state = Node.STATE.ATTACKABLE_HIDDEN;
                }
                else 
                {
                    node.state = Node.STATE.SELECTABLE;
                }
            }
        }
        rangedAttackableMoves = new List<Node>();
        foreach (Node node in NodeUtils.BFSNodesAdj(currentPosition, attackRange).GetChildrens())
        {
            if (TurnManager.Instance.currentPlayer.visibleNodes.Contains(node))
            {
                if (node.Attackable(currentPosition))
                {
                    node.state = Node.STATE.ATTACKABLE_HIDDEN;
                    rangedAttackableMoves.Add(node);
                }
            }
        }


        return possibleMoves;
    }

    public override void Unselect()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in rends)
        {
            if (rend.name == "Selector")
            {
                rend.enabled = false;
            }
        }
        HidePossibleMoves();
    }

    public void HidePossibleMoves()
    {
        if (possibleMoves != null)
        {
            foreach (Node node in possibleMoves)
            {
                node.MakeIdle();
            }
            possibleMoves = null;
        }
        if(rangedAttackableMoves != null)
        {
            foreach (Node node in rangedAttackableMoves)
            {
                node.MakeIdle();
            }
            rangedAttackableMoves = null;
        }
    }

    public void ShowPotentialMove(Node target)
    {
        potentialPath = currentPositionWrapped.GetPath(target); //empty if ranged is out
        
        foreach (Node node in potentialPath)
        {
            if (node == currentPosition)
            {
                node.MakeIdle();
            }else if (node.Attackable(currentPosition))
            {
                node.state = Node.STATE.ATTACKABLE;
            }
            else 
            {
                node.state = Node.STATE.ON_UNIT_PATH;
            }
        }
        if (rangedAttackableMoves.Contains(target) || //Meaning attackable at range
            (currentMovementPoints + attackRange > potentialPath.Count &&
            target.Attackable(currentPosition)
            ))
        {
            target.state = Node.STATE.ATTACKABLE;
            Node attackSource = currentPosition;
            int removeRange = Mathf.Min(potentialPath.Count, attackRange);
            for (int i = 1; i < removeRange; i++)
            {
                potentialPath[i].state = Node.STATE.SELECTABLE;
            }
            if (potentialPath.Count >= 1 && attackRange<potentialPath.Count)
            {
                attackSource = potentialPath[removeRange]; //0 is enemy, 1 is last movable node
            }
            if (rangedAttackAnimation != null) //Sanity check
            {
                rangedAttackAnimation.ShowAttackPreview(attackSource, target);
            }

        }
    }

    public void HidePotentialMove(Node target)
    {
        if (rangedAttackAnimation != null)
        {
            rangedAttackAnimation.HideAttackPreview();
        }
        if (rangedAttackableMoves.Contains(target)) //Meaning attackable at range
        {
            target.state = Node.STATE.ATTACKABLE_HIDDEN;
        }
        if (potentialPath != null)
        {
            foreach (Node node in potentialPath)
            {
                if (node.Equals(currentPosition))
                {
                    node.MakeIdle();
                }
                else if (node.Attackable(currentPosition))
                {
                    node.state = Node.STATE.ATTACKABLE_HIDDEN;
                }
                else
                {
                    node.state = Node.STATE.SELECTABLE;
                }
                
            }
            potentialPath = null;
        }
    }
    protected void FaceNextNode(Node nextNode)
    {
        direction = nextNode.position - movementSphere.position;
        direction.y = 0;
        movementSphere.localRotation = Quaternion.LookRotation(direction);
    }

    protected virtual IEnumerator Attack(Node target, bool riposte)
    {
        anim.SetTrigger("Attack1Trigger");
        FinishMove();
        path = new List<Node>();
        currentMovementPoints = 0;
        FaceNextNode(target);
        if (currentPosition.adjacentNodes.Contains(target))
        {
            GameObject attackAnim = (GameObject)Instantiate(meleeAttackAnimation, target.position + attackOffset, new Quaternion(0, 0, 0, 0));
            Destroy(attackAnim, 5);
            yield return new WaitForSeconds(0.5f);
        }else if(rangedAttackAnimation != null)
        {
            rangedAttackAnimation.HideAttackPreview();
            rangedAttackAnimation.PlayAnimation();
            yield return new WaitForSeconds(rangedAttackAnimation.animationDuration + rangedAttackAnimation.delay);
        }
        
            animTransform.localPosition = new Vector3(0f, 0f, 0f);
            if (target.unit != null)
            {
                target.unit.IsAttacked(attack + currentAttackModifier, this, riposte);
            }
            else
            {
                target.Damage(attack + currentAttackModifier);
            }
        yield return new WaitForEndOfFrame();
    }

    protected virtual void FinishMove()
    {
        anim.ResetTrigger("Moving");
        moving = false;
        path = new List<Node>();
        Selector.Instance.Unselect(); //Unselect this, and thus MakeIdle all nodes
        HidePossibleMoves();
    }
    protected virtual void MoveStep()
    {
        float delta = visible? 0.5f : 1.5f;
        if (path.Count == 0)
        {
            FinishMove();
        }
        else if ((movementSphere.position.x <= path[0].position.x + delta && movementSphere.position.x >= path[0].position.x - delta) && (movementSphere.position.z <= path[0].position.z + delta && movementSphere.position.z >= path[0].position.z - delta))
        {
            movementSphere.position = new Vector3(path[0].position.x, movementSphere.position.y, path[0].position.z);
            EndMoveStep();
            path.Remove(path[0]);
            SetupNextMoveStep();
        }
        else
        {
            movementSphere.localPosition += direction * Time.deltaTime * (visible? speed:3.9f);
        }
    }
    protected virtual void SetupNextMoveStep()
    {
        if (path.Count == 0)
        {
            FinishMove();
        }
        else
        {
            FaceNextNode(path[0]);
            if(!TurnManager.Instance.currentPlayer.isAi || TurnManager.Instance.inactivePlayer.visibleNodes.Contains(path[0]))
            {
                SetVisible(true);
            }
            else
            {
                SetVisible(false);
            }
            if (path[0].Attackable(this.currentPosition))
            {
                StartCoroutine(Attack(path[0], false));
            }
            else if (attackRange>0 && path.Count <= attackRange && path[path.Count - 1].Attackable(this.currentPosition))
            {
                StartCoroutine(Attack(path[path.Count - 1], false));
            }
        }
    }

    protected virtual void EndMoveStep()
    {
        //Update currentPosition
        currentPosition.ResetNode();
        currentPosition = path[0];
        currentPosition.UpdateUnit(this);
        currentMovementPoints -= 1;
        if (!(TurnManager.Instance.currentPlayer.isAi))
        {
            UpdateCardDisplayInfo();
        }
        TurnManager.Instance.currentPlayer.UpdateVisibleNodes();
    }

    public virtual IEnumerator StartMoving(Node target)
    {
        if (rangedAttackableMoves.Contains(target))
        {
            yield return StartCoroutine(Attack(target, false));
        }
        else
        {
            moving = true;
            path = potentialPath;
            if (TurnManager.Instance.currentPlayer.isAi)
            {
                HidePotentialMove(path[path.Count - 1]);
                HidePossibleMoves();
            }
            if (path.Count >= 0)
            {
                anim.SetTrigger("Moving");
                path.Reverse();
                path.Remove(path[0]);
                SetupNextMoveStep();
                bool hiddenPath = false;
                if (TurnManager.Instance.currentPlayer.isAi)
                {
                    hiddenPath = true;
                    foreach (Node n in path)
                    {
                        if (TurnManager.Instance.inactivePlayer.visibleNodes.Contains(n))
                        {
                            hiddenPath = false;
                        }
                    }
                }
                yield return new WaitForSeconds(currentMovementPoints < 1 || hiddenPath ? 0.5f : currentMovementPoints - 1.0f);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public override void UpdateCardDisplayInfo()
    {
        string keywordsDescription = "";
        if (currentEffect != null) //sanity check as displayInfo can be displayed without the unit being instantiated
        {
            foreach (UnitEffect ue in currentEffect)
            {
                string effect = ue.GetDescriptionRelative();
                keywordsDescription += effect + "\n";
            }
        }
        TextMeshProUGUI[] elem = CardDisplay.Instance.EnableUnitCardDisplay(currentHealth, maxHealth, sprite, keywordsDescription);
        if (keywordsDescription != "")
        {
            elem = CardDisplay.Instance.EnableUnitCardDisplay(currentHealth, maxHealth, sprite, keywordsDescription);
            //TMPPRO bug fix?
        }
        foreach (TextMeshProUGUI e in elem)
        {
            switch (e.name)
            {
                case "CardNameText":
                    e.text = cardName;
                    break;
                case "CardCostText":
                    e.text = goldCost.ToString();
                    break;
                case "CardAttackText":
                    if (currentAttackModifier > 0)
                    {
                        e.color = Color.green;
                    }else if(currentAttackModifier < 0)
                    {
                        e.color = Color.red;
                    }
                    else
                    {
                        e.color = Color.white;
                    }
                    e.text = (attack + currentAttackModifier).ToString();
                    break;
                case "CardHealthText":
                    e.text = currentHealth + "/" + maxHealth;
                    break;
                case "CardMovementpointsText":
                    e.text = currentMovementPoints + "/" + maxMovementPoints;
                    break;
                case "CardEffectText":
                    e.text = effectDescription;
                    break;
            }
        }
    }
    public abstract Type GetSpawnPoint();
}
