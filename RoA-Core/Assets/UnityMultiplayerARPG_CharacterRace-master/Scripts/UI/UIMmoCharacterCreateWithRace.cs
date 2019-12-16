using LiteNetLibManager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

namespace MultiplayerARPG.MMO
{
    public class UIMmoCharacterCreateWithRace : UIMmoCharacterCreateExtension
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

        protected override void OnClickCreate()
        {
            PlayerCharacterData characterData = new PlayerCharacterData();
            characterData.Id = GenericUtils.GetUniqueId();
            characterData.SetNewPlayerCharacterData(inputCharacterName.text.Trim(), SelectedDataId, SelectedEntityId);
            characterData.FactionId = SelectedFactionId;
            MMOClientInstance.Singleton.RequestCreateCharacter(characterData, OnRequestedCreateCharacter);
            }

        private readonly HashSet<CharacterRace> selectedRaces = new HashSet<CharacterRace>();
        private readonly HashSet<CharacterGender> selectedGenders = new HashSet<CharacterGender>();

        private void OnRaceToggleUpdate(CharacterRace race, bool isOn)
        {
            if (isOn)
            {
                selectedRaces.Add(race);
                srace = race;
                GetCreatableCharacters();
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
                LoadCharacters();
                GetCreatableCharacters();
                //Debug.Log(gender);
            }
            else
                selectedGenders.Remove(gender);
        }
        //------------------------------

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

        private void OnRequestedCreateCharacter(AckResponseCode responseCode, BaseAckMessage message)
        {
            ResponseCreateCharacterMessage castedMessage = (ResponseCreateCharacterMessage)message;

            switch (responseCode)
            {
                case AckResponseCode.Error:
                    string errorMessage = string.Empty;
                    switch (castedMessage.error)
                    {
                        case ResponseCreateCharacterMessage.Error.NotLoggedin:
                            errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_NOT_LOGGED_IN.ToString());
                            break;
                        case ResponseCreateCharacterMessage.Error.InvalidData:
                            errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_INVALID_DATA.ToString());
                            break;
                        case ResponseCreateCharacterMessage.Error.TooShortCharacterName:
                            errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_CHARACTER_NAME_TOO_SHORT.ToString());
                            break;
                        case ResponseCreateCharacterMessage.Error.TooLongCharacterName:
                            errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_CHARACTER_NAME_TOO_LONG.ToString());
                            break;
                        case ResponseCreateCharacterMessage.Error.CharacterNameAlreadyExisted:
                            errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_CHARACTER_NAME_EXISTED.ToString());
                            break;
                    }
                    UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), errorMessage);
                    break;
                case AckResponseCode.Timeout:
                    UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(UITextKeys.UI_ERROR_CONNECTION_TIMEOUT.ToString()));
                    break;
                default:
                    if (eventOnCreateCharacter != null)
                        eventOnCreateCharacter.Invoke();
                    break;
            }
        }
    }
}
