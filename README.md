### UnityGameFramework  
此框架参考、借鉴、抄袭、山寨`GameFramework`框架，原框架链接：[https://github.com/EllanJiang/GameFramework](https://github.com/EllanJiang/GameFramework)  
因为现框架，也加入整理了我的不少想法，所以新建项目保存

---

### 内置模块介绍

---
#### 一、事件模块 `EventManager`
整个框架以事件作为驱动，以达到各个功能之间的解耦效果。除了可以自定义扩展事件以外，框架中还会自带一些事件，后面再详细列表。
1. 新建事件，新建一个类并继承`GameEventArgs`
```csharp
/// <summary>
/// 场景加载中事件
/// </summary>
public class SceneLoadingEventArgs : GameEventArgs<SceneLoadingEventArgs>
{
    /// <summary>
    /// 场景名称
    /// </summary>
    public string SceneName;
    /// <summary>
    /// 场景加载进度
    /// </summary>
    public float Progress;
}
```
2. 订阅事件
```csharp
GameFrameworkMode.GetModule<EventManager>().AddListener<SceneLoadingEventArgs>(OnSceneLoadingCallbak);
```
3. 取消事件订阅
```csharp
GameFrameworkMode.GetModule<EventManager>().RemoveListener<SceneLoadingEventArgs>(OnSceneLoadingCallbak);
```
4. 事件回调的函数实现
```csharp
private void OnSceneLoadingCallbak(object sender, IEventArgs e)
{
    SceneLoadingEventArgs ne = (SceneLoadingEventArgs) e;
    //...
}
```
5. 事件触发
```csharp
//第一种方式 带参数触发事件
GameFrameworkMode.GetModule<EventManager>()
	        .Trigger(this, new SceneLoadingEventArgs() {SceneName = "xxx", Progress = 0.85f});
//第二种方式 不带参数触发事件， 不带参数， 就不用生成新的对象，会直接传null
// GameFrameworkMode.GetModule<EventManager>().Trigger<SceneLoadingEventArgs>(this);
```

---
### 二、游戏状态模块 `GameStateManager`
游戏状态是整个项目的核心逻辑，建议将所有的逻辑都写再状态之中。增加状态管理几乎将各个类型的项目的开发都能尽量模式话，常用的状态:版本更新状态->配置加载状态->资源预加载->开始游戏->...
1. 增加状态，所有的状态都需要继承GameState,并在类名上添加类标记[GameState]
```csharp
[GameState]
public class LoadConfigState : GameState
{
    /// <summary>
    /// 初始化 -- 只执行一次
    /// </summary>
    public override void OnInit()
    {
        base.OnInit();
    }

    /// <summary>
    /// 进入状态
    /// </summary>
    /// <param name="parameters">不确定参数</param>
    public override void OnEnter(params object[] parameters)
    {
        base.OnEnter(parameters);
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
    }

    /// <summary>
    /// 固定帧函数
    /// </summary>
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    /// <summary>
    /// 渲染帧函数
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
```
2. 状态的类标记有四种类似  
* `[GameState]` 普通状态
* `[GameState(GameStateType.Normal)]` 普通状态
* `[GameState(GameStateType.Ignore)]` 忽略状态，表示在状态管理中忽略这个类的存在
* `[GameState(GameStateType.Start)]` 开始状态，在运行时，第一个运行的状态类标记
3. 状态切换,每个状态都有一个ChangeState函数
```csharp
//切换到开始状态
ChangeState<StartState>();
```

---
### 三、资源管理模块 `ResourceManager`
1. 资源更新

2. 资源加载(AssetBundle)

3. 内置对象池
