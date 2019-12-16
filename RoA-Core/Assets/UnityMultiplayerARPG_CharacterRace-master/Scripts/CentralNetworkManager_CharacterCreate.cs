using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
    public partial class CentralNetworkManager
    {
        [DevExtMethods("SerializeCreateCharacterExtra")]
        public void SerializeCreateCharacterExtra_Custom(PlayerCharacterData characterData, NetDataWriter writer)
        {
            // Item 0 = Hair ?
            // Item 1 = Beard ?
            // Item 2 = Face ?
            // Item 3 = Eyebrow ?
            for (int i = 0; i < 4; ++i)
            {
                characterData.EquipItems[i].Serialize(writer);
            }
        }

        [DevExtMethods("DeserializeCreateCharacterExtra")]
        public void DeserializeCreateCharacterExtra_Custom(PlayerCharacterData characterData, NetDataReader reader)
        {
            for (int i = 0; i < 4; ++i)
            {
                CharacterItem tempItem = new CharacterItem();
                tempItem.Deserialize(reader);
                characterData.EquipItems[i] = tempItem;
            }
        }
    }
}