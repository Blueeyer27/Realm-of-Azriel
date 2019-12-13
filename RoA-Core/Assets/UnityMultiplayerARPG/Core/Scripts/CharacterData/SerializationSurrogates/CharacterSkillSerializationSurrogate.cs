﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class CharacterSkillSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(System.Object obj,
                              SerializationInfo info, StreamingContext context)
    {
        CharacterSkill data = (CharacterSkill)obj;
        info.AddValue("dataId", data.dataId);
        info.AddValue("level", data.level);
    }

    public System.Object SetObjectData(System.Object obj,
                                       SerializationInfo info, StreamingContext context,
                                       ISurrogateSelector selector)
    {
        CharacterSkill data = (CharacterSkill)obj;
        data.dataId = info.GetInt32("dataId");
        data.level = info.GetInt16("level");
        obj = data;
        return obj;
    }
}
