<div align="center">
  <img src="https://upload.wikimedia.org/wikipedia/zh/1/17/Minecraft_explore_landscape.png" alt="">
  <h1>MineCraftForUnity</h1>
  <p>unity + MineCraft </p>
  <p>用Unity实现我的世界 </p>
</div>



## 地图生成

### 初步试验

- https://zhuanlan.zhihu.com/p/571622819

按照文章的内容对生成的单个区块的地形生成做了实现，大致思路如下：

1. 生成噪声
2. 噪声值与高度的乘积决定上表面
3. 与最高上表面和水面高度进行判断
   1. 最高表面为`Grass_Dirt`，其余下面为`Dirt`
   2. 高于最高表面的取最大值
   3. 低于水面的上表面材质换成`sand`，高于的正常材质



> Block类型说明：
>
> ​	`Sand`:灰色、`Grass_Dirt`:绿色、`Dirt`:黄色、`Water`:蓝色

![image-20251017082040415](README/image-20251017082040415.png)

虽然模拟实现了地形区块生成的过程，但是地形过大的时候会导致游戏性能开销极大，因此方法只能作为一个最初的思路（也许还有其他方向？）

### 优化

在生成的地形中可以看到许多的被**遮挡的表面**是**没有必要被渲染**的，但是仍然需要保留方块信息的存在，因此需要维护两个方块数据表，一个存放所有`Block`信息，一个存放需要被渲染的`Block`的实体。为了进一步的性能提升还可以采用面优化技术，就是将玩家永远看不到的面剔除。

> 原作者的简单介绍：
>
> “我们知道在Unity中，一个面由2个三角面构成，这两个三角面又是4个顶点的组合按照左手螺旋的顺序来构成的。所以要想单独渲染一个面，我们就得提供顶点和三角形。所以我们实现Minecraft的地形时采用的动态Mesh生成，而不是把方块做成Prefab,然后实例化。针对不需要渲染的面，我们就不给它提供顶点或者三角面即可。（建议读者可以去了解下Unity中的Mesh）”

### 整体逻辑

首先，利用动态生成Mesh代替Prefab实例化去生成地形。然后就是将基于三种基本单位：

1. Face 面
2. Block 方块
3. Chunk 区块，同MC中的区块类似，将范围内的方块合并成一个区块，渲染时，以区块为单位渲染。

三种单位之间的具体关系可以概述为：几个Face构成一个Block，很多Block构成一个Chunk。

> MC中的Chunk是16x16x256包含许多方块的集合，由于一个世界极大并且包含极多区块，因此游戏仅加载部分区块以使游戏可玩，游戏不会运算已卸载的区块。
>
> - https://minecraft.fandom.com/zh/wiki/%E5%8C%BA%E5%9D%97
>
> ![latest (手机)](README/latest (手机).png)





## 方块编辑器



## 功能清单

- [ ] 地形生成
- [ ] 方块编辑
- [ ] 人物控制
- [ ] 工具使用
  - [ ] 稿子
  - [ ] 剑
  - [ ] 斧头
- [ ] 多人联机



## 参考资料

- https://developer.unity.cn/projects/5f35326fedbc2a002071984d
  - https://github.com/Jin-Yuhan/MinecraftClone-Unity
- https://zhuanlan.zhihu.com/p/571622819

- 【【Unity教程搬运】如何在Unity中创建Minecraft：终极指南 (2024)】 https://www.bilibili.com/video/BV1cJ4m137oj/?share_source=copy_web&vd_source=03b89d7c0f287bbb21e7897d1e86a944
