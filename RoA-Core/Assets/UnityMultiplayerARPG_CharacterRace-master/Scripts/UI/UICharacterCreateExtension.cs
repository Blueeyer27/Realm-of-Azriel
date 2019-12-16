using MultiplayerARPG;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class UICharacterCreateExtension : UICharacterCreate
{
    public Text HairList;
    public Text FaceList;
    public Text BeardList;
    public Text EyebrowList;

    protected int selectedHair = 1;
    protected int selectedFace = 1;
    protected int selectedBeard = 1;
    protected int selectedEyebrow = 1;

    public override void Show()
    {
       
        EquipHair();
        EquipBeard();
        EquipFace();
        EquipEyebrow();
        LoadHair();
        LoadBeard();
        LoadFace();
        LoadEyebrow();



        base.Show();
    }

    protected virtual List<Item> GetSelectableHair()
    {
        List<Item> items = GameInstance.Items.Values.ToList();
        List<Item> Hairs = new List<Item>();

        foreach (Item item in items)
        {
            if (item.ArmorType.name.ToString().Equals("Hair"))
                Hairs.Add(item);
        }

        return Hairs;
    }

    protected virtual List<Item> GetSelectableFaces()
    {
        List<Item> items = GameInstance.Items.Values.ToList();
        List<Item> Faces = new List<Item>();

        foreach (Item item in items)
        {
            if (item.ArmorType.name.ToString().Equals("Face"))
                Faces.Add(item);
        }

        return Faces;
    }

    protected virtual List<Item> GetSelectableBeard()
    {
        List<Item> items = GameInstance.Items.Values.ToList();
        List<Item> Beard = new List<Item>();

        foreach (Item item in items)
        {
            if (item.ArmorType.name.ToString().Equals("Beard"))
                Beard.Add(item);
        }

        return Beard;
    }

    protected virtual List<Item> GetSelectableEyebrow()
    {
        List<Item> items = GameInstance.Items.Values.ToList();
        List<Item> Eyebrow = new List<Item>();

        foreach (Item item in items)
        {
            if (item.ArmorType.name.ToString().Equals("Eyebrow"))
                Eyebrow.Add(item);
        }

        return Eyebrow;
    }

    protected virtual void LoadHair()
    {
        HairList.text = selectedHair + " / " + GetSelectableHair().Count.ToString();
        Debug.Log(GetSelectableHair().Count.ToString());
    }
    protected virtual void LoadFace()
    {
        FaceList.text = selectedFace + " / " + GetSelectableFaces().Count.ToString();
        Debug.Log(GetSelectableFaces().Count.ToString());
    }
    protected virtual void LoadBeard()
    {
        BeardList.text = selectedBeard + " / " + GetSelectableBeard().Count.ToString();
        Debug.Log(GetSelectableFaces().Count.ToString());
    }
    protected virtual void LoadEyebrow()
    {
        EyebrowList.text = selectedEyebrow + " / " + GetSelectableEyebrow().Count.ToString();
        Debug.Log(GetSelectableEyebrow().Count.ToString());
    }

    public void EquipHair()
    {
        PlayerCharacterEntity player = null;
        foreach (Transform child in characterModelContainer)
            if (child.gameObject.activeInHierarchy == true)
                player = child.gameObject.GetComponent<PlayerCharacterEntity>();
        if (player != null)
            foreach (BasePlayerCharacterEntity BplayerCharacter in GameInstance.PlayerCharacterEntities.Values.ToList())
                foreach (PlayerCharacter playerCharacter in BplayerCharacter.playerCharacters)
                    playerCharacter.armorItems[0] = GetSelectableHair()[selectedHair - 1];
    }
    public void EquipBeard()
    {
        PlayerCharacterEntity player = null;
        foreach (Transform child in characterModelContainer)
            if (child.gameObject.activeInHierarchy == true)
                player = child.gameObject.GetComponent<PlayerCharacterEntity>();
        if (player != null)
            foreach (BasePlayerCharacterEntity BplayerCharacter in GameInstance.PlayerCharacterEntities.Values.ToList())
                foreach (PlayerCharacter playerCharacter in BplayerCharacter.playerCharacters)
                    playerCharacter.armorItems[1] = GetSelectableBeard()[selectedBeard - 1];
    }
    public void EquipFace()
    {
        PlayerCharacterEntity player = null;
        foreach (Transform child in characterModelContainer)
            if (child.gameObject.activeInHierarchy == true)
                player = child.gameObject.GetComponent<PlayerCharacterEntity>();
        if (player != null)
            foreach (BasePlayerCharacterEntity BplayerCharacter in GameInstance.PlayerCharacterEntities.Values.ToList())
                foreach (PlayerCharacter playerCharacter in BplayerCharacter.playerCharacters)
                    playerCharacter.armorItems[2] = GetSelectableFaces()[selectedFace - 1];
    }
    public void EquipEyebrow()
    {
        PlayerCharacterEntity player = null;
        foreach (Transform child in characterModelContainer)
            if (child.gameObject.activeInHierarchy == true)
                player = child.gameObject.GetComponent<PlayerCharacterEntity>();
        if (player != null)
            foreach (BasePlayerCharacterEntity BplayerCharacter in GameInstance.PlayerCharacterEntities.Values.ToList())
                foreach (PlayerCharacter playerCharacter in BplayerCharacter.playerCharacters)
                    playerCharacter.armorItems[3] = GetSelectableEyebrow()[selectedEyebrow - 1];
    }

    public void NextHair()
    {
        if (selectedHair < GetSelectableHair().Count)
        {
            selectedHair++;
            EquipHair();
            LoadHair();
            LoadCharacters();
        }
    }

    public void PreviousHair()
    {
        if (selectedHair - 1 > 0)
        {
            selectedHair--;
            EquipHair();
            LoadHair();
            LoadCharacters();
        }
    }

    public void NextBeard()
    {
        if (selectedBeard < GetSelectableBeard().Count)
        {
            selectedBeard++;
            EquipBeard();
            LoadBeard();
            LoadCharacters();
        }
    }

    public void PreviousBeard()
    {
        if (selectedBeard - 1 > 0)
        {
            selectedBeard--;
            EquipBeard();
            LoadBeard();
            LoadCharacters();
        }
    }

    public void NextFace()
    {
        if (selectedFace < GetSelectableFaces().Count)
        {
            selectedFace++;
            EquipFace();
            LoadFace();
            LoadCharacters();
        }

    }

    public void PreviousFace()
    {
        if (selectedFace - 1 > 0)
        {
            selectedFace--;
            EquipFace();
            LoadFace();
            LoadCharacters();
        }
    }

    public void NextEyebrowm()
    {
        if (selectedEyebrow < GetSelectableEyebrow().Count)
        {
            selectedEyebrow++;
            EquipEyebrow();
            LoadEyebrow();
            LoadCharacters();
        }

    }

    public void PreviousEyebrowm()
    {
        if (selectedEyebrow - 1 > 0)
        {
            selectedEyebrow--;
            EquipEyebrow();
            LoadEyebrow();
            LoadCharacters();
        }
    }
}
