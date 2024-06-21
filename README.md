# BFramework-Ex
## 简介
`BFEX`基于本人制作的基础热更框架<a href='https://github.com/ToxicStar8/BFramework'>`BFramework`</a>的带有丰富功能的扩展框架。

## MainPackage
+ 使用`DowloadManager`原生实现了对可配置的资源下载，实现基本的热更需求。

## Editor
+ 带有便捷的编辑器选项，可以通过菜单栏快速进行`资源名、图集、AB包`的生成操作。
+ 原生实现的自定义打包配置`ABConfig`以及扩展面板的操作。
+ 在全局框架管理类`GameGod`的面板上进行了`对象池`和`计时器`的扩展显示。

## Framework
### Base
+ 全部游戏对象都可以继承自`GameBase`类，内部封装了便捷调用`Manager`的方式。

### Component
+ 拥有多达十几种的自定义的不涉及业务的通用组件。

### Extensions
+ 基于原生`Unity`的数据结构以及各种组件的扩展实现。

### Manager
+ Audio</br>
支持播放背景音乐和音效。

+ Event</br>
实现基础事件中心。

+ FSM</br>
实现基础状态机。

+ Load</br>
实现可寻址的同步/异步加载资源、加载精灵。

+ Module</br>
数据存储位置，对于网络游戏`Module`就是数据请求器以及数据缓存点，对于单机游戏`Module`就是数据的存档。

+ Net</br>
原生实现可自定义头部的Http请求。</br>
基于<a href='https://github.com/psygames/UnityWebSocket'>`UnityWebSocket`</a>实现的可自定义头部的`WebSocket`。

+ Pool</br>

+ RedPoint</br>

+ Table</br>

+ Timer</br>

+ UI</br>


## GameData
