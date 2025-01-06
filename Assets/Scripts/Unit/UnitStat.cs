/*
	각종 스테이터스 값들 저장하고 불러올 수 있는 역할 등 해야하나
	아직 방법을 못찾음
*/

public struct UnitStat 
{
	// public RegenParam hp;
	// public RegenParam poise;
	public int strength;
	public int defense;

	public UnitStat(int maxHP, UIBar hpBar, int maxPoise, int strength, int defense)
	{
		// hp = new RegenParam(maxHP, maxHP, 0, false, hpBar);
		// poise = new RegenParam(maxPoise, maxPoise, (int)(maxPoise/4), true, null);
		this.strength = strength;	
		this.defense = defense;
	}
}
