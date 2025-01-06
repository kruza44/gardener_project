using UnityEngine;

public interface IWeaponAttack
{
    public void DoAttack(Vector2 targetDirection, Vector2 targetLocation);
}