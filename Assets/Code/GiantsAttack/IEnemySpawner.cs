namespace GiantsAttack
{
    public interface IEnemySpawner
    {
        IMonster SpawnEnemy(EnemyID id);
    }
}