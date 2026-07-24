using UnityEngine;

public class ItemScript 
{
    public enum ItemType
    {
        SkillA,
        SkillB,
        SkillC,
        SkillD,
    }

    public ItemType itemType;
    public int amount;

    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:
            case ItemType.SkillA: return ItemAssets.Instance.skillA;
            case ItemType.SkillB: return ItemAssets.Instance.skillB;
            case ItemType.SkillC: return ItemAssets.Instance.skillC;
            case ItemType.SkillD: return ItemAssets.Instance.skillD;
        }
    }
}


