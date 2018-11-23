using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBSManager : MonoBehaviour {

    private Dictionary<int, SBSGroup> sbsGroups;

    [SerializeField] private bool hookUpOnEnable = true;
    [SerializeField] private bool hookedToInput_;
    public bool HookedToInput { get { return hookedToInput_; } }


    /************************************************************************************/
    /********************************* UNITY BEHAVIOURS *********************************/
    /************************************************************************************/

    private void Awake()
    {
        RefreshSelectables();
    }

    private void OnEnable()
    {
        if (hookUpOnEnable) { HookToInput(); }
    }

    private void Start()
    {      
        FocusAllCurrents();
    }

    private void OnDisable()
    {
        UnHookFromInput();
    }

    /************************************************************************************/
    /************************************ BEHAVIOURS ************************************/
    /************************************************************************************/

    public void RefreshSelectables()
    {
        sbsGroups = new Dictionary<int, SBSGroup>();

        SingleButtonSelectable[] unsortedSelectables = GetComponentsInChildren<SingleButtonSelectable>(true);
        SortIntoGroups(unsortedSelectables);
        foreach (int group in sbsGroups.Keys)
        {
            SortGroupByTabIndex(group);
        }
    }

    /// <summary>
    /// Sorts an array of SingleButtonSelectables into the selectables dictionary based on their group.
    /// </summary>
    /// <param name="unsortedSelectables"></param>
    private void SortIntoGroups(SingleButtonSelectable[] unsortedSelectables)
    {
        foreach(SingleButtonSelectable sbs in unsortedSelectables) {
            AddGroup(sbs.Group);

            sbsGroups[sbs.Group].Selectables.Add(sbs);
        }
    }

    /// <summary>
    /// Sorts a specified group of SingleButtonSelectables in descending order based on their tabIndex.
    /// </summary>
    /// <param name="group">The group to sort. If the group doesn't exist, the method is aborted.</param>
    private void SortGroupByTabIndex(int group)
    {
        if (!sbsGroups.ContainsKey(group)) { return; }
        SingleButtonSelectable temp;

        for (int i = 0; i < sbsGroups[group].Selectables.Count; i++) {
            for (int j = 0; j < sbsGroups[group].Selectables.Count - 1; j++) {
                if (sbsGroups[group].Selectables[j].TabIndex > sbsGroups[group].Selectables[j + 1].TabIndex) {
                    temp = sbsGroups[group].Selectables[j + 1];
                    sbsGroups[group].Selectables[j + 1] = sbsGroups[group].Selectables[j];
                    sbsGroups[group].Selectables[j] = temp;
                }
            }
        }
    }

    /// <summary>
    /// Adds a group to selectables.
    /// </summary>
    /// <param name="groupId">The id of the group to add.</param>
    private void AddGroup(int groupId)
    {
        if (sbsGroups.ContainsKey(groupId)) { return; }
        sbsGroups.Add(groupId, new SBSGroup());
    }

    /// <summary>
    /// Adds a new SingleButtonSelectable into the SBSManager.
    /// </summary>
    /// <param name="sbs">The SingleButtonSelectable to add.</param>
    public void AddSelectable(SingleButtonSelectable sbs)
    {
        AddGroup(sbs.Group);

        for(int i = 0; i <= sbsGroups[sbs.Group].Selectables.Count; i++) {
            if(sbs.TabIndex < sbsGroups[sbs.Group].Selectables[i].TabIndex) {
                sbsGroups[sbs.Group].Selectables.Insert(i, sbs);
                return;
            }
        }

        sbsGroups[sbs.Group].Selectables.Add(sbs);
    }

    /// <summary>
    /// Removes a SingleButtonSelectable from the SBSManager.
    /// </summary>
    /// <param name="sbs">The SingleButtonSelectable to remove.</param>
    public void RemoveSelectable(SingleButtonSelectable sbs)
    {
        if (!sbsGroups.ContainsKey(sbs.Group)) {
            return;
        }

        sbsGroups[sbs.Group].Selectables.Remove(sbs);
    }
    
    /// <summary>
    /// Gets the length of a specified group. Returns 0 if the group does not exist.
    /// </summary>
    /// <param name="group">The group to get the lenght of.</param>
    /// <returns>The length of the group.</returns>
    public int GetGroupLength(int group)
    {
        if (!sbsGroups.ContainsKey(group)) { return 0; }

        return sbsGroups[group].Selectables.Count;
    }

    /// <summary>
    /// Adds a listener to the InputGrabber events.
    /// </summary>
    public void HookToInput()
    {
        if (hookedToInput_) { return; }

        InputGrabber.Instance.TabEvent += InputGrabber_TabEvent;
        InputGrabber.Instance.SelectEvent += InputGrabber_SelectEvent;
        InputGrabber.Instance.InputEventStart += InputGrabber_InputEventStart;

        hookedToInput_ = true;
    }

    /// <summary>
    /// Removes listeners from InputGrabber events.
    /// </summary>
    public void UnHookFromInput()
    {
        if (!hookedToInput_) { return; }

        InputGrabber.Instance.TabEvent -= InputGrabber_TabEvent;
        InputGrabber.Instance.SelectEvent -= InputGrabber_SelectEvent;
        InputGrabber.Instance.InputEventStart -= InputGrabber_InputEventStart;

        hookedToInput_ = false;
    }

    /// <summary>
    /// Focus the next selectable in a group.
    /// </summary>
    /// <param name="group">The group to focus within.</param>
    public void FocusNext(int group)
    {
        if (!sbsGroups.ContainsKey(group)) { return; }
        if (sbsGroups[group].Selectables.Count == 0) { return; }

        sbsGroups[group].Selectables[sbsGroups[group].CurrentlySelectedIndex].Defocus();

        int safetyBreak = 0;

        do //loop while the CurrentlySelectedIndex is pointing to an inactive selectable.
        {
            if (safetyBreak >= sbsGroups[group].Selectables.Count) { //If the iteration of this loop is greater than the number of selectables, throw error.
                throw new System.Exception("Attempting to FocusNext on group with zero active selectables.");
            }

            if (sbsGroups[group].CurrentlySelectedIndex == sbsGroups[group].Selectables.Count - 1) { //If currently selected index is at the end of the list, return to zero.
                sbsGroups[group].CurrentlySelectedIndex = 0;
            }
            else { //Otherwise increment.
                sbsGroups[group].CurrentlySelectedIndex++;
            }

            safetyBreak++; //Monitor iteration count.

        } while (!sbsGroups[group].Selectables[sbsGroups[group].CurrentlySelectedIndex].gameObject.activeSelf);


        sbsGroups[group].Selectables[sbsGroups[group].CurrentlySelectedIndex].Focus();
    }

    /// <summary>
    /// Focuses the currentIndex of all groups with no noise.
    /// </summary>
    public void FocusAllCurrents()
    {
        foreach(int g in sbsGroups.Keys)
        {
            if(sbsGroups[g].CurrentlySelectedIndex >= 0 && sbsGroups[g].CurrentlySelectedIndex < sbsGroups[g].Selectables.Count)
            {
                if (sbsGroups[g].Selectables[sbsGroups[g].CurrentlySelectedIndex].gameObject.activeSelf)
                {
                    sbsGroups[g].Selectables[sbsGroups[g].CurrentlySelectedIndex].Focus(false);
                }
            }
        }
    }

    /// <summary>
    /// Call the Select event on the currently selected selectable in a group.
    /// </summary>
    /// <param name="group">The group to select from.</param>
    public void SelectActiveIndex(int group)
    {
        if (!sbsGroups.ContainsKey(group)) { return; }
        //If the currently selected index is greater than the number of selectable items in the group, return.
        if(sbsGroups[group].CurrentlySelectedIndex >= sbsGroups[group].Selectables.Count) { return; }
        //If the currently selected selectable is inactive, return.
        if (!sbsGroups[group].Selectables[sbsGroups[group].CurrentlySelectedIndex].gameObject.activeSelf) { return; }

        sbsGroups[group].Selectables[sbsGroups[group].CurrentlySelectedIndex].Select();
    }

    /// <summary>
    /// Add a SelectionMeter to a group.
    /// </summary>
    /// <param name="group">The group to add a selection meter to.</param>
    /// <param name="selectionMeter">The selection meter to add.</param>
    public void AddSelectionMeter(int group, SelectionMeter selectionMeter)
    {
        if (!sbsGroups.ContainsKey(group)) {
            Debug.LogWarning("Attempting to add selection meter to non existant group " + group.ToString() + ". Aborting.");
            return;
        }

        sbsGroups[group].SelectionMeter = selectionMeter;
    }

    /// <summary>
    /// Sets the value of hookUpOnEnable.
    /// </summary>
    /// <param name="b"></param>
    public void SetHookUpOnEnable(bool b)
    {
        hookUpOnEnable = b;
    }

    /************************************************************************************/
    /************************************ COROUTINES ************************************/
    /************************************************************************************/

    /// <summary>
    /// Fills a specified SelectionMeter based on an associated timer in the InputGrabber.
    /// </summary>
    /// <param name="groupID">The group the SelectionMeter is associated with.</param>
    /// <param name="selectionMeter">The SelectionMeter to fill.</param>
    /// <returns></returns>
    protected IEnumerator FillInputMeter(int groupID, SelectionMeter selectionMeter)
    {
        while (selectionMeter.Value < 1)
        {
            selectionMeter.Value = InputGrabber.Instance.GetSelectionTime(groupID) / InputGrabber.Instance.TimeToSelect;
            selectionMeter.Alpha = selectionMeter.Value;
            yield return null;
        }

        selectionMeter.Value = 0;
        selectionMeter.Alpha = selectionMeter.Value;
    }

    /************************************************************************************/
    /********************************* EVENT LISTENERS **********************************/
    /************************************************************************************/

    protected virtual void InputGrabber_SelectEvent(object sender, InputGrabber.SelectEventArgs e)
    {
        sbsGroups[e.playerIndex].SelectionMeter.Value = 0;
        SelectActiveIndex(e.playerIndex);
    }

    protected void InputGrabber_TabEvent(object sender, InputGrabber.TabEventArgs e)
    {
        sbsGroups[e.playerIndex].SelectionMeter.Value = 0;
        FocusNext(e.playerIndex);
        CoroutineManager.HaltCoroutine(ref sbsGroups[e.playerIndex].CR_MeterFill, this);
    }

    protected void InputGrabber_InputEventStart(object sender, InputGrabber.InputEventStartArgs e)
    {
        CoroutineManager.BeginCoroutine(FillInputMeter(e.playerIndex, sbsGroups[e.playerIndex].SelectionMeter), ref sbsGroups[e.playerIndex].CR_MeterFill, this);
    }

    /************************************************************************************/
    /************************************ SUBCLASSES ************************************/
    /************************************************************************************/

    private class SBSGroup
    {
        public int CurrentlySelectedIndex = 0;
        public SelectionMeter SelectionMeter;
        public Coroutine CR_MeterFill;
        public List<SingleButtonSelectable> Selectables;

        public SBSGroup()
        {
            Selectables = new List<SingleButtonSelectable>();
            SelectionMeter = null;
            CR_MeterFill = null;
        }

    }

}

