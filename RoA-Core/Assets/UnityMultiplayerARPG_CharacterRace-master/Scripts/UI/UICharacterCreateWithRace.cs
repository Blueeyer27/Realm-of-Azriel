using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UICharacterCreateWithRace : UICharacterCreateExtension
    {
        public CharacterRaceTogglePair[] raceToggles;
        public CharacterGenderTogglePair[] genderToggles;
        public CharacterRace srace = null;
        public CharacterGender sGender = null;

        private Dictionary<CharacterRace, Toggle> cacheRaceToggles;
        public Dictionary<CharacterRace, Toggle> CacheRaceToggles
        {
            get
            {
                if (cacheRaceToggles == null)
                {
                    cacheRaceToggles = new Dictionary<CharacterRace, Toggle>();
                    foreach (CharacterRaceTogglePair raceToggle in raceToggles)
                    {
                        cacheRaceToggles[raceToggle.race] = raceToggle.toggle;
                    }
                }
                return cacheRaceToggles;
            }
        }

        private Dictionary<CharacterGender, Toggle> cacheGenderToggles;
        public Dictionary<CharacterGender, Toggle> CacheGenderToggles
        {
            get
            {
                if (cacheGenderToggles == null)
                {
                    cacheGenderToggles = new Dictionary<CharacterGender, Toggle>();
                    foreach (CharacterGenderTogglePair genderToggle in genderToggles)
                    {
                        cacheGenderToggles[genderToggle.Gender] = genderToggle.toggle;
                    }
                }
                return cacheGenderToggles;
            }
        }

        private readonly HashSet<CharacterRace> selectedRaces = new HashSet<CharacterRace>();
        private readonly HashSet<CharacterGender> selectedGenders = new HashSet<CharacterGender>();

        public override void Show()
        {
            foreach (KeyValuePair<CharacterRace, Toggle> raceToggle in CacheRaceToggles)
            {
                raceToggle.Value.onValueChanged.RemoveAllListeners();
                raceToggle.Value.onValueChanged.AddListener((isOn) =>
                {
                    OnRaceToggleUpdate(raceToggle.Key, isOn);
                });
                OnRaceToggleUpdate(raceToggle.Key, raceToggle.Value.isOn);
            }

            foreach (KeyValuePair<CharacterGender, Toggle> genderToggle in CacheGenderToggles)
            {
                genderToggle.Value.onValueChanged.RemoveAllListeners();
                genderToggle.Value.onValueChanged.AddListener((isOn) =>
                {
                    OnGenderToggleUpdate(genderToggle.Key, isOn);
                });
                OnGenderToggleUpdate(genderToggle.Key, genderToggle.Value.isOn);
            }
            base.Show();
        }

        protected override List<BasePlayerCharacterEntity> GetCreatableCharacters()
        {
            if (srace == null || sGender == null)
            {
                var character = GameInstance.PlayerCharacterEntities.Values.Where((a) => a.race.Equals(raceToggles[0].race)).ToList();
                var gendr = character.Where((a) => a.gender.Equals(genderToggles[0].Gender)).ToList();
                return gendr;
            }
            else
            {
                var character = GameInstance.PlayerCharacterEntities.Values.Where((a) => a.race.Equals(srace)).ToList();
                var gendr = character.Where((a) => a.gender.Equals(sGender)).ToList();
                return gendr;
            }
        }

        private void OnRaceToggleUpdate(CharacterRace race, bool isOn)
        {
            if (isOn)
            {
                selectedRaces.Add(race);
                srace = race;
                EquipHair();
                LoadCharacters();
                if (race.name.ToString().Equals("Human"))
                    SelectedFactionId = GetSelectableFactions()[0].DataId;
                else
                    SelectedFactionId = GetSelectableFactions()[1].DataId;


            }
            else
                selectedRaces.Remove(race);
        }

        private void OnGenderToggleUpdate(CharacterGender gender, bool isOn)
        {
            if (isOn)
            {
                selectedGenders.Add(gender);
                sGender = gender;

                EquipHair();
                LoadCharacters();
                //Debug.Log(gender);
            }
            else
                selectedGenders.Remove(gender);
        }
    }
}
