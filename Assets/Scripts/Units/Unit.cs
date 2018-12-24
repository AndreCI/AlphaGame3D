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
    float rotationSpeed = 180f;


    [Header("Unit Information")]
    public int maxHealth;
    public int attack;
    public int maxMovementPoints;
    public int attackRange;
    public string effectDescription;
    public bool attackAnimationIsFromSource = false;
    [Header("Flying attribute")]
    public bool flying;

    [Header("Internal variables (no preset is needed)")]
    public int currentHealth;
    public int currentMovementPoints;
    public int currentAttackModifier;
    public List<UnitEffect> currentEffect;
    public List<UnitAbility> abilities;
    public int armor;
    public int foodConso;
    public int currentVisionRangeModifier;
    private int oldVisionRangeModifier;
    public Vector3 direction;
    private float orientation;
    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }
    private HexCellPriorityQueue searchFrontier;
    protected Animator anim;
    protected List<HexCell> potentialPath; //Current path for the target node
  //  protected List<HexCell> path; //selected path to node
    protected HexCell cellToGo;
    protected bool moving;
    protected List<HexCell> possibleMoves; //List of all the nodes where the unit can go
    //protected List<HexCell> rangedAttackableMoves; //List of all the nodes where the unit can go
    protected float yPositionOffset; //used in flying units

    //Mouvement data
    private Vector3 a, b, c;
    private float mouvementTime;
    private int currentColumn;
    public HexCell mouvementStartCell;

    public virtual void Setup()
    {
            anim = GetComponentInChildren<Animator>();
            anim.logWarnings = false;
        currentAttackModifier = 0;
        currentVisionRangeModifier = 0;
        currentHealth = maxHealth;
        currentMovementPoints = maxMovementPoints;
        healthDisplay.fillAmount = (float)currentHealth / (float)(maxHealth);
        healthDisplay.color = TurnManager.Instance.currentPlayer.Equals(Player.Player1) ? Color.green : Color.red;
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
        
    }

    public override void Notify(Player player, TurnSubject.NOTIFICATION_TYPE type)
    {
        if (player.Equals(owner))
        {
            if (type == TurnSubject.NOTIFICATION_TYPE.START_OF_TURN)
            {
                currentMovementPoints = maxMovementPoints;
                currentAttackModifier = 0;
                // HexGrid.Instance.DecreaseVisibility(currentPosition, visionRange + currentVisionRangeModifier, owner, additionalElevation: Mathf.FloorToInt(yPositionOffset));
                oldVisionRangeModifier = currentVisionRangeModifier;
                currentVisionRangeModifier = 0;
                armor = 0;
                
                StartCoroutine(DisplayAndApplyCurrentEffects(owner, currentEffect));
             

            }
        }


    }

    public IEnumerator DisplayAndApplyCurrentEffects(Player currentPlayer, List<UnitEffect> currentEffects)
    {
        foreach(UnitEffect ue in currentEffects)
        {
            if(ue.duration <= 0)
            {
                ue.End(); //end animations
            }
        }
        currentEffects.RemoveAll(ue => ue.duration <= 0); //safe removing of elements (direct apply buffs, which sticks even to 0 duration for a turn)

        notificationPanel.SetActive(true);
        notificationPanel.transform.rotation = HexMapCamera.instance.transform.rotation;//Camera.main.transform.rotation;

        for (int i = 0; i < currentEffects.Count; i++) {
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
            StartCoroutine(Death());
        }
        else
        {
            
            HexGrid.Instance.DecreaseVisibilityFromRadius(currentPosition, visionRange, visionRange + oldVisionRangeModifier, owner, additionalElevation: Mathf.FloorToInt(yPositionOffset));
            
        }
    }
    public IEnumerator DisplayNotifications(Dictionary<Utils.NotificationTypes, int> notifications)
    {
        notificationPanel.SetActive(true);
        notificationPanel.transform.rotation = Camera.main.transform.rotation;
        foreach (Utils.NotificationTypes type in notifications.Keys)
        {
            yield return StartCoroutine(FadeNotification(notifications[type].ToString(), type));
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
        foreach (Renderer r in movementSphere.GetComponentsInChildren<Renderer>())
        {
            if (r.name != "Selector")
            {
                r.enabled = v;
            }
        }
        //prefab.SetActive(v);
        healthCanvas.enabled = v;
        visible = v;
        
     //   rotationSpeed += v && rotationSpeed > 180f ? -100f : (v ? 0: 100f);
    //    speed += v && speed > 10 ? -10 : (v ? 0: 10);
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
        StartCoroutine(LookAt(source.currentPosition.Position + new Vector3(0, 1, 0)));
        TakesDamage(amount);
        if (currentPosition.GetNeighbors().Contains(source.currentPosition) &&
            attackRange == 0 &&
            !riposte &&
            currentHealth > 0)
        {
            StartCoroutine(Attack(source.currentPosition, true));
        }
    }

    public virtual void TakesDamage(int amount, bool unsafeDeath = false)
    {
        int amountReduced = amount - armor;
        if (amountReduced <= 0)
        {
            amountReduced = 0;
        }
        currentHealth = currentHealth - amountReduced;
        StartCoroutine(DisplayNotifications(new Dictionary<Utils.NotificationTypes, int> { { Utils.NotificationTypes.DAMAGE, amountReduced } }));
        if (currentHealth <= 0 && !unsafeDeath)
        {
            StartCoroutine(Death());
        }

        healthDisplay.fillAmount = (float)currentHealth / (float)(maxHealth);
    }

    public IEnumerator Death(bool AIcall = false)
    {
        HexGrid.Instance.DecreaseVisibility(currentPosition, visionRange + currentVisionRangeModifier, owner, additionalElevation: Mathf.FloorToInt(yPositionOffset));
        if (owner.Equals(Player.Player1))
        {
            NotificationsList.Instance.AddNotification("Your unit ("+ cardName+ ") died!",
                Notification.NOTIFICATION_TYPE.UNIT_DEATH,
                currentPosition);
        }
        anim.SetTrigger("Death");
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
            if (!(owner.isAi && TurnManager.Instance.currentPlayer.Equals(owner))) {

               // currentPosition.ResetNode(); //No double reset as a new unit could stand here now
                owner.currentUnits.Remove(this);
                yield return new WaitForSeconds(2.5f);
                Destroy(prefab);
            } //For riposte,safe remove after
            Debug.Log("     reset correctly");
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            currentPosition.unit = null;
            SetVisible(false);
        }
        Debug.Log("Unit died correctly");
        

    }
    public override void Select()
    {
        UpdateCardDisplayInfo();
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        //TODO: non efficient
        foreach (Renderer rend in rends)
        {
            if (rend.name == "Selector")
            {
                rend.enabled = true;
            }
        }
        mouvementStartCell = currentPosition;
        SearchAndShowPossibleMoves();
    }

    public void ClearPossibleMoves(HexCell previousCell =null)
    {
        HexCell temp = (previousCell == null ? mouvementStartCell : previousCell);
       // Debug.Log("Clear possible Move from " + temp.ToString());
        List<HexCell> cellsToClear = new List<HexCell>();
        Queue<HexCell> possibleMoves = new Queue<HexCell>();
        possibleMoves.Enqueue(previousCell==null?mouvementStartCell:previousCell);
        while (possibleMoves.Count > 0)
        {
            HexCell current = possibleMoves.Dequeue();
            foreach (HexCell cell in current.PathTo)
            {
                possibleMoves.Enqueue(cell);
            }
            cellsToClear.Add(current);
        }
        //cellsToClear.AddRange(rangedAttackableMoves);
        foreach(HexCell cell in cellsToClear)
        {
            cell.State = HexCell.STATE.IDLE;
        }
      //  rangedAttackableMoves = new List<HexCell>();
    }

    public void SetPathVisible(HexCell target, bool v=true)
    {
        if (!moving)
        {
            List<HexCell> currentPath = new List<HexCell>();
            HexCell current = target;
            currentPath.Add(current);
            if (target.State == HexCell.STATE.UNIT_POSSIBLE_PATH ||
                target.State == HexCell.STATE.UNIT_CURRENT_PATH ||
                target.State == HexCell.STATE.UNIT_POSSIBLE_ATTACK ||
                target.State == HexCell.STATE.UNIT_CURRENT_ATTACK)
            {
                
                while (current != currentPosition)
                {
                    //int turn = (current.Distance - 1) / speed;
                    //current.SetLabel(turn.ToString());
                    if (current.State == HexCell.STATE.UNIT_POSSIBLE_ATTACK ||
                    current.State == HexCell.STATE.UNIT_CURRENT_ATTACK)
                    {
                        current.State = v ? HexCell.STATE.UNIT_CURRENT_ATTACK : HexCell.STATE.UNIT_POSSIBLE_ATTACK;
                    }
                    else if(current.State == HexCell.STATE.UNIT_POSSIBLE_PATH ||
                    current.State == HexCell.STATE.UNIT_CURRENT_PATH)
                    {
                        current.State = v ? HexCell.STATE.UNIT_CURRENT_PATH : HexCell.STATE.UNIT_POSSIBLE_PATH;
                    }
                    current = current.PathFrom;
                    currentPath.Add(current);
                }
            }
            if (v && attackRange>0)
            {
                ShowPotentialRangeAttack(currentPath);
            }
            else if (rangedAttackAnimation != null)
            {
                rangedAttackAnimation.HideAttackPreview();
            }
        }
    }
    

    public void SearchAndShowPossibleMoves()
        //Should search using efficient algo (A* and stuff) and display all possible moves for the current unit.
        //Update the state of the node and unpdate searchFrontier in order to have all possible moves.
        //TODO: if heuristic range<attackrange, switch in mode rangeattack and allow 'movement' to go throguh cliffs and such
        //also add possibleattackrange as in prev vers
    {
        HexGrid.Instance.searchFrontierPhase += 2; //Making it +2 allows us to not have to reset this property.
        if (searchFrontier == null)
        {
            searchFrontier = new HexCellPriorityQueue();
        }
        else
        {
            searchFrontier.Clear();
        }
        currentPosition.SearchPhase = HexGrid.Instance.searchFrontierPhase;
        currentPosition.Distance = 0;
        currentPosition.RangeDistance = 0;
        currentPosition.SearchRange = false;
        searchFrontier.Enqueue(currentPosition);
        while (searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            current.SearchPhase += 1;
           if (current.Distance < currentMovementPoints + Mathf.Max(attackRange-1,0))
            {
                
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = current.GetNeighbor(d);
                    if (
                        neighbor == null ||
                        neighbor.SearchPhase > HexGrid.Instance.searchFrontierPhase
                    )
                    {
                        continue;
                    }
                    int moveCost = GetMoveCost(current, neighbor, d);
                    int rangeCost = GetRangeCost(current, neighbor, d);
                    if (moveCost < 0 && rangeCost<0)
                    {
                        continue;
                    }
                    bool nSearchRange = moveCost < 0 && rangeCost >= 0 || current.SearchRange;
                    int distance = current.Distance + (nSearchRange? rangeCost : moveCost);
                    if (neighbor.SearchPhase < HexGrid.Instance.searchFrontierPhase)
                    {
                        neighbor.SearchPhase = HexGrid.Instance.searchFrontierPhase;
                        neighbor.Distance = distance;
                        neighbor.RangeDistance = 0;
                        neighbor.PathFrom = current;
                        current.PathTo.Add(neighbor);
                        neighbor.SearchHeuristic = 0;
                        neighbor.SearchRange = nSearchRange;
                        if (IsAttackable(neighbor))
                        {
                            neighbor.State = HexCell.STATE.UNIT_POSSIBLE_ATTACK;
                            HexCell goback = neighbor;
                            while (goback != currentPosition)
                            {
                                goback.OnAttackPath = true;
                                goback = goback.PathFrom;
                            }
                        }
                        else if(neighbor.Distance <= currentMovementPoints && !nSearchRange)
                        {
                            neighbor.State = HexCell.STATE.UNIT_POSSIBLE_PATH;
                            searchFrontier.Enqueue(neighbor);
                        }
                        else if(current.RangeDistance+1 < attackRange)
                        {
                            //  rangedAttackableMoves.Add(neighbor);
                            neighbor.RangeDistance = current.RangeDistance + 1;
                            neighbor.EnableHighlight(Color.cyan, true);
                            searchFrontier.Enqueue(neighbor);
                        }
                        // neighbor.coordinates.DistanceTo(toCell.coordinates);
                    }
                    else if (searchFrontier.Contains(neighbor) && 
                        (distance < neighbor.Distance || 
                        (neighbor.SearchRange &&
                        neighbor.State!=HexCell.STATE.UNIT_POSSIBLE_ATTACK) &&
                        !neighbor.OnAttackPath))
                    {
                        int oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        neighbor.PathFrom.PathTo.Remove(neighbor);
                        neighbor.PathFrom = current;
                        current.PathTo.Add(neighbor);
                        searchFrontier.Change(neighbor, oldPriority);
                        neighbor.SearchRange = nSearchRange;
                        if (neighbor.Distance <= currentMovementPoints && !nSearchRange)
                        {
                            neighbor.State = HexCell.STATE.UNIT_POSSIBLE_PATH;
                        }

                        }

                }
            }
        }
    }
    public virtual bool IsValidDestination(HexCell cell)
    {
        return cell.IsExplored(owner) && 
            (!cell.IsUnderwater || IsAttackable(cell)) && 
            (!cell.unit || cell.unit.owner!=owner) && 
            !cell.building;
    }
    public bool IsAttackable(HexCell cell)
    {
        return cell.unit != null && 
            cell.unit.currentHealth>0 && 
            cell.unit.owner != owner &&
        //    (!cell.unit.flying || attackRange>0) && //(!cell.IsUnderwater || 
            ((!cell.unit.flying || attackRange>0)); //flying check is redundant but makes it more clear
    }
    public virtual int GetMoveCost(
        HexCell fromCell, HexCell toCell, HexDirection direction)
    {
        if (!IsValidDestination(toCell))
        {
            return -1;
        }
        HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
        if (edgeType == HexEdgeType.Cliff)
        {
            return -1;
        }
        int moveCost;
        if (fromCell.Walled != toCell.Walled)
        {
            return -1;
        }
        else
        {
            //	moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
            //	moveCost +=
            //		toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
            moveCost = 1;
        }
        return moveCost;
    }

    public int GetRangeCost(
        HexCell fromCell, HexCell toCell, HexDirection direction)
    {
        if (!toCell.IsExplored(owner) || toCell.IsUnderwater)
        {
            return -1;
        }
        HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
        if (edgeType == HexEdgeType.Cliff && fromCell.Elevation < toCell.Elevation)
        {
            return -1;
        }
        int rangeCost;
        if (fromCell.Walled != toCell.Walled)
        {
            return -1;
        }
        else
        {
            //	moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
            //	moveCost +=
            //		toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
            rangeCost = 1;
        }
        return rangeCost;
    } public override void Unselect()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in rends)
        {
            if (rend.name == "Selector")
            {
                rend.enabled = false;
            }
        }
        if (currentPosition != null)
        {
            ClearPossibleMoves();
        }
    }
    public void ValidateLocation()
    {
        movementSphere.position = currentPosition.Position + new Vector3(0, yPositionOffset, 0);
    }
    
    public void ShowPotentialRangeAttack(List<HexCell> potentialPath)
    {
        HexCell target = potentialPath[0];
        if (//rangedAttackableMoves.Contains(target) || //Meaning attackable at range
            (currentMovementPoints + attackRange >= potentialPath.Count &&
            IsAttackable(target)
            ))
        {
            target.State = HexCell.STATE.UNIT_CURRENT_ATTACK;
            HexCell attackSource = currentPosition;
            int removeRange = Mathf.Min(potentialPath.Count, attackRange);
            for (int i = 1; i < removeRange; i++)
            {
                if (potentialPath[i].State == HexCell.STATE.UNIT_CURRENT_PATH)
                {
                    potentialPath[i].State = HexCell.STATE.UNIT_POSSIBLE_PATH;
                }
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
   IEnumerator LookAt(Vector3 point)
    {
        if (HexMetrics.Wrapping)
        {
            float xDistance = point.x - movementSphere.position.x;
            if (xDistance < -HexMetrics.innerRadius * HexMetrics.wrapSize)
            {
                point.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
            else if (xDistance > HexMetrics.innerRadius * HexMetrics.wrapSize)
            {
                point.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
        }

        point.y = movementSphere.position.y;
        Quaternion fromRotation = movementSphere.localRotation;
        Quaternion toRotation =
            Quaternion.LookRotation(point - movementSphere.position);
        float angle = Quaternion.Angle(fromRotation, toRotation);

        if (angle > 0f)
        {
            float speed = rotationSpeed / angle;
            for (
                float t = Time.deltaTime * speed;
                t < 1f;
                t += Time.deltaTime * speed
            )
            {
                movementSphere.localRotation =
                    Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }
        }

        movementSphere.LookAt(point);
        orientation = movementSphere.localRotation.eulerAngles.y;
    }

    protected virtual IEnumerator Attack(HexCell target, bool riposte)
    {
        yield return StartCoroutine(LookAt(target.Position + attackOffset));
        anim.SetTrigger("Attack1Trigger");
        currentMovementPoints = 0;
        if (currentPosition.GetNeighbors().Contains(target) && meleeAttackAnimation != null)
        {
            if (attackAnimationIsFromSource)
            {
                foreach(ParticleSystem ps in meleeAttackAnimation.GetComponentsInChildren<ParticleSystem>())
                {
                    ps.transform.LookAt(target.Position + attackOffset);
                    ps.Play();
                }
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                GameObject attackAnim = (GameObject)Instantiate(meleeAttackAnimation, target.Position + attackOffset, new Quaternion(0, 0, 0, 0));
                Destroy(attackAnim, 5);
                yield return new WaitForSeconds(0.5f);
            }
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

    protected virtual bool SetupAttack(List<HexCell> nextCells)
    {
        return IsAttackable(nextCells[0]) ||
            (attackRange > 0 &&
            nextCells.Count <= attackRange &&
            nextCells.Count > 1 &&
            IsAttackable(nextCells[nextCells.Count - 1]));
    }
 

    public IEnumerator MoveTo(HexCell target)
    {
        if (!moving)
        {
            HexCell previousCell = currentPosition;
            //Setup Path
            List<HexCell> pathToTravel = new List<HexCell>();
            HexCell current = target;
            while (current != currentPosition)
            {
                pathToTravel.Add(current);
                current = current.PathFrom;
            }
            pathToTravel.Add(current);
            pathToTravel.Reverse();
            yield return StartCoroutine(LookAt(pathToTravel[1].Position));
            bool attackNext = SetupAttack(pathToTravel.GetRange(1, pathToTravel.Count - 1));
            if (attackNext)
            {
                if (IsAttackable(pathToTravel[1]))
                {
                    ClearPossibleMoves();
                    yield return StartCoroutine(Attack(pathToTravel[1], false));
                }
                else if (IsAttackable(pathToTravel[pathToTravel.Count - 1]))
                {
                    ClearPossibleMoves();
                    yield return StartCoroutine(Attack(pathToTravel[pathToTravel.Count - 1], false));
                }
            }
            else
            {
                //Start moving
                a = b = c = pathToTravel[0].Position + new Vector3(0, yPositionOffset, 0);
                moving = true;
                anim.SetTrigger("Moving");

               // HexGrid.Instance.DecreaseVisibility(pathToTravel[0], visionRange);
                currentColumn = pathToTravel[0].ColumnIndex;

                mouvementTime = Time.deltaTime * speed;
                int i;
                for (i = 1; i < pathToTravel.Count; i++)
                {
                    attackNext = SetupAttack(pathToTravel.GetRange(i, pathToTravel.Count - i));
                    if (!attackNext)
                    {
                        yield return StartCoroutine(MoveStep(pathToTravel[i - 1], pathToTravel[i]));
                    }
                    else
                    {
                        break;
                    }
                    HexGrid.Instance.DecreaseVisibility(pathToTravel[i-1], visionRange + currentVisionRangeModifier, owner, additionalElevation: Mathf.FloorToInt(yPositionOffset));
                    HexGrid.Instance.IncreaseVisibility(pathToTravel[i], visionRange + currentVisionRangeModifier, owner, additionalElevation: Mathf.FloorToInt(yPositionOffset));
                }
                yield return StartCoroutine(FinishMove(previousCell));
                if (attackNext)
                {
                    if (IsAttackable(pathToTravel[i]))
                    {
                        yield return StartCoroutine(Attack(pathToTravel[i], false));
                    }
                    else if (IsAttackable(pathToTravel[pathToTravel.Count - 1]))
                    {
                        yield return StartCoroutine(Attack(pathToTravel[pathToTravel.Count - 1], false));
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator FinishMove(HexCell previousCell)
    {
        a = c;
        b = currentPosition.Position + new Vector3(0, yPositionOffset, 0);
        c = b;
        for (; mouvementTime < 1f; mouvementTime += Time.deltaTime * speed)
        {
            movementSphere.position = Bezier.GetPoint(a, b, c, mouvementTime);
            Vector3 d = Bezier.GetDerivative(a, b, c, mouvementTime);
            d.y = 0f;
            movementSphere.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }

        movementSphere.position = currentPosition.Position + new Vector3(0, yPositionOffset, 0);
        orientation = movementSphere.localRotation.eulerAngles.y;
        //HexGrid.Instance.DecreaseVisibility(previousCell, visionRange + currentVisionRangeModifier, owner, additionalElevation: Mathf.FloorToInt(yPositionOffset));
      //  HexGrid.Instance.IncreaseVisibility(currentPosition, visionRange + currentVisionRangeModifier, owner, additionalElevation: Mathf.FloorToInt(yPositionOffset));
        anim.ResetTrigger("Moving");
        moving = false;
        ClearPossibleMoves(previousCell: previousCell);
        if(Selector.Instance.currentObject == this)
        {
            Selector.Instance.Unselect();
        }
        yield return new WaitForEndOfFrame();
    }


    protected virtual IEnumerator MoveStep(HexCell previousCell, HexCell nextCell)
    {
        HexCell currentTravelLocation = nextCell;
        a = c;
        b = previousCell.Position + new Vector3(0, yPositionOffset, 0);

        int nextColumn = currentTravelLocation.ColumnIndex;
        if (currentColumn != nextColumn)
        {
            if (nextColumn < currentColumn - 1)
            {
                a.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
                b.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
            else if (nextColumn > currentColumn + 1)
            {
                a.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
                b.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
            //HexGrid.Instance.MakeChildOfColumn(movementSphere, nextColumn);
            currentColumn = nextColumn;
        }

        c = (b + currentTravelLocation.Position) * 0.5f;
        float hideSpeed = speed;
        if(TurnManager.Instance.againstAI &&
            currentPosition.visibility_p1 <=0 &&
            nextCell.visibility_p1 <= 0)
        {
            hideSpeed += 10;
        }
        currentPosition.unit = null;
        currentPosition = nextCell;
        currentPosition.unit = this;
        for (; mouvementTime < 1f; mouvementTime+= Time.deltaTime * hideSpeed)
        {
            movementSphere.position = Bezier.GetPoint(a, b, c, mouvementTime);
            Vector3 d = Bezier.GetDerivative(a, b, c, mouvementTime);
            d.y = 0f;
            movementSphere.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }
        mouvementTime -= 1f;
        currentMovementPoints -= 1;
        if (!TurnManager.Instance.againstAI || TurnManager.Instance.currentPlayer.Equals(Player.Player1))
        {
            UpdateCardDisplayInfo();
        }
        yield return new WaitForEndOfFrame();
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

    public int GetVisionRange()
    {
        return currentVisionRangeModifier + visionRange < 0 ? 0 : currentVisionRangeModifier + visionRange;
    }

    public abstract Type GetSpawnPoint();
}
