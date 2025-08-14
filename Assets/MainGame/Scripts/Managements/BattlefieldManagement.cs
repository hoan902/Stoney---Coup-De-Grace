using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.Util;

public class BattlefieldManagement : ComponentSingleton<BattlefieldManagement>
{
    [SerializeField] List<GameObject> m_slotsTeam1;
    [SerializeField] List<GameObject> m_slotsTeam2;
    [SerializeField] AudioClip m_victoryClip;
    [SerializeField] AudioClip m_loseClip;
    HashSet<BaseCharacter> m_allCharOnBoard;
    HashSet<BaseCharacter> m_allTeam1Char;
    HashSet<BaseCharacter> m_allTeam2Char;

    private void Start()
    {
        m_allCharOnBoard = new HashSet<BaseCharacter>();
        m_allTeam1Char = new HashSet<BaseCharacter>();
        m_allTeam2Char = new HashSet<BaseCharacter>();  
    }

    public void SetupPosSpawnCharacters()
    {
        int maxSlotTeam1 = m_slotsTeam1.Count;
        int maxSlotTeam2 = m_slotsTeam2.Count;
        int i = 0;
        int ii = 0;
        foreach (var chara in m_allTeam1Char)
        {
            if (i < maxSlotTeam1)
            {
                chara.characterController.enabled = false;
                chara.transform.position = m_slotsTeam1[i].transform.position;
                chara.transform.SetParent(m_slotsTeam1[i].transform, true);
                i++;
                chara.characterController.enabled = true;
            }
        }
        foreach (var chara in m_allTeam2Char)
        {
            if (ii < maxSlotTeam2)
            {
                chara.characterController.enabled = false;
                chara.transform.SetPositionAndRotation(m_slotsTeam2[ii].transform.position, new Quaternion(0, 180f, 0, 0));
                chara.transform.SetParent(m_slotsTeam2[ii].transform, true);
                ii++;
                chara.characterController.enabled = true;
            }
        }
    }

    public void CleanAll()
    {
        foreach (var item in m_allCharOnBoard)
            Destroy(item.gameObject);
        foreach (var item in m_allTeam1Char)
            Destroy(item.gameObject);
        foreach (var item in m_allTeam2Char)
            Destroy(item.gameObject);
        m_allCharOnBoard.Clear();
        m_allTeam1Char.Clear();
        m_allTeam2Char.Clear();
    }

    public void AddToTotalChar(BaseCharacter chara)
    {
        m_allCharOnBoard.Add(chara);
    }

    public void AddCharToTeam1(BaseCharacter chara)
    {
        m_allTeam1Char.Add(chara);
    }

    public void AddCharToTeam2(BaseCharacter chara)
    {
        m_allTeam2Char.Add(chara);
    }

    public void RemoveChar(BaseCharacter chara, Team team)
    {
        m_allCharOnBoard.Remove(chara);
        switch (team)
        {
            case Team.Team1:
                m_allTeam1Char.Remove(chara);
                break;
            case Team.Team2:
                m_allTeam2Char.Remove(chara);
                break;
        }
    }

    public void CheckingBattleStatus()
    {
        Debug.Log("Overall Team 1 Troop: " + m_allTeam1Char.Count);
        Debug.Log("Overall Team 2 Troop: " + m_allTeam2Char.Count);
        if (m_allTeam1Char.Count == 0 || m_allTeam2Char.Count == 0)
            OnBattleEnd(m_allTeam1Char.Count == 0 ? Team.Team2 : Team.Team1);
    }
    void OnBattleEnd(Team winingTeam)
    {
        SoundManager.PlaySound("boxing-bell", false);
        GameController.OnEndedMatch(winingTeam);
        GameController.ActiveInput(false);
        switch (winingTeam)
        {
            case Team.Team1:
                SoundManager.PlaySound(m_victoryClip, false);
                foreach (var chara in m_allTeam1Char)
                    chara.OnVictory();
                //HoanDN: Winning Screen + lv up, Replay or next lv button, Swap mode button, Full Clean level to generate new level or Swaping mode
                break;
            case Team.Team2:
                SoundManager.PlaySound(m_loseClip, false);
                foreach (var chara in m_allTeam2Char)
                    chara.OnVictory();
                break;
        }
    }
    //-----------------------------------------------------------------
    public HashSet<BaseCharacter> GetTotalCharOnBoard()
    {
        return m_allCharOnBoard;
    }

    public HashSet<BaseCharacter> GetTeam1OnBoard()
    {
        return m_allTeam1Char;
    }

    public HashSet<BaseCharacter> GetTeam2OnBoard()
    {
        return m_allTeam2Char;
    }
}
