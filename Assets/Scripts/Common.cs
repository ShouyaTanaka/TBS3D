
namespace Utility
{
    public enum Map
    {
        // これらは透明なもので表されるため論理のみ
        Blocked = 0, //通行不可
        Ground = 1, //通行可能

        PlayerSpawn1 = 2, //プレイヤー1のスポーン位置
        PlayerSpawn2 = 3, //プレイヤー2のスポーン位置
        PlayerSpawn3 = 4, //プレイヤー3のスポーン位置
        PlayerSpawn4 = 5, //プレイヤー4のスポーン位置

        EnemySpawn1 = 6, //敵のスポーン位置(主将)
        EnemySpawn2 = 7, //敵のスポーン位置
        EnemySpawn3 = 8, //敵のスポーン位置
        EnemySpawn4 = 9, //敵のスポーン位置

    }
}