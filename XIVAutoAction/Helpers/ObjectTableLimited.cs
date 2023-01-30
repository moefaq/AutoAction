using AutoAction.Updaters;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAction.Helpers
{
    internal class ObjectTableLimited
    {
        #nullable enable
        // 该选取几个目标？
        private static int ObjectsLimit()
        {
            return TargetUpdater.InPVP ? 5 : Service.Configuration.MaxObjectsLimit;
        }
        // 实现带有限制的找id
        public static GameObject? SearchById(uint objectId)
        {
            if (objectId == 3758096384u || objectId == 0)
            {
                return null;
            }
            for (var i = 0; i < ObjectsLimit() * 2; i += 2)
            {
                var item = Service.ObjectTable[i];
                if (item is not GameObject obj)
                    continue;
                if (obj.ObjectId == objectId)
                {
                    return item;
                }
            }
            return null;
        }
        // 枚举不是自己的BattleChara
        public static IEnumerable<BattleChara>? GetOtherCharas()
        {
            List<BattleChara> BCList = new List<BattleChara>();
            for (var i = 0; i < ObjectsLimit() * 2; i += 2)
            {
                var item = Service.ObjectTable[i];
                if (item is not BattleChara obj || obj.ObjectId == Service.ClientState.LocalPlayer.ObjectId)
                    continue;
                BCList.Add(obj);
            }
            IEnumerable<BattleChara> battleCharas = BCList.AsQueryable();
            return battleCharas;
        }
        // 查找亲信战友
        public static IEnumerable<BattleChara>? GetSubKind(byte kind)
        {
            List<BattleChara> BCList = new List<BattleChara>();
            for (var i = 0; i < ObjectsLimit() * 2; i += 2)
            {
                var item = Service.ObjectTable[i];
                if (item is not BattleChara obj || obj.SubKind != kind)
                    continue;
                BCList.Add(obj);
            }
            IEnumerable<BattleChara> battleCharas = BCList.AsQueryable();
            return battleCharas;
        }
        // 枚举PlayerCharacter
        public static IEnumerable<PlayerCharacter>? GetPlayers()
        {
            List<PlayerCharacter> PCList = new List<PlayerCharacter>();
            for (var i = 0; i < ObjectsLimit() * 2; i += 2)
            {
                var item = Service.ObjectTable[i];
                if (item is PlayerCharacter player)
                {
                    PCList.Add(player);
                }
            }
            IEnumerable<PlayerCharacter> playerCharacters = PCList.AsQueryable();
            return playerCharacters;
        }
        // 查找GameObject
        public static IEnumerable<GameObject>? GetGameObjects()
        {
            List<GameObject> GOList = new List<GameObject>();
            for (var i = 0; i < ObjectsLimit() * 2; i += 2)
            {
                var item = Service.ObjectTable[i];
                if(item is GameObject gameObject) {
                    GOList.Add(gameObject);
                }
            }
            IEnumerable<GameObject> gameObjects = GOList.AsQueryable();
            return gameObjects;
        }
    }
}
