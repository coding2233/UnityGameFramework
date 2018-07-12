### UnityGameFramework  
此框架参考`GameFramework`：[https://github.com/EllanJiang/GameFramework](https://github.com/EllanJiang/GameFramework)  
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
#### 二、游戏状态模块 `GameStateManager`
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
#### 三、资源管理模块 `ResourceManager`
1. 资源加载
```csharp
TextAsset textAsset= GameFrameworkMode.GetModule<ResourceManager>().LoadAsset<TextAsset>("Assets/TextAssets/test.txt");
```
2. 资源异步加载
* 添加资源异步加载的事件监听
```csharp
//资源异步加载成功
GameFrameworkMode.GetModule<EventManager>().AddListener<ResourceLoadAsyncSuccessEventArgs>(OnResLoadSuccess);
//资源异步加载失败
GameFrameworkMode.GetModule<EventManager>().AddListener<ResourceLoadAsyncFailureEventArgs>(OnResLoadFailure);
```
* 添加事件监听的回调函数
```csharp
//资源异步加载失败回调
private void OnResLoadFailure(object sender, IEventArgs e)
{
    ResourceLoadAsyncFailureEventArgs ne = (ResourceLoadAsyncFailureEventArgs) e;
}
//资源异步加载成功回调
private void OnResLoadSuccess(object sender, IEventArgs e)
{
    ResourceLoadAsyncSuccessEventArgs ne = (ResourceLoadAsyncSuccessEventArgs)e;
}
```
* 异步加载资源  
```csharp
GameFrameworkMode.GetModule<ResourceManager>().LoadAssetAsync<TextAsset>("Assets/TextAssets/test.txt");
```  
3. 内置对象池
* 添加预设
```csharp
GameFrameworkMode.GetModule<ResourceManager>().AddPrefab("Assets/Prefab/Player.prefab",
					new PoolPrefabInfo() {Prefab = playerPrefab,PreloadAmount=3, MaxAmount = 10});
```
* 生成对象
```csharp
GameObject player= GameFrameworkMode.GetModule<ResourceManager>().Spawn("Assets/Prefab/Player.prefab");
```
* 销毁对象
```csharp
GameFrameworkMode.GetModule<ResourceManager>().Despawn(player);
```
3. 加载场景,场景只支持异步加载
```csharp
AsyncOperation asyncOperation= GameFrameworkMode.GetModule<ResourceManager>().LoadSceneAsync("Assets/Scene/Main.unity");
```
4. 支持编辑器内资源的直接读取和AssetBundle资源读取两种方式的一键切换，避免测试的时候需要重复的打包AssetBundle资源

---
#### 四、UI管理模块 `UIManager`
1. 新建ui预设，新建ui类，继承类`UIView`,绑定并在类名上标明预设的资源路径
```csharp
[UIView("Assets/Prefab/UI/LoadingView.prefab")]
public class LoadingUIView : UIView
{
	/// <summary>
	/// 打开界面
	/// </summary>
	/// <param name="parameters">不确定参数</param>
	public override void OnEnter(params object[] parameters)
	{
		throw new System.NotImplementedException();
	}
	/// <summary>
	/// 退出界面
	/// </summary>
	public override void OnExit()
	{
		throw new System.NotImplementedException();
	}
	/// <summary>
	/// 暂停界面
	/// </summary>
	public override void OnPause()
	{
		throw new System.NotImplementedException();
	}
	/// <summary>
	/// 恢复界面
	/// </summary>
	public override void OnResume()
	{
		throw new System.NotImplementedException();
	}
}
```
2. 打开ui
```csharp
GameFrameworkMode.GetModule<UIManager>().Push<LoadingUIView>();
```
3. 关闭ui,在看到`push`&`pop`的时候，就知道`UIManager`是基于堆栈管理`UI`的，`pop`自然关闭的是最新打开的界面
```csharp
GameFrameworkMode.GetModule<UIManager>().Pop();
```

---
#### 五、数据节点模块 `NodeManager`
数据节点只用来存储在运行中的数据,用法类似`PlayerPrefs`
1. 存数据
```csharp
GameFrameworkMode.GetModule<NodeManager>().SetInt("Level", 10);
```
2. 取数据
```csharp
int level= GameFrameworkMode.GetModule<NodeManager>().GetInt("Level");
```

---
#### 六、Http网页请求模块 `WebRequestManager`
网页请求目前主要包含读取http上的文本文件和下载http服务器上的资源到本地两大功能
1. 请求文本
```csharp
//请求文本
GameFrameworkMode.GetModule<WebRequestManager>().ReadHttpText("http://nothing.com/AssetVersion.txt");
```
2. 请求下载
```csharp
GameFrameworkMode.GetModule<WebRequestManager>().StartDownload("http://nothing.com/AssetVersion.txt", "C:/AssetVersion.txt");
```
3. 事件监听
```csharp
//监听文本请求成功
GameFrameworkMode.GetModule<EventManager>().AddListener<HttpReadTextSuccessEventArgs>(OnHttpReadTextSuccess);
//文本请求失败
GameFrameworkMode.GetModule<EventManager>().AddListener<HttpReadTextFaileEventArgs>(OnHttpReadTextFaile);
//文件下载成功
GameFrameworkMode.GetModule<EventManager>().AddListener<DownloadSuccessEventArgs>(OnDownloadSuccess);
//文件下载失败
GameFrameworkMode.GetModule<EventManager>().AddListener<DownloadFaileEventArgs>(OnDownloadFaile);
```

---
### 内置工具
---
#### 一、AssetBundle打包工具
* 打包工具兼容unity自身右下角的assetbundle的标签设计
* 工具栏在Tools/VrCoreSystem/AssetBundle Editor,快捷键为ctrl+shift+o
* 为适配框架的ResourceManager，建议将最后打包的PlatformMainfest的名称都设置为AssetBundles
