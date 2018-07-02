//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class CommonMethodManager {

	public CommonMethodManager()
    {
        EventManager.OnDamage += OnDamage;
    }

    void OnDamage(Entity entity, int damage, bool isMagic, int attackNumber, int maxAttackNumber)
    {
        var hashcode = entity.gameObject.GetHashCode();

        if (!isMagic)
        {
            entity.health = entity.health - ((damage < entity.defense) ? (damage - entity.defense) : 1);
        }
        else
        {
            entity.health = (entity.magicResist != 0) ? ((int)((float)entity.health - (float)damage * (((float)100 - (float)entity.magicResist) / (float)100))) : (entity.health - damage);
        }
    }
}
