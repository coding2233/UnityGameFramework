### UnityGameFramework  

**此框架参考:**   

`GameFramework`：[https://github.com/EllanJiang/GameFramework](https://github.com/EllanJiang/GameFramework)  
`UniRx`: [https://github.com/neuecc/UniRx](https://github.com/neuecc/UniRx)  
`UniTask`: [https://github.com/Cysharp/UniTask](https://github.com/Cysharp/UniTask)  
`odin-serializer`: [https://github.com/TeamSirenix/odin-serializer](https://github.com/TeamSirenix/odin-serializer)  

---

### Demo
`XLua Demo`实现: [https://github.com/coding2233/UnityGameFramework-xLua](https://github.com/coding2233/UnityGameFramework-xLua)  
`ILRuntime`为老版实现，需要在当前仓库切换到`ILRuntime`分支

---

### 内置模块介绍

---

### DataTable
DataTable为了配置修改不再动态生成或修改对应的序列化的类，进行全动态的解析。
为了做到通用性解析，配置表有一定的规则，以Excel操作的配置表为例。
* 每一行前面第一列带`#`号代表忽略。前面四行是固定的
* 第一行是配置表的名称;
* 第二行是配置表的键值;
* 第三行为当前列的数据类型,`[bool,int,long,float,double,string,Vector2,Vector3,Color]`,`Vector2`示例`100,100`,`Color`示例`#F0F`或者`#FF00FF`;
* 第四行是每一列的说明。
* 实际数据以第五行,第二列开始，第二列的数据一定为`int`类型的唯一识别`id`
* 最后用excel导出为`Unicode 文本`格式即可。

 配置表示例 

|||||||||  
|-|-|-|-|-|-|-|-|  
|#|关卡配置表|  
|#|Id|LevelSort|UIFormId|LevelName|LevelDesc|Leveldubing|IsHide|
|#|int|int|int|string|string|int|bool|
|#|关卡Id|关卡排序|界面编号|关卡名称|关卡描述|配音|是否隐藏|
||20010001|1|300|测试名称|测试说明|2352|false|
||20010002|2|200|测试名称02|测试说明02|23521|false|

使用示例

```csharp
//DataTable加载事件监听
GameMode.Event.AddListener<LoadDataTableEventArgs>(OnLoadDataTable);
//加载DataTable
GameMode.DataTable.LoadDataTable("Assets/Game/DataTable/GameCheckpoint.txt");

//DataTable加载事件回调
private void OnLoadDataTable(object sender,IEventArgs e)
{
    LoadDataTableEventArgs ne = e as LoadDataTableEventArgs;
    if(ne!=null)
    {
        IDataTable idt =  ne.Data;

        TableData td=idt[20010012]["UIFormId"];
        int uiFormId = (int)td;
        
        Debug.Log($"#################################:{ne.Message}");
        foreach (var item in idt)
        {
            Debug.Log($"-------------------------------------------------------");
            TableData td02 = idt[item];
            foreach (var item02 in td02)
            {
                Debug.Log(item02.ToString());
            }
        }
    }
}

//使用已加载的DataTable
IDataTable idt = GameMode.DataTable.GetDataTable("Assets/Game/DataTable/GameCheckpoint.txt");

```
`IDataTable`是当前配置表的所有数据的集合，可使用`foreach`获取到数据的`key`值，`TableData`是具体数据存储对象，主要支持上述的第三行的基本类型。


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

资源加载使用`async-await`来做异步加载资源

1. 资源加载(异步加载 )

```csharp
//加载普通资源
TextAsset textAsset= await GameFrameworkMode.GetModule<ResourceManager>().LoadAsset<TextAsset>("datatable","Assets/TextAssets/test.txt");
//实例化GameObject
GameObject obj = await GameFrameworkMode.GetModule<ResourceManager>().LoadAsset<GameObject>("player","Assets/Players/player.prefab");
GameObject player = Instantiate(obj);
```

2. 资源加载(同步加载)

```csharp
//先加载assetbundle
GameFrameworkMode.GetModule<ResourceManager>().LoadAssetBundle("hotfix");
//再加载资源
GameFrameworkMode.GetModule<ResourceManager>().LoadAssetSync("hotfix","main");
```

3. 内置对象池

* 添加预设
```csharp
GameFrameworkMode.GetModule<ResourceManager>().AddPrefab("player","Assets/Prefab/Player.prefab",
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

4. 加载场景,场景只支持异步加载

```csharp
AsyncOperation asyncOperation= await GameFrameworkMode.GetModule<ResourceManager>().LoadSceneAsync("mainscene","Assets/Scene/Main.unity");
```

5. 支持编辑器内资源的直接读取和AssetBundle资源读取两种方式的一键切换，避免测试的时候需要重复的打包AssetBundle资源

---

#### 四、UI管理模块 `UIManager`

1. 新建ui预设，新建ui类，继承类`UIView`,绑定并在类名上标明预设的资源路径

```csharp
[UIView("ui","Assets/Prefab/UI/LoadingView.prefab")]
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
//文件下载进度
 GameFrameworkMode.GetModule<EventManager>().AddListener<DownloadProgressEventArgs>(OnDownloadProgress);
```

---  

#### 七、音频管理器模块 `AudioManager`  

统一的声音播放管理，支持默认的背景音乐、ui音效、其他音效已经物体绑定的AudioSource多种模式，以下以播放ui音效为例

1. 添加ui音效音频

```csharp
GameFrameworkMode.GetModule<AudioManager>().AddUISound("soundclip","Assets/Audio/UI/default.wav");
```  

2. 播放ui音效

```csharp
GameFrameworkMode.GetModule<AudioManager>().PlayUISound("soundclip","Assets/Audio/UI/default.wav");
```

3. 停止ui音效,默认停止当前正在播放的音频

```csharp
GameFrameworkMode.GetModule<AudioManager>().StopUISound();
```  

4. 移除ui音效音频

```csharp
GameFrameworkMode.GetModule<AudioManager>().RemoveUISound("Assets/Audio/UI/default.wav");
```  

---  

#### 八、本地化管理模块 `LocalizationManager`

将配置文件中的本地化文件，读取语言存为字典保存在`LocalizationManager`中，使用`LocalizationText`绑定在`UGUI`的`Text`组件上。
同时支持动态设置

```csharp
go.GetComponent<LocalizationText>().Text="GameName";
```  

---

#### 九、设置模块 `SettingMangaer`

默认封装`PlayerPrefs`,使用方法类似。同时添加了`SetQuality`&`SetAllSoundVolume`&`SetBackgroundMusicVolume`&`SetUISoundVolume`&`SetSoundEffectVolume`等默认的设置  
具体使用`GameFrameworkMode.GetModule<SettingMangaer>()`一目了然  

---  

#### 十、网络模块 `NetworkManager`

正在增加中，首先会封装局域网内的连接通信，互联网后面增加。目前使用`kcp`将udp转为可靠传输，传输协议使用`Protobuf`

---

### 内置工具

---

#### 一、AssetBundle打包工具

* 打包工具兼容unity自身右下角的assetbundle的标签设计
* 工具栏在`Tools/AssetBundles Options`,快捷键为ctrl+shift+o
* 打包当前平台`Tools/Build AssetBundles`,快捷键为ctrl+shift+T
* 打包多个平台`Tools/Build AssetBundles Targets`,快捷键为ctrl+shift+Y

---

### 编辑器扩展

#### Game Module编辑扩展
* 继承`ModuleEditorBase`
* 构造函数同`ModuleEditorBase`
* 在类上添加标记`CustomModuleEditor`
