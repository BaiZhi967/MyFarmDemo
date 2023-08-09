using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    public SceneRouteDataList_SO sceneRouteDate;
    public List<NPCPosition> npcPositionList;
    
    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();
    
    protected override void Awake()
    {
        base.Awake();

        InitSceneRouteDict();
    }

    
    
    /// <summary>
    /// 初始化路径字典
    /// </summary>
    private void InitSceneRouteDict()
    {
        if (sceneRouteDate.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute route in sceneRouteDate.sceneRouteList)
            {
                var key = route.fromSceneName + route.gotoSceneName;

                if (sceneRouteDict.ContainsKey(key))
                    continue;

                sceneRouteDict.Add(key, route);
            }
        }
    }
    /// <summary>
    /// 获得两个场景间的路径
    /// </summary>
    /// <param name="fromSceneName">起始场景</param>
    /// <param name="gotoSceneName">目标场景</param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromSceneName, string gotoSceneName)
    {
        return sceneRouteDict[fromSceneName + gotoSceneName];
    }
}
